using Library;
using Server.Infrastructure.Network;
using System;
using System.Collections.Generic;

namespace Server.Envir
{
    public class BroadcastService(SConnectionManager ConnectionManager)
    {
        //TODO: dont think this should be here
        internal void SendOnlineCount()
        {
            foreach (SConnection conn in ConnectionManager.Connections)
            {
                conn.ReceiveChat(string.Format(conn.Language.OnlineCount, ConnectionManager.Players, ConnectionManager.Observers), MessageType.Hint);

                switch (conn.Stage)
                {
                    case GameStage.Game:
                        if (conn.Player.Character.Observable)
                            conn.ReceiveChat(string.Format(conn.Language.ObserverCount, conn.Observers.Count), MessageType.Hint);
                        break;
                    case GameStage.Observer:
                        conn.ReceiveChat(string.Format(conn.Language.ObserverCount, conn.Observed.Observers.Count), MessageType.Hint);
                        break;
                }
            }
        }

        //TODO: dont think this should be here - this is also so convoluted because server owns the languages
        internal void BroadcastSystemMessage(Func<SConnection, string> messageSupplier)
        {
            foreach (SConnection con in ConnectionManager.Connections)
            {
                switch (con.Stage)
                {
                    case GameStage.Game:
                    case GameStage.Observer:
                        con.ReceiveChat(messageSupplier.Invoke(con), MessageType.System);
                        break;
                    default:
                        continue;
                }
            }
        }

        //TODO: dont think this should be here
        internal void BroadcastMessage(string text, List<ClientUserItem> linkedItems, MessageType messageType, Predicate<SConnection> shouldReceive)
        {
            foreach (SConnection con in ConnectionManager.Connections)
            {
                switch (con.Stage)
                {
                    case GameStage.Game:
                    case GameStage.Observer:
                        if (!shouldReceive.Invoke(con)) continue;

                        con.ReceiveChat(text, messageType, linkedItems);
                        break;
                    default: continue;
                }
            }
        }
    }
}
