namespace ProjectMarworyn.Models.Enums
{
    internal enum BiosexModifier//100 = 1% 
    {
        Female = 5015,//51.0% - (1.7%/2) = 50.15%
        Male = 4815,//49.0% - (1.7%/2) = 48.15%
        Intersex = 170,//1.7% as per https://www.ohchr.org/en/sexual-orientation-and-gender-identity/intersex-people
        //Note: ONS census data from 2021 did not include intersex.
        //Male/female modifier is based on ONS data (https://www.ons.gov.uk/datasets/TS008/editions/2021/versions/4),
        //with the UN intersex % then evenly taken from male and female %s to arrive at a 100% stat
    }
}