namespace ProjectMarworyn.Models
{
    internal class SimulationClock
    {
        public long TickCount { get; set; }
        public bool IsRunning { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime SimulationTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}