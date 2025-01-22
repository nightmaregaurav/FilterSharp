using System.Linq.Expressions;
using FilterSharp.Models;

namespace FilterSharp;

public static class FilterSharp
{
    public static IQueryable<T> Filter<T>(this IQueryable<T> queryable, IList<FilterDescriptor> filters, IList<FilterRule<T>> rules)
    {
        foreach (var filter in filters.Where(x => x.Filter != null))
        {
            var filterDescriptor = rules.SingleOrDefault(x => x.Name == filter.By);
            if (filterDescriptor == null)
            {
                throw new NotImplementedException($"Filter for property {filter.By} is not implemented!"); 
            }

            var propertyAccessor = filterDescriptor.Accessor;
            var propertyType = filterDescriptor.DataType;
            var validFilterTypes = GetValidFiltersForType(propertyType);
            if (validFilterTypes.All(x => x != filter.Filter))
            {
                throw new NotImplementedException($"Filter type {filter.Filter} is not valid for property {filter.By}!");
            }
            queryable = queryable.FilterBasedOnFilterTypeAndPropertyType(propertyType, propertyAccessor, filter.Filter!.Value, filter.Query ?? "");
        }

        var sortingStarted = false;
        foreach (var sort in filters.Where(x => x.Order != null))
        {
            var sortDescriptor = rules.SingleOrDefault(x => x.Name == sort.By);
            if (sortDescriptor == null)
            {
                throw new NotImplementedException($"Sorting for property {sort.By} is not implemented!"); 
            }
            
            var propertyAccessor = sortDescriptor.Accessor; 
            if (!sortingStarted)
            {
                queryable = sort.Order == SortType.Ascending 
                    ? queryable.OrderBy(propertyAccessor)
                    : queryable.OrderByDescending(propertyAccessor);
                sortingStarted = true;
            }
            else
            {
                queryable = sort.Order == SortType.Ascending 
                    ? ((IOrderedQueryable<T>)queryable).ThenBy(propertyAccessor) 
                    : ((IOrderedQueryable<T>)queryable).ThenByDescending(propertyAccessor);
            }
        }

        return queryable;
    }
    
    public static IEnumerable<FilterType> GetValidFiltersForType(Type type)
    {
        IEnumerable<FilterType> validFilterTypes;

        if (type == typeof(int)) validFilterTypes = FilterTypeGroup.NumberFilters;
        else if (type == typeof(long)) validFilterTypes = FilterTypeGroup.NumberFilters;
        else if (type == typeof(DateOnly)) validFilterTypes = FilterTypeGroup.DateTimeFilters;
        else if (type == typeof(TimeOnly)) validFilterTypes = FilterTypeGroup.DateTimeFilters;
        else if (type == typeof(DateTime)) validFilterTypes = FilterTypeGroup.DateTimeFilters;
        else if (type == typeof(bool)) validFilterTypes = FilterTypeGroup.BooleanFilters;
        else validFilterTypes = FilterTypeGroup.StringFilters;

        return validFilterTypes;
    }
    
    private static IQueryable<T> FilterBasedOnFilterTypeAndPropertyType<T>(
        this IQueryable<T> queryable,
        Type propertyType,
        Expression<Func<T, object?>> propertyAccessor,
        FilterType filterType,
        string filterQuery
    ) {
        Expression<Func<T, bool>> predicate;
        switch (filterType)
        {
            case FilterType.Contains:
                predicate = t => ((string)(propertyAccessor.Call()(t) ?? "")).ToLower().Contains(filterQuery.ToLower());
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.NotContains:
                predicate = t => !((string)(propertyAccessor.Call()(t) ?? "")).ToLower().Contains(filterQuery.ToLower());
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.StartsWith:
                predicate = t => ((string)(propertyAccessor.Call()(t) ?? "")).ToLower().StartsWith(filterQuery.ToLower());
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.EndsWith:
                predicate = t => ((string)(propertyAccessor.Call()(t) ?? "")).ToLower().EndsWith(filterQuery.ToLower());
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.Equals:
                if (propertyType == typeof(int))
                {
                    int? intQuery = int.TryParse(filterQuery, out var intValue) ? intValue : null;
                    predicate = t => (int?)(propertyAccessor.Call()(t) ?? null) == intQuery;
                }
                else if (propertyType == typeof(long))
                {
                    long? longQuery = long.TryParse(filterQuery, out var longValue) ? longValue : null;
                    predicate = t => (long?)(propertyAccessor.Call()(t) ?? null) == longQuery;
                }
                else if (propertyType == typeof(DateOnly)) {
                    DateOnly? dateQuery = DateOnly.TryParse(filterQuery, out var dateValue) ? dateValue : null;
                    predicate = t => (DateOnly?)(propertyAccessor.Call()(t) ?? null) == dateQuery;
                }
                else if (propertyType == typeof(TimeOnly))
                {
                    TimeOnly? timeQuery = TimeOnly.TryParse(filterQuery, out var timeValue) ? timeValue : null;
                    predicate = t => (TimeOnly?)(propertyAccessor.Call()(t) ?? null) == timeQuery;
                }
                else if (propertyType == typeof(DateTime))
                {
                    DateTime? dateTimeQuery = DateTime.TryParse(filterQuery, out var dateTimeValue) ? dateTimeValue : null;
                    predicate = t => (DateTime?)(propertyAccessor.Call()(t) ?? null) == dateTimeQuery;
                }
                else if (propertyType == typeof(bool))
                {
                    bool? booleanQuery = bool.TryParse(filterQuery, out var booleanValue) ? booleanValue : null;
                    predicate = t => (bool?)(propertyAccessor.Call()(t) ?? null) == booleanQuery;
                }
                else
                    predicate = t => ((string)(propertyAccessor.Call()(t) ?? "")).ToLower() == filterQuery.ToLower();

                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.NotEquals:
                if (propertyType == typeof(int))
                {
                    int? intQuery = int.TryParse(filterQuery, out var intValue) ? intValue : null;
                    predicate = t => (int?)(propertyAccessor.Call()(t) ?? null) != intQuery;
                }
                else if (propertyType == typeof(long))
                {
                    long? longQuery = long.TryParse(filterQuery, out var longValue) ? longValue : null;
                    predicate = t => (long?)(propertyAccessor.Call()(t) ?? null) != longQuery;
                }
                else if (propertyType == typeof(DateOnly)) {
                    DateOnly? dateQuery = DateOnly.TryParse(filterQuery, out var dateValue) ? dateValue : null;
                    predicate = t => (DateOnly?)(propertyAccessor.Call()(t) ?? null) != dateQuery;
                }
                else if (propertyType == typeof(TimeOnly))
                {
                    TimeOnly? timeQuery = TimeOnly.TryParse(filterQuery, out var timeValue) ? timeValue : null;
                    predicate = t => (TimeOnly?)(propertyAccessor.Call()(t) ?? null) != timeQuery;
                }
                else if (propertyType == typeof(DateTime))
                {
                    DateTime? dateTimeQuery = DateTime.TryParse(filterQuery, out var dateTimeValue) ? dateTimeValue : null;
                    predicate = t => (DateTime?)(propertyAccessor.Call()(t) ?? null) != dateTimeQuery;
                }
                else if (propertyType == typeof(bool))
                {
                    bool? booleanQuery = bool.TryParse(filterQuery, out var booleanValue) ? booleanValue : null;
                    predicate = t => (bool?)(propertyAccessor.Call()(t) ?? null) != booleanQuery;
                }
                else
                    predicate = t => ((string)(propertyAccessor.Call()(t) ?? "")).ToLower() != filterQuery.ToLower();

                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.GreaterThan when double.TryParse(filterQuery, out var number):
                predicate = t => (double)(propertyAccessor.Call()(t) ?? 0) > number;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.GreaterThan:
                return queryable;
            case FilterType.GreaterThanOrEqual when double.TryParse(filterQuery, out var number):
                predicate = t => (double)(propertyAccessor.Call()(t) ?? 0) >= number;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.GreaterThanOrEqual:
                return queryable;
            case FilterType.LessThan when double.TryParse(filterQuery, out var number):
                predicate = t => (double)(propertyAccessor.Call()(t) ?? 0) < number;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.LessThan:
                return queryable;
            case FilterType.LessThanOrEqual when double.TryParse(filterQuery, out var number):
                predicate = t => (double)(propertyAccessor.Call()(t) ?? 0) <= number;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.LessThanOrEqual:
                return queryable;
            case FilterType.In:
            {
                var split = filterQuery.Split(',');
                if (split.Length == 0) return queryable;
                split = split.Select(x => x.Trim()).ToArray();
                predicate = t => split.Contains((string)(propertyAccessor.Call()(t) ?? ""));
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            }
            case FilterType.NotIn:
            {
                var split = filterQuery.Split(',');
                if (split.Length == 0) return queryable;
                split = split.Select(x => x.Trim()).ToArray();
                predicate = t => !split.Contains((string)(propertyAccessor.Call()(t) ?? ""));
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            }
            case FilterType.Between:
            {
                var split = filterQuery.Split(',');
                if (split.Length != 2) return queryable;

                if (double.TryParse(split[0].Trim(), out var first) && double.TryParse(split[1].Trim(), out var second))
                {
                    predicate = t => (double)(propertyAccessor.Call()(t) ?? 0) >= first && (double)(propertyAccessor.Call()(t) ?? 0) <= second;
                    queryable = queryable.Where(predicate.AsTranslatableExpression());
                }
                else return queryable;
                break;
            }
            case FilterType.NotBetween:
            {
                var split = filterQuery.Split(',');
                if (split.Length != 2) return queryable;

                if (double.TryParse(split[0].Trim(), out var first) && double.TryParse(split[1].Trim(), out var second))
                {
                    predicate = t => (double)(propertyAccessor.Call()(t) ?? 0) < first || (double)(propertyAccessor.Call()(t) ?? 0) > second;
                    queryable = queryable.Where(predicate.AsTranslatableExpression());
                }
                else return queryable;
                break;
            }
            case FilterType.IsTrue:
                predicate = t => ((bool?)propertyAccessor.Call()(t) ?? null) == true;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.IsFalse:
                predicate = t => ((bool?)propertyAccessor.Call()(t) ?? null) == false;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.IsTrueOrNull:
                predicate = t => ((bool?)propertyAccessor.Call()(t) ?? true) == true;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.IsFalseOrNull:
                predicate = t => ((bool?)propertyAccessor.Call()(t) ?? false) == false;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.IsNull:
                predicate = t => propertyAccessor.Call()(t) == null;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            case FilterType.IsNotNull:
                predicate = t => propertyAccessor.Call()(t) != null;
                queryable = queryable.Where(predicate.AsTranslatableExpression());
                break;
            default:
                throw new FilterNotSupportedException(filterType.ToString());
        }

        return queryable;
    }
    
    private class FilterNotSupportedException(string filter) : Exception("The filter " + filter + " is not supported.");
}