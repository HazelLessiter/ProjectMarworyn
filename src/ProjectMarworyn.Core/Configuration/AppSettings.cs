namespace ProjectMarworyn.Core.Configuration
{
    public class AppSettings
    {
        public string InitialPeopleFilePath { get; set; }
        public string SeedWordFilePath { get; set; }
        public TimeSpan DayDuration { get; set; }
        public int FertilityCooldownYears { get; set; }//True elapsed years (was 3 under the old 1-based DateTime.Year representation; 2 preserves the same 730-day cooldown)
        public double TransgenderProbability { get; set; }//In %. Chance of a binary gender flip. Default 0.2% = trans man 0.10% + trans woman 0.10% per ONS census 2021 (https://www.ons.gov.uk/peoplepopulationandcommunity/culturalidentity/genderidentity/bulletins/genderidentityenglandandwales/census2021)
        //Note: the census's remaining 0.24% (no specific identity given) and 0.04% (all other identities) are not modelled
        public List<DeathBracket> DeathBrackets { get; set; }//Ordered ascending by MaxAge - the first bracket the age fits wins
        public List<OrientationWeight> OrientationWeights { get; set; }//One entry per Orientation value, weights sum to 100. Defaults from ONS census 2021 (https://www.ons.gov.uk/peoplepopulationandcommunity/culturalidentity/sexuality/bulletins/sexualorientationenglandandwales/census2021):
        //Gay/Lesbian 1.5%, Bisexual 1.3%, Pansexual 0.23% and Asexual 0.06% from the "Other" write-ins; Heterosexual takes the remainder (non-responses folded in, as with BiosexModifier)
        //Aromantic and Aroace are invented placeholders - no census records them; pending research into EU figures
        public double NeverPairProbability { get; set; }//In %. Chance a newborn never pairs regardless of orientation (WillPair = false). Balance placeholder - no census figure for this; initial people carry WillPair explicitly in InitialPeople.json instead of rolling
        public double IntersexFertileProbability { get; set; }//In %. Chance an intersex newborn is fertile. Modelling estimate, not census-backed - fertility varies widely by condition (many DSDs cause infertility, some don't) and no clean published split exists; revisit if better figures surface
        public double NonBinaryProbability { get; set; }//In %. Default 0.06% who identified as non-binary (same census source). Rolled independently of TransgenderProbability, with non-binary taking precedence
    }
}