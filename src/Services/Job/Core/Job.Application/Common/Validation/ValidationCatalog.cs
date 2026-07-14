using Job.Application.Abstractions.Validation;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Job.Application.Common.Validation
{
    public static partial class ValidationConstants
    {
        public const string EnglishNamePattern = @"^(?=.*[A-Za-z])(?!.*([^0-9])\1{2,})(?!.*([',.&/\-()_])\2+)[A-Za-z0-9 '&,.\-/()_]+$";

        public const string BanglaNamePattern = @"^(?=.*[\p{IsBengali}-[^\p{L}]])(?!.*([',.&/\-()_])\1+)[\p{IsBengali} \u200C\u200D'&,.\-/()_]+$";

        public const string EnglishNameWithQuestionPattern =
            @"^(?=.*[A-Za-z])(?!.*([^0-9])\1{2,})(?!.*([',.&/\-()?\u003F])\2+)[A-Za-z0-9 '&,.\-/()?\u003F]+$";

        public const string BanglaNameWithQuestionPattern =
            @"^(?=.*[\p{IsBengali}-[^\p{L}]])(?!.*([',.&/\-()?\u003F])\1+)[\p{IsBengali} \u200C\u200D'&,.\-/()?\u003F]+$";

        // One or more emails separated by comma/semicolon (spaces allowed)
        public const string EmailListPattern =
            @"^\s*(?:[A-Za-z0-9](?:[A-Za-z0-9_%+\-]|(?:\.(?!\.)))*[A-Za-z0-9]?@(?:[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?\.)+[A-Za-z]{2,24})(?:\s*[,;]\s*(?:[A-Za-z0-9](?:[A-Za-z0-9_%+\-]|(?:\.(?!\.)))*[A-Za-z0-9]?@(?:[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?\.)+[A-Za-z]{2,24}))*\s*$";

        // Phone list: items above separated by comma or semicolon (spaces allowed around separators)
        public const string PhoneListPattern =
            @"^\s*(?:\+)?(?:\(\d+\)|\d+)(?:[ \-]?(?:\(\d+\)|\d+))*" +
            @"(?:\s*[,;]\s*(?:\+)?(?:\(\d+\)|\d+)(?:[ \-]?(?:\(\d+\)|\d+))*)*\s*$";

        public const string UrlPattern =
                @"^(?:(?:https?):\/\/(?:(?:localhost)|(?:[A-Za-z0-9-]+\.)+[A-Za-z]{2,24}|(?:\d{1,3}\.){3}\d{1,3})(?::\d{1,5})?(?:\/\S*)?"
              + @"|\/\/(?:(?:localhost)|(?:[A-Za-z0-9-]+\.)+[A-Za-z]{2,24}|(?:\d{1,3}\.){3}\d{1,3})(?::\d{1,5})?(?:\/\S*)?"
              + @"|\/\S+)$";

        public const string RoutePathPattern = @"^\/[A-Za-z0-9\-._~\/{}:]*$";

        public const string BloodGroupPattern = @"^(?:A|B|AB|O)\s*[+\-]\s*(?:ve)?$";
        public const string FileExtensionPattern = @"^\.?[A-Za-z0-9]+$";
        public const string OnlyNumericPattern = @"^[0-9]+$";
        public const string OnlyAlphabet = @"^[A-Za-z][A-Za-z]*$";
        public const string NoOnlySpecialCharsPattern = @"^(?=.*[\p{L}\p{Nd}]).+$";
        public const string AlphaNumericPattern = @"(?i)^[A-Z0-9]+$";
        public const string DecimalUpTo2Pattern = @"^\d+\.\d{1,2}$";
        public const string Exactly9DigitsPattern = @"^\d{9}$";

        // 8 or 11 chars: 4 letters (bank) + 2 letters (country) + 2 alnum (location) + optional 3 alnum (branch)
        public const string SwiftBicPattern = @"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$";

        // New: 8–35 chars, at least 1 uppercase and 1 special char (non letter/digit/underscore)
        public const string PasswordPattern = @"^(?=.*[A-Z])(?=.*[\W_]).{8,35}$";

        public const string NotOnlyDigitsOrSpecialsPattern = @"^(?=.*\p{L}).+$";

        public const string AlphaNumericHyphenSlashPattern = @"^[A-Za-z0-9\/\-]+$";

        [GeneratedRegex(EnglishNamePattern, RegexOptions.CultureInvariant)]
        public static partial Regex EnglishNameRegex();
        [GeneratedRegex(OnlyAlphabet, RegexOptions.CultureInvariant)]
        public static partial Regex EnglishAlphabetRegex();

        [GeneratedRegex(BanglaNamePattern, RegexOptions.CultureInvariant)]
        public static partial Regex BanglaNameRegex();

        [GeneratedRegex(EnglishNameWithQuestionPattern, RegexOptions.CultureInvariant)]
        public static partial Regex EnglishNameWithQuestionRegex();

        [GeneratedRegex(BanglaNameWithQuestionPattern, RegexOptions.CultureInvariant)]
        public static partial Regex BanglaNameWithQuestionRegex();

        [GeneratedRegex(EmailListPattern, RegexOptions.CultureInvariant)]
        public static partial Regex EmailListRegex();

        [GeneratedRegex(PhoneListPattern, RegexOptions.CultureInvariant)]
        public static partial Regex PhoneListRegex();

        [GeneratedRegex(FileExtensionPattern, RegexOptions.CultureInvariant)]
        public static partial Regex FileExtensionRegex();

        [GeneratedRegex(PasswordPattern, RegexOptions.CultureInvariant)]
        public static partial Regex PasswordRegex();

        [GeneratedRegex(OnlyNumericPattern, RegexOptions.CultureInvariant)]
        public static partial Regex OnlyNumericRegex();

        [GeneratedRegex(NoOnlySpecialCharsPattern, RegexOptions.CultureInvariant)]
        public static partial Regex NoOnlySpecialCharsRegex();

        [GeneratedRegex(AlphaNumericPattern, RegexOptions.CultureInvariant)]
        public static partial Regex AlphaNumericRegex();

        [GeneratedRegex(DecimalUpTo2Pattern, RegexOptions.CultureInvariant)]
        public static partial Regex DecimalUpTo2Regex();

        [GeneratedRegex(NotOnlyDigitsOrSpecialsPattern, RegexOptions.CultureInvariant)]
        public static partial Regex NotOnlyDigitsOrSpecialsRegex();

        [GeneratedRegex(Exactly9DigitsPattern, RegexOptions.CultureInvariant)]
        public static partial Regex Exactly9DigitsRegex();

        [GeneratedRegex(SwiftBicPattern, RegexOptions.CultureInvariant)]
        public static partial Regex SwiftBicRegex();

        [GeneratedRegex(AlphaNumericHyphenSlashPattern, RegexOptions.CultureInvariant)]
        public static partial Regex AlphaNumericHyphenSlashRegex();

        [GeneratedRegex(UrlPattern, RegexOptions.CultureInvariant)]
        public static partial Regex UrlRegex();

        [GeneratedRegex(RoutePathPattern, RegexOptions.CultureInvariant)]
        public static partial Regex RoutePathRegex();
    }

    // ---- Implementation ----
    public sealed class ValidationCatalog : IValidationCatalog
    {
        private static readonly IReadOnlyDictionary<string, string> _patterns =
            new Dictionary<string, string>
            {
                { "EnglishName",   ValidationConstants.EnglishNamePattern },
                { "BanglaName",    ValidationConstants.BanglaNamePattern },
                { "EnglishAlphabet",   ValidationConstants.OnlyAlphabet },
                { "EnglishNameWithQuestion",  ValidationConstants.EnglishNameWithQuestionPattern },
                { "BanglaNameWithQuestion",   ValidationConstants.BanglaNameWithQuestionPattern },
                { "EmailList",         ValidationConstants.EmailListPattern },
                { "PhoneList",         ValidationConstants.PhoneListPattern },
                { "FileExtension", ValidationConstants.FileExtensionPattern },
                { "Password",      ValidationConstants.PasswordPattern },
                { "OnlyNumeric",   ValidationConstants.OnlyNumericPattern },
                { "NoOnlySpecialChars", ValidationConstants.NoOnlySpecialCharsPattern },
                { "AlphaNumeric", ValidationConstants.AlphaNumericPattern },
                { "DecimalUpTo2", ValidationConstants.DecimalUpTo2Pattern },
                { "NotOnlyDigitsOrSpecials", ValidationConstants.NotOnlyDigitsOrSpecialsPattern},
                { "SwiftBic", ValidationConstants.SwiftBicPattern},
                { "AlphaNumericHyphenSlash", ValidationConstants.AlphaNumericHyphenSlashPattern },
                { "Url",           ValidationConstants.UrlPattern },
                { "RoutePath",     ValidationConstants.RoutePathPattern }
            };

        private static readonly IReadOnlyDictionary<string, string> _messages =
        new Dictionary<string, string>
        {
            // Core
            { "Required",               "{PropertyName} is required." },
            { "MaxLength",              "{PropertyName} length can't be more than {MaxLength}." },
            { "MinLength",              "{PropertyName} length must be at least {MinLength}." },
            { "LengthRange",            "{PropertyName} length must be between {MinLength} and {MaxLength}." },
            { "Range",                  "{PropertyName} must be between {From} and {To}." },
            { "MinValue",               "{PropertyName} can't be less than {ComparisonValue}." },
            { "MaxValue",               "{PropertyName} can't be more than {ComparisonValue}." },
            { "PositiveNumber",         "{PropertyName} must be a valid positive number." },
            { "GteZero",                "{PropertyName} must be greater than or equal to 0." },

            // Days / dates
            { "LessThanOneYearDays",    "{PropertyName} must be less than or equal to 365 days." },
            { "EndDateAfterStart",      "{PropertyName} must be after Start date." },
            { "NoFutureDate",           "{PropertyName} cannot be in the future." },

            // Name rules
            { "EnglishNameRule",
              "{PropertyName}: letters (A–Z), digits (0–9), space, apostrophe ('), hyphen (-), dot (.), ampersand (&), comma (,), parentheses (), and slash (/). Must include a letter, not be digits-only, must not contain more than two identical characters in a row or repeated punctuation." },

            { "EnglishAlphabetRule",
              "{PropertyName}: letters (A–Z). Must include only letters." },

            { "BanglaNameRule",
              "{PropertyName}: Bangla (Bengali script) characters and digits, space, apostrophe ('), hyphen (-), dot (.), ampersand (&), comma (,), parentheses (), and slash (/). Must include a Bangla letter; not digits-only; no repeated punctuation." },

            { "EnglishNameWithQuestionRule",
              "{PropertyName}: letters (A–Z), digits (0–9), space, underscore (_), apostrophe ('), hyphen (-), dot (.), ampersand (&), comma (,), parentheses (), slash (/), and question mark (?). Must include a letter, not be digits-only, and must not contain more than two identical characters in a row or repeated punctuation." },

            { "BanglaNameWithQuestionRule",
              "{PropertyName}: Bangla (Bengali script) characters and digits, space, underscore (_), apostrophe ('), hyphen (-), dot (.), ampersand (&), comma (,), parentheses (), slash (/), and question mark (?). Must include a Bangla letter; not digits-only; no repeated punctuation." },

            // Lists & formats
            { "EmailListRule",          "{PropertyName} must be one or more valid emails separated by a comma (,) or semicolon (;). Each must look like name@example.com. Allowed before @: letters, digits, dot (.), underscore (_), percent (%), plus (+), hyphen (-)." },
            { "PhoneListRule",          "{PropertyName} must be comma- or semicolon-separated phones; each at least 7 characters and matching digits, spaces, '+', '-', and '()'." },

            // File / extension
            { "FileExtensionRule",      "{PropertyName} must contain letters/numbers only with an optional leading dot (e.g., .pdf)." },

            // Password / characters
            { "PasswordRule",           "{PropertyName} must be 8–35 characters and include at least one uppercase letter and one special character." },
            { "OnlyNumericRule",        "{PropertyName} must contain only digits (0-9)." },
            { "NoOnlySpecialChars",     "{PropertyName} must include at least one letter or digit." },
            { "AlphaNumeric",           "{PropertyName} must contain only letters and digits."  },
            { "AlphaNumericHyphenSlash","{PropertyName} must contain only letters, digits, hyphen (-) and slash (/)."  },
            { "DecimalUpTo2Rule",       "{PropertyName} must include a decimal point with up to 2 digits after it (e.g., 3.50)." },
            { "NotOnlyDigitsOrSpecials","{PropertyName} must include at least one letter." },

            // Specific IDs / codes
            { "Exactly9DigitsRule",     "{PropertyName} must be exactly 9 digits." },
            { "SwiftBic",               "{PropertyName} must be a valid SWIFT/BIC: 4 letters (bank) + 2 letters (country) + 2 letters/digits (location) + optional 3 letters/digits (branch). Use uppercase A–Z and digits only (no spaces). Example: ABCDGB2L or ABCDGB2LXXX." },

            // URLs & routes
            { "UrlInvalid",             "{PropertyName} must be a valid http/https URL (domain/localhost/IP), optional port, and optional path/query/fragment." },
            { "RoutePathInvalid",       "{PropertyName} must be an app route starting with '/', without scheme/host, e.g. /employees, /employees/{id}, /v1/users/:id." }
        };

        public IReadOnlyDictionary<string, string> Patterns => _patterns;
        public IReadOnlyDictionary<string, string> Messages => _messages;

        public string OneOf(params string[] values) =>
            $"{{PropertyName}} must be one of: {string.Join(", ", values)}.";

        public Regex EnglishNameRegex() => ValidationConstants.EnglishNameRegex();
        public Regex BanglaNameRegex() => ValidationConstants.BanglaNameRegex();
        public Regex EnglishNameWithQuestionRegex() => ValidationConstants.EnglishNameWithQuestionRegex();
        public Regex BanglaNameWithQuestionRegex() => ValidationConstants.BanglaNameWithQuestionRegex();
        public Regex EmailListRegex() => ValidationConstants.EmailListRegex();
        public Regex PhoneListRegex() => ValidationConstants.PhoneListRegex();
        public Regex FileExtensionRegex() => ValidationConstants.FileExtensionRegex();
        public Regex PasswordRegex() => ValidationConstants.PasswordRegex();
        public Regex OnlyNumericRegex() => ValidationConstants.OnlyNumericRegex();
        public Regex NoOnlySpecialCharsRegex() => ValidationConstants.NoOnlySpecialCharsRegex();
        public Regex AlphaNumericRegex() => ValidationConstants.AlphaNumericRegex();
        public Regex DecimalUpTo2Regex() => ValidationConstants.DecimalUpTo2Regex();
        public Regex NotOnlyDigitsOrSpecialsRegex() => ValidationConstants.NotOnlyDigitsOrSpecialsRegex();
        public Regex Exactly9DigitsRegex() => ValidationConstants.Exactly9DigitsRegex();
        public Regex SwiftBicRegex() => ValidationConstants.SwiftBicRegex();
        public Regex AlphaNumericHyphenSlashRegex() => ValidationConstants.AlphaNumericHyphenSlashRegex();
        public Regex UrlRegex() => ValidationConstants.UrlRegex();
        public Regex RoutePathRegex() => ValidationConstants.RoutePathRegex();
        public Regex EnglishAlphabetRegex() => ValidationConstants.EnglishAlphabetRegex();
    }
}
