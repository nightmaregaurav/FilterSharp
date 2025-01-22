using System.Linq.Expressions;
using System.Reflection;

namespace FilterSharp;

public static class ExpressionVisitorWrapper
{
    public static TFunc Call<TFunc>(this Expression<TFunc> _) => throw new InvalidOperationException("This method should never be called. It is a marker for constructing filter expressions.");
    public static Expression<TFunc> AsTranslatableExpression<TFunc>(this Expression<TFunc> expression)
    {
        var visitor = new SubstituteExpressionCallVisitor();
        return (Expression<TFunc>)visitor.Visit(expression);
    }
}

public class SubstituteExpressionCallVisitor : ExpressionVisitor
{
    private readonly MethodInfo _markerDescription;

    public SubstituteExpressionCallVisitor() => _markerDescription = typeof(ExpressionVisitorWrapper).GetMethod(nameof(ExpressionVisitorWrapper.Call))!.GetGenericMethodDefinition();

    protected override Expression VisitInvocation(InvocationExpression node)
    {
        if (node.Expression.NodeType != ExpressionType.Call || !IsMarker((MethodCallExpression)node.Expression)) return base.VisitInvocation(node);
        var parameterReplacer = new SubstituteParameterVisitor(node.Arguments.ToArray(),
            Unwrap((MethodCallExpression)node.Expression));
        var target = parameterReplacer.Replace();
        return Visit(target);
    }

    private LambdaExpression Unwrap(MethodCallExpression node)
    {
        var target = node.Arguments[0];
        return (LambdaExpression)Expression.Lambda(target).Compile().DynamicInvoke()!;
    }

    private bool IsMarker(MethodCallExpression node) => node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == _markerDescription;
}

public class SubstituteParameterVisitor : ExpressionVisitor
{
    private readonly LambdaExpression _expressionToVisit;
    private readonly Dictionary<ParameterExpression, Expression> _substitutionByParameter;

    public SubstituteParameterVisitor(Expression[] parameterSubstitutions, LambdaExpression expressionToVisit)
    {
        _expressionToVisit = expressionToVisit;
        _substitutionByParameter = expressionToVisit.Parameters
            .Select(
                (parameter, index) =>
                    new { Parameter = parameter, Index = index })
            .ToDictionary(pair => pair.Parameter,
                pair => parameterSubstitutions[pair.Index]);
    }

    public Expression Replace() => Visit(_expressionToVisit.Body);
    protected override Expression VisitParameter(ParameterExpression node) => _substitutionByParameter.TryGetValue(node, out var substitution) ? Visit(substitution) : base.VisitParameter(node);
}