namespace ProjectMarworyn.Tests.Mocks
{
    internal class ControlledRandom : Random//TODO: This is a code smell. Needs interface, firstly. Secondly, this test project needs a proper mocking framework. I am avoiding Moq due to the controversy. NSubstitute is a common recommendation. Put a pin in it
    {
        private readonly double _value;

        public ControlledRandom(double value)
        {
            _value = value;
        }

        protected override double Sample() => _value;

        public override double NextDouble() => _value;
    }
}