namespace ProjectMarworyn
{
    internal class DiceGenerator : IDiceGenerator
    {
        public Random Create(int worldSeed)
        {
            return new Random(worldSeed);
        }
    }
}