namespace ProjectMarworyn.Core
{
    internal interface IHeartbeat
    {
        void Start();
        void Stop();
        void Tick();
        void Reset();
        DateTime GetCurrentTime();
    }
}