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
        }

        public void GenerateChildren(List<Name> names)
        {
            var pairs = PairNames(names);

            foreach (var pair in pairs)
            {
                var numberOfChildren = new Random()
                    .Next(0, 3);

                if (numberOfChildren == 0)
                {
                    Console.WriteLine($"Pair {pair.FName} + {pair.MName} had no children");
                }
                else
                {
                    var gender = new Gender();
                    for (int i = 0; i < numberOfChildren; i++)
                    {
                        var genderRandomiser = new Random()
                            .Next(0, 1);
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
                                Gender = Gender.Female,
                                Prefix = pair.MName.Prefix,
                                Suffix = pair.FName.Suffix,
                            };
                        }
                        if (gender == Gender.Male)
                        {
                            name = new Name
                            {
                                Gender = Gender.Male,
                                Prefix = pair.FName.Prefix,
                                Suffix = pair.MName.Suffix,
                            };
                        }

                        names.Add(name);
                        Console.WriteLine($"Child {name.Prefix+name.Suffix} was born to {pair.FName.Prefix+pair.FName.Suffix} +" +
                            $"{pair.MName.Prefix + pair.MName.Suffix}");
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
                var mName = mNames[index];

                if (mName != null)
                {
                    pairs.Add(new Pair
                    {
                        FName = fName,
                        MName = mName
                    });

                    Console.WriteLine($"Pair: {fName} + {mName}");
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
