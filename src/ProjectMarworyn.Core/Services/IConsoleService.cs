namespace ProjectMarworyn.Core.Services
{
    public interface IConsoleService
    {
        void WriteLine(string message, ConsoleColor colour = ConsoleColor.White);
        void Delay();
    }
}