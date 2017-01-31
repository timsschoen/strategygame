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
        NetworkServer mNetworkServer;

        ServerGameSession mGameSession;
        ServerLobbySession mLobbySession;
        Thread mServerThread;

        ILogger mLogger = new ConsoleLogger();

        volatile bool mStopFlag;

        public Server()
        {
            mLobbySession = new ServerLobbySession();                      

            JsonSerializer Serializer = new JsonSerializer();

            GameConfiguration Configuration = Serializer.Deserialize<GameConfiguration>(new JsonTextReader(new StreamReader(File.OpenRead("Config.json"))));

            mNetworkServer = new NetworkServer(mLogger);
            mGameSession = new ServerGameSession(mNetworkServer, mLogger, Configuration);
            mServerThread = new Thread(new ThreadStart(ServerLoop));
        }
        
        public void Start()
        {
            if (mServerThread.ThreadState == ThreadState.Unstarted && mServerThread != null)
            {
                mServerThread.Start();
                mNetworkServer.Start();
            }
        }

        void ServerLoop()
        {            
            while (!mStopFlag)
            {
                IMessage message = mNetworkServer.TryGetNewMessage();
                if(message != null)
                {
                    mGameSession.handleNetworkMessage(message);

                    //sort between general, lobby and game messages
                }

                //update lobby and game sessions  
                mGameSession.Update();        
            }
        }        
    }
}
