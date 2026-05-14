using ProjectMarworyn.Models;
using ProjectMarworyn.Models.Enums;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class DeathEngineTests
    {
        private readonly MockOutputService _mockOutputService;

        public DeathEngineTests()
        {
            _mockOutputService = new MockOutputService();
        }

        private static Person CreatePerson(int age, bool isAlive = true) =>
            new Person
            {
                Id = 1,
                Name = new Name { FullName = "TestPerson" },
                Age = age,
                Gender = Gender.Male,
                IsAlive = isAlive
            };

        private static Generation CreateGeneration(int iteration = 1) =>
            new Generation { Iteration = iteration, People = new List<Person>() };

        private DeathEngine CreateEngine(double nextDoubleValue)
        {
            var mockDiceGenerator = new MockDiceGenerator(new ControlledRandom(nextDoubleValue));
            return new DeathEngine(_mockOutputService,
                mockDiceGenerator);
        }

        [Fact]
        public void ProcessDeaths_WithEmptyPeopleList_ReturnsEmptyLists()
        {
            var engine = CreateEngine(1.0);

            var currentGeneration = engine.ProcessDeaths(new List<Person>(),
                CreateGeneration(),
                0);

            Assert.Empty(currentGeneration.People);
        }

        [Fact]
        public void ProcessDeaths_WithEmptyPeopleList_PreservesIteration()
        {
            var engine = CreateEngine(1.0);

            var currentGeneration = engine.ProcessDeaths(new List<Person>(),
                CreateGeneration(5),
                0);

            Assert.Equal(5, currentGeneration.Iteration);
        }

        [Fact]
        public void ProcessDeaths_ReturnsNonNullLists()
        {
            var engine = CreateEngine(1.0);

            var currentGeneration = engine.ProcessDeaths(new List<Person>(),
                CreateGeneration(),
                0);

            Assert.NotNull(currentGeneration);
            Assert.NotNull(currentGeneration.People);
        }

        [Fact]
        public void ProcessDeaths_SkipsPeopleWithIsAliveFalse()
        {
            // 0.0 is below every bracket's threshold — any alive person would die,
            // which proves dead people are genuinely filtered before the death roll
            var engine = CreateEngine(0.0);
            var people = new List<Person> { CreatePerson(50, isAlive: false) };

            var result = engine.ProcessDeaths(people,
                CreateGeneration(),
                0);

            Assert.Empty(result.People);
            Assert.Empty(_mockOutputService.Messages);
        }

        [Fact]
        public void ProcessDeaths_MixedAliveAndDeadPeople_OnlyProcessesAlivePeople()
        {
            var engine = CreateEngine(1.0);
            var alivePerson = new Person { Id = 1, Name = new Name { FullName = "Alive" }, Age = 30, Gender = Gender.Male, IsAlive = true };
            var deadPerson = new Person { Id = 2, Name = new Name { FullName = "Dead" }, Age = 30, Gender = Gender.Female, IsAlive = false };
            var people = new List<Person> { alivePerson, deadPerson };

            var result = engine.ProcessDeaths(people,
                CreateGeneration(),
                0);

            Assert.Single(result.People);
            Assert.Contains(alivePerson, result.People);
        }

        [Fact]
        public void ProcessDeaths_WhenPersonDies_SetsIsAliveToFalse()
        {
            var engine = CreateEngine(0.0);
            var person = CreatePerson(50);

            engine.ProcessDeaths(new List<Person> { person },
                CreateGeneration(),
                0);

            Assert.False(person.IsAlive);
        }

        [Fact]
        public void ProcessDeaths_WhenPersonSurvives_IsAliveRemainsTrue()
        {
            var engine = CreateEngine(1.0);
            var person = CreatePerson(30);

            engine.ProcessDeaths(new List<Person> { person },
                CreateGeneration(),
                0);

            Assert.True(person.IsAlive);
        }

        [Fact]
        public void ProcessDeaths_SurvivorsListContainsOnlyAlivePeople()
        {
            var engine = CreateEngine(1.0);
            var people = new List<Person> { CreatePerson(10), CreatePerson(20), CreatePerson(30) };

            var result = engine.ProcessDeaths(people,
                CreateGeneration(),
                0);

            Assert.All(result.People, person => Assert.True(person.IsAlive));
        }

        [Fact]
        public void ProcessDeaths_SurvivingPeopleStoredInGeneration()
        {
            var engine = CreateEngine(1.0);
            var person1 = new Person { Id = 1, Name = new Name { FullName = "Survivor1" }, Age = 10, Gender = Gender.Female, IsAlive = true };
            var person2 = new Person { Id = 2, Name = new Name { FullName = "Survivor2" }, Age = 15, Gender = Gender.Male, IsAlive = true };
            var people = new List<Person> { person1, person2 };

            var currentGeneration = engine.ProcessDeaths(people,
                CreateGeneration(),
                0);

            Assert.Equal(2, currentGeneration.People.Count);
            Assert.All(currentGeneration.People, person => Assert.True(person.IsAlive));
        }

        [Fact]
        public void ProcessDeaths_PreservesGenerationIteration()
        {
            var engine = CreateEngine(1.0);

            var currentGeneration = engine.ProcessDeaths(new List<Person> { CreatePerson(20) },
                CreateGeneration(7),
                0);

            Assert.Equal(7, currentGeneration.Iteration);
        }

        [Fact]
        public void ProcessDeaths_OnDeath_WritesDeathMessageToConsole()
        {
            var engine = CreateEngine(0.0);

            engine.ProcessDeaths(new List<Person> { CreatePerson(50) },
                CreateGeneration(),
                0);

            Assert.NotEmpty(_mockOutputService.Messages);
        }

        [Fact]
        public void ProcessDeaths_OnSurvival_NoConsoleOutput()
        {
            var engine = CreateEngine(1.0);

            engine.ProcessDeaths(new List<Person> { CreatePerson(30) },
                CreateGeneration(),
                0);

            Assert.Empty(_mockOutputService.Messages);
        }

        [Fact]
        public void ProcessDeaths_DeathMessage_ContainsPersonNameAndAge()
        {
            var engine = CreateEngine(0.0);
            var person = new Person
            {
                Id = 1,
                Name = new Name { FullName = "John Smith" },
                Age = 55,
                Gender = Gender.Male,
                IsAlive = true
            };

            engine.ProcessDeaths(new List<Person> { person },
                CreateGeneration(),
                0);

            Assert.Single(_mockOutputService.Messages);
            Assert.Contains("John Smith", _mockOutputService.Messages[0]);
            Assert.Contains("55", _mockOutputService.Messages[0]);
        }

        // Verifies the exact death probability thresholds for all 11 age brackets.
        // Formula: deathChance = (int)deathModifier / 100.0; death when NextDouble() * 100 <= deathChance
        // Threshold (NextDouble boundary) = (int)deathModifier / 10000
        // Test values use a 10% margin either side of each threshold.
        [Theory]
        [InlineData(5,   0.0009,  true)]  // Zero   (0-9,   modifier  10): threshold 0.001
        [InlineData(5,   0.0011,  false)]
        [InlineData(15,  0.00009, true)]  // Ten    (10-19, modifier   1): threshold 0.0001
        [InlineData(15,  0.00011, false)]
        [InlineData(25,  0.00018, true)]  // Twenty (20-29, modifier   2): threshold 0.0002
        [InlineData(25,  0.00022, false)]
        [InlineData(35,  0.00045, true)]  // Thirty (30-39, modifier   5): threshold 0.0005
        [InlineData(35,  0.00055, false)]
        [InlineData(45,  0.0009,  true)]  // Fourty (40-49, modifier  10): threshold 0.001
        [InlineData(45,  0.0011,  false)]
        [InlineData(55,  0.00135, true)]  // Fifty  (50-59, modifier  15): threshold 0.0015
        [InlineData(55,  0.00165, false)]
        [InlineData(65,  0.0018,  true)]  // Sixty  (60-69, modifier  20): threshold 0.002
        [InlineData(65,  0.0022,  false)]
        [InlineData(75,  0.00225, true)]  // Seventy(70-79, modifier  25): threshold 0.0025
        [InlineData(75,  0.00275, false)]
        [InlineData(85,  0.0045,  true)]  // Eighty (80-89, modifier  50): threshold 0.005
        [InlineData(85,  0.0055,  false)]
        [InlineData(95,  0.009,   true)]  // Ninety (90-99, modifier 100): threshold 0.01
        [InlineData(95,  0.011,   false)]
        [InlineData(105, 0.0225,  true)]  // Hundred(100+,  modifier 250): threshold 0.025
        [InlineData(105, 0.0275,  false)]
        public void ProcessDeaths_DeathProbabilityThreshold_CorrectlyDeterminesOutcome(int age,
            double nextDoubleValue,
            bool expectsDeath)
        {
            var engine = CreateEngine(nextDoubleValue);

            var result = engine.ProcessDeaths(new List<Person> { CreatePerson(age) },
                CreateGeneration(),
                0);

            if (expectsDeath)
                Assert.Empty(result.People);
            else
                Assert.Single(result.People);
        }

        // Verifies the switch expression assigns the correct DeathModifier at every age boundary.
        // A NextDouble value between the two adjacent thresholds produces opposite outcomes
        // for each side, proving the boundary is wired correctly.
        // The 9→10 boundary is an exception: infant mortality means 0-9 has a higher modifier than 10-19.
        [Theory]
        [InlineData(9,  10,  0.0005,  true,  false)]  // Zero→Ten:      age 9 dies, age 10 survives
        [InlineData(19, 20,  0.00015, false, true)]   // Ten→Twenty:    age 19 survives, age 20 dies
        [InlineData(29, 30,  0.00035, false, true)]   // Twenty→Thirty
        [InlineData(39, 40,  0.00075, false, true)]   // Thirty→Fourty
        [InlineData(49, 50,  0.00125, false, true)]   // Fourty→Fifty
        [InlineData(59, 60,  0.00175, false, true)]   // Fifty→Sixty
        [InlineData(69, 70,  0.00225, false, true)]   // Sixty→Seventy
        [InlineData(79, 80,  0.00375, false, true)]   // Seventy→Eighty
        [InlineData(89, 90,  0.0075,  false, true)]   // Eighty→Ninety
        [InlineData(99, 100, 0.0175,  false, true)]   // Ninety→Hundred
        public void ProcessDeaths_AgeBoundaryTransition_CorrectModifierApplied(int lowerAge,
            int upperAge,
            double nextDoubleValue,
            bool lowerDies,
            bool upperDies)
        {
            var engine = CreateEngine(nextDoubleValue);
            var generation = CreateGeneration();

            var lowerSurvivors = engine.ProcessDeaths(new List<Person> { CreatePerson(lowerAge) },
                generation,
                0).People;
            var upperSurvivors = engine.ProcessDeaths(new List<Person> { CreatePerson(upperAge) },
                generation,
                0).People;

            if (lowerDies)
                Assert.Empty(lowerSurvivors);
            else
                Assert.Single(lowerSurvivors);

            if (upperDies)
                Assert.Empty(upperSurvivors);
            else
                Assert.Single(upperSurvivors);
        }

        // Guards the bug fix: old code used random.Next(0, 101) (integer) against deathChance (decimal).
        // With ControlledRandom returning 0.0055, the old integer path computes
        // Next(0,101) = (int)(0.0055 * 101) = 0, so 0 <= 0.5 wrongly triggers death.
        // The correct float path gives 0.0055 * 100 = 0.55 > 0.5, correctly giving survival.
        [Fact]
        public void ProcessDeaths_BugFix_FloatComparisonPreventsSpuriousDeath()
        {
            var engine = CreateEngine(0.0055);

            var result = engine.ProcessDeaths(new List<Person> { CreatePerson(85) },
                CreateGeneration(),
                0);

            Assert.Single(result.People);
        }
    }
}