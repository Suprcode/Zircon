using Client.Controls;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class BuffDialog : DXWindow
    {
        #region Properties
        private Dictionary<ClientBuffInfo, DXImageControl> Icons = new Dictionary<ClientBuffInfo, DXImageControl>();

        public override WindowType Type => WindowType.BuffBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;
        #endregion

        public BuffDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0.6F;
            
            Size = new Size(30, 30);
        }

        #region Methods
        public void BuffsChanged()
        {
            foreach (DXImageControl control in Icons.Values)
                control.Dispose();

            Icons.Clear();

            List<ClientBuffInfo> buffs = MapObject.User.Buffs.ToList();

            Stats permStats = new Stats();

            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                ClientBuffInfo buff = buffs[i];

                switch (buff.Type)
                {
                    case BuffType.ItemBuff:
                        if (buff.RemainingTime != TimeSpan.MaxValue) continue;

                        permStats.Add(Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex).Stats);

                        buffs.Remove(buff);
                        break;
                    case BuffType.Ranking:
                    case BuffType.Developer:
                        buffs.Remove(buff);
                        break;
                }
            }
            
            if (permStats.Count > 0)
                buffs.Add(new ClientBuffInfo { Index = 0, Stats = permStats, Type = BuffType.ItemBuffPermanent, RemainingTime = TimeSpan.MaxValue });

            buffs.Sort((x1, x2) => x2.RemainingTime.CompareTo(x1.RemainingTime));

            foreach (ClientBuffInfo buff in buffs)
            {
                DXImageControl icon;
                Icons[buff] = icon = new DXImageControl
                {
                    Parent = this,
                    LibraryFile = LibraryFile.CBIcon,
                    HintPosition = HintPosition.BottomLeft
                };

                switch (buff.Type)
                {
                    case BuffType.Castle:
                        icon.Index = 242;
                        break;
                    case BuffType.Observable:
                        icon.Index = 172;
                        break;
                    case BuffType.Veteran:
                        icon.Index = 171;
                        break;
                    case BuffType.Brown:
                        icon.Index = 229;
                        break;
                    case BuffType.PKPoint:
                        icon.Index = 266;
                        break;
                    case BuffType.ItemBuff:
                        icon.Index = Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex).BuffIcon;
                        break;
                    case BuffType.PvPCurse:
                        icon.Index = 241;
                        break;
                    case BuffType.ItemBuffPermanent:
                        icon.Index = 81;
                        break;
                    case BuffType.HuntGold:
                        icon.Index = 264;
                        break;
                    case BuffType.Companion:
                        icon.Index = 137;
                        break;
                    case BuffType.MapEffect:
                        icon.Index = 76;
                        break;
                    case BuffType.InstanceEffect:
                        icon.Index = 76;
                        break;
                    case BuffType.Guild:
                        icon.Index = 140;
                        break;
                    case BuffType.Fame:
                        icon.Index = 80;
                        break;

                    case BuffType.Heal:
                        icon.Index = 78;
                        break;
                    case BuffType.Invisibility:
                        icon.Index = 74;
                        break;
                    case BuffType.MagicResistance:
                        icon.Index = 92;
                        break;
                    case BuffType.Resilience:
                        icon.Index = 91;
                        break;
                    case BuffType.PoisonousCloud:
                        icon.Index = 98;
                        break;
                    case BuffType.FullBloom:
                        icon.Index = 162;
                        break;
                    case BuffType.WhiteLotus:
                        icon.Index = 163;
                        break;
                    case BuffType.RedLotus:
                        icon.Index = 164;
                        break;
                    case BuffType.MagicShield:
                        icon.Index = 100;
                        break;
                    case BuffType.FrostBite:
                        icon.Index = 221;
                        break;
                    case BuffType.ElementalSuperiority:
                        icon.Index = 93;
                        break;
                    case BuffType.BloodLust:
                        icon.Index = 90;
                        break;
                    case BuffType.Cloak:
                        icon.Index = 160;
                        break;
                    case BuffType.GhostWalk:
                        icon.Index = 160;
                        break;
                    case BuffType.TheNewBeginning:
                        icon.Index = 166;
                        break;
                    case BuffType.Redemption:
                        icon.Index = 258;
                        break;
                    case BuffType.Renounce:
                        icon.Index = 94;
                        break;
                    case BuffType.Defiance:
                        icon.Index = 97;
                        break;
                    case BuffType.Might:
                        icon.Index = 96;
                        break;
                    case BuffType.ReflectDamage:
                        icon.Index = 98;
                        break;
                    case BuffType.Endurance:
                        icon.Index = 95;
                        break;
                    case BuffType.JudgementOfHeaven:
                        icon.Index = 99;
                        break;
                    case BuffType.StrengthOfFaith:
                        icon.Index = 141;
                        break;
                    case BuffType.CelestialLight:
                        icon.Index = 142;
                        break;
                    case BuffType.SoulResonance:
                        icon.Index = 149;
                        break;
                    case BuffType.Transparency:
                        icon.Index = 160;
                        break;
                    case BuffType.LifeSteal:
                        icon.Index = 98;
                        break;
                    case BuffType.DefensiveBlow:
                        icon.Index = 157;
                        break;
                    case BuffType.DarkConversion:
                        icon.Index = 166;
                        break;
                    case BuffType.DragonRepulse:
                        icon.Index = 165;
                        break;
                    case BuffType.Evasion:
                        icon.Index = 167;
                        break;
                    case BuffType.RagingWind:
                        icon.Index = 168;
                        break;
                    case BuffType.MagicWeakness:
                        icon.Index = 182;
                        break;
                    case BuffType.Concentration:
                        icon.Index = 200;
                        break;
                    case BuffType.Spiritualism:
                        icon.Index = 202;
                        break;
                    case BuffType.LastStand:
                        icon.Index = 204;
                        break;
                    case BuffType.Invincibility:
                        icon.Index = 203;
                        break;
                    case BuffType.ElementalHurricane:
                        icon.Index = 98;
                        break;
                    case BuffType.SuperiorMagicShield:
                        icon.Index = 161;
                        break;
                    default:
                        icon.Index = 73;
                        break;
                }

                icon.ProcessAction = () =>
                {
                    if (MouseControl == icon)
                        icon.Hint = GetBuffHint(buff);
                };
            }

            for (int i = 0; i < buffs.Count; i++)
                Icons[buffs[i]].Location = new Point(3 + (i%6)*27, 3 + (i/6)*27);

            Size = new Size(3 + Math.Min(6, Math.Max(1, Icons.Count))*27, 3 + Math.Max(1, 1 +  (Icons.Count - 1)/6) * 27);    
        }

        private string GetBuffHint(ClientBuffInfo buff)
        {
            string text = string.Empty;

            Stats stats = buff.Stats;

            switch (buff.Type)
            {
                case BuffType.Server:
                    text = $"Server Settings\n";
                    break;
                case BuffType.HuntGold:
                    text = $"Hunt Gold\n";
                    break;
                case BuffType.Observable:
                    text = $"Observable\n\n" +
                           $"You are allowing people to watch you play.\n";
                    break;
                case BuffType.Veteran:
                    text = $"Veteran\n";
                    break;
                case BuffType.Brown:
                    text = $"Brown\n";
                    break;
                case BuffType.PKPoint:
                    text = $"PK Points\n";
                    break;
                case BuffType.Redemption:
                    text = $"Redemption Key Stone\n";
                    break;
                case BuffType.Castle:
                    text = $"Castle Owner\n";
                    break;
                case BuffType.Guild:
                    text = $"Guild\n";
                    break;
                case BuffType.MapEffect:
                    text = $"Map Effect\n";
                    break;
                case BuffType.InstanceEffect:
                    text = $"Instance Effect\n";
                    break;
                case BuffType.ItemBuff:
                    {
                        ItemInfo info = Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex);
                        text = info.ItemName + "\n";
                        stats = info.Stats;
                    }
                    break;
                case BuffType.ItemBuffPermanent:
                    text = "Permanent Item Buffs\n";
                    break;
                case BuffType.Companion:
                    text = $"Companion\n";
                    break;
                case BuffType.Fame:
                    {
                        FameInfo info = Globals.FameInfoList.Binding.First(x => x.Index == buff.Stats[Stat.Fame]);
                        text = $"{info.Name}\n";

                        if (!string.IsNullOrEmpty(info.Description))
                        {
                            text += "\n" + Functions.BreakStringIntoLines(info.Description, 45) + "\n";
                        }
                    }
                    break;

                case BuffType.Defiance:
                    text = $"Defiance\n";
                    break;
                case BuffType.Might:
                    text = $"Might\n";
                    break;
                case BuffType.Endurance:
                    text = $"Endurance\n";
                    break;
                case BuffType.ReflectDamage:
                    text = $"Reflect Damage\n";
                    break;
                case BuffType.Renounce:
                    text = $"Renounce\n";
                    break;
                case BuffType.MagicShield:
                    text = $"Magic Shield\n";
                    break;
                case BuffType.FrostBite:
                    text = $"Frost Bite\n";
                    break;
                case BuffType.JudgementOfHeaven:
                    text = $"Judgement Of Heaven\n";
                    break;
                case BuffType.Heal:
                    text = $"Heal\n";
                    break;
                case BuffType.Invisibility:
                    text = $"Invisibility\n";
                    text += $"Hide in plain sight.\n";
                    break;
                case BuffType.MagicResistance:
                    text = $"Magic Resistance\n";
                    break;
                case BuffType.Resilience:
                    text = $"Resilience\n";
                    break;
                case BuffType.ElementalSuperiority:
                    text = $"Elemental Superiority\n";
                    break;
                case BuffType.BloodLust:
                    text = $"Blood Lust\n";
                    break;
                case BuffType.StrengthOfFaith:
                    text = $"Strength Of Faith\n";
                    break;
                case BuffType.CelestialLight:
                    text = $"Celestial Light\n";
                    break;
                case BuffType.Transparency:
                    text = $"Transparency\n";
                    break;
                case BuffType.LifeSteal:
                    text = $"Life Steal\n";
                    break;
                case BuffType.Spiritualism:
                    text = $"Spiritualism\n";
                    break;
                case BuffType.PoisonousCloud:
                    text = $"Poisonous Cloud\n";
                    break;
                case BuffType.FullBloom:
                    text = $"Full Bloom\n";
                    break;
                case BuffType.WhiteLotus:
                    text = $"White Lotus\n";
                    break;
                case BuffType.RedLotus:
                    text = $"Red Lotus\n";
                    break;
                case BuffType.Cloak:
                    text = $"Cloak\n";
                    break;
                case BuffType.GhostWalk:
                    text = $"Ghost Walk\n\n" +
                           $"Allows you to move faster whilst cloaked.";
                    break;
                case BuffType.TheNewBeginning:
                    text = $"The New Beginning\n";
                    break;
                case BuffType.DarkConversion:
                    text = $"Dark Conversion\n";
                    break;
                case BuffType.DragonRepulse:
                    text = $"Dragon Repulse\n";
                    break;
                case BuffType.Evasion:
                    text = $"Evasion\n";
                    break;
                case BuffType.RagingWind:
                    text = $"Raging Wind\n";
                    break;
                case BuffType.Concentration:
                    text = $"Concentration\n";
                    break;
                case BuffType.LastStand:
                    text = $"Last Stand\n";
                    break;
                case BuffType.Invincibility:
                    text = $"Invincibility\n";
                    break;
                case BuffType.ElementalHurricane:
                    text = $"Elemental Hurricane\n";
                    break;
                case BuffType.SuperiorMagicShield:
                    text = $"Superior Magic Shield\n";
                    break;
                case BuffType.DefensiveBlow:
                    text = $"Defensive Blow\n";
                    break;
                case BuffType.SoulResonance:
                    text = $"Soul Resonance\n";
                    break;
                case BuffType.MagicWeakness:
                    text = $"Magic Weakness\n\n" +
                           $"Your Magic Resistance has been greatly reduced.\n";
                    break;
            }
            
            if (stats != null && stats.Count > 0)
            {
                foreach (KeyValuePair<Stat, int> pair in stats.Values)
                {
                    if (pair.Key == Stat.Duration) continue;

                    string temp = stats.GetDisplay(pair.Key);

                    if (temp == null) continue;
                    text += $"\n{temp}";
                }

                if (buff.RemainingTime != TimeSpan.MaxValue)
                    text += $"\n";
            }

            if (buff.RemainingTime != TimeSpan.MaxValue)
                text += $"\nDuration: {Functions.ToString(buff.RemainingTime, true)}";

            if (buff.Pause) text += "\nPaused (Not in Effect).";

            return text;
        }

        public override void Process()
        {
            base.Process();

            foreach (KeyValuePair<ClientBuffInfo, DXImageControl> pair in Icons)
            {
                if (pair.Key.Pause)
                {
                    pair.Value.ForeColour = Color.IndianRed;
                    continue;
                }
                    if (pair.Key.RemainingTime == TimeSpan.MaxValue) continue;

                if (pair.Key.RemainingTime.TotalSeconds >= 10)
                {
                    pair.Value.ForeColour = Color.White;
                    continue;
                }
                
                float rate = pair.Key.RemainingTime.Milliseconds / (float)1000;

                pair.Value.ForeColour = Functions.Lerp(Color.White, Color.CadetBlue, rate);
            }

            Hint = Icons.Count > 0 ? null : "Buff Area";
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (KeyValuePair<ClientBuffInfo, DXImageControl> pair in Icons)
                {
                    if (pair.Value == null) continue;

                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                Icons.Clear();
                Icons = null;
            }

        }

        #endregion
    }

}
