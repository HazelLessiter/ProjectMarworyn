using ProjectMarworyn.Core.Generators;

namespace ProjectMarworyn.UnitTests
{
    public class DiceGeneratorTests
    {
        private readonly DiceGenerator _diceGenerator;

        public DiceGeneratorTests()
        {
            _diceGenerator = new DiceGenerator();
        }

        [Fact]
        public void Create_DatesThatWouldHaveCollidedUnderOldFormula_ProduceDifferentSequences()
        {
            // Day=1,Month=12 and Day=12,Month=1 both summed to 13 under the old Day+Month+Year formula
            var diceA = _diceGenerator.Create(0,
                new DateTime(2000, 12, 1));
            var diceB = _diceGenerator.Create(0,
                new DateTime(2000, 1, 12));

            Assert.NotEqual(diceA.Next(),
                diceB.Next());
        }

        [Fact]
        public void Create_DatesThatWouldHaveCollidedUnderMonthTimesTenFormula_ProduceDifferentSequences()
        {
            // Month=1,Day=21 / Month=2,Day=11 / Month=3,Day=1 all give Month*10+Day=31, colliding under a Year*100+Month*10+Day encoding
            var diceA = _diceGenerator.Create(0,
                new DateTime(1, 1, 21));
            var diceB = _diceGenerator.Create(0,
                new DateTime(1, 2, 11));
            var diceC = _diceGenerator.Create(0,
                new DateTime(1, 3, 1));

            var resultA = diceA.Next();
            var resultB = diceB.Next();
            var resultC = diceC.Next();

            Assert.NotEqual(resultA, resultB);
            Assert.NotEqual(resultB, resultC);
            Assert.NotEqual(resultA, resultC);
        }

        [Fact]
        public void Create_DifferentDays_ProducesDifferentSequences()
        {
            var diceA = _diceGenerator.Create(0,
                new DateTime(1, 1, 1));
            var diceB = _diceGenerator.Create(0,
                new DateTime(1, 1, 2));

            Assert.NotEqual(diceA.Next(),
                diceB.Next());
        }

        [Fact]
        public void Create_SameWorldSeedAndDate_ProducesSameSequence()
        {
            var diceA = _diceGenerator.Create(42,
                new DateTime(5, 6, 15));
            var diceB = _diceGenerator.Create(42,
                new DateTime(5, 6, 15));

            Assert.Equal(diceA.Next(),
                diceB.Next());
            Assert.Equal(diceA.Next(),
                diceB.Next());
            Assert.Equal(diceA.Next(),
                diceB.Next());
        }

        [Fact]
        public void Create_WithWorldSeedOnly_SameSeedProducesSameSequence()
        {
            var diceA = _diceGenerator.Create(7);
            var diceB = _diceGenerator.Create(7);

            Assert.Equal(diceA.Next(),
                diceB.Next());
        }

        [Fact]
        public void Next_ReturnsValueWithinRequestedRange()
        {
            var dice = _diceGenerator.Create(0);

            var result = _diceGenerator.Next(dice,
                5,
                10);

            Assert.InRange(result, 5, 9);
        }

        [Fact]
        public void NextDouble_ReturnsValueWithinZeroToOneRange()
        {
            var dice = _diceGenerator.Create(0);

            var result = _diceGenerator.NextDouble(dice);

            Assert.InRange(result, 0.0, 1.0);
        }
    }
}