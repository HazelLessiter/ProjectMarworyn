using System;

namespace ProjectMarworyn
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Simulation();
            game.Run();
        }
    }
}