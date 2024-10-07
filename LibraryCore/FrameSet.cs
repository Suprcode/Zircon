using System;
using System.Collections.Generic;

namespace Library
{
    public sealed class FrameSet
    {
        public static Dictionary<MirAnimation, Frame> Players;

        public static Dictionary<MirAnimation, Frame> DefaultItem;

        public static Dictionary<MirAnimation, Frame> DefaultNPC;

        public static Dictionary<MirAnimation, Frame> DefaultMonster;


        public static Dictionary<MirAnimation, Frame>
            ForestYeti, ChestnutTree, CarnivorousPlant,
            DevouringGhost,
            Larva,
            ZumaGuardian, ZumaKing,
            Monkey,
            NumaMage, CursedCactus, NetherWorldGate,
            WestDesertLizard,
            BanyaGuard, EmperorSaWoo,
            JinchonDevil,
            ArchLichTaeda,
            ShinsuBig,
            PachonTheChaosBringer,
            IcySpiritGeneral,
            FieryDancer, EmeraldDancer, QueenOfDawn,
            JinamStoneGate, OYoungBeast, YumgonWitch, JinhwanSpirit, ChiwooGeneral, DragonQueen, DragonLord,
            FerociousIceTiger,
            SamaFireGuardian, Phoenix, EnshrinementBox, BloodStone, SamaCursedBladesman, SamaCursedSlave, SamaProphet, SamaSorcerer,
            EasterEvent,
            OrangeTiger, RedTiger, OrangeBossTiger, BigBossTiger,

            SDMob3, SDMob8, SDMob15, SDMob16, SDMob17, SDMob18, SDMob19, SDMob21, SDMob22, SDMob23, SDMob24, SDMob25, SDMob26,

            LobsterLord, LobsterSpawn,

            DeadTree, BobbitWorm,
            MonasteryMon1, MonasteryMon3,

            Terracotta1, Terracotta2, Terracotta3, TerracottaSub, TerracottaBoss,

            SeaHorseCavalry, Seamancer, CoralStoneDuin, Brachiopod, GiantClam, BlueMassif, Mollusk, GiantClam1,

            Tornado,

            InfernalSoldier,

            CastleFlag, SabukGate;


        static FrameSet()
        {
            Players = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Running] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.CreepStanding] = new Frame(1680, 4, 10, TimeSpan.FromMilliseconds(500)), //Stealth Standing
                [MirAnimation.CreepWalkFast] = new Frame(1760, 6, 10, TimeSpan.FromMilliseconds(100)), //Stealth Walk
                [MirAnimation.CreepWalkSlow] = new Frame(1760, 6, 10, TimeSpan.FromMilliseconds(200)), //Stealth Walk
                [MirAnimation.Pushed] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                //[MirAnimation.Pushed2] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true },
                [MirAnimation.Stance] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Harvest] = new Frame(480, 2, 10, TimeSpan.FromMilliseconds(300)),
                [MirAnimation.Combat1] = new Frame(560, 5, 10, TimeSpan.FromMilliseconds(100)), //Proj Spell
                [MirAnimation.Combat2] = new Frame(640, 5, 10, TimeSpan.FromMilliseconds(100)), //Target Spell
                [MirAnimation.Combat3] = new Frame(720, 6, 10, TimeSpan.FromMilliseconds(100)), //Default Attack (WWT)
                [MirAnimation.Combat4] = new Frame(800, 6, 10, TimeSpan.FromMilliseconds(100)), //Default 1 Handed (Sin)
                [MirAnimation.Combat5] = new Frame(880, 10, 10, TimeSpan.FromMilliseconds(60)), //Lotus 1 Handed
                [MirAnimation.Combat6] = new Frame(960, 10, 10, TimeSpan.FromMilliseconds(60)), // Blade Storm ??
                [MirAnimation.Combat7] = new Frame(1040, 10, 10, TimeSpan.FromMilliseconds(100)), //Kick
                [MirAnimation.Combat8] = new Frame(1120, 6, 10, TimeSpan.FromMilliseconds(50)) { StaticSpeed = true }, //Dash
                [MirAnimation.Combat9] = new Frame(1200, 10, 10, TimeSpan.FromMilliseconds(100)), //Evasion Cast
                [MirAnimation.Combat10] = new Frame(1280, 10, 10, TimeSpan.FromMilliseconds(60)), //Sweet Brier 1 Handed 
                [MirAnimation.Combat11] = new Frame(1360, 10, 10, TimeSpan.FromMilliseconds(60)), //Duel Wield default
                [MirAnimation.Combat12] = new Frame(1440, 10, 10, TimeSpan.FromMilliseconds(60)), //Sweet brier Duel wield
                [MirAnimation.Combat13] = new Frame(1520, 6, 10, TimeSpan.FromMilliseconds(100)), //Lotus Duel wield
                [MirAnimation.Combat14] = new Frame(1600, 8, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
                [MirAnimation.Combat15] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.DragonRepulseStart] = new Frame(1600, 6, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
                [MirAnimation.DragonRepulseMiddle] = new Frame(1605, 1, 10, TimeSpan.FromMilliseconds(1000)), //Summon Puppet ?
                [MirAnimation.DragonRepulseEnd] = new Frame(1606, 2, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
                [MirAnimation.Struck] = new Frame(1840, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(1920, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(1929, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.FishingCast] = new Frame(2000, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.FishingWait] = new Frame(2080, 6, 10, TimeSpan.FromMilliseconds(120)),
                [MirAnimation.FishingReel] = new Frame(2160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.HorseStanding] = new Frame(2240, 4, 10, TimeSpan.FromMilliseconds(500)), //Horse Standing
                [MirAnimation.HorseWalking] = new Frame(2320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.HorseRunning] = new Frame(2400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.HorseStruck] = new Frame(2480, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.ChannellingStart] = new Frame(560, 4, 10, TimeSpan.FromMilliseconds(100)), //Proj Spell (channelled)
                [MirAnimation.ChannellingMiddle] = new Frame(563, 1, 10, TimeSpan.FromMilliseconds(1000)), //Proj Spell (channelled)
                [MirAnimation.ChannellingEnd] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(60)), //Proj Spell (channelled)
                //Repeated?
            };

            Players[MirAnimation.Combat1].Delays[1] = TimeSpan.FromMilliseconds(200);
            Players[MirAnimation.Combat2].Delays[3] = TimeSpan.FromMilliseconds(200);

            /*
            Assassin = new Dictionary<MirAction, Frame>
            {
                [MirAction.Standing] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAction.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Running] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Stance] = new Frame(560, 3, 10, TimeSpan.FromMilliseconds(500)),

                [MirAction.Harvest] = new Frame(480, 2, 10, TimeSpan.FromMilliseconds(300)),
                [MirAction.Attack1] = new Frame(720, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Struck] = new Frame(1840, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Die] = new Frame(1920, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Dead] = new Frame(1929, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };*/

            DefaultItem = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(1000)),
            };

            DefaultNPC = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 4, 0, TimeSpan.FromMilliseconds(1000)),
            };

            DefaultMonster = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(329, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Skeleton] = new Frame(880, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Show] = new Frame(640, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Hide] = new Frame(640, 10, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
                [MirAnimation.StoneStanding] = new Frame(640, 1, 10, TimeSpan.FromMilliseconds(500)),
                //Die etc etc
            };

            ForestYeti = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Die] = new Frame(320, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(323, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };


            ChestnutTree = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            CarnivorousPlant = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 4, 0, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Show] = new Frame(640, 8, 0, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
                [MirAnimation.Hide] = new Frame(640, 8, 0, TimeSpan.FromMilliseconds(100)),
            };

            DevouringGhost = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            Larva = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(500)),
            };

            ZumaGuardian = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(640, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            ZumaKing = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(640, 20, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.StoneStanding] = new Frame(640, 1, 0, TimeSpan.FromMilliseconds(500)),
            };

            Monkey = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            NetherWorldGate = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 0, TimeSpan.FromMilliseconds(200)),
            };

            CursedCactus = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            NumaMage = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat3] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            WestDesertLizard = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            BanyaGuard = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            JinchonDevil = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Combat2] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Combat3] = new Frame(480, 8, 10, TimeSpan.FromMilliseconds(70)),
            };


            EmperorSaWoo = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            ArchLichTaeda = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(720, 20, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(739, 1, 20, TimeSpan.FromMilliseconds(500)),
            };
            
            PachonTheChaosBringer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.DragonRepulseStart] = new Frame(480, 7, 10, TimeSpan.FromMilliseconds(100)), 
                [MirAnimation.DragonRepulseMiddle] = new Frame(486, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.DragonRepulseEnd] = new Frame(487, 3, 10, TimeSpan.FromMilliseconds(100)), 
            };

            IcySpiritGeneral = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat3] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            FieryDancer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),
                //Die etc etc
            };


            EmeraldDancer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 20, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 20, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(320, 20, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(480, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(560, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(569, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            QueenOfDawn = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            OYoungBeast = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 5, 10, TimeSpan.FromMilliseconds(100)),
            };

            YumgonWitch = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),
            };

            JinhwanSpirit = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            ChiwooGeneral = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            DragonQueen = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(327, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            DragonLord = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),
            };

            FerociousIceTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(480, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(560, 16, 0, TimeSpan.FromMilliseconds(40)),
                [MirAnimation.Combat3] = new Frame(560, 16, 0, TimeSpan.FromMilliseconds(100)),
            };
            SamaFireGuardian = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            Phoenix = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(489, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            EnshrinementBox = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Die] = new Frame(80, 10, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(89, 1, 0, TimeSpan.FromMilliseconds(500)),
            };

            BloodStone = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 4, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(240, 2, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Die] = new Frame(320, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 0, TimeSpan.FromMilliseconds(500)),
            };
            SamaCursedBladesman = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            SamaCursedSlave = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            SamaProphet = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(50, 4, 0, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(130, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(210, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(290, 10, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(370, 3, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(450, 10, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(459, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            SamaSorcerer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(489, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            EasterEvent = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Show] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Hide] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
                [MirAnimation.StoneStanding] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.DragonRepulseStart] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
                [MirAnimation.DragonRepulseMiddle] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(1000)), //Summon Puppet ?
                [MirAnimation.DragonRepulseEnd] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
            };

            OrangeTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            RedTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            OrangeBossTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 0, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                //Unknown 240 3 fame
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)), //Roar
                [MirAnimation.Combat3] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(406, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            BigBossTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 0, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },

                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                //Unknown 240 3 fame
                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(329, 1, 10, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Combat2] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)), //Stab

                [MirAnimation.Combat3] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),//Roar

                [MirAnimation.Combat4] = new Frame(560, 10, 10, TimeSpan.FromMilliseconds(100)),//Roar 2

            };

            SDMob3 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(640, 10, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
                [MirAnimation.Hide] = new Frame(640, 10, 10, TimeSpan.FromMilliseconds(100)) ,
            };

            SDMob8 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            SDMob15 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),
                
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Struck] = new Frame(320, 4, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            SDMob16 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Walking] = new Frame(80, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 7, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },

                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)), //Bugged?

                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            SDMob17 = new Dictionary<MirAnimation, Frame>
            {

                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            SDMob18 = new Dictionary<MirAnimation, Frame>
            {

                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            SDMob19 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),

                //  [MirAnimation.Show] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)),
                //    [MirAnimation.Hide] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
            };

            SDMob21 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),

                //  [MirAnimation.Show] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)),
                //    [MirAnimation.Hide] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
            };

            SDMob22 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Combat1] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),

                //  [MirAnimation.Show] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)),
                //    [MirAnimation.Hide] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
            };
            SDMob23 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },

                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(70)),

                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(327, 1, 10, TimeSpan.FromMilliseconds(500)),

                //  [MirAnimation.Show] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)),
                //    [MirAnimation.Hide] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
            };

            SDMob24 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },

                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(70)),

                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Combat2] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(70)),
            };

            SDMob25 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },

                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(70)),

                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(70)),
            };

            SDMob26 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },

                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(70)),

                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(70)),

                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            LobsterLord = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(20, 6, 0, TimeSpan.FromMilliseconds(500)),

                [MirAnimation.Combat1] = new Frame(30, 7, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(40, 7, 0, TimeSpan.FromMilliseconds(100)), //Right Side, Left Claw
                [MirAnimation.Combat3] = new Frame(60, 7, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat4] = new Frame(70, 7, 0, TimeSpan.FromMilliseconds(100)), //Left Side, Right Claw?
                [MirAnimation.Combat5] = new Frame(80, 7, 0, TimeSpan.FromMilliseconds(100)), //Left Side, Right Claw?
                [MirAnimation.Combat6] = new Frame(110, 8, 0, TimeSpan.FromMilliseconds(100)), //Left Side, Right Claw?
                [MirAnimation.Combat7] = new Frame(120, 4, 0, TimeSpan.FromMilliseconds(100)), //Left Side, Right Claw?

                [MirAnimation.Struck] = new Frame(50, 4, 0, TimeSpan.FromMilliseconds(100)),
                
                [MirAnimation.Die] = new Frame(130, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(138, 1, 0, TimeSpan.FromMilliseconds(500)),
            };

            JinamStoneGate = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
            };


            DeadTree = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Die] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Dead] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
            };

            MonasteryMon1 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 15, 20, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(488, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };
            MonasteryMon3 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 15, 20, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(480, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(560, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(568, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };


            Terracotta1 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(160, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 11, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(490, 1, 20, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Hide] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            Terracotta2 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(160, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 12, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(491, 1, 20, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Hide] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            Terracotta3 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(160, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(489, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Hide] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            TerracottaSub = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(160, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(480, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(560, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(572, 1, 20, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Hide] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            TerracottaBoss = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 11, 20, TimeSpan.FromMilliseconds(120)),
                [MirAnimation.Dead] = new Frame(411, 1, 20, TimeSpan.FromMilliseconds(1000)),
            };

            BobbitWorm = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)) ,
                [MirAnimation.Hide] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            Tornado = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(0, 10, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Standing] = new Frame(10, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(10, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(10, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Hide] = new Frame(20, 7, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(20, 7, 0, TimeSpan.FromMilliseconds(100)),
            };

            InfernalSoldier = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Show] = new Frame(480, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Hide] = new Frame(480, 9, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true }
            };

            SeaHorseCavalry = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(329, 1, 10, TimeSpan.FromMilliseconds(1000))
            };
            
            Seamancer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(329, 1, 10, TimeSpan.FromMilliseconds(1000))
            };
            
            CoralStoneDuin = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 5, 10, TimeSpan.FromMilliseconds(100)),        
                [MirAnimation.Die] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(490, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Show] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Hide] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true }
            };
            
            Brachiopod = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(560, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),        
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(410, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Show] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100))
            };
            
            GiantClam = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 5, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(330, 1, 10, TimeSpan.FromMilliseconds(1000))
            };
            
            BlueMassif = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(410, 1, 10, TimeSpan.FromMilliseconds(1000))
            };
            
            Mollusk = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 5, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(330, 1, 10, TimeSpan.FromMilliseconds(1000))
            };
            
            GiantClam1 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 8, 10, TimeSpan.FromMilliseconds(100)),
                //Walking?
                //Pushed?
                //Combat1?
                [MirAnimation.Struck] = new Frame(240, 5, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(488, 1, 10, TimeSpan.FromMilliseconds(1000))
            };

            CastleFlag = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(0, 10, 0, TimeSpan.FromMilliseconds(100)),
            };

            SabukGate = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(640, 7, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(640, 7, 0, TimeSpan.FromMilliseconds(100)) { Reversed = true },
                [MirAnimation.Die] = new Frame(320, 8, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(327, 1, 0, TimeSpan.FromMilliseconds(1000))
            };
        }
    }


    public sealed class Frame
    {
        public static Frame EmptyFrame = new Frame(0, 0, 0, TimeSpan.Zero);

        public int StartIndex;
        public int FrameCount;
        public int OffSet;

        public bool Reversed, StaticSpeed;

        public TimeSpan[] Delays; //Index = Duration to freeze
        public double Sum
        {
            get
            {
                TimeSpan sum = TimeSpan.Zero;
                foreach (var timeSpan in Delays)
                    sum = sum.Add(timeSpan);
                return sum.TotalMilliseconds;
            }
        }

        public Frame(int startIndex, int frameCount, int offSet, TimeSpan frameDelay)
        {
            StartIndex = startIndex;
            FrameCount = frameCount;
            OffSet = offSet;

            Delays = new TimeSpan[FrameCount];
            for (int i = 0; i < Delays.Length; i++)
                Delays[i] = frameDelay;
        }
        public Frame(Frame frame)
        {
            StartIndex = frame.StartIndex;
            FrameCount = frame.FrameCount;
            OffSet = frame.OffSet;

            Delays = new TimeSpan[FrameCount];
            for (int i = 0; i < Delays.Length; i++)
                Delays[i] = frame.Delays[i];
        }
        public int GetFrame(DateTime start, DateTime now, bool doubleSpeed)
        {
            TimeSpan enlapsed = now - start;

            if (doubleSpeed && !StaticSpeed)
                enlapsed += enlapsed;


            if (Reversed)
            {
                for (int i = 0; i < Delays.Length; i++)
                {
                    enlapsed -= Delays[Delays.Length - 1 - i];
                    if (enlapsed >= TimeSpan.Zero) continue;

                    return i;
                }
            }
            else
            {
                for (int i = 0; i < Delays.Length; i++)
                {
                    enlapsed -= Delays[i];
                    if (enlapsed >= TimeSpan.Zero) continue;

                    return i;
                }
            }


            return FrameCount;
        }
    }


}
