using Library;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SummonPuppet)]
    public class SummonPuppet : MagicObject
    {
        protected override Element Element => Element.Fire;
        public override bool UpdateCombatTime => false;

        public SummonPuppet(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            /*  if (CurrentMap.Info.SkillDelay > 0)
              {
                  Connection.ReceiveChat(string.Format(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                  foreach (SConnection con in Connection.Observers)
                      con.ReceiveChat(string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                  return;
              }*/

            List<UserMagic> magics = new List<UserMagic> { Magic };

            //Summon Puppets.

            int count = Magic.Level + 1;

            var elementalPuppet = GetAugmentedSkill(MagicType.ElementalPuppet);
            var artOfShadows = GetAugmentedSkill(MagicType.ArtOfShadows);

            Stats darkstoneStats = new();
            if (elementalPuppet != null)
            {
                if (Player.Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)
                    darkstoneStats = Player.Equipment[(int)EquipmentSlot.Amulet].Info.Stats;

                Player.DamageDarkStone(10);

                magics.Add(elementalPuppet);

                Player.LevelMagic(elementalPuppet);
            }

            int range = 1;
            if (artOfShadows != null)
            {
                count += artOfShadows.GetPower();
                range = 3;

                magics.Add(artOfShadows);

                Player.LevelMagic(artOfShadows);
            }

            for (int i = 0; i < count; i++)
            {
                Puppet mob = new Puppet
                {
                    MonsterInfo = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.SummonPuppet),
                    Player = Player,
                    DarkStoneStats = darkstoneStats,
                    Direction = Direction,
                    TameTime = SEnvir.Now.AddDays(365)
                };

                foreach (UserMagic m in magics)
                {
                    mob.Magics.Add(m);
                }

                if (mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, range)))
                {
                    Player.Pets.Add(mob);
                    mob.PetOwner = Player;
                }
            }

            /*
            if (CurrentMap.Info.SkillDelay > 0)
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
            }*/

            Cell cell = CurrentMap.GetCell(CurrentMap.GetRandomLocation(CurrentLocation, 4));

            if (cell != null) Player.CurrentCell = cell;

            CloakEnd(Magic, true);

            Player.Broadcast(new S.ObjectTurn { ObjectID = Player.ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public void CloakEnd(UserMagic magic, bool forceGhost)
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.Cloak)) return;

            var pledgeofBlood = GetAugmentedSkill(MagicType.PledgeOfBlood);

            int value = 0;
            if (pledgeofBlood != null)
            {
                value = pledgeofBlood.GetPower();
                Player.LevelMagic(pledgeofBlood);
            }

            Stats buffStats = new()
            {
                [Stat.Cloak] = 1,
                [Stat.CloakDamage] = Player.Stats[Stat.Health] * (20 - magic.Level - value) / 1000,
            };

            Player.BuffAdd(BuffType.Cloak, TimeSpan.MaxValue, buffStats, true, false, TimeSpan.FromSeconds(2));

            Player.LevelMagic(magic);

            if (!forceGhost)
            {
                var ghostWalk = GetAugmentedSkill(MagicType.GhostWalk);

                if (ghostWalk == null) return;

                int rate = (ghostWalk.Level + 1) * 3;

                if (SEnvir.Random.Next(2 + rate) >= rate) return;

                Player.LevelMagic(ghostWalk);
            }

            Player.BuffAdd(BuffType.GhostWalk, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetDC() * Magic.GetPower() / 100;

            return power;
        }
    }
}
