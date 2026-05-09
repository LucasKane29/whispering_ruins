public class CountdownTimer : Timer
{
    public CountdownTimer(float initialTime) : base(initialTime) { }

    public override void Tick(float deltaTime)
    {
        if(IsRunning && Time > 0)
        {
            Time -= deltaTime;
        }

        if (IsRunning && Time <= 0)
        {
            Stop();
        }
    }

    public void Reset() => Time = initialTime;

    public void Reset(float newTime)
    {
        initialTime = newTime;
        Reset();
    }
}