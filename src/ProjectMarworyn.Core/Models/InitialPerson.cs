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
    }
}