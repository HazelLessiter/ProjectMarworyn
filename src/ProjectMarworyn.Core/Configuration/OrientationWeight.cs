using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.Core.Configuration
{
    public class OrientationWeight
    {
        public Orientation Orientation { get; set; }
        public double Weight { get; set; }//In %. All weights together must cover every Orientation value and sum to 100
    }
}