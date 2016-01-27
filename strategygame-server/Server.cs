using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace strategygame_server
{
    public class Server
    {
        Thread serverThread;

        public Server()
        {
            serverThread = new Thread(new ThreadStart(ServerLoop));            
        }
        
        public void Start()
        {
            if(serverThread.ThreadState == ThreadState.Unstarted && serverThread != null)
                serverThread.Start();
        }

        void ServerLoop()
        {
            //TODO
        }

        void Tick()
        {
            
        }
    }
}
