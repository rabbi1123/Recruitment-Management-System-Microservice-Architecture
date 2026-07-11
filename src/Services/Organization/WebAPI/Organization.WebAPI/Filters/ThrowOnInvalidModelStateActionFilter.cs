using Common.Platform.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Organization.WebAPI.Filters
{
	public sealed class ThrowOnInvalidModelStateActionFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			var ms = context.ModelState;
			if (ms.IsValid) return;

			// Identify binder/JSON errors
			var binderEntries = ms.Where(kv => kv.Value is { Errors.Count: > 0 } && HasBinderError(kv.Value)).ToList();
			if (binderEntries.Count == 0) return; // let MediatR/FV handle validator errors

			var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			// 1) Binder/JSON errors
			foreach (var (rawKey, entry) in binderEntries)
			{
				var key = NormalizeKey(rawKey);
				if (string.IsNullOrEmpty(key)) continue;

				var err = entry.Errors.FirstOrDefault(e =>
								   !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
								   e.ErrorMessage.Contains("Path:", StringComparison.OrdinalIgnoreCase))
						  ?? entry.Errors.First();

				var raw = string.IsNullOrWhiteSpace(err.ErrorMessage)
					? err.Exception?.Message ?? "Invalid value."
					: err.ErrorMessage;

				string msg;
				if (raw.IndexOf("Path:", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					var head = FirstSentenceBeforePath(raw);       // only first sentence before "Path:"
					msg = SanitizeClrTypeNames(head);              // hide System.* types

					msg = "Invalid value provided.";
				}
				else
				{
					msg = raw; // no Path: → keep as-is
				}

				fields.TryAdd(key, msg);
			}

			// 2) Include validator (DA/FV) errors AS-IS if they coexist with binder errors
			var validatorEntries = ms.Where(kv => kv.Value is { Errors.Count: > 0 } && !HasBinderError(kv.Value));
			foreach (var (rawKey, entry) in validatorEntries)
			{
				var key = NormalizeKey(rawKey);
				if (string.IsNullOrEmpty(key)) continue;
				if (entry?.AttemptedValue is null) continue;

				var err = entry.Errors.FirstOrDefault(e => e.Exception is null && !string.IsNullOrWhiteSpace(e.ErrorMessage))
						  ?? entry.Errors.First();

				var msg = string.IsNullOrWhiteSpace(err.ErrorMessage) ? "Invalid information. Please check the highlighted fields." : err.ErrorMessage;
				fields.TryAdd(key, msg);
			}

			throw new ValidationException(
				Result.Failure(HttpResponseStatusCodes.BadRequest, Error.BadRequest("Invalid information. Please check the highlighted fields.", fields)).Error
			);
		}

		public void OnActionExecuted(ActionExecutedContext context) { }

		private static bool HasBinderError(ModelStateEntry e) =>
			e.Errors.Any(err =>
				err.Exception is JsonException or FormatException or OverflowException ||
				(!string.IsNullOrWhiteSpace(err.ErrorMessage) && (
					err.ErrorMessage.Contains("Path:", StringComparison.OrdinalIgnoreCase) ||
					err.ErrorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase) ||
					err.ErrorMessage.Contains("is not valid for", StringComparison.OrdinalIgnoreCase) ||
					err.ErrorMessage.Contains("is an invalid JSON literal", StringComparison.OrdinalIgnoreCase) ||
					err.ErrorMessage.Contains("invalid JSON", StringComparison.OrdinalIgnoreCase) ||
					err.ErrorMessage.Contains("A non-empty request body is required", StringComparison.OrdinalIgnoreCase)
				))
			);

		// Keep only the first sentence that appears before "Path:"
		private static string FirstSentenceBeforePath(string message)
		{
			if (string.IsNullOrWhiteSpace(message)) return "Invalid value.";

			var idx = message.IndexOf("Path:", StringComparison.OrdinalIgnoreCase);
			var head = (idx >= 0 ? message[..idx] : message)
				.Replace("\r", " ").Replace("\n", " ").Trim();

			if (head.Length == 0) return "Invalid value.";

			// sentence boundary = ". ", "! ", "? " (avoid cutting "System.Int64")
			int boundary = IndexOfAny(head, new[] { ". ", "! ", "? " });
			if (boundary >= 0) return head[..(boundary + 1)];
			return head.EndsWith('.') || head.EndsWith('!') || head.EndsWith('?') ? head : head + ".";
		}

		private static int IndexOfAny(string s, string[] needles)
		{
			var best = -1;
			foreach (var n in needles)
			{
				var i = s.IndexOf(n, StringComparison.Ordinal);
				if (i >= 0 && (best < 0 || i < best)) best = i;
			}
			return best;
		}

		// Replace CLR type names with friendly, tech-agnostic words
		private static string SanitizeClrTypeNames(string message)
		{
			if (string.IsNullOrWhiteSpace(message)) return "Invalid value.";

			// Unwrap Nullable`1[...] -> inner
			message = Regex.Replace(message, @"System\.Nullable`1\[(?<inner>[^\]]+)\]", "${inner}");

			// Map common types to friendly phrases
			static string Map(string clrType)
			{
				// strip namespace & generic arity
				var simple = clrType.Split('.').Last();
				simple = Regex.Replace(simple, @"`[0-9]+$", "");

				return simple switch
				{
					"Byte" or "SByte" or "Int16" or "UInt16" or "Int32" or "UInt32" or "Int64" or "UInt64"
					or "Single" or "Double" or "Decimal" => "a number",
					"Boolean" => "true or false",
					"String" => "text",
					"Guid" => "a valid GUID",
					"DateTime" or "DateOnly" or "TimeOnly" => "a valid date/time",
					_ => "the expected type"
				};
			}

			// Replace any System.* type tokens (incl. nested generics/arrays) with friendly phrase
			message = Regex.Replace(
				message,
				@"System\.[\w`]+(?:\[[^\]]+\])*(?:\[\])?",
				m => Map(m.Value),
				RegexOptions.CultureInvariant);

			return message;
		}

		private static string NormalizeKey(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw)) return "";
			if (raw.StartsWith("$.", StringComparison.Ordinal)) raw = raw[2..];                 // "$.id" -> "id"
			if (raw.StartsWith("request.", StringComparison.OrdinalIgnoreCase)) raw = raw[8..]; // "request.id" -> "id"
			if (raw.Equals("request", StringComparison.OrdinalIgnoreCase)) return "";
			if (char.IsUpper(raw[0])) raw = char.ToLowerInvariant(raw[0]) + raw[1..];          // "Id" -> "id"
			return raw;
		}
	}
}
