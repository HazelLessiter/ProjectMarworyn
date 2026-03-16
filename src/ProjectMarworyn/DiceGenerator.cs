namespace ProjectMarworyn
{
    internal class DiceGenerator : IDiceGenerator
    {
        public Random Create()
        {
            return new Random();
        }
    }
}