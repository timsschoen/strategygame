using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using strategygame_common;
using System.Collections.Generic;

namespace strategygame_server
{
    public class Server
    {
        NetworkServer networkServer;

        ServerGameSession gameSession;
        ServerLobbySession lobbySession;
        Thread serverThread;

        ILogger Logger = new ConsoleLogger();

        volatile bool StopFlag;

        public Server()
        {
            lobbySession = new ServerLobbySession();
            serverThread = new Thread(new ThreadStart(ServerLoop));
            networkServer = new NetworkServer(Logger);
        }
        
        public void Start()
        {
            if (serverThread.ThreadState == ThreadState.Unstarted && serverThread != null)
            {
                serverThread.Start();
                networkServer.Start();
            }
        }

        void ServerLoop()
        {            
            while (!StopFlag)
            {
                IMessage message = networkServer.TryGetNewMessage();
                if(message != null)
                {
                    
                    //sort between general, lobby and game messages
                }      
                
                //update lobby and game sessions          
            }
        }        
    }
}
