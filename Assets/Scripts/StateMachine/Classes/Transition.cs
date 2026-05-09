public class Transition: ITransition
{
    public IState ToState { get; private set; }
    public IPredicate Condition { get; private set; }
    public Transition(IState toState, IPredicate condition)
    {
        ToState = toState;
        Condition = condition;
    }
}