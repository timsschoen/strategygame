using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using strategygame_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client
{
    class ClientLobbySession
    {
        ILogger Logger;
        ContentManager Content;

        public ClientLobbySession(ContentManager Content, ILogger Logger)
        {
            this.Logger = Logger;
            this.Content = Content;
        }

        public void handleMessage(IMessage Message)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

        public void Update()
        {

        }
    }
}
