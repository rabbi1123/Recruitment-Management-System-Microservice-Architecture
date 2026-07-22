using System;
using System.Collections.Generic;
using System.Linq;

namespace Recruitment.Application.Common.Validation
{
    public static class ValidationUtils
    {
        public static string Normalize(string? value)
            => (value ?? string.Empty).Trim().ToLowerInvariant();

        public static bool HasNoDuplicates<T, TKey>(
            IEnumerable<T>? items,
            Func<T, TKey> keySelector)
        {
            if (items is null) return true;

            var seen = new HashSet<TKey>();
            foreach (var item in items)
            {
                if (!seen.Add(keySelector(item)))
                    return false;
            }
            return true;
        }

        public static bool BeCommaSeparatedIntegers(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            return value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .All(x => int.TryParse(x.Trim(), out _));
        }

        public static void NormalizeStrings<T>(T entity)
        {
            var props = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

            foreach (var p in props)
            {
                var raw = (string?)p.GetValue(entity);
                if (raw is null) continue;
                var trimmed = raw.Trim();
                p.SetValue(entity, string.IsNullOrEmpty(trimmed) ? null : trimmed);
            }
        }


        public static bool HasOverlap(
            DateOnly start1,
            DateOnly? end1,
            DateOnly start2,
            DateOnly? end2)
        {
            var normalizedEnd1 = end1 ?? DateOnly.MaxValue;
            var normalizedEnd2 = end2 ?? DateOnly.MaxValue;

            return start1 <= normalizedEnd2
                   && normalizedEnd1 >= start2;
        }
    }
}
