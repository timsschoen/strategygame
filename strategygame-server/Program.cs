using System;

namespace strategiespiel_server
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Server server = new Server();
            server.Start();            
        }
    }
#endif
}
