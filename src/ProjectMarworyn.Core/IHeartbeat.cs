namespace ProjectMarworyn.Core
{
    public interface IHeartbeat
    {
        void Start();
        void Stop();
        void Tick();
        void Reset();
        DateTime GetCurrentTime();
    }
}