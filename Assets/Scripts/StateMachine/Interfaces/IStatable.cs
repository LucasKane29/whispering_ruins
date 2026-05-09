public interface IStatable
{
    public void At(IState from, IState to, IPredicate condition);
    public void Any(IState to, IPredicate condition);
}
