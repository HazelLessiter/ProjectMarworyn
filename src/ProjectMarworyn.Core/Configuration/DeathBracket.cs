namespace ProjectMarworyn.Core.Configuration
{
    public class DeathBracket
    {
        public int? MaxAge { get; set; }//Inclusive upper age bound. Null = no upper bound (the catch-all bracket, must be last)
        public double DailyDeathChance { get; set; }//In %. Rolled per person per simulated day
    }
}
