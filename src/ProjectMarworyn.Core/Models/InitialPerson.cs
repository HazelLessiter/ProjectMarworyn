using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.Core.Models
{
    public class InitialPerson
    {
        public string FullName { get; set; }
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public Biosex Biosex { get; set; }
        public Gender Gender { get; set; }
        public Orientation Orientation { get; set; }//Defaults to Heterosexual when absent from the data file
        public bool WillPair { get; set; } = true;//Opt-out flag: only the never-pairing people carry this in the data file
    }
}