using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class HeartbeatTests
    {
        private readonly Heartbeat _heartbeat;
        private readonly MockConsoleService _mockConsoleService;
        private readonly SimulationClock _simulationClock;

        public HeartbeatTests()
        {
            _mockConsoleService = new MockConsoleService();
            _simulationClock = new SimulationClock();
            _heartbeat = new Heartbeat(_mockConsoleService, _simulationClock);
        }

        [Fact]
        public void Constructor_InitializesSimulationClock_ZeroTickCount()
        {
            var consoleService = new MockConsoleService();
            var clock = new SimulationClock();
            var heartbeat = new Heartbeat(consoleService,
                clock);

            Assert.Equal(0, clock.TickCount);
        }

        [Fact]
        public void Constructor_InitializesSimulationClock_IsRunningFalse()
        {
            var consoleService = new MockConsoleService();
            var clock = new SimulationClock();
            var heartbeat = new Heartbeat(consoleService,
                clock);

            Assert.False(clock.IsRunning);
        }

        [Fact]
        public void Start_IsRunningTrue()
        {
            _heartbeat.Start();

            Assert.True(_simulationClock.IsRunning);
        }

        [Fact]
        public void Start_StartTimeYear0001()
        {
            _heartbeat.Start();

            Assert.Equal(new DateTime(1, 1, 1), _simulationClock.StartTime);
        }

        [Fact]
        public void Start_SimulationTimeStartTime()
        {
            _heartbeat.Start();

            Assert.Equal(_simulationClock.StartTime, _simulationClock.SimulationTime);
        }

        [Fact]
        public void Start_AlreadyRunning_DoNotRestartClock()
        {
            _heartbeat.Start();
            var originalStartTime = _simulationClock.StartTime;
            
            _heartbeat.Start();

            Assert.Equal(originalStartTime, _simulationClock.StartTime);
        }

        [Fact]
        public void Stop_IsRunningFalse()
        {
            _heartbeat.Start();
            
            _heartbeat.Stop();

            Assert.False(_simulationClock.IsRunning);
        }

        [Fact]
        public void Stop_SetEndTime()
        {
            _heartbeat.Start();
            
            _heartbeat.Stop();

            Assert.Equal(_simulationClock.SimulationTime, _simulationClock.EndTime);
        }

        [Fact]
        public void Tick_IncrementTickCount()
        {
            _heartbeat.Start();
            
            _heartbeat.Tick();

            Assert.Equal(1, _simulationClock.TickCount);
        }

        [Fact]
        public void Tick_MultipleTimes_IncrementTickCount()
        {
            _heartbeat.Start();
            
            _heartbeat.Tick();
            _heartbeat.Tick();
            _heartbeat.Tick();

            Assert.Equal(3, _simulationClock.TickCount);
        }

        [Fact]
        public void Tick_AdvanceSimulationTimeByOneDay()
        {
            _heartbeat.Start();
            var initialTime = _simulationClock.SimulationTime;
            
            _heartbeat.Tick();

            Assert.Equal(initialTime.AddDays(1), _simulationClock.SimulationTime);
        }

        [Fact]
        public void Tick_UpdateElapsedTime()
        {
            _heartbeat.Start();
            
            _heartbeat.Tick();

            Assert.Equal(TimeSpan.FromDays(1), _simulationClock.ElapsedTime);
        }

        [Fact]
        public void Tick_MultipleTimes_IncreaseElapsedTime()
        {
            _heartbeat.Start();
            
            _heartbeat.Tick();
            _heartbeat.Tick();
            _heartbeat.Tick();

            Assert.Equal(TimeSpan.FromDays(3), _simulationClock.ElapsedTime);
        }

        [Fact]
        public void Tick_NotRunning_DoNotIncrementTickCount()
        {
            _heartbeat.Tick();

            Assert.Equal(0, _simulationClock.TickCount);
        }

        [Fact]
        public void Reset_TickCountZero()
        {
            _heartbeat.Start();
            _heartbeat.Tick();
            _heartbeat.Tick();
            
            _heartbeat.Reset();

            Assert.Equal(0, _simulationClock.TickCount);
        }

        [Fact]
        public void Reset_IsRunningFalse()
        {
            _heartbeat.Start();
            
            _heartbeat.Reset();

            Assert.False(_simulationClock.IsRunning);
        }

        [Fact]
        public void Reset_ResetStartTime()
        {
            _heartbeat.Start();
            _heartbeat.Tick();
            
            _heartbeat.Reset();

            Assert.Equal(new DateTime(1, 1, 1), _simulationClock.StartTime);
        }

        [Fact]
        public void Reset_ResetSimulationTime()
        {
            _heartbeat.Start();
            _heartbeat.Tick();
            
            _heartbeat.Reset();

            Assert.Equal(new DateTime(1, 1, 1), _simulationClock.SimulationTime);
        }

        [Fact]
        public void SimulationClock_IsSingleton_SharedAcrossServices()
        {
            var clock = new SimulationClock();
            var heartbeat1 = new Heartbeat(_mockConsoleService,
                clock);
            var heartbeat2 = new Heartbeat(_mockConsoleService,
                clock);

            heartbeat1.Start();
            heartbeat1.Tick();

            Assert.Equal(1, clock.TickCount);
        }

        [Fact]
        public void Tick_365Times_AdvanceSimulationTimeByOneYear()
        {
            _heartbeat.Start();
            var startTime = _simulationClock.SimulationTime;

            for (int i = 0; i < 365; i++)
            {
                _heartbeat.Tick();
            }

            Assert.Equal(startTime.AddDays(365), _simulationClock.SimulationTime);
            Assert.Equal(365, _simulationClock.TickCount);
        }

        [Fact]
        public void Start_Stop_Start_ContinueFromPreviousState()
        {
            _heartbeat.Start();
            _heartbeat.Tick();
            var tickCountAfterFirstRun = _simulationClock.TickCount;
            
            _heartbeat.Stop();
            _heartbeat.Start();
            _heartbeat.Tick();

            Assert.Equal(tickCountAfterFirstRun + 1, _simulationClock.TickCount);
        }

        [Fact]
        public void ElapsedTime_SimulationTimeDifference()
        {
            _heartbeat.Start();
            
            _heartbeat.Tick();
            _heartbeat.Tick();
            _heartbeat.Tick();

            var expectedElapsed = _simulationClock.SimulationTime - _simulationClock.StartTime;

            Assert.Equal(expectedElapsed, _simulationClock.ElapsedTime);
        }
    }
}