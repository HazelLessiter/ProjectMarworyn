using ProjectMarworyn.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectMarworyn.Models
{
    internal class InitialPerson
    {
        public string FullName { get; set; }
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public Gender Gender { get; set; }
    }
}
