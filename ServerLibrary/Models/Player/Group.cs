using Library;
using Library.Network;
using Server.Envir;
using Server.Models.Magics;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    //TODO - If leader leaves group, it passes the leader to the next person. Make sure to handle LFG in this situation
    //TODO - Bug - update group when allow group changed to false
    //TODO - Rename methods and tidy variables

    public partial class PlayerObject
    {
        #region Properties

        public PlayerObject GroupInvitation, GroupInvitationRequest;

        #endregion

        #region LFG Properties

        public string LFGName { get; set; }
        public string LFGType { get; set; }
        public int LFGMaxCount { get; set; }
        public bool LFGEnabled { get; set; }
        public DateTime LFGEnabledDateTime { get; set; }

        private bool LFGNeedUpdate { get; set; }
        public bool LFGReceiveUpdates { get; set; }

        #endregion

        public void ProcessGroup()
        {
            //disable after 1 hour, broadcast update

            if (LFGEnabled && SEnvir.Now > LFGEnabledDateTime)
            {
                LFGEnabled = false;
                LFGNeedUpdate = true;

                //Send expired message
            }

            if (LFGNeedUpdate)
            {
                BroadcastLFG();

                LFGNeedUpdate = false;
            }
        }

        public void BroadcastLFG()
        {
            for (int i = SEnvir.Players.Count - 1; i >= 0; i--)
            {
                var player = SEnvir.Players[i];

                if (player.Connection == null || player.Node == null) continue;

                if (!player.LFGReceiveUpdates) continue;

                player.Enqueue(new S.GroupUpdate { Group = ToClientGroup() });
            }
        }

        public void SendLFGList()
        {
            var list = new List<ClientGroup>();

            for (int i = SEnvir.Players.Count - 1; i >= 0; i--)
            {
                var player = SEnvir.Players[i];

                if (player.Connection == null || player.Node == null) continue;

                if (!player.LFGEnabled) continue;

                list.Add(player.ToClientGroup());
            }

            Enqueue(new S.GroupLFG { List = list });
        }

        public void LFGUpdate(C.GroupLFGUpdate p)
        {
            LFGName = p.Name;
            LFGMaxCount = p.MaxCount;
            LFGType = p.Type;

            UpdateLFGStatus(p.Enabled);

            GroupSwitch(true);

            LFGNeedUpdate = true;
        }

        public void UpdateLFGStatus(bool enabled)
        {
            bool old = LFGEnabled;

            LFGEnabled = enabled;

            if (LFGEnabled)
            {
                LFGEnabledDateTime = SEnvir.Now.AddHours(1);

                //Send 1 hour message
            }

            if (old != LFGEnabled)
            {
                LFGNeedUpdate = true;
            }
        }

        #region Group

        public void GroupSwitch(bool allowGroup)
        {
            if (Character.Account.AllowGroup == allowGroup) return;

            if (GroupMembers != null && GroupMembers.Any(x => x.CurrentMap.Instance != null))
            {
                Connection.ReceiveChat(Connection.Language.InstanceNoAction, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.InstanceNoAction, MessageType.System);
                return;
            }

            Character.Account.AllowGroup = allowGroup;

            Enqueue(new S.GroupSwitch { Allow = Character.Account.AllowGroup });

            if (GroupMembers != null)
                GroupLeave();
        }

        public void GroupRemove(string name)
        {
            if (GroupMembers == null)
            {
                Connection.ReceiveChat(Connection.Language.GroupNoGroup, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupNoGroup, MessageType.System);
                return;
            }

            if (GroupMembers[0] != this)
            {
                Connection.ReceiveChat(Connection.Language.GroupNotLeader, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupNotLeader, MessageType.System);
                return;
            }

            if (GroupMembers.Any(x => x.CurrentMap.Instance != null))
            {
                Connection.ReceiveChat(Connection.Language.InstanceNoAction, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.InstanceNoAction, MessageType.System);
                return;
            }

            foreach (PlayerObject member in GroupMembers)
            {
                if (string.Compare(member.Name, name, StringComparison.OrdinalIgnoreCase) != 0) continue;

                member.GroupLeave();
                return;
            }

            Connection.ReceiveChat(string.Format(Connection.Language.GroupMemberNotFound, name), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(string.Format(con.Language.GroupMemberNotFound, name), MessageType.System);

        }
        public void GroupInvite(string name)
        {
            if (GroupMembers != null && GroupMembers[0] != this)
            {
                Connection.ReceiveChat(Connection.Language.GroupNotLeader, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupNotLeader, MessageType.System);
                return;
            }

            PlayerObject player = SEnvir.GetPlayerByCharacter(name);

            if (player == null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.CannotFindPlayer, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.CannotFindPlayer, name), MessageType.System);
                return;
            }

            if (player.GroupMembers != null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupAlreadyGrouped, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupAlreadyGrouped, name), MessageType.System);
                return;
            }

            if (player.GroupInvitation != null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupAlreadyInvited, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupAlreadyInvited, name), MessageType.System);
                return;
            }

            if (!player.Character.Account.AllowGroup || SEnvir.IsBlocking(Character.Account, player.Character.Account))
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupInviteNotAllowed, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupInviteNotAllowed, name), MessageType.System);
                return;
            }

            if (player == this)
            {
                Connection.ReceiveChat(Connection.Language.GroupSelf, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupSelf, MessageType.System);
                return;
            }

            if (GroupMembers != null && CurrentMap.Instance != null)
            {
                if (!CurrentMap.Instance.UserRecord.TryGetValue(player.Name, out byte instanceSequence) || CurrentMap.InstanceSequence != instanceSequence)
                {
                    Connection.ReceiveChat(Connection.Language.InstanceNoAction, MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(con.Language.InstanceNoAction, MessageType.System);

                    return;
                }
            }

            if (GroupInvitationRequest != null)
            {
                player.GroupInvitation = this;
                player.GroupJoin();
                player.GroupInvitation = null;

                GroupInvitationRequest = null;
                return;
            }

            player.GroupInvitation = this;
            player.Enqueue(new S.GroupInvite { Name = Name, ObserverPacket = false });
        }
        public void GroupRequest(string groupLeader)
        {
            if (GroupMembers != null)
            {
                //You are already group
                return;
            }

            PlayerObject leader = SEnvir.GetPlayerByCharacter(groupLeader);

            if (leader == null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.CannotFindPlayer, groupLeader), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.CannotFindPlayer, groupLeader), MessageType.System);
                return;
            }

            if (leader == this)
            {
                Connection.ReceiveChat(Connection.Language.GroupSelf, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupSelf, MessageType.System);
                return;
            }

            if (!leader.Character.Account.AllowGroup || SEnvir.IsBlocking(leader.Character.Account, Character.Account))
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupInviteNotAllowed, groupLeader), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupInviteNotAllowed, groupLeader), MessageType.System);
                return;
            }

            if (leader.CurrentMap.Instance != null)
            {
                if (!leader.CurrentMap.Instance.UserRecord.TryGetValue(Name, out byte instanceSequence) || leader.CurrentMap.InstanceSequence != instanceSequence)
                {
                    Connection.ReceiveChat(Connection.Language.InstanceNoAction, MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(con.Language.InstanceNoAction, MessageType.System);

                    return;
                }
            }

            if (!leader.LFGEnabled)
            {
                return;
            }

            if (GroupMembers != null && GroupMembers.Count >= leader.LFGMaxCount)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupMemberLimit, GroupInvitation.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupMemberLimit, GroupInvitation.Name), MessageType.System);

                return;
            }

            if (leader.GroupInvitationRequest != null)
            {
                //TODO - Add to a queue and try again in 5 seconds. Max Retries
                //Then send message that request has timed out

                return;
            }

            leader.GroupInvitationRequest = this;
            leader.Enqueue(new S.GroupRequest { Name = Name, ObserverPacket = false });
        }

        public void GroupJoin()
        {
            if (GroupInvitation != null && GroupInvitation.Node == null) GroupInvitation = null;

            if (GroupInvitation == null || GroupMembers != null) return;

            if (GroupInvitation.GroupMembers == null)
            {
                GroupInvitation.GroupSwitch(true);
                GroupInvitation.GroupMembers = new List<PlayerObject> { GroupInvitation };
                GroupInvitation.Enqueue(new S.GroupMember { ObjectID = GroupInvitation.ObjectID, Name = GroupInvitation.Name }); //<-- Setting group leader?
            }
            else if (GroupInvitation.GroupMembers[0] != GroupInvitation)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupAlreadyGrouped, GroupInvitation.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupAlreadyGrouped, GroupInvitation.Name), MessageType.System);
                return;
            }
            else if (GroupInvitation.GroupMembers.Count >= Globals.GroupLimit)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupMemberLimit, GroupInvitation.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupMemberLimit, GroupInvitation.Name), MessageType.System);
                return;
            }

            if (CurrentMap.Instance != null)
            {
                Connection.ReceiveChat(Connection.Language.InstanceNoAction, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.InstanceNoAction, MessageType.System);
                return;
            }

            GroupMembers = GroupInvitation.GroupMembers;
            GroupMembers.Add(this);

            UpdateLFGStatus(false);

            foreach (PlayerObject ob in GroupMembers)
            {
                if (ob == this) continue;

                ob.Enqueue(new S.GroupMember { ObjectID = ObjectID, Name = Name });
                Enqueue(new S.GroupMember { ObjectID = ob.ObjectID, Name = ob.Name });

                ob.AddAllObjects();
                ob.RefreshStats();
                ob.ApplyGuildBuff();
            }

            AddAllObjects();
            ApplyGuildBuff();

            LFGNeedUpdate = true;

            RefreshStats();
            Enqueue(new S.GroupMember { ObjectID = ObjectID, Name = Name });
        }
        public void GroupLeave()
        {
            Packet p = new S.GroupRemove { ObjectID = ObjectID };

            GroupMembers.Remove(this);
            List<PlayerObject> oldGroup = GroupMembers;
            GroupMembers = null;

            if (Buffs.Any(x => x.Type == BuffType.SoulResonance))
                SoulResonance.Remove(this);

            foreach (PlayerObject ob in oldGroup)
            {
                ob.Enqueue(p);
                ob.RemoveAllObjects();
                ob.RefreshStats();
                ob.ApplyGuildBuff();
            }

            if (oldGroup.Count > 0)
                LFGNeedUpdate = true;

            if (oldGroup.Count == 1) oldGroup[0].GroupLeave();

            GroupMembers = null;

            Enqueue(p);
            RemoveAllObjects();
            RefreshStats();
            ApplyGuildBuff();
        }

        #endregion

        public ClientGroup ToClientGroup()
        {
            return new ClientGroup
            {
                GroupName = LFGName,
                LeaderName = Name,
                GroupType = LFGType,
                CurrentCount = GroupMembers?.Count ?? 1,
                MaxCount = LFGMaxCount,
                Enabled = LFGEnabled
            };
        }
    }
}
