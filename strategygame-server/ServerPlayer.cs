using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strategygame_server
{
    class ServerPlayer
    {
        public enum ServerPlayerState
        {
            InGame, 
            InLobby,
            Connected
        }

        public ServerPlayerState State;
        public string Name;
    }
}
