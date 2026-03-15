namespace ProjectMarworyn
{
    internal class ConsoleService : IConsoleService
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Delay(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}