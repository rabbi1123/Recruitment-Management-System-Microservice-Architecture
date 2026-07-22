using System.Text.Json;
using System.Text.Json.Serialization;

namespace Recruitment.Infrastructure.Serialization
{
	public sealed class TrimmingStringJsonConverter : JsonConverter<string>
	{
		public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> reader.TokenType == JsonTokenType.String
				? (reader.GetString()?.Trim()) switch { "" => null, var s => s }
				: null;

		public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value);
	}
}
