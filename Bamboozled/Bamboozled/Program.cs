using System;

namespace Bamboozled
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Bamboozled game = new Bamboozled())
            {
                game.Run();
            }
        }
    }
#endif
}

