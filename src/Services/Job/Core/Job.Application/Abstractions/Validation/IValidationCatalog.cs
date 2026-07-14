using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Job.Application.Abstractions.Validation
{
    public interface IValidationCatalog
    {
        IReadOnlyDictionary<string, string> Patterns { get; }
        IReadOnlyDictionary<string, string> Messages { get; }

        string OneOf(params string[] values);

        Regex EnglishNameRegex();
        Regex EnglishAlphabetRegex();
        Regex BanglaNameRegex();
        Regex PhoneListRegex();
        Regex FileExtensionRegex();
        Regex OnlyNumericRegex();
        Regex NoOnlySpecialCharsRegex();
        Regex AlphaNumericRegex();
        Regex DecimalUpTo2Regex();
        Regex NotOnlyDigitsOrSpecialsRegex();
        Regex SwiftBicRegex();
        Regex Exactly9DigitsRegex();
        Regex AlphaNumericHyphenSlashRegex();
    }
}
