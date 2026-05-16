namespace ProjectMarworyn.Generators
{
    internal interface IDiceGenerator
    {
        public Random Create(int worldSeed);
        double NextDouble(Random random);
    }
}