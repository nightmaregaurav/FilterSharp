namespace FilterSharp.Models
{
    public class AllowedFilters(string name, IEnumerable<FilterType> validFiltersForType)
    {
        public string Name { get; } = name;
        public IEnumerable<FilterType> ValidFiltersForType { get; } = validFiltersForType;
    }
}