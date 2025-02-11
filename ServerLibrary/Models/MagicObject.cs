using Library;
using Server.DBModels;
using Server.Envir;
using Server.Models.Magics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.Models
{
    //TODO - Send list to client of which magics are toggled, casted, targetted etc (allows removing of lots of client switches)
    public abstract class MagicObject
    {
        public PlayerObject Player { get; }
        public UserMagic Magic { get; }

        public MagicType Type => Magic.Info.Magic;
        public List<DelayedAction> ActionList => Player.ActionList;
        public Map CurrentMap => Player.CurrentMap;
        public Point CurrentLocation => Player.CurrentLocation;
        public MirDirection Direction => Player.Direction;

        protected abstract Element Element { get; }
        public virtual bool UpdateCombatTime => true;

        //Magic variables
        public virtual bool MagicSkill => false; //TODO
        public virtual bool CanStruck => true;
        protected virtual int Slow => 0;
        protected virtual int SlowLevel => 0;
        protected virtual int Repel => 0;
        protected virtual int Silence => 0;
        protected virtual int Shock => 0;
        protected virtual int Burn => 0;
        protected virtual int BurnLevel => 0;

        //Attack variables
        public virtual bool AttackSkill => false;
        public virtual bool IgnoreAccuracy => false;
        public virtual bool IgnorePhysicalDefense => false;
        public virtual int MaxLifeSteal => 750;
        public virtual bool HasFlameSplash(bool primary)
        {
            return false;
        }
        public virtual bool HasMassacre => false;

        public MagicObject(PlayerObject player, UserMagic magic) 
        {
            Player = player;
            Magic = magic;
        }

        public virtual bool CanUseMagic()
        {
            if (Magic.ItemRequired)
            {
                var magicItem = Player.Equipment.FirstOrDefault(x => x != null && x.Info.ItemEffect == ItemEffect.MagicRing && x.Info.Shape == Magic.Info.Index);

                if (magicItem == null) return false;
            }
            else if (Player.Level < Magic.Info.NeedLevel1)
            {
                return false;
            }

            return true;
        }

        public virtual bool CheckCost()
        {
            if (Magic.Cost > Player.CurrentMP)
            {
                return false;
            }

            return true;
        }

        public virtual MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            return new MagicCast();
        }

        public virtual void MagicComplete(params object[] data)
        {

        }

        public virtual void MagicConsume()
        {
            if (Magic.Info.School == MagicSchool.Discipline)
            {
                Player.ChangeFP(-Magic.Cost);
                return;
            }

            Player.ChangeMP(-Magic.Cost);
        }

        public virtual void MagicFinalise()
        {
            if (!Player.GetMagic(MagicType.Stealth, out Stealth stealth) || !stealth.CheckCloak())
            {
                Player.BuffRemove(BuffType.Cloak);
            }

            Player.BuffRemove(BuffType.Transparency);
        }

        public virtual void ResetCombatTime()
        {
            if (UpdateCombatTime)
            {
                Player.CombatTime = SEnvir.Now;
            }
        }

        public virtual void Process()
        {

        }

        public virtual void RefreshToggle()
        {
            
        }

        public virtual void Toggle(bool canUse)
        {

        }

        /// <summary>
        /// Gets called during a physical attack if marked as an <see cref="AttackSkill"/>
        /// </summary>
        /// <param name="attackType">The attack magic the player has requested to cast</param>
        /// <param name="validMagic">The magic which has been selected to be used as main attack</param>
        /// <returns>AttackCast object</returns>
        public virtual AttackCast AttackCast(MagicType attackType)
        {
            return new AttackCast();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="attackDelay">Millisecond delay until next attack can occur</param>
        public virtual void AttackLocationSuccess(int attackDelay)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="magics">List of magics enabled from attack, should be passed through to the actual attack call</param>
        public virtual void SecondaryAttackLocation(List<MagicType> magics)
        {

        }

        public virtual decimal LifeSteal(bool primary, decimal lifestealAmount)
        {
            return lifestealAmount;
        }

        /// <summary>
        /// Perform any magic actions after the Attack damage has been dealt to the target object
        /// </summary>
        /// <param name="target"></param>
        public virtual void AttackComplete(MapObject target)
        {
            Player.LevelMagic(Magic);
        }

        /// <summary>
        /// Perform any magic actions after the Attack damage has been dealt, even if this magic has not been cast
        /// </summary>
        /// <param name="target">Attacked target</param>
        /// <param name="types">List of magics used in attack</param>
        public virtual void AttackCompletePassive(MapObject target, List<MagicType> types)
        {
        }

        protected DateTime GetDelayFromDistance(int start, MapObject target)
        {
            var delay = SEnvir.Now.AddMilliseconds(start + Functions.Distance(Player.CurrentLocation, target.CurrentLocation) * 48);

            return delay;
        }

        public virtual int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            return power;
        }

        public virtual int ModifyPowerMultiplier(bool primary, int power, Stats stats = null, int extra = 0)
        {
            return power;
        }

        public virtual Stats GetPassiveStats()
        {
            return new Stats();
        }

        public virtual Element GetElement(Element element)
        {
            if (Element != Element.None)
                return Element;

            return element;
        }

        public virtual int GetSlow(int slow, Stats stats = null)
        {
            if (Slow > 0)
                return Slow;

            return slow;
        }

        public virtual int GetSlowLevel(int slowLevel, Stats stats = null)
        {
            if (SlowLevel > 0)
                return SlowLevel;

            return slowLevel;
        }

        public virtual int GetRepel(int repel, Stats stats = null)
        {
            if (Repel > 0)
                return Repel;

            return repel;
        }

        public virtual int GetSilence(int silence, Stats stats = null)
        {
            if (Silence > 0)
                return Silence;

            return silence;
        }

        public virtual int GetShock(int shock, Stats stats = null)
        {
            if (Shock > 0)
                return Shock;

            return shock;
        }

        public virtual int GetBurn(int burn, Stats stats = null)
        {
            if (Burn > 0)
                return Burn;

            return burn;
        }

        public virtual int GetBurnLevel(int burnLevel, Stats stats = null)
        {
            if (BurnLevel > 0)
                return BurnLevel;

            return burnLevel;
        }

        protected UserMagic GetAugmentedSkill(MagicType type)
        {
            if (Player.GetMagic(type, out MagicObject augMagic))
            {
                if (augMagic.CanUseMagic())
                {
                    return augMagic.Magic;
                }
            }

            return null;
        }

        /// <summary>
        /// Perform any magic actions after the MagicAttack damage has been dealt to the target object
        /// </summary>
        /// <param name="ob">Target object that was attacked</param>
        /// <param name="damageDealt">Calculated damage which was dealt to the target object</param>
        public virtual void MagicAttackSuccess(MapObject ob, int damageDealt)
        {
            Player.LevelMagic(Magic);
        }

        /// <summary>
        /// Perform any magic actions after the MagicAttack damage has been dealt, even if this magic has not been cast
        /// </summary>
        /// <param name="ob">Attacked target</param>
        /// <param name="types">List of magics used in attack</param>
        public virtual void MagicAttackSuccessPassive(MapObject ob, List<MagicType> types)
        {

        }

        public static List<MagicType> GetOrderedMagic(List<MagicType> types)
        {
            return null;
        }
    }

    public class MagicList : Dictionary<MagicType, MagicObject>
    {
        private IEnumerable<MagicType> orderedKeys;

        public IEnumerable<MagicType> OrderedKeys
        {
            get
            {
                orderedKeys ??= this.Keys.OrderBy(key => this[key].Type).ToList();

                return orderedKeys;
            }
        }

        public new void Add(MagicType key, MagicObject value)
        {
            orderedKeys = null;

            base[key] = value;
        }
    }

    public class MagicCast
    {
        /// <summary>
        /// List of locations spell has locked on to
        /// </summary>
        public List<Point> Locations = new ();

        /// <summary>
        /// List of targets spell has locked on to
        /// </summary>
        public List<uint> Targets = new ();

        /// <summary>
        /// Targetted object for the spell. Will be used to automatically update players direction to face target
        /// </summary>
        public MapObject Ob = null;

        /// <summary>
        /// has the spell been cast, sends cooldown to the client
        /// </summary>
        public bool Cast = true;

        /// <summary>
        /// Should the players direction be overwritten
        /// </summary>
        public MirDirection? Direction = null;

        /// <summary>
        /// Dont send a return packet to the client
        /// </summary>
        public bool Return = false;
    }

    public class AttackCast
    {
        /// <summary>
        /// List of all magics to be used in attack. Can only use Magics marked as "AttackSkill"
        /// </summary>
        public List<MagicType> Magics = new();

        /// <summary>
        /// Indicates the requested Attack Magic has been successfully activated for use.
        /// </summary>
        public bool Cast = false;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MagicTypeAttribute : Attribute
    {
        public readonly MagicType Type;
        public MagicTypeAttribute(MagicType type)
        {
            Type = type;
        }
    }
}
