namespace ProjectMarworyn.Core.Configuration
{
    public class AppSettings
    {
        public int Delay { get; set; }
        public string InitialPeopleFilePath { get; set; }
        public string SeedWordFilePath { get; set; }
        public TimeSpan DayDuration { get; set; }
    }
}