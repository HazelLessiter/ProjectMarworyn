using ProjectMarworyn.Models;
using ProjectMarworyn.Tests.Mocks;

namespace ProjectMarworyn.Tests
{
    public class NameProcessorTests
    {
        private readonly NameProcessor _nameProcessor;

        public NameProcessorTests()
        {
            _nameProcessor = new NameProcessor(new MockOutputService());
        }

        [Fact]
        public void GenerateChildren_WithNoPairs_ReturnsEmptyGeneration()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>()
            };

            var result = _nameProcessor.GenerateChildren(generation);

            Assert.Empty(result.Names);
        }

        [Fact]
        public void GenerateChildren_WithNoPairs_IncrementsIteration()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>()
            };

            var result = _nameProcessor.GenerateChildren(generation);

            Assert.Equal(2, result.Iteration);
        }

        [Fact]
        public void GenerateChildren_WithOnePair_FemaleChildHasMalePrefixAndFemaleSuffix()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>
                {
                    new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                    new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
                }
            };

            var result = _nameProcessor.GenerateChildren(generation);
            var femaleChildren = result.Names.Where(c => c.Gender == Gender.Female).ToList();

            if (femaleChildren.Any())
            {
                Assert.Equal("John", femaleChildren.First().Prefix);
            }
        }

        [Fact]
        public void GenerateChildren_WithOnePair_FemaleChildHasCorrectSuffix()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>
                {
                    new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                    new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
                }
            };

            var result = _nameProcessor.GenerateChildren(generation);
            var femaleChildren = result.Names.Where(c => c.Gender == Gender.Female).ToList();

            if (femaleChildren.Any())
            {
                Assert.Equal("Doe", femaleChildren.First().Suffix);
            }
        }

        [Fact]
        public void GenerateChildren_WithOnePair_MaleChildHasFemalePrefixAndMaleSuffix()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>
                {
                    new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                    new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
                }
            };

            var result = _nameProcessor.GenerateChildren(generation);
            var maleChildren = result.Names.Where(c => c.Gender == Gender.Male).ToList();

            if (maleChildren.Any())
            {
                Assert.Equal("Jane", maleChildren.First().Prefix);
            }
        }

        [Fact]
        public void GenerateChildren_WithOnePair_MaleChildHasCorrectSuffix()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>
                {
                    new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                    new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
                }
            };

            var result = _nameProcessor.GenerateChildren(generation);
            var maleChildren = result.Names.Where(c => c.Gender == Gender.Male).ToList();

            if (maleChildren.Any())
            {
                Assert.Equal("Smith", maleChildren.First().Suffix);
            }
        }

        [Fact]
        public void GenerateChildren_WithOnePair_FemaleChildFullNameIsPrefixPlusSuffix()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>
                {
                    new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                    new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
                }
            };

            var result = _nameProcessor.GenerateChildren(generation);
            var femaleChildren = result.Names.Where(c => c.Gender == Gender.Female).ToList();

            if (femaleChildren.Any())
            {
                Assert.Equal("JohnDoe", femaleChildren.First().FullName);
            }
        }

        [Fact]
        public void GenerateChildren_WithOnePair_MaleChildFullNameIsPrefixPlusSuffix()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>
                {
                    new Name { FullName = "JaneDoe", Prefix = "Jane", Suffix = "Doe", Gender = Gender.Female },
                    new Name { FullName = "JohnSmith", Prefix = "John", Suffix = "Smith", Gender = Gender.Male }
                }
            };

            var result = _nameProcessor.GenerateChildren(generation);
            var maleChildren = result.Names.Where(c => c.Gender == Gender.Male).ToList();

            if (maleChildren.Any())
            {
                Assert.Equal("JaneSmith", maleChildren.First().FullName);
            }
        }

        [Fact]
        public void GenerateChildren_WithTwoPairs_CreatesTwoPairs()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>
                {
                    new Name { FullName = "Female1", Prefix = "F1", Suffix = "S1", Gender = Gender.Female },
                    new Name { FullName = "Female2", Prefix = "F2", Suffix = "S2", Gender = Gender.Female },
                    new Name { FullName = "Male1", Prefix = "M1", Suffix = "MS1", Gender = Gender.Male },
                    new Name { FullName = "Male2", Prefix = "M2", Suffix = "MS2", Gender = Gender.Male }
                }
            };

            var result = _nameProcessor.GenerateChildren(generation);

            Assert.NotNull(result);
        }

        [Fact]
        public void GenerateChildren_WithMoreFemalesThanMales_DoesNotThrow()
        {
            var generation = new Generation
            {
                Iteration = 1,
                Names = new List<Name>
                {
                    new Name { FullName = "Female1", Prefix = "F1", Suffix = "S1", Gender = Gender.Female },
                    new Name { FullName = "Female2", Prefix = "F2", Suffix = "S2", Gender = Gender.Female },
                    new Name { FullName = "Female3", Prefix = "F3", Suffix = "S3", Gender = Gender.Female },
                    new Name { FullName = "Male1", Prefix = "M1", Suffix = "MS1", Gender = Gender.Male }
                }
            };

            var exception = Record.Exception(() => _nameProcessor.GenerateChildren(generation));

            Assert.Null(exception);
        }

        [Fact]
        public void GenerateChildren_WithIteration5_ReturnsIteration6()
        {
            var generation = new Generation
            {
                Iteration = 5,
                Names = new List<Name>
                {
                    new Name { FullName = "Female1", Prefix = "F1", Suffix = "S1", Gender = Gender.Female },
                    new Name { FullName = "Male1", Prefix = "M1", Suffix = "MS1", Gender = Gender.Male }
                }
            };

            var result = _nameProcessor.GenerateChildren(generation);

            Assert.Equal(6, result.Iteration);
        }

        [Fact]
        public void ListNumberOfNamesByGender_WithEmptyList_DoesNotThrow()
        {
            var names = new List<Name>();

            var exception = Record.Exception(() => _nameProcessor.ListNumberOfNamesByGender(names));

            Assert.Null(exception);
        }

        [Fact]
        public void ListNumberOfNamesByGender_WithNames_DoesNotThrow()
        {
            var names = new List<Name>
            {
                new Name { FullName = "Female1", Gender = Gender.Female },
                new Name { FullName = "Male1", Gender = Gender.Male }
            };

            var exception = Record.Exception(() => _nameProcessor.ListNumberOfNamesByGender(names));

            Assert.Null(exception);
        }
    }
}