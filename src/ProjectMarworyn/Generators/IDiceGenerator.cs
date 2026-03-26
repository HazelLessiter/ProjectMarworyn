namespace ProjectMarworyn.Generators
{
    internal interface IDiceGenerator
    {
        public Random Create(int worldSeed);
    }
}