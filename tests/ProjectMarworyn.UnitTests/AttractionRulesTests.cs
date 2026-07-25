using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.UnitTests
{
    public class AttractionRulesTests
    {
        // The full policy matrix: attraction is decided by own orientation + own gender
        // against the candidate's gender, never biosex.
        [Theory]
        // Heterosexual: any gender different from one's own
        [InlineData(Orientation.Heterosexual, Gender.Female, Gender.Female, false)]
        [InlineData(Orientation.Heterosexual, Gender.Female, Gender.Male, true)]
        [InlineData(Orientation.Heterosexual, Gender.Female, Gender.NonBinary, true)]
        [InlineData(Orientation.Heterosexual, Gender.NonBinary, Gender.Female, true)]
        [InlineData(Orientation.Heterosexual, Gender.NonBinary, Gender.Male, true)]
        [InlineData(Orientation.Heterosexual, Gender.NonBinary, Gender.NonBinary, false)]
        // Homosexual: same gender only
        [InlineData(Orientation.Homosexual, Gender.Male, Gender.Male, true)]
        [InlineData(Orientation.Homosexual, Gender.Male, Gender.Female, false)]
        [InlineData(Orientation.Homosexual, Gender.Male, Gender.NonBinary, false)]
        [InlineData(Orientation.Homosexual, Gender.NonBinary, Gender.NonBinary, true)]
        [InlineData(Orientation.Homosexual, Gender.NonBinary, Gender.Male, false)]
        // Bisexual: both binary genders - reaching non-binary people is what pansexual adds
        [InlineData(Orientation.Bisexual, Gender.Female, Gender.Female, true)]
        [InlineData(Orientation.Bisexual, Gender.Female, Gender.Male, true)]
        [InlineData(Orientation.Bisexual, Gender.Female, Gender.NonBinary, false)]
        // Pansexual: every gender
        [InlineData(Orientation.Pansexual, Gender.Male, Gender.Female, true)]
        [InlineData(Orientation.Pansexual, Gender.Male, Gender.Male, true)]
        [InlineData(Orientation.Pansexual, Gender.Male, Gender.NonBinary, true)]
        // Asexual: pairs with any gender - romance, not sex; reproduction is gated in GenerateChildren
        [InlineData(Orientation.Asexual, Gender.Female, Gender.Male, true)]
        [InlineData(Orientation.Asexual, Gender.Female, Gender.Female, true)]
        [InlineData(Orientation.Asexual, Gender.Female, Gender.NonBinary, true)]
        // Aromantic and aroace: no romantic pairing at all
        [InlineData(Orientation.Aromantic, Gender.Female, Gender.Male, false)]
        [InlineData(Orientation.Aroace, Gender.Male, Gender.Female, false)]
        public void IsAttractedTo_OrientationGenderMatrix_ReturnsExpected(Orientation orientation,
            Gender ownGender,
            Gender candidateGender,
            bool expected)
        {
            var person = CreatePerson(orientation,
                ownGender);
            var candidate = CreatePerson(Orientation.Pansexual,
                candidateGender);

            Assert.Equal(expected, AttractionRules.IsAttractedTo(person, candidate));
        }

        [Fact]
        public void AreMutuallyAttracted_OneSidedAttraction_ReturnsFalse()
        {
            // The heterosexual woman is attracted to the man, but he is homosexual
            var woman = CreatePerson(Orientation.Heterosexual,
                Gender.Female);
            var man = CreatePerson(Orientation.Homosexual,
                Gender.Male);

            Assert.False(AttractionRules.AreMutuallyAttracted(woman, man));
            Assert.False(AttractionRules.AreMutuallyAttracted(man, woman));
        }

        [Fact]
        public void AreMutuallyAttracted_BothAttracted_ReturnsTrue()
        {
            var woman = CreatePerson(Orientation.Heterosexual,
                Gender.Female);
            var man = CreatePerson(Orientation.Heterosexual,
                Gender.Male);

            Assert.True(AttractionRules.AreMutuallyAttracted(woman, man));
        }

        [Theory]
        [InlineData(Orientation.Heterosexual, true)]
        [InlineData(Orientation.Homosexual, true)]
        [InlineData(Orientation.Bisexual, true)]
        [InlineData(Orientation.Pansexual, true)]
        [InlineData(Orientation.Asexual, true)]
        [InlineData(Orientation.Aromantic, false)]
        [InlineData(Orientation.Aroace, false)]
        public void CanPair_PerOrientation_ReturnsExpected(Orientation orientation,
            bool expected)
        {
            var person = CreatePerson(orientation,
                Gender.Female);

            Assert.Equal(expected, AttractionRules.CanPair(person));
        }

        private static Person CreatePerson(Orientation orientation,
            Gender gender)
        {
            return new Person
            {
                Id = 1,
                Name = new Name { FullName = "Test" },
                Age = 25,
                Gender = gender,
                Orientation = orientation,
                WillPair = true,
                IsAlive = true
            };
        }
    }
}