namespace ProjectMarworyn.Core.Configuration
{
    public class AppSettings
    {
        public int Delay { get; set; }
        public string InitialPeopleFilePath { get; set; }
        public string SeedWordFilePath { get; set; }
        public TimeSpan DayDuration { get; set; }
        public int FertilityCooldownYears { get; set; }
        public double TransgenderProbability { get; set; }//In %. Default 0.5% based on ONS census 2021 (https://www.ons.gov.uk/peoplepopulationandcommunity/culturalidentity/genderidentity/bulletins/genderidentityenglandandwales/census2021)
        public double NonBinaryProbability { get; set; }//In %. A slice within TransgenderProbability, not additional to it - ONS's 0.5% umbrella includes the 0.06% who identified as non-binary (same census source)
    }
}