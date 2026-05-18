using ProjectMarworyn.Models;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class Heartbeat : IHeartbeat
    {
        private readonly IConsoleService _consoleService;
        private SimulationClock _simulationClock;

        public Heartbeat(IConsoleService consoleService,
            SimulationClock simulationClock)
        {
            _consoleService = consoleService;
            _simulationClock = simulationClock;
        }

        public void Start()
        {
            if (!_simulationClock.IsRunning)
            {
                _simulationClock.StartTime = new DateTime(1, 1, 1);
                _simulationClock.IsRunning = true;
                _consoleService.WriteLine($"[SimulationClock] Started at {_simulationClock.StartTime:yyyy-MM-dd}");

                _simulationClock.SimulationTime = _simulationClock.StartTime;
            }
        }

        public void Stop()
        {
            if (_simulationClock.IsRunning)
            {
                _simulationClock.IsRunning = false;
                _consoleService.WriteLine($"[SimulationClock] Stopped at day {_simulationClock.TickCount}");

                _simulationClock.EndTime = _simulationClock.SimulationTime;
            }
        }

        public void Tick()
        {
            if (!_simulationClock.IsRunning)
            {
                _consoleService.WriteLine("[SimulationClock] Warning: Tick called while clock is not running");
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
            _consoleService.WriteLine("[SimulationClock] Reset");
            _simulationClock.StartTime = new DateTime(1, 1, 1);
            _simulationClock.SimulationTime = _simulationClock.StartTime;
        }

        public DateTime GetCurrentTime()
        {
            return _simulationClock.SimulationTime;
        }
    }
}