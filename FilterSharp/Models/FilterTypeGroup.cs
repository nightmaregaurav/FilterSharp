namespace FilterSharp.Models;

public abstract class FilterTypeGroup
{
    public static readonly IEnumerable<FilterType> StringFilters =
    [
        FilterType.Contains,
        FilterType.NotContains,
        FilterType.StartsWith,
        FilterType.EndsWith,
        FilterType.Equals,
        FilterType.NotEquals,
        FilterType.In,
        FilterType.NotIn,
        FilterType.IsNull,
        FilterType.IsNotNull
    ];

    public static readonly IEnumerable<FilterType> NumberFilters =
    [
        FilterType.Equals,
        FilterType.NotEquals,
        FilterType.GreaterThan,
        FilterType.GreaterThanOrEqual,
        FilterType.LessThan,
        FilterType.LessThanOrEqual,
        FilterType.In,
        FilterType.NotIn,
        FilterType.Between,
        FilterType.NotBetween,
        FilterType.IsNull,
        FilterType.IsNotNull
    ];

    public static readonly IEnumerable<FilterType> DateTimeFilters =
    [
        FilterType.Equals,
        FilterType.NotEquals,
        FilterType.GreaterThan,
        FilterType.GreaterThanOrEqual,
        FilterType.LessThan,
        FilterType.LessThanOrEqual,
        FilterType.Between,
        FilterType.NotBetween,
        FilterType.IsNull,
        FilterType.IsNotNull
    ];

    public static readonly IEnumerable<FilterType> BooleanFilters =
    [
        FilterType.Equals,
        FilterType.NotEquals,
        FilterType.IsTrue,
        FilterType.IsFalse,
        FilterType.IsTrueOrNull,
        FilterType.IsFalseOrNull,
        FilterType.IsNull,
        FilterType.IsNotNull
    ];
}