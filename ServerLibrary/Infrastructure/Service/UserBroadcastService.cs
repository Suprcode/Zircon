using Library;
using Server.Infrastructure.Service.Connection;
using System;
using System.Collections.Generic;

namespace Server.Infrastructure.Service
{
    public class UserBroadcastService(UserConnectionService UserConnectionService)
    {
        //TODO: some of this is convoluted because Language is owned by Server. It should be owned by client server should just send (i.e S.TemplatedMessage { type = 1, args = [...] } and let the client handle local translations
        //TODO: not 100% happy - any other improvements?

        internal void BroadcastOnlineCount()
        {
            foreach (UserConnection conn in UserConnectionService.ActiveConnections)
            {
                conn.ReceiveChat(string.Format(conn.Language.OnlineCount, UserConnectionService.Players, UserConnectionService.Observers), MessageType.Hint);

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
        internal void BroadcastSystemMessage(Func<UserConnection, string> messageSupplier)
        {
            foreach (UserConnection con in UserConnectionService.ActiveConnections)
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

        internal void BroadcastMessage(string text, List<ClientUserItem> linkedItems, MessageType messageType, Predicate<UserConnection> shouldReceive)
        {
            foreach (UserConnection con in UserConnectionService.ActiveConnections)
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
