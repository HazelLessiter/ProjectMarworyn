namespace ProjectMarworyn.Core.Configuration
{
    public class AppSettings
    {
        public int Delay { get; set; }
        public string InitialPeopleFilePath { get; set; }
        public string SeedWordFilePath { get; set; }
        public TimeSpan DayDuration { get; set; }
        public int FertilityCooldownYears { get; set; }//True elapsed years (was 3 under the old 1-based DateTime.Year representation; 2 preserves the same 730-day cooldown)
        public double TransgenderProbability { get; set; }//In %. Chance of a binary gender flip. Default 0.2% = trans man 0.10% + trans woman 0.10% per ONS census 2021 (https://www.ons.gov.uk/peoplepopulationandcommunity/culturalidentity/genderidentity/bulletins/genderidentityenglandandwales/census2021)
        //Note: the census's remaining 0.24% (no specific identity given) and 0.04% (all other identities) are not modelled
        public double NonBinaryProbability { get; set; }//In %. Default 0.06% who identified as non-binary (same census source). Rolled independently of TransgenderProbability, with non-binary taking precedence
    }
}