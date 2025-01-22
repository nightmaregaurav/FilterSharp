namespace FilterSharp.Models;

public class FilterDescriptor
{
    public required string By { get; set; }
    public FilterType? Filter { get; set; }
    public string? Query { get; set; }
    public SortType? Order { get; set; }
}