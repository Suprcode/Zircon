using Library;
using Library.Network;
using Server.DBModels;
using Server.Envir;
using Server.Models.Magics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public sealed class SpellObject : MapObject
    {
        public override ObjectType Race => ObjectType.Spell;

        public override bool Blocking => false;

        public Point DisplayLocation;
        public SpellEffect Effect;
        public int TickCount;
        public TimeSpan TickFrequency;
        public DateTime TickTime;
        public MapObject Owner;
        public UserMagic Magic;
        public int Power;

        public List<MapObject> Targets = new List<MapObject>();

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override void Process()
        {
            base.Process();

            if (Owner != null && (Owner.Node == null || Owner.Dead))
            {
                Despawn();
                return;
            }

            if (SEnvir.Now < TickTime) return;
            
            if (TickCount-- <= 0)
            {
                switch (Effect)
                {
                    case SpellEffect.MonsterDeathCloud:
                        MonsterObject monster = Owner as MonsterObject;
                        if (monster == null) break;

                        for (int i = CurrentCell.Objects.Count - 1; i >= 0; i--)
                        {
                            if (i >= CurrentCell.Objects.Count) continue;

                            MapObject ob = CurrentCell.Objects[i];

                            if (!monster.CanAttackTarget(ob)) continue;


                            monster.Attack(ob, 4000, Element.None);
                            monster.Attack(ob, 4000, Element.None);
                        }


                        break;
                }

                Despawn();
                return;
            }

            TickTime = SEnvir.Now + TickFrequency;


            switch (Effect)
            {
                case SpellEffect.TrapOctagon:
                    
                    for (int i = Targets.Count - 1; i >= 0; i--)
                    {
                        MapObject ob = Targets[i];

                        if (ob.Node != null && ob.ShockTime != DateTime.MinValue) continue;

                        Targets.Remove(ob);
                    }

                    if (Targets.Count == 0) Despawn();
                    break;
                default:

                    if (CurrentCell == null)
                    {
                        SEnvir.Log($"[ERROR] {Effect} CurrentCell Null.");
                        return;
                    }

                    if (CurrentCell.Objects == null)
                    {
                        SEnvir.Log($"[ERROR] {Effect} CurrentCell.Objects Null.");
                        return;
                    }

                    for (int i = CurrentCell.Objects.Count - 1; i >= 0; i--)
                    {
                        if (i >= CurrentCell.Objects.Count) continue;
                        if (CurrentCell.Objects[i] == this) continue;

                        ProcessSpell(CurrentCell.Objects[i]);

                        if (CurrentCell == null)
                        {
                            SEnvir.Log($"[ERROR] {Effect} CurrentCell Null Loop.");
                            return;
                        }

                        if (CurrentCell.Objects == null)
                        {
                            SEnvir.Log($"[ERROR] {Effect} CurrentCell.Objects Null Loop.");
                            return;
                        }


                    }
                    break;
            }
        }

        public void ProcessSpell(MapObject ob)
        {
            bool explode = false;

            switch (Effect)
            {
                case SpellEffect.PoisonousCloud:
                    if (!Owner.CanHelpTarget(ob)) return;

                    BuffInfo buff = ob.Buffs.FirstOrDefault(x=> x.Type == BuffType.PoisonousCloud);
                    TimeSpan remaining = TickTime - SEnvir.Now;

                    if (buff != null)
                        if (buff.RemainingTime > remaining) return;

                    ob.BuffAdd(BuffType.PoisonousCloud, remaining, new Stats { [Stat.Agility] = Power }, false, false, TimeSpan.Zero);
                    break;
                case SpellEffect.FireWall:
                    {
                        if (Owner is MonsterObject monster)
                        {
                            if (monster == null || !monster.CanAttackTarget(ob)) return;

                            monster.Attack(ob, monster.GetDC(), Element.Fire);
                        }
                        else if (Owner is PlayerObject player)
                        {
                            if (!player.CanAttackTarget(ob)) return;

                            int damage = player.MagicAttack(new List<MagicType> { MagicType.FireWall }, ob, true);

                            if (damage > 0 && ob.Race == ObjectType.Player)
                            {
                                foreach (SpellObject spell in player.SpellList)
                                {
                                    if (spell.Effect != Effect) continue;

                                    spell.TickCount--;
                                }
                            }
                        }
                    }
                    break;
                case SpellEffect.Tempest:
                    {
                        PlayerObject player = Owner as PlayerObject;
                        if (player == null || !player.CanAttackTarget(ob)) return;

                        int damage = player.MagicAttack(new List<MagicType> { MagicType.Tempest }, ob, true);

                        if (damage > 0 && ob.Race == ObjectType.Player)
                        {
                            foreach (SpellObject spell in player.SpellList)
                            {
                                if (spell.Effect != Effect) continue;

                                spell.TickCount--;
                            }
                        }
                    }
                    break;
                case SpellEffect.DarkSoulPrison:
                    {
                        if (Owner is not PlayerObject player || !player.CanAttackTarget(ob)) return;

                        int damage = player.MagicAttack(new List<MagicType> { MagicType.DarkSoulPrison }, ob, true);

                        if (damage > 0 && ob.Race == ObjectType.Player)
                        {
                            foreach (SpellObject spell in player.SpellList)
                            {
                                if (spell.Effect != Effect) continue;

                                spell.TickCount--;
                            }
                        }
                    }
                    break;
                case SpellEffect.BurningFire:
                    {
                        if (Owner is not PlayerObject player || !player.CanAttackTarget(ob)) return;

                        explode = true;
                    }
                    break;
            }

            if (explode)
            {
                switch (Effect)
                {
                    case SpellEffect.BurningFire:
                        {
                            if ((Owner is PlayerObject owner) && owner.GetMagic(MagicType.BurningFire, out BurningFire burningFire))
                            {
                                burningFire.Explode(ob.CurrentMap, ob.CurrentLocation);
                            }
                        }
                        break;
                }

                TickCount = 0;
                TickTime = SEnvir.Now;
            }
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            Owner?.SpellList.Add(this);
            
            AddAllObjects();

            Activate();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();

            Owner?.SpellList.Remove(this);
        }
        public override void OnSafeDespawn()
        {
            base.OnSafeDespawn();

            Owner?.SpellList.Remove(this);
        }

        public override void CleanUp()
        {
            base.CleanUp();
            
            Owner = null;
            Magic = null;

            Targets?.Clear();
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            return new S.ObjectSpell
            {
                ObjectID = ObjectID,
                Location = DisplayLocation,
                Effect = Effect,
                Direction = Direction,
                Power = Power,
            };
        }
        public override Packet GetDataPacket(PlayerObject ob)
        {
            return null;
        }

        public override bool CanDataBeSeenBy(PlayerObject ob)
        {
            return false;
        }

        public override void Activate()
        {
            if (Activated) return;

            if (Effect == SpellEffect.SafeZone) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        public override void DeActivate()
        {
            return;
        }

        public override void ProcessHPMP()
        {
        }
        public override void ProcessNameColour()
        {
        }
        public override void ProcessBuff()
        {
        }
        public override void ProcessPoison()
        {
        }
    }
}
