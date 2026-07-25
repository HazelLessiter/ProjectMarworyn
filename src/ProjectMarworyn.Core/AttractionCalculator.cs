using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.Core
{
    internal class AttractionCalculator : IAttractionCalculator
    {
        //Aromantic and aroace people never form pairs; WillPair is the separate
        //orientation-independent opt-out rolled at birth
        public bool CanPair(Person person)
        {
            return person.Orientation != Orientation.Aromantic &&
                person.Orientation != Orientation.Aroace;
        }

        public bool AreMutuallyAttracted(Person personA,
            Person personB)
        {
            return IsAttractedTo(personA,
                    personB) &&
                IsAttractedTo(personB,
                    personA);
        }

        //Attraction runs on the candidate's gender, never their biosex - reproduction is
        //gated separately on biosex in GenerateChildren. Heterosexual means any gender
        //different from one's own, so a heterosexual non-binary person is attracted to
        //both binary genders. Asexual people pair (romance, not sex) with any gender
        public bool IsAttractedTo(Person person,
            Person candidate)
        {
            return person.Orientation switch
            {
                Orientation.Heterosexual => candidate.Gender != person.Gender,
                Orientation.Homosexual => candidate.Gender == person.Gender,
                Orientation.Bisexual => candidate.Gender == Gender.Female ||
                    candidate.Gender == Gender.Male,
                Orientation.Pansexual => true,
                Orientation.Asexual => true,
                _ => false
            };
        }
    }
}