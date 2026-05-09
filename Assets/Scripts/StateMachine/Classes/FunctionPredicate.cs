using System;

public class FunctionPredicate : IPredicate
{
    readonly Func<bool> _predicate;

    public FunctionPredicate(Func<bool> predicate)
    {
        _predicate = predicate;
    }

    public bool Evaluate() => _predicate.Invoke();
}