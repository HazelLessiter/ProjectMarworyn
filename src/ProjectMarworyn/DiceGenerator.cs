namespace ProjectMarworyn
{
    internal class DiceGenerator : IDiceGenerator
    {
        public Random Create()
        {
            return new Random();//TODO: This is getting replaced with a custom randomiser. Not concerned about rapid instantiation at this stage. See Issue#7
        }
    }
}