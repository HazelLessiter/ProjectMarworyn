namespace ProjectMarworyn.Core.Services
{
    internal interface IConsoleService
    {
        void WriteLine(string message, ConsoleColor colour = ConsoleColor.White);
        void Delay();
    }
}