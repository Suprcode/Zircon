using Library.SystemModels;
using MirDB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class Functions
    {
        public static TimeSpan Max(TimeSpan value1, TimeSpan value2)
        {
            return value1 > value2 ? value1 : value2;
        }
        public static TimeSpan Min(TimeSpan value1, TimeSpan value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        public static Element GetAttackElement(Stats stats)
        {
            Element attackElement = Element.None;
            int value = 0;

            if (stats[Stat.FireAttack] > value)
            {
                attackElement = Element.Fire;
                value = stats[Stat.FireAttack];
            }

            if (stats[Stat.IceAttack] > value)
            {
                attackElement = Element.Ice;
                value = stats[Stat.IceAttack];
            }

            if (stats[Stat.LightningAttack] > value)
            {
                attackElement = Element.Lightning;
                value = stats[Stat.LightningAttack];
            }

            if (stats[Stat.WindAttack] > value)
            {
                attackElement = Element.Wind;
                value = stats[Stat.WindAttack];
            }

            if (stats[Stat.HolyAttack] > value)
            {
                attackElement = Element.Holy;
                value = stats[Stat.HolyAttack];
            }

            if (stats[Stat.DarkAttack] > value)
            {
                attackElement = Element.Dark;
                value = stats[Stat.DarkAttack];
            }

            if (stats[Stat.PhantomAttack] > value)
            {
                attackElement = Element.Phantom;
            }

            return attackElement;
        }

        public static Element GetAffinityElement(Stats stats)
        {
            Element affinityElement = Element.None;
            int value = 0;

            if (stats[Stat.FireAffinity] > value)
            {
                affinityElement = Element.Fire;
                value = stats[Stat.FireAffinity];
            }

            if (stats[Stat.IceAffinity] > value)
            {
                affinityElement = Element.Ice;
                value = stats[Stat.IceAffinity];
            }

            if (stats[Stat.LightningAffinity] > value)
            {
                affinityElement = Element.Lightning;
                value = stats[Stat.LightningAffinity];
            }

            if (stats[Stat.WindAffinity] > value)
            {
                affinityElement = Element.Wind;
                value = stats[Stat.WindAffinity];
            }

            if (stats[Stat.HolyAffinity] > value)
            {
                affinityElement = Element.Holy;
                value = stats[Stat.HolyAffinity];
            }

            if (stats[Stat.DarkAffinity] > value)
            {
                affinityElement = Element.Dark;
                value = stats[Stat.DarkAffinity];
            }

            if (stats[Stat.PhantomAffinity] > value)
            {
                affinityElement = Element.Phantom;
            }

            return affinityElement;
        }

        public static MirAnimation GetAttackAnimation(MirClass @class, int weaponShape, MagicType magicType)
        {
            MirAnimation animation;
     
            switch (magicType)
            {
                case MagicType.Slaying:
                case MagicType.Thrusting:
                case MagicType.FlamingSword:
                case MagicType.DefensiveBlow:
                    animation = MirAnimation.Combat3;
                    break;
                case MagicType.HalfMoon:
                case MagicType.DestructiveSurge:
                    animation = MirAnimation.Combat4;
                    break;
                case MagicType.DragonRise:
                    animation = MirAnimation.Combat5;
                    break;
                case MagicType.BladeStorm:
                    animation = MirAnimation.Combat6;
                    break;
                case MagicType.FullBloom:
                case MagicType.WhiteLotus:
                case MagicType.RedLotus:
                case MagicType.DanceOfSwallow:
                    if (weaponShape >= 1200)
                        animation = MirAnimation.Combat13;
                    else if (weaponShape >= 1100)
                        animation = MirAnimation.Combat5;
                    else
                        animation = MirAnimation.Combat3;
                    break;
                case MagicType.SweetBrier:
                case MagicType.Karma:
                    if (weaponShape >= 1200)
                        animation = MirAnimation.Combat12;
                    else if (weaponShape >= 1100)
                        animation = MirAnimation.Combat10;
                    else
                        animation = MirAnimation.Combat3;
                    break;
                default:
                    switch (@class)
                    {
                        case MirClass.Assassin:
                            if (weaponShape >= 1200)
                                animation = MirAnimation.Combat11;
                            else if (weaponShape >= 1100)
                                animation = MirAnimation.Combat4;
                            else
                                animation = MirAnimation.Combat3;
                            break;
                        default:
                            animation = MirAnimation.Combat3;
                            //switch weapon shape
                            break;
                    }

                    break;
            }

            return animation;
        }

        public static MirAnimation GetMagicAnimation(MagicType m)
        {
            switch (m)
            {
                case MagicType.Beckon:
                case MagicType.MassBeckon:
                case MagicType.ElementalSwords:

                case MagicType.FireBall:
                case MagicType.IceBolt:
                case MagicType.LightningBall:
                case MagicType.GustBlast:
                case MagicType.ScortchedEarth:
                case MagicType.LightningBeam:
                case MagicType.AdamantineFireBall:
                case MagicType.FireBounce:
                case MagicType.IceBlades:
                case MagicType.FrozenEarth:
                case MagicType.MeteorShower:
                case MagicType.LightningStrike:

                case MagicType.ExplosiveTalisman:
                case MagicType.EvilSlayer:
                case MagicType.MagicResistance:
                case MagicType.Resilience:
                case MagicType.MassInvisibility:
                case MagicType.GreaterEvilSlayer:
                case MagicType.GreaterFrozenEarth:
                case MagicType.Parasite:
                case MagicType.ElementalSuperiority:
                case MagicType.BloodLust:
                case MagicType.LifeSteal:
                case MagicType.ImprovedExplosiveTalisman:
                case MagicType.Neutralize:
                case MagicType.CorpseExploder:
                case MagicType.SoulResonance:
                case MagicType.SearingLight:

                case MagicType.Hemorrhage:
                    return MirAnimation.Combat1;

                case MagicType.Interchange:

                case MagicType.Repulsion:
                case MagicType.ElectricShock:
                case MagicType.LightningWave:
                case MagicType.Cyclone:
                case MagicType.Teleportation:
                case MagicType.FireWall:
                case MagicType.FireStorm:
                case MagicType.BlowEarth:
                case MagicType.ExpelUndead:
                case MagicType.MagicShield:
                case MagicType.IceStorm:
                case MagicType.DragonTornado:
                case MagicType.ChainLightning:
                case MagicType.GeoManipulation:
                case MagicType.Transparency:
                case MagicType.ThunderBolt:
                case MagicType.Renounce:
                case MagicType.FrostBite:
                case MagicType.Tempest:
                case MagicType.JudgementOfHeaven:
                case MagicType.ThunderStrike:
                case MagicType.MirrorImage:
                case MagicType.Asteroid:
                case MagicType.SuperiorMagicShield:
                case MagicType.IceRain:
                case MagicType.Tornado:

                case MagicType.Heal:
                case MagicType.PoisonDust:
                case MagicType.Invisibility:
                case MagicType.TrapOctagon:
                case MagicType.MassHeal:
                case MagicType.Resurrection:
                case MagicType.Purification:
                case MagicType.SummonSkeleton:
                case MagicType.SummonJinSkeleton:
                case MagicType.SummonShinsu:
                case MagicType.StrengthOfFaith:
                case MagicType.CelestialLight:
                case MagicType.AugmentPoisonDust:
                case MagicType.SummonDemonicCreature:
                case MagicType.DemonExplosion:
                case MagicType.CursedDoll:
                case MagicType.DarkSoulPrison:
                case MagicType.SummonDead:
                    return MirAnimation.Combat2;

                case MagicType.ElementalHurricane:
                    return MirAnimation.ChannellingStart;

                case MagicType.PoisonousCloud:
                case MagicType.SummonPuppet:
                case MagicType.Containment:
                    return MirAnimation.Combat14;

                case MagicType.DragonRepulse:
                    return MirAnimation.DragonRepulseStart;

                case MagicType.ThunderKick:
                case MagicType.CombatKick:
                    return MirAnimation.Combat7;

                case MagicType.Cloak:
                case MagicType.WraithGrip:
                case MagicType.HellFire:
                case MagicType.TheNewBeginning:
                case MagicType.DarkConversion:
                case MagicType.Abyss:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.Concentration:
                case MagicType.BurningFire:
                case MagicType.Chain:
                    return MirAnimation.Combat9;

                case MagicType.Rake:
                case MagicType.MagicCombustion:
                    return MirAnimation.Combat5;

                case MagicType.FlashOfLight:
                    return MirAnimation.Combat10;

                case MagicType.Defiance:
                case MagicType.Might:
                case MagicType.ReflectDamage:
                case MagicType.Fetter:
                case MagicType.Endurance:
                case MagicType.Invincibility:

                case MagicType.Spiritualism:
                    return MirAnimation.Combat15;

                case MagicType.Shuriken:
                case MagicType.SwiftBlade:
                case MagicType.SeismicSlam:
                case MagicType.CrushingWave:
                    return MirAnimation.Combat3;

                default:
                    throw new NotImplementedException();
            }
        }

        public static Color GetElementColour(Element element)
        {
            Color colour = Globals.NoneColour;

            switch (element)
            {
                case Element.Fire:
                    colour = Globals.FireColour;
                    break;
                case Element.Ice:
                    colour = Globals.IceColour;
                    break;
                case Element.Lightning:
                    colour = Globals.LightningColour;
                    break;
                case Element.Wind:
                    colour = Globals.WindColour;
                    break;
                case Element.Holy:
                    colour = Globals.HolyColour;
                    break;
                case Element.Dark:
                    colour = Globals.DarkColour;
                    break;
                case Element.Phantom:
                    colour = Globals.PhantomColour;
                    break;
            }

            return colour;
        }

        public static bool IsMatch(byte[] a, byte[] b, long offSet = 0)
        {
            if (b == null || a == null || b.Length + offSet > a.Length || offSet < 0) return false;

            for (int i = 0; i < b.Length; i++)
                if (a[offSet + i] != b[i])
                    return false;

            return true;
        }
        public static bool IsMatch(Stats a, Stats b)
        {
            if (a == null || b == null || a.Values.Count != b.Values.Count) return false;

            foreach (KeyValuePair<Stat, int> pair in a.Values)
                if (pair.Value != b[pair.Key]) return false;

            return true;
        }

        public static Color Lerp(Color source, Color destination, float rate)
        {
            if (rate >= 1)
                return destination;

            if (rate <= 0)
                return source;

            int a = destination.A - source.A, r = destination.R - source.R, g = destination.G - source.G, b = destination.B - source.B;

            return Color.FromArgb(Math.Min(byte.MaxValue, source.A + (int)(a * rate)), Math.Min(byte.MaxValue, source.R + (int)(r * rate)),
                                  Math.Min(byte.MaxValue, source.G + (int)(g * rate)), Math.Min(byte.MaxValue, source.B + (int)(b * rate)));
        }

        public static int Distance(Point source, Point dest)
        {
            return Math.Max(Math.Abs(source.X - dest.X), Math.Abs(source.Y - dest.Y));
        }

        /// <summary>
        /// How far away two points are when you can only move Up, Down, Left or Right. (No Diagonal)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static int Distance4Directions(Point source, Point dest)
        {
            return Math.Abs(source.X - dest.X) + Math.Abs(source.Y - dest.Y);
        }
        public static bool InRange(Point a, Point b, int i)
        {
            return InRange(a.X, a.Y, b.X, b.Y, i);
        }
        public static bool InRange(int x1, int y1, int x2, int y2, int i)
        {
            return Math.Abs(x1 - x2) <= i && Math.Abs(y1 - y2) <= i;
        }
        public static MirDirection DirectionFromPoint(Point source, Point dest)
        {
            if (source.X < dest.X)
            {
                if (source.Y < dest.Y)
                    return MirDirection.DownRight;
                if (source.Y > dest.Y)
                    return MirDirection.UpRight;
                return MirDirection.Right;
            }

            if (source.X > dest.X)
            {
                if (source.Y < dest.Y)
                    return MirDirection.DownLeft;
                if (source.Y > dest.Y)
                    return MirDirection.UpLeft;
                return MirDirection.Left;
            }

            return source.Y < dest.Y ? MirDirection.Down : MirDirection.Up;
        }
        public static double Distance(PointF p1, PointF p2)
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;
            return Math.Sqrt(x * x + y * y);
        }
        public static MirDirection ShiftDirection(MirDirection dir, int i)
        {
            return (MirDirection)(((int)dir + i + 8) % 8);
        }
        public static Point Move(Point location, MirDirection direction, int distance = 1)
        {
            switch (direction)
            {
                case MirDirection.Up:
                    location.Offset(0, -distance);
                    break;
                case MirDirection.UpRight:
                    location.Offset(distance, -distance);
                    break;
                case MirDirection.Right:
                    location.Offset(distance, 0);
                    break;
                case MirDirection.DownRight:
                    location.Offset(distance, distance);
                    break;
                case MirDirection.Down:
                    location.Offset(0, distance);
                    break;
                case MirDirection.DownLeft:
                    location.Offset(-distance, distance);
                    break;
                case MirDirection.Left:
                    location.Offset(-distance, 0);
                    break;
                case MirDirection.UpLeft:
                    location.Offset(-distance, -distance);
                    break;
            }
            return location;
        }
        public static bool CorrectSlot(ItemType type, EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    return type == ItemType.Weapon;
                case EquipmentSlot.Armour:
                    return type == ItemType.Armour;
                case EquipmentSlot.Helmet:
                    return type == ItemType.Helmet;
                case EquipmentSlot.Torch:
                    return type == ItemType.Torch;
                case EquipmentSlot.Necklace:
                    return type == ItemType.Necklace;
                case EquipmentSlot.BraceletL:
                case EquipmentSlot.BraceletR:
                    return type == ItemType.Bracelet;
                case EquipmentSlot.RingL:
                case EquipmentSlot.RingR:
                    return type == ItemType.Ring;
                case EquipmentSlot.Shoes:
                    return type == ItemType.Shoes;
                case EquipmentSlot.Poison:
                    return type == ItemType.Poison;
                case EquipmentSlot.Amulet:
                    return type == ItemType.Amulet || type == ItemType.DarkStone;
                case EquipmentSlot.Flower:
                    return type == ItemType.Flower;
                case EquipmentSlot.Emblem:
                    return type == ItemType.Emblem;
                case EquipmentSlot.HorseArmour:
                    return type == ItemType.HorseArmour;
                case EquipmentSlot.Shield:
                    return type == ItemType.Shield;
                case EquipmentSlot.Costume:
                    return type == ItemType.Costume;
                case EquipmentSlot.Hook:
                    return type == ItemType.Hook;
                case EquipmentSlot.Float:
                    return type == ItemType.Float;
                case EquipmentSlot.Bait:
                    return type == ItemType.Bait;
                case EquipmentSlot.Finder:
                    return type == ItemType.Finder;
                case EquipmentSlot.Reel:
                    return type == ItemType.Reel;
                default:
                    return false;
            }
        }
        public static bool CorrectSlot(ItemType type, CompanionSlot slot)
        {
            switch (slot)
            {
                case CompanionSlot.Bag:
                    return type == ItemType.CompanionBag;
                case CompanionSlot.Head:
                    return type == ItemType.CompanionHead;
                case CompanionSlot.Back:
                    return type == ItemType.CompanionBack;
                case CompanionSlot.Food:
                    return type == ItemType.CompanionFood;
                default:
                    return false;
            }
        }

        public static double DegreesToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }

        public static int Direction16(Point source, Point destination)
        {
            PointF c = new PointF(source.X, source.Y);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF(destination.X, destination.Y);
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (destination.X < c.X) angle = 360 - angle;
            angle += 11.25F;
            if (angle > 360) angle -= 360;

            return (int)(angle / 22.5F);
        }

        public static string ToString(TimeSpan time, bool details, bool small = false)
        {
            string textD = null;
            string textH = null;
            string textM = null;
            string textS = null;

            if (time.Days >= 2) textD = $"{time.Days} {(small ? "Ds" : "Days")}";
            else if (time.Days >= 1) textD = $"{time.Days} {(small ? "D" : "Day")}";

            if (time.Hours >= 2) textH = $"{time.Hours} {(small ? "Hrs" : "Hours")}";
            else if (time.Hours >= 1) textH = $"{time.Hours} {(small ? "H" : "Hour")}";

            if (time.Minutes >= 2) textM = $"{time.Minutes} {(small ? "Mins" : "Minutes")}";
            else if (time.Minutes >= 1) textM = $"{time.Minutes} {(small ? "Min" : "Minute")}";

            if (time.Seconds >= 2) textS = $"{time.Seconds} {(small ? "Secs" : "Seconds")}";
            else if (time.Seconds >= 1) textS = $"{time.Seconds} {(small ? "Sec" : "Second")}";
            else if (time.TotalSeconds > 0) textS = "less than a second";

            if (!details)
                return textD ?? textH ?? textM ?? textS;

            if (textD != null)
                return textD + " " + (textH ?? textM ?? textS);

            if (textH != null)
                return textH + " " + (textM ?? textS);

            if (textM != null)
                return textM + " " + textS;

            return textS ?? string.Empty;
        }

        public static string RandomString(Random Random, int length)
        {
            StringBuilder str = new StringBuilder();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

            for (int i = 0; i < length; i++)
                str.Append(chars[Random.Next(chars.Length)]);

            return str.ToString();
        }

        public static bool ValidFishingDistance(int throwDistance, int throwLevel)
        {
            switch (throwLevel)
            {
                default:
                case 1:
                    return throwDistance <= 4;
                case 2:
                    return throwDistance <= 6;
                case 3:
                    return throwDistance <= 8;
                case 4:
                    return throwDistance <= 9;
            }
        }

        public static int FishingThrowQuality(int throwDistance)
        {
            switch (throwDistance)
            {
                default:
                case 4:
                    return 1;
                case 5:
                case 6:
                    return 2;
                case 7:
                case 8:
                    return 3;
                case 9:
                    return 4;
            }
        }

        public static FishingInfo FishingZone(DBCollection<FishingInfo> col, MapInfo info, int mapWidth, int mapHeight, Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.X > mapWidth || location.Y > mapHeight)
                return null;

            foreach (var zone in col.Binding)
            {
                if (zone.Region != null && zone.Region.Map == info)
                {
                    if (zone.Region.PointList == null)
                        zone.Region.CreatePoints(mapWidth);

                    if (zone.Region.PointList.Contains(location))
                    {
                        return zone;
                    }
                }
            }

            return null;

        }

        public static Point[] CreateGridPoints(int gridSize, int middleIgnore = -1)
        {
            List<Point> gridPoints = new List<Point>();

            // Iterate through X and Y coordinates
            for (int x = -gridSize; x <= gridSize; x++)
            {
                for (int y = -gridSize; y <= gridSize; y++)
                {
                    if (middleIgnore >= 0 && Math.Abs(x) <= middleIgnore && Math.Abs(y) <= middleIgnore) continue;

                    gridPoints.Add(new Point(x, y));
                }
            }

            return gridPoints.ToArray();
        }

        public static string BreakStringIntoLines(string input, int maxLineLength)
        {
            // Split the input string into an array of words
            string[] words = input.Split(' ');

            // Initialize variables
            int currentLineLength = 0;
            string result = "";

            // Iterate through the words
            foreach (string word in words)
            {
                // Check if adding the current word exceeds the maxLineLength limit
                if (currentLineLength + word.Length + 1 <= maxLineLength)
                {
                    // Add the word and a space to the result
                    result += word + " ";
                    currentLineLength += word.Length + 1;
                }
                else
                {
                    // Start a new line and reset the line length
                    result += "\n" + word + " ";
                    currentLineLength = word.Length + 1;
                }
            }

            // Trim any leading or trailing whitespace
            result = result.Trim();

            return result;
        }
    }
}
