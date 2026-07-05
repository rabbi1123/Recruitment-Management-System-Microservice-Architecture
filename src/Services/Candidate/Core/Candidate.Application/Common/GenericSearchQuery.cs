using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Candidate.Application.Common
{
    public record GenericSearchQuery
    {
        [Range(0, int.MaxValue, ErrorMessage = "Page no must be greater than or equal to 0")]
        public int PageNo { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Page size must be greater than or equal to 0")]
        public int PageSize { get; set; }
        public string? GlobalSearch { get; set; }
        public List<SearchFilter> Search { get; set; }
        public SortOption SortBy { get; set; }
    }

    public record SearchFilter
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }

        public ComparisonOperator Comparison { get; set; } = ComparisonOperator.Equals; // ✅ Default

        public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;     // ✅ Default
    }

    public record SortOption
    {
        public string PropertyName { get; set; }
        public string SortBy { get; set; } = "asc"; // ✅ Default// "asc" or "desc"
    }

    public enum ComparisonOperator
    {
        // Text
        Contains = 0,
        Equals = 1,
        StartsWith = 2,
        EndsWith = 3,
        NotContains = 4,
        NotEquals = 5,
        IsEmpty = 6,
        IsNotEmpty = 7,

        // Number
        GreaterThan = 10,
        GreaterThanOrEqual = 11,
        LessThan = 12,
        LessThanOrEqual = 13,

        // Boolean
        IsTrue = 20,
        IsFalse = 21,

        // Date
        IsBefore = 30,
        IsAfter = 31,
        IsOnOrBefore = 32,
        IsOnOrAfter = 33,
        IsExactly = 34,
        IsNotExactly = 35,
        IsBetween = 36,

        // Array
        IncludesAny = 40,
        IncludesAll = 41,
        NotIncludes = 42,

        // Null Checks
        IsNull = 50,
        IsNotNull = 51
    }
    public enum LogicalOperator
    {
        And = 0,
        Or = 1
    }
}
