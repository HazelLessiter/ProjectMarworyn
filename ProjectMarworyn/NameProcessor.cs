using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal class NameProcessor : INameProcessor
    {
        public void ListNumberOfNamesByGender(List<Name> names)
        {
            var fNames = names.Where(x => x.Gender == Gender.Female)
                .Count();
            var mNames = names.Where(x => x.Gender == Gender.Male)
                .Count();

            Console.WriteLine($"Number of female names: {fNames}, Number of male names: {mNames}");
            Thread.Sleep(500);//TODO: Make configurable. "slow" = 1000, "medium" = 500, "fast"
        }

        public void GenerateChildren(List<Name> names)
        {
            var pairs = PairNames(names);

            var numberOfChildren = new Random()//TODO: I used to work for a gambling company and .Random() would not pass srutiny from the Gambling Commission - Not random enough
                .Next(0, 3);
            var genderRandomiser = new Random()
                .Next(0, 1);

            foreach (var pair in pairs)
            {
                if (numberOfChildren == 0)
                {
                    Console.WriteLine($"Pair {pair.FName.FullName} + {pair.MName.FullName} had no children");
                    Thread.Sleep(500);
                }
                else
                {
                    var gender = new Gender();
                    for (int i = 0; i < numberOfChildren; i++)
                    {
                        switch (genderRandomiser)
                        {
                            case 0:
                                gender = Gender.Female;
                                break;
                            case 1:
                                gender = Gender.Male;
                                break;
                        }

                        Name name = new Name();
                        if (gender == Gender.Female)
                        {
                            name = new Name
                            {
                                FullName = pair.MName.Prefix + pair.FName.Suffix,
                                Prefix = pair.MName.Prefix,
                                Suffix = pair.FName.Suffix,
                                Gender = Gender.Female
                            };
                        }
                        if (gender == Gender.Male)
                        {
                            name = new Name
                            {
                                FullName = pair.FName.Prefix + pair.MName.Suffix,
                                Prefix = pair.FName.Prefix,
                                Suffix = pair.MName.Suffix,
                                Gender = Gender.Male,
                            };
                        }

                        names.Add(name);
                        Console.WriteLine($"Child {name.FullName} was born to {pair.FName.FullName} and {pair.MName.FullName}");
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private List<Pair> PairNames(List<Name> names)
        {
            var fNames = GetNamesByGender(names,
                Gender.Female);
            var mNames = GetNamesByGender(names,
                Gender.Male);

            var pairs = new List<Pair>();
            var index = 0;

            foreach (var fName in fNames)
            {
                if (index >= mNames.Count())
                {
                    break;
                }

                var mName = mNames[index];

                if (mName != null)
                {
                    pairs.Add(new Pair
                    {
                        FName = fName,
                        MName = mName
                    });

                    Console.WriteLine($"Pair: {fName.FullName} + {mName.FullName}");
                    Thread.Sleep(500);
                }

                index++;
            }

            return pairs;
        }

        private List<Name> GetNamesByGender(List<Name> names,
            Gender gender)
        {
            var namesByGender = names.Where(x => x.Gender == gender)
                .ToList();

            return namesByGender;
        }
    }
}
