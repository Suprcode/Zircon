using Library;
using Server.DBModels;
using Server.Envir;
using System;

namespace Server.Models.Magics
{
    [MagicType(MagicType.LastStand)]
    public class LastStand : MagicObject
    {
        protected override Element Element => Element.None;
        private bool Active { get; set; }
        public bool LowHP
        {
            get { return (Player.CurrentHP * 100 / Player.Stats[Stat.Health]) < 30; }
        }

        public LastStand(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void Process()
        {
            if (!CanUseMagic()) return;

            if (!Active && LowHP)
            {
                Active = true;

                if (SEnvir.Random.Next(Globals.MagicMaxLevel + 1) > Magic.Level)
                {
                    return;
                }

                Stats buffStats = new()
                {
                    [Stat.PhysicalDefencePercent] = Magic.GetPower()
                };

                Player.BuffAdd(BuffType.LastStand, TimeSpan.MaxValue, buffStats, false, false, TimeSpan.Zero);
                Player.LevelMagic(Magic);
            }
            else if (Active && !LowHP)
            {
                Active = false;
                Player.BuffRemove(BuffType.LastStand);
            }
        }
    }
}
