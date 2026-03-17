namespace ProjectMarworyn
{
    internal interface IDiceGenerator
    {
        public Random Create(int worldSeed);
    }
}