using System.Linq.Expressions;

namespace FilterSharp.Models;

public class FilterRule<T>(string name, Type dataType, Expression<Func<T, object?>> accessor)
{
    public string Name { get; } = name;
    public Type DataType { get; } = dataType;
    public Expression<Func<T, object?>> Accessor { get; } = accessor;
}