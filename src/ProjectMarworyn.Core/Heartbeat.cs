using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    public class Heartbeat : IHeartbeat
    {
        private SimulationClock _simulationClock;

        public Heartbeat(SimulationClock simulationClock)
        {
            _simulationClock = simulationClock;
        }

        public void Start()
        {
            if (!_simulationClock.IsRunning)
            {
                _simulationClock.StartTime = new DateTime(1, 1, 1);
                _simulationClock.IsRunning = true;

                _simulationClock.SimulationTime = _simulationClock.StartTime;
            }
        }

        public void Stop()
        {
            if (_simulationClock.IsRunning)
            {
                _simulationClock.IsRunning = false;

                _simulationClock.EndTime = _simulationClock.SimulationTime;
            }
        }

        public void Tick()
        {
            if (!_simulationClock.IsRunning)
            {
                return;
            }

            _simulationClock.TickCount += 1;
            _simulationClock.SimulationTime = _simulationClock.SimulationTime.AddDays(1);//TODO: Make configurable based on simulation speed setting
            _simulationClock.ElapsedTime = _simulationClock.SimulationTime - _simulationClock.StartTime;
        }

        public void Reset()
        {
            _simulationClock.TickCount = 0;
            _simulationClock.IsRunning = false;
            _simulationClock.StartTime = new DateTime(1, 1, 1);
            _simulationClock.SimulationTime = _simulationClock.StartTime;
        }

        public DateTime GetCurrentTime()
        {
            return _simulationClock.SimulationTime;
        }
    }
}