namespace ProjectMarworyn.Core.Configuration
{
    internal static class SimulationConstants
    {
        //Used for duration-style counters (fertility cooldown) - a fixed 365 days per year.
        //The simulation calendar itself keeps real leap years; aging runs on birthdays against it
        public const int DaysPerYear = 365;
    }
}
