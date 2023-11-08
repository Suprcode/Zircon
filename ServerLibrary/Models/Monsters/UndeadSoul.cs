using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public sealed class UndeadSoul : MonsterObject
    {
        public UndeadSoul()
        {
            Visible = false;
            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(500), ActionType.Function));
            Direction = MirDirection.DownLeft;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            CurrentMap.Broadcast(CurrentLocation, new S.MapEffect { Location = CurrentLocation, Effect = Effect.UndeadSoul });

            ActionTime = SEnvir.Now.AddMilliseconds(1000);
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.Function:
                    Appear();
                    return;
            }

            base.ProcessAction(action);
        }

        public void Appear()
        {
            Visible = true;
            AddAllObjects();
        }
    }
}
