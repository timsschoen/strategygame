using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using strategygame_common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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

            JsonSerializer Serializer = new JsonSerializer();

            GameConfiguration Configuration = Serializer.Deserialize<GameConfiguration>(new JsonTextReader(new StreamReader(File.OpenRead("Config.json"))));

            gameSession = new ServerGameSession(networkServer, Logger, Configuration);
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
