using FilterSharp.Models;

namespace FilterSharp;

internal class Test(string name, string address)
{
    public string Name { get; } = name;
    public string Address { get; } = address;
}

public class Program
{
    # region This should be defined in a configuration file or somewhere it's easier to manage
    private static readonly List<FilterRule<Test>> RuleDescriptors =
    [
        new("Name", typeof(string), x => x.Name),
        new("Address", typeof(string), x => x.Address)
    ];
    #endregion
    public static void Main()
    {
        # region This comes from API
        var filterDescriptors = new List<FilterDescriptor>
        {
            new()
            {
                By = "Name",
                Filter = FilterType.Contains,
                Query = "John",
                Order = SortType.Ascending
            },
            new()
            {
                By = "Address",
                Filter = FilterType.Contains,
                Query = "123",
                Order = SortType.Descending
            }
        };
        #endregion

        #region Using a dummy queryable for now but this should be replaced with actual data source
        var queryable = new List<Test>
        {
            new("John Doe", "123 Main"),
            new("Jane Doe", "456 Main"),
            new("John Smith", "789 Main"),
            new("Albert Einstein", "123 Main"),
            new("Alan Turing", "456 Main"),
            new("John Nash", "789 Main"),
            new("Issac Newton", "123 Main")
        }.AsQueryable();
        #endregion
        
        var filtered = queryable.Filter(filterDescriptors, RuleDescriptors);
        foreach (var item in filtered)
        {
            Console.WriteLine($"{item.Name} - {item.Address}");
        }
    }
    
    #region This should be used to render the filters in the UI 
    public IList<AllowedFilters> GetAllowedFiltersForTestEntity()
    {
        return RuleDescriptors.Select(x => new AllowedFilters(x.Name, FilterSharp.GetValidFiltersForType(x.DataType))).ToList();
    }
    #endregion
}