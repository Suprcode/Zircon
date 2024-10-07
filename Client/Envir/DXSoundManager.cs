using Library;
using SlimDX.DirectSound;
using System;
using System.Collections.Generic;


namespace Client.Envir
{
    public static class DXSoundManager
    {
        private const string SoundPath = @".\Sound\";

        public static DirectSound Device;
        public static bool Error;

        #region SoundList

        public static Dictionary<SoundIndex, DXSound> SoundList = new Dictionary<SoundIndex, DXSound>
        {
            #region Music
            [SoundIndex.LoginScene] = new DXSound(SoundPath + @"Opening.wav", SoundType.Music) { Loop = true },
            [SoundIndex.LoginScene2] = new DXSound(SoundPath + @"Main.wav", SoundType.Music) { Loop = true },
            [SoundIndex.LoginScene3] = new DXSound(SoundPath + @"Ending.wav", SoundType.Music) { Loop = true },
            [SoundIndex.SelectScene] = new DXSound(SoundPath + @"SelChr.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B000] = new DXSound(SoundPath + @"B000.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B100] = new DXSound(SoundPath + @"B100.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B2] = new DXSound(SoundPath + @"B2.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B400] = new DXSound(SoundPath + @"B400.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B8] = new DXSound(SoundPath + @"B8.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD00] = new DXSound(SoundPath + @"BD00.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD01] = new DXSound(SoundPath + @"BD01.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD02] = new DXSound(SoundPath + @"BD02.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD041] = new DXSound(SoundPath + @"BD041.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD042] = new DXSound(SoundPath + @"BD042.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD210] = new DXSound(SoundPath + @"BD210.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD50] = new DXSound(SoundPath + @"BD50.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD60] = new DXSound(SoundPath + @"BD60.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD70] = new DXSound(SoundPath + @"BD70.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD99] = new DXSound(SoundPath + @"BD99.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BDUnderseaCave] = new DXSound(SoundPath + @"BDUnderseaCave.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BDUnderseaCaveBoss] = new DXSound(SoundPath + @"BDUnderseaCaveBoss.wav", SoundType.Music) { Loop = true },

            #endregion

            #region Players
            [SoundIndex.Foot1] = new DXSound(SoundPath + @"1.wav", SoundType.Player),
            [SoundIndex.Foot2] = new DXSound(SoundPath + @"2.wav", SoundType.Player),
            [SoundIndex.Foot3] = new DXSound(SoundPath + @"3.wav", SoundType.Player),
            [SoundIndex.Foot4] = new DXSound(SoundPath + @"4.wav", SoundType.Player),

            [SoundIndex.FishingCast] = new DXSound(SoundPath + @"84.wav", SoundType.Player),
            [SoundIndex.FishingBob] = new DXSound(SoundPath + @"85.wav", SoundType.Player),
            [SoundIndex.FishingReel] = new DXSound(SoundPath + @"86.wav", SoundType.Player),

            [SoundIndex.HorseWalk1] = new DXSound(SoundPath + @"33.wav", SoundType.Player),
            [SoundIndex.HorseWalk2] = new DXSound(SoundPath + @"34.wav", SoundType.Player),
            [SoundIndex.HorseRun] = new DXSound(SoundPath + @"35.wav", SoundType.Player),

            [SoundIndex.GenericStruckPlayer] = new DXSound(SoundPath + @"61.wav", SoundType.Player),

            [SoundIndex.DaggerSwing] = new DXSound(SoundPath + @"50.wav", SoundType.Player),
            [SoundIndex.WoodSwing] = new DXSound(SoundPath + @"51.wav", SoundType.Player),
            [SoundIndex.IronSwordSwing] = new DXSound(SoundPath + @"52.wav", SoundType.Player),
            [SoundIndex.ShortSwordSwing] = new DXSound(SoundPath + @"53.wav", SoundType.Player),
            [SoundIndex.AxeSwing] = new DXSound(SoundPath + @"54.wav", SoundType.Player),
            [SoundIndex.ClubSwing] = new DXSound(SoundPath + @"55.wav", SoundType.Player),
            [SoundIndex.WandSwing] = new DXSound(SoundPath + @"56.wav", SoundType.Player),
            [SoundIndex.FistSwing] = new DXSound(SoundPath + @"57.wav", SoundType.Player),
            [SoundIndex.GlaiveAttack] = new DXSound(SoundPath + @"63.wav", SoundType.Player),
            [SoundIndex.ClawAttack] = new DXSound(SoundPath + @"64.wav", SoundType.Player),

            [SoundIndex.MaleStruck] = new DXSound(SoundPath + @"138.wav", SoundType.Player),
            [SoundIndex.FemaleStruck] = new DXSound(SoundPath + @"139.wav", SoundType.Player),
            [SoundIndex.MaleDie] = new DXSound(SoundPath + @"144.wav", SoundType.Player),
            [SoundIndex.FemaleDie] = new DXSound(SoundPath + @"145.wav", SoundType.Player),

            #endregion

            #region System
            [SoundIndex.ButtonA] = new DXSound(SoundPath + @"103.wav", SoundType.System),
            [SoundIndex.ButtonB] = new DXSound(SoundPath + @"104.wav", SoundType.System),
            [SoundIndex.ButtonC] = new DXSound(SoundPath + @"105.wav", SoundType.System),

            [SoundIndex.SelectWarriorMale] = new DXSound(SoundPath + @"JMCre.wav", SoundType.System),
            [SoundIndex.SelectWarriorFemale] = new DXSound(SoundPath + @"JWCre.wav", SoundType.System),
            [SoundIndex.SelectWizardMale] = new DXSound(SoundPath + @"SMCre.wav", SoundType.System),
            [SoundIndex.SelectWizardFemale] = new DXSound(SoundPath + @"SWCre.wav", SoundType.System),
            [SoundIndex.SelectTaoistMale] = new DXSound(SoundPath + @"DMCre.wav", SoundType.System),
            [SoundIndex.SelectTaoistFemale] = new DXSound(SoundPath + @"DWCre.wav", SoundType.System),
            [SoundIndex.SelectAssassinMale] = new DXSound(SoundPath + @"AMCre.wav", SoundType.System),
            [SoundIndex.SelectAssassinFemale] = new DXSound(SoundPath + @"AWCre.wav", SoundType.System),

            [SoundIndex.TeleportIn] = new DXSound(SoundPath + @"109.wav", SoundType.System),
            [SoundIndex.TeleportOut] = new DXSound(SoundPath + @"110.wav", SoundType.System),

            [SoundIndex.ItemPotion] = new DXSound(SoundPath + @"108.wav", SoundType.System),
            [SoundIndex.ItemWeapon] = new DXSound(SoundPath + @"111.wav", SoundType.System),
            [SoundIndex.ItemArmour] = new DXSound(SoundPath + @"112.wav", SoundType.System),
            [SoundIndex.ItemRing] = new DXSound(SoundPath + @"113.wav", SoundType.System),
            [SoundIndex.ItemBracelet] = new DXSound(SoundPath + @"114.wav", SoundType.System),
            [SoundIndex.ItemNecklace] = new DXSound(SoundPath + @"115.wav", SoundType.System),
            [SoundIndex.ItemHelmet] = new DXSound(SoundPath + @"116.wav", SoundType.System),
            [SoundIndex.ItemShoes] = new DXSound(SoundPath + @"117.wav", SoundType.System),
            [SoundIndex.ItemDefault] = new DXSound(SoundPath + @"118.wav", SoundType.System),

            [SoundIndex.GoldPickUp] = new DXSound(SoundPath + @"120.wav", SoundType.System),
            [SoundIndex.GoldGained] = new DXSound(SoundPath + @"122.wav", SoundType.System),

            [SoundIndex.RollDice] = new DXSound(SoundPath + @"dice_roll.wav", SoundType.System),
            [SoundIndex.RollYut] = new DXSound(SoundPath + @"yut_sticks.wav", SoundType.System),

            [SoundIndex.QuestTake] = new DXSound(SoundPath + @"Qtake.wav", SoundType.System),
            [SoundIndex.QuestComplete] = new DXSound(SoundPath + @"Qcomp.wav", SoundType.System),

            #endregion

            #region Magic
            [SoundIndex.SlayingMale] = new DXSound(SoundPath + @"M7-1.wav", SoundType.Magic),
            [SoundIndex.SlayingFemale] = new DXSound(SoundPath + @"M7-2.wav", SoundType.Magic),

            [SoundIndex.EnergyBlast] = new DXSound(SoundPath + @"M12-1.wav", SoundType.Magic),

            [SoundIndex.HalfMoon] = new DXSound(SoundPath + @"M25-1.wav", SoundType.Magic),

            [SoundIndex.FlamingSword] = new DXSound(SoundPath + @"M26-3.wav", SoundType.Magic),
            [SoundIndex.DragonRise] = new DXSound(SoundPath + @"M26-1.wav", SoundType.Magic),
            [SoundIndex.BladeStorm] = new DXSound(SoundPath + @"M34-1.wav", SoundType.Magic),
            [SoundIndex.DefensiveBlow] = new DXSound(SoundPath + @"M140-1.wav", SoundType.Magic),

            [SoundIndex.DestructiveSurge] = new DXSound(SoundPath + @"M103-1.wav", SoundType.Magic),

            [SoundIndex.DefianceStart] = new DXSound(SoundPath + @"M106-3.wav", SoundType.Magic),

            [SoundIndex.ReflectDamageStart] = new DXSound(SoundPath + @"M126-0.wav", SoundType.Magic),
            
            [SoundIndex.InvincibilityStart] = new DXSound(SoundPath + @"M137-2.wav", SoundType.Magic),
            
            [SoundIndex.AssaultStart] = new DXSound(SoundPath + @"M109-1.wav", SoundType.Magic),

            [SoundIndex.SwiftBladeEnd] = new DXSound(SoundPath + @"M131-2.wav", SoundType.Magic),
            
            [SoundIndex.ElementalSwordStart] = new DXSound(SoundPath + @"cs252-1.wav", SoundType.Magic),
            [SoundIndex.ElementalSwordEnd] = new DXSound(SoundPath + @"cs252-2.wav", SoundType.Magic),

            [SoundIndex.FireBallStart] = new DXSound(SoundPath + @"M1-1.wav", SoundType.Magic),
            [SoundIndex.FireBallTravel] = new DXSound(SoundPath + @"M1-2.wav", SoundType.Magic),
            [SoundIndex.FireBallEnd] = new DXSound(SoundPath + @"M1-3.wav", SoundType.Magic),

            [SoundIndex.ThunderBoltStart] = new DXSound(SoundPath + @"M41-1.wav", SoundType.Magic),
            [SoundIndex.ThunderBoltTravel] = new DXSound(SoundPath + @"M41-2.wav", SoundType.Magic),
            [SoundIndex.ThunderBoltEnd] = new DXSound(SoundPath + @"M41-3.wav", SoundType.Magic),

            [SoundIndex.IceBoltStart] = new DXSound(SoundPath + @"M39-1.wav", SoundType.Magic),
            [SoundIndex.IceBoltTravel] = new DXSound(SoundPath + @"M39-2.wav", SoundType.Magic),
            [SoundIndex.IceBoltEnd] = new DXSound(SoundPath + @"M39-3.wav", SoundType.Magic),

            [SoundIndex.GustBlastStart] = new DXSound(SoundPath + @"M67-1.wav", SoundType.Magic),
            [SoundIndex.GustBlastTravel] = new DXSound(SoundPath + @"M67-2.wav", SoundType.Magic),
            [SoundIndex.GustBlastEnd] = new DXSound(SoundPath + @"M67-3.wav", SoundType.Magic),

            [SoundIndex.RepulsionEnd] = new DXSound(SoundPath + @"M8-2.wav", SoundType.Magic),

            [SoundIndex.ElectricShockStart] = new DXSound(SoundPath + @"M20-1.wav", SoundType.Magic),
            [SoundIndex.ElectricShockEnd] = new DXSound(SoundPath + @"M20-3.wav", SoundType.Magic),

            [SoundIndex.GreaterFireBallStart] = new DXSound(SoundPath + @"M5-1.wav", SoundType.Magic),
            [SoundIndex.GreaterFireBallTravel] = new DXSound(SoundPath + @"M5-2.wav", SoundType.Magic),
            [SoundIndex.GreaterFireBallEnd] = new DXSound(SoundPath + @"M5-3.wav", SoundType.Magic),

            [SoundIndex.LightningStrikeStart] = new DXSound(SoundPath + @"M11-1.wav", SoundType.Magic),
            [SoundIndex.LightningStrikeEnd] = new DXSound(SoundPath + @"M11-2.wav", SoundType.Magic),

            [SoundIndex.GreaterIceBoltStart] = new DXSound(SoundPath + @"M40-1.wav", SoundType.Magic),
            [SoundIndex.GreaterIceBoltTravel] = new DXSound(SoundPath + @"M40-2.wav", SoundType.Magic),
            [SoundIndex.GreaterIceBoltEnd] = new DXSound(SoundPath + @"M40-3.wav", SoundType.Magic),

            [SoundIndex.CycloneStart] = new DXSound(SoundPath + @"M74-1.wav", SoundType.Magic),
            [SoundIndex.CycloneEnd] = new DXSound(SoundPath + @"M74-3.wav", SoundType.Magic),

            [SoundIndex.LavaStrikeStart] = new DXSound(SoundPath + @"M9-1.wav", SoundType.Magic),
            //[SoundIndex.LavaStrikeEnd] = new DXSound(SoundPath + @"M9-3.wav", SoundType.Magic),

            [SoundIndex.LightningBeamEnd] = new DXSound(SoundPath + @"M10-1.wav", SoundType.Magic),

            [SoundIndex.TeleportationStart] = new DXSound(SoundPath + @"M21-1.wav", SoundType.Magic),

            [SoundIndex.FireWallStart] = new DXSound(SoundPath + @"M22-1.wav", SoundType.Magic),
            [SoundIndex.FireWallEnd] = new DXSound(SoundPath + @"M22-2.wav", SoundType.Magic),

            [SoundIndex.FireStormStart] = new DXSound(SoundPath + @"M23-1.wav", SoundType.Magic),
            [SoundIndex.FireStormEnd] = new DXSound(SoundPath + @"M23-3.wav", SoundType.Magic),

            [SoundIndex.LightningWaveStart] = new DXSound(SoundPath + @"M24-1.wav", SoundType.Magic),
            [SoundIndex.LightningWaveEnd] = new DXSound(SoundPath + @"M24-2.wav", SoundType.Magic),

            [SoundIndex.FrozenEarthStart] = new DXSound(SoundPath + @"M53-1.wav", SoundType.Magic),
            [SoundIndex.FrozenEarthEnd] = new DXSound(SoundPath + @"M53-3.wav", SoundType.Magic),

            [SoundIndex.BlowEarthStart] = new DXSound(SoundPath + @"M73-1.wav", SoundType.Magic),
            [SoundIndex.BlowEarthTravel] = new DXSound(SoundPath + @"M73-3.wav", SoundType.Magic),
            [SoundIndex.BlowEarthEnd] = new DXSound(SoundPath + @"M73-3.wav", SoundType.Magic),


            [SoundIndex.ExpelUndeadStart] = new DXSound(SoundPath + @"M32-1.wav", SoundType.Magic),
            [SoundIndex.ExpelUndeadStart] = new DXSound(SoundPath + @"M32-3.wav", SoundType.Magic),
            [SoundIndex.MagicShieldStart] = new DXSound(SoundPath + @"M31-1.wav", SoundType.Magic),

            [SoundIndex.IceStormStart] = new DXSound(SoundPath + @"M33-1.wav", SoundType.Magic),
            [SoundIndex.IceStormEnd] = new DXSound(SoundPath + @"M33-3.wav", SoundType.Magic),

            [SoundIndex.DragonTornadoStart] = new DXSound(SoundPath + @"M72-1.wav", SoundType.Magic),
            [SoundIndex.DragonTornadoEnd] = new DXSound(SoundPath + @"M72-3.wav", SoundType.Magic),

            [SoundIndex.GreaterFrozenEarthStart] = new DXSound(SoundPath + @"M53-1.wav", SoundType.Magic),
            [SoundIndex.GreaterFrozenEarthEnd] = new DXSound(SoundPath + @"M53-3.wav", SoundType.Magic),

            [SoundIndex.ChainLightningStart] = new DXSound(SoundPath + @"M111-1.wav", SoundType.Magic),
            [SoundIndex.ChainLightningEnd] = new DXSound(SoundPath + @"M111-3.wav", SoundType.Magic),
            
            [SoundIndex.FrostBiteStart] = new DXSound(SoundPath + @"m135-2.wav", SoundType.Magic),

            [SoundIndex.ParasiteTravel] = new DXSound(SoundPath + @"m139-1.wav", SoundType.Magic),
            [SoundIndex.ParasiteExplode] = new DXSound(SoundPath + @"m139-2.wav", SoundType.Magic),

            [SoundIndex.ElementalHurricane] = new DXSound(SoundPath + @"m141-1.wav", SoundType.Magic),

            [SoundIndex.TornadoStart] = new DXSound(SoundPath + @"cs255-1.wav", SoundType.Magic),

            [SoundIndex.HealStart] = new DXSound(SoundPath + @"M2-1.wav", SoundType.Magic),
            [SoundIndex.HealEnd] = new DXSound(SoundPath + @"M2-3.wav", SoundType.Magic),

            [SoundIndex.PoisonDustStart] = new DXSound(SoundPath + @"M6-1.wav", SoundType.Magic),
            [SoundIndex.PoisonDustEnd] = new DXSound(SoundPath + @"M6-3.wav", SoundType.Magic),

            [SoundIndex.ExplosiveTalismanStart] = new DXSound(SoundPath + @"M13-1.wav", SoundType.Magic),
            [SoundIndex.ExplosiveTalismanTravel] = new DXSound(SoundPath + @"M13-2.wav", SoundType.Magic),
            [SoundIndex.ExplosiveTalismanEnd] = new DXSound(SoundPath + @"M13-3.wav", SoundType.Magic),

            [SoundIndex.HolyStrikeStart] = new DXSound(SoundPath + @"M37-1.wav", SoundType.Magic),
            [SoundIndex.HolyStrikeTravel] = new DXSound(SoundPath + @"M37-2.wav", SoundType.Magic),
            [SoundIndex.HolyStrikeEnd] = new DXSound(SoundPath + @"M37-3.wav", SoundType.Magic),

            [SoundIndex.ImprovedHolyStrikeStart] = new DXSound(SoundPath + @"M38-1.wav", SoundType.Magic),
            [SoundIndex.ImprovedHolyStrikeTravel] = new DXSound(SoundPath + @"M38-2.wav", SoundType.Magic),
            [SoundIndex.ImprovedHolyStrikeEnd] = new DXSound(SoundPath + @"M38-3.wav", SoundType.Magic),

            [SoundIndex.MagicResistanceTravel] = new DXSound(SoundPath + @"M14-2.wav", SoundType.Magic),
            [SoundIndex.MagicResistanceEnd] = new DXSound(SoundPath + @"M14-3.wav", SoundType.Magic),

            [SoundIndex.ResilienceTravel] = new DXSound(SoundPath + @"M15-2.wav", SoundType.Magic),
            [SoundIndex.ResilienceEnd] = new DXSound(SoundPath + @"M15-3.wav", SoundType.Magic),

            [SoundIndex.ShacklingTalismanStart] = new DXSound(SoundPath + @"M16-1.wav", SoundType.Magic),
            [SoundIndex.ShacklingTalismanEnd] = new DXSound(SoundPath + @"M16-3.wav", SoundType.Magic),

            [SoundIndex.SummonSkeletonStart] = new DXSound(SoundPath + @"M17-1.wav", SoundType.Magic),
            [SoundIndex.SummonSkeletonEnd] = new DXSound(SoundPath + @"M17-3.wav", SoundType.Magic),

            [SoundIndex.CursedDollEnd] = new DXSound(SoundPath + @"M137-2.wav", SoundType.Magic),

            [SoundIndex.InvisibilityEnd] = new DXSound(SoundPath + @"M18-1.wav", SoundType.Magic),

            [SoundIndex.MassInvisibilityTravel] = new DXSound(SoundPath + @"M19-2.wav", SoundType.Magic),
            [SoundIndex.MassInvisibilityEnd] = new DXSound(SoundPath + @"M19-3.wav", SoundType.Magic),

            [SoundIndex.TaoistCombatKickStart] = new DXSound(SoundPath + @"M36-1.wav", SoundType.Magic),

            [SoundIndex.MassHealStart] = new DXSound(SoundPath + @"M29-1.wav", SoundType.Magic),
            [SoundIndex.MassHealEnd] = new DXSound(SoundPath + @"M29-3.wav", SoundType.Magic),

            [SoundIndex.BloodLustTravel] = new DXSound(SoundPath + @"M94-2.wav", SoundType.Magic),
            [SoundIndex.BloodLustEnd] = new DXSound(SoundPath + @"M94-3.wav", SoundType.Magic),

            [SoundIndex.ResurrectionStart] = new DXSound(SoundPath + @"M77-1.wav", SoundType.Magic),

            [SoundIndex.PurificationStart] = new DXSound(SoundPath + @"M120-1.wav", SoundType.Magic),
            [SoundIndex.PurificationEnd] = new DXSound(SoundPath + @"M120-3.wav", SoundType.Magic),

            [SoundIndex.SummonShinsuStart] = new DXSound(SoundPath + @"M30-1.wav", SoundType.Magic),
            [SoundIndex.SummonShinsuEnd] = new DXSound(SoundPath + @"M30-3.wav", SoundType.Magic),

            [SoundIndex.StrengthOfFaithStart] = new DXSound(SoundPath + @"M123-1.wav", SoundType.Magic),
            [SoundIndex.StrengthOfFaithEnd] = new DXSound(SoundPath + @"M123-3.wav", SoundType.Magic),

            [SoundIndex.NeutralizeTravel] = new DXSound(SoundPath + @"M19-2.wav", SoundType.Magic),
            [SoundIndex.NeutralizeEnd] = new DXSound(SoundPath + @"m138-2.wav", SoundType.Magic),

            [SoundIndex.DarkSoulPrison] = new DXSound(SoundPath + @"m136-2.wav", SoundType.Magic),

            [SoundIndex.SummonDeadEnd] = new DXSound(SoundPath + @"cs258-1.wav", SoundType.Magic),

            [SoundIndex.PoisonousCloudStart] = new DXSound(SoundPath + @"as_157-1.wav", SoundType.Magic),

            [SoundIndex.CloakStart] = new DXSound(SoundPath + @"as_163.wav", SoundType.Magic),

            [SoundIndex.FullBloom] = new DXSound(SoundPath + @"as_165.wav", SoundType.Magic),
            [SoundIndex.WhiteLotus] = new DXSound(SoundPath + @"as_166.wav", SoundType.Magic),
            [SoundIndex.RedLotus] = new DXSound(SoundPath + @"as_167.wav", SoundType.Magic),

            [SoundIndex.SweetBrier] = new DXSound(SoundPath + @"as_168.wav", SoundType.Magic),
            [SoundIndex.SweetBrierMale] = new DXSound(SoundPath + @"as_168-m.wav", SoundType.Magic),
            [SoundIndex.SweetBrierFemale] = new DXSound(SoundPath + @"as_168-f.wav", SoundType.Magic),
            
            [SoundIndex.CalamityOfFullMoon] = new DXSound(SoundPath + @"as_171.wav", SoundType.Magic),
            [SoundIndex.WaningMoon] = new DXSound(SoundPath + @"as_176.wav", SoundType.Magic),

            [SoundIndex.Karma] = new DXSound(SoundPath + @"as_172.wav", SoundType.Magic),
            [SoundIndex.TheNewBeginning] = new DXSound(SoundPath + @"as_174.wav", SoundType.Magic),
            [SoundIndex.Concentration] = new DXSound(SoundPath + @"M134-2.wav", SoundType.Magic),
            
            [SoundIndex.SummonPuppet] = new DXSound(SoundPath + @"as_164-1.wav", SoundType.Magic),

            [SoundIndex.WraithGripStart] = new DXSound(SoundPath + @"as_159-1.wav", SoundType.Magic),
            [SoundIndex.HellFireStart] = new DXSound(SoundPath + @"as_160-2.wav", SoundType.Magic),

            [SoundIndex.AbyssStart] = new DXSound(SoundPath + @"M14-3.wav", SoundType.Magic),
            [SoundIndex.FlashOfLightEnd] = new DXSound(SoundPath + @"M123-3-1.wav", SoundType.Magic),

            [SoundIndex.RagingWindStart] = new DXSound(SoundPath + @"243-5.wav", SoundType.Magic),
            [SoundIndex.EvasionStart] = new DXSound(SoundPath + @"M26-1.wav", SoundType.Magic),

            [SoundIndex.CorpseExploderEnd] = new DXSound(SoundPath + @"m125-2.wav", SoundType.Magic),

            [SoundIndex.ChainofFireExplode] = new DXSound(SoundPath + @"cs261-1.wav", SoundType.Magic),

            #endregion

            #region Monsters
            [SoundIndex.GenericStruckMonster] = new DXSound(SoundPath + @"61.wav", SoundType.Monster),

            [SoundIndex.ChickenAttack] = new DXSound(SoundPath + @"220-2.wav", SoundType.Monster),
            [SoundIndex.ChickenStruck] = new DXSound(SoundPath + @"220-4.wav", SoundType.Monster),
            [SoundIndex.ChickenDie] = new DXSound(SoundPath + @"220-5.wav", SoundType.Monster),

            [SoundIndex.PigAttack] = new DXSound(SoundPath + @"300-2.wav", SoundType.Monster),
            [SoundIndex.PigStruck] = new DXSound(SoundPath + @"300-4.wav", SoundType.Monster),
            [SoundIndex.PigDie] = new DXSound(SoundPath + @"300-5.wav", SoundType.Monster),

            [SoundIndex.CowAttack] = new DXSound(SoundPath + @"301-2.wav", SoundType.Monster),
            [SoundIndex.CowStruck] = new DXSound(SoundPath + @"301-4.wav", SoundType.Monster),
            [SoundIndex.CowDie] = new DXSound(SoundPath + @"301-5.wav", SoundType.Monster),

            [SoundIndex.DeerAttack] = new DXSound(SoundPath + @"221-2.wav", SoundType.Monster),
            [SoundIndex.DeerStruck] = new DXSound(SoundPath + @"221-4.wav", SoundType.Monster),
            [SoundIndex.DeerDie] = new DXSound(SoundPath + @"221-5.wav", SoundType.Monster),

            [SoundIndex.SheepAttack] = new DXSound(SoundPath + @"258-2.wav", SoundType.Monster),
            [SoundIndex.SheepStruck] = new DXSound(SoundPath + @"258-4.wav", SoundType.Monster),
            [SoundIndex.SheepDie] = new DXSound(SoundPath + @"258-5.wav", SoundType.Monster),

            [SoundIndex.SkyStingerAttack] = new DXSound(SoundPath + @"69-2.wav", SoundType.Monster),
            [SoundIndex.SkyStingerStruck] = new DXSound(SoundPath + @"69-4.wav", SoundType.Monster),
            [SoundIndex.SkyStingerDie] = new DXSound(SoundPath + @"69-5.wav", SoundType.Monster),

            [SoundIndex.ClawCatAttack] = new DXSound(SoundPath + @"238-2.wav", SoundType.Monster),
            [SoundIndex.ClawCatStruck] = new DXSound(SoundPath + @"238-4.wav", SoundType.Monster),
            [SoundIndex.ClawCatDie] = new DXSound(SoundPath + @"238-5.wav", SoundType.Monster),

            [SoundIndex.WolfAttack] = new DXSound(SoundPath + @"265-2.wav", SoundType.Monster),
            [SoundIndex.WolfStruck] = new DXSound(SoundPath + @"265-4.wav", SoundType.Monster),
            [SoundIndex.WolfDie] = new DXSound(SoundPath + @"265-5.wav", SoundType.Monster),

            [SoundIndex.ForestYetiAttack] = new DXSound(SoundPath + @"230-2.wav", SoundType.Monster),
            [SoundIndex.ForestYetiStruck] = new DXSound(SoundPath + @"230-4.wav", SoundType.Monster),
            [SoundIndex.ForestYetiDie] = new DXSound(SoundPath + @"230-5.wav", SoundType.Monster),

            [SoundIndex.CarnivorousPlantAttack] = new DXSound(SoundPath + @"231-2.wav", SoundType.Monster),
            [SoundIndex.CarnivorousPlantStruck] = new DXSound(SoundPath + @"231-4.wav", SoundType.Monster),
            [SoundIndex.CarnivorousPlantDie] = new DXSound(SoundPath + @"231-5.wav", SoundType.Monster),

            [SoundIndex.YobAttack] = new DXSound(SoundPath + @"211-2.wav", SoundType.Monster),
            [SoundIndex.YobStruck] = new DXSound(SoundPath + @"211-4.wav", SoundType.Monster),
            [SoundIndex.YobDie] = new DXSound(SoundPath + @"211-5.wav", SoundType.Monster),

            [SoundIndex.OmaAttack] = new DXSound(SoundPath + @"223-2.wav", SoundType.Monster),
            [SoundIndex.OmaStruck] = new DXSound(SoundPath + @"223-4.wav", SoundType.Monster),
            [SoundIndex.OmaDie] = new DXSound(SoundPath + @"223-5.wav", SoundType.Monster),

            [SoundIndex.TigerSnakeAttack] = new DXSound(SoundPath + @"257-2.wav", SoundType.Monster),
            [SoundIndex.TigerSnakeStruck] = new DXSound(SoundPath + @"257-4.wav", SoundType.Monster),
            [SoundIndex.TigerSnakeDie] = new DXSound(SoundPath + @"257-5.wav", SoundType.Monster),

            [SoundIndex.SpittingSpiderAttack] = new DXSound(SoundPath + @"225-2.wav", SoundType.Monster),
            [SoundIndex.SpittingSpiderStruck] = new DXSound(SoundPath + @"225-4.wav", SoundType.Monster),
            [SoundIndex.SpittingSpiderDie] = new DXSound(SoundPath + @"225-5.wav", SoundType.Monster),

            [SoundIndex.ScarecrowAttack] = new DXSound(SoundPath + @"240-2.wav", SoundType.Monster),
            [SoundIndex.ScarecrowStruck] = new DXSound(SoundPath + @"240-4.wav", SoundType.Monster),
            [SoundIndex.ScarecrowDie] = new DXSound(SoundPath + @"240-5.wav", SoundType.Monster),

            [SoundIndex.OmaHeroAttack] = new DXSound(SoundPath + @"224-2.wav", SoundType.Monster),
            [SoundIndex.OmaHeroStruck] = new DXSound(SoundPath + @"224-4.wav", SoundType.Monster),
            [SoundIndex.OmaHeroDie] = new DXSound(SoundPath + @"224-5.wav", SoundType.Monster),

            [SoundIndex.CaveBatAttack] = new DXSound(SoundPath + @"229-2.wav", SoundType.Monster),
            [SoundIndex.CaveBatStruck] = new DXSound(SoundPath + @"229-4.wav", SoundType.Monster),
            [SoundIndex.CaveBatDie] = new DXSound(SoundPath + @"229-5.wav", SoundType.Monster),

            [SoundIndex.ScorpionAttack] = new DXSound(SoundPath + @"228-2.wav", SoundType.Monster),
            [SoundIndex.ScorpionStruck] = new DXSound(SoundPath + @"228-4.wav", SoundType.Monster),
            [SoundIndex.ScorpionDie] = new DXSound(SoundPath + @"228-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonAttack] = new DXSound(SoundPath + @"232-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonStruck] = new DXSound(SoundPath + @"232-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonDie] = new DXSound(SoundPath + @"232-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonAxeManAttack] = new DXSound(SoundPath + @"234-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonAxeManStruck] = new DXSound(SoundPath + @"234-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonAxeManDie] = new DXSound(SoundPath + @"234-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonAxeThrowerAttack] = new DXSound(SoundPath + @"233-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonAxeThrowerStruck] = new DXSound(SoundPath + @"233-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonAxeThrowerDie] = new DXSound(SoundPath + @"233-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonWarriorAttack] = new DXSound(SoundPath + @"235-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonWarriorStruck] = new DXSound(SoundPath + @"235-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonWarriorDie] = new DXSound(SoundPath + @"235-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonLordAttack] = new DXSound(SoundPath + @"236-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonLordStruck] = new DXSound(SoundPath + @"236-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonLordDie] = new DXSound(SoundPath + @"236-5.wav", SoundType.Monster),


            [SoundIndex.CaveMaggotAttack] = new DXSound(SoundPath + @"237-2.wav", SoundType.Monster),
            [SoundIndex.CaveMaggotStruck] = new DXSound(SoundPath + @"237-4.wav", SoundType.Monster),
            [SoundIndex.CaveMaggotDie] = new DXSound(SoundPath + @"237-5.wav", SoundType.Monster),

            [SoundIndex.GhostSorcererAttack] = new DXSound(SoundPath + @"248-2.wav", SoundType.Monster),
            [SoundIndex.GhostSorcererStruck] = new DXSound(SoundPath + @"248-4.wav", SoundType.Monster),
            [SoundIndex.GhostSorcererDie] = new DXSound(SoundPath + @"248-5.wav", SoundType.Monster),

            [SoundIndex.GhostMageAppear] = new DXSound(SoundPath + @"249-0.wav", SoundType.Monster),
            [SoundIndex.GhostMageAttack] = new DXSound(SoundPath + @"249-2.wav", SoundType.Monster),
            [SoundIndex.GhostMageStruck] = new DXSound(SoundPath + @"249-4.wav", SoundType.Monster),
            [SoundIndex.GhostMageDie] = new DXSound(SoundPath + @"249-5.wav", SoundType.Monster),

            [SoundIndex.VoraciousGhostAttack] = new DXSound(SoundPath + @"252-2.wav", SoundType.Monster),
            [SoundIndex.VoraciousGhostStruck] = new DXSound(SoundPath + @"252-4.wav", SoundType.Monster),
            [SoundIndex.VoraciousGhostDie] = new DXSound(SoundPath + @"252-5.wav", SoundType.Monster),

            [SoundIndex.GhoulChampionAttack] = new DXSound(SoundPath + @"253-2.wav", SoundType.Monster),
            [SoundIndex.GhoulChampionStruck] = new DXSound(SoundPath + @"253-4.wav", SoundType.Monster),
            [SoundIndex.GhoulChampionDie] = new DXSound(SoundPath + @"253-5.wav", SoundType.Monster),

            [SoundIndex.ArmoredAntAttack] = new DXSound(SoundPath + @"214-2.wav", SoundType.Monster),
            [SoundIndex.ArmoredAntStruck] = new DXSound(SoundPath + @"214-4.wav", SoundType.Monster),
            [SoundIndex.ArmoredAntDie] = new DXSound(SoundPath + @"214-5.wav", SoundType.Monster),

            [SoundIndex.AntNeedlerAttack] = new DXSound(SoundPath + @"296-2.wav", SoundType.Monster),
            [SoundIndex.AntNeedlerStruck] = new DXSound(SoundPath + @"296-4.wav", SoundType.Monster),
            [SoundIndex.AntNeedlerDie] = new DXSound(SoundPath + @"296-5.wav", SoundType.Monster),

            [SoundIndex.ShellNipperAttack] = new DXSound(SoundPath + @"260-2.wav", SoundType.Monster),
            [SoundIndex.ShellNipperStruck] = new DXSound(SoundPath + @"260-4.wav", SoundType.Monster),
            [SoundIndex.ShellNipperDie] = new DXSound(SoundPath + @"260-5.wav", SoundType.Monster),

            [SoundIndex.VisceralWormAttack] = new DXSound(SoundPath + @"261-2.wav", SoundType.Monster),
            [SoundIndex.VisceralWormStruck] = new DXSound(SoundPath + @"261-4.wav", SoundType.Monster),
            [SoundIndex.VisceralWormDie] = new DXSound(SoundPath + @"261-5.wav", SoundType.Monster),

            [SoundIndex.KeratoidAttack] = new DXSound(SoundPath + @"263-2.wav", SoundType.Monster),
            [SoundIndex.KeratoidStruck] = new DXSound(SoundPath + @"263-4.wav", SoundType.Monster),
            [SoundIndex.KeratoidDie] = new DXSound(SoundPath + @"263-5.wav", SoundType.Monster),

            [SoundIndex.MutantFleaAttack] = new DXSound(SoundPath + @"325-2.wav", SoundType.Monster),
            [SoundIndex.MutantFleaStruck] = new DXSound(SoundPath + @"325-4.wav", SoundType.Monster),
            [SoundIndex.MutantFleaDie] = new DXSound(SoundPath + @"325-5.wav", SoundType.Monster),

            [SoundIndex.PoisonousMutantFleaAttack] = new DXSound(SoundPath + @"326-2.wav", SoundType.Monster),
            [SoundIndex.PoisonousMutantFleaStruck] = new DXSound(SoundPath + @"326-4.wav", SoundType.Monster),
            [SoundIndex.PoisonousMutantFleaDie] = new DXSound(SoundPath + @"326-5.wav", SoundType.Monster),

            [SoundIndex.BlasterMutantFleaAttack] = new DXSound(SoundPath + @"327-2.wav", SoundType.Monster),
            [SoundIndex.BlasterMutantFleaStruck] = new DXSound(SoundPath + @"327-4.wav", SoundType.Monster),
            [SoundIndex.BlasterMutantFleaDie] = new DXSound(SoundPath + @"327-5.wav", SoundType.Monster),


            [SoundIndex.WasHatchlingAttack] = new DXSound(SoundPath + @"271-2.wav", SoundType.Monster),
            [SoundIndex.WasHatchlingStruck] = new DXSound(SoundPath + @"271-4.wav", SoundType.Monster),
            [SoundIndex.WasHatchlingDie] = new DXSound(SoundPath + @"271-5.wav", SoundType.Monster),

            [SoundIndex.CentipedeAttack] = new DXSound(SoundPath + @"266-2.wav", SoundType.Monster),
            [SoundIndex.CentipedeStruck] = new DXSound(SoundPath + @"266-4.wav", SoundType.Monster),
            [SoundIndex.CentipedeDie] = new DXSound(SoundPath + @"266-5.wav", SoundType.Monster),

            [SoundIndex.ButterflyWormAttack] = new DXSound(SoundPath + @"272-2.wav", SoundType.Monster),
            [SoundIndex.ButterflyWormStruck] = new DXSound(SoundPath + @"272-4.wav", SoundType.Monster),
            [SoundIndex.ButterflyWormDie] = new DXSound(SoundPath + @"272-5.wav", SoundType.Monster),

            [SoundIndex.MutantMaggotAttack] = new DXSound(SoundPath + @"268-2.wav", SoundType.Monster),
            [SoundIndex.MutantMaggotStruck] = new DXSound(SoundPath + @"268-4.wav", SoundType.Monster),
            [SoundIndex.MutantMaggotDie] = new DXSound(SoundPath + @"268-5.wav", SoundType.Monster),

            [SoundIndex.EarwigAttack] = new DXSound(SoundPath + @"269-2.wav", SoundType.Monster),
            [SoundIndex.EarwigStruck] = new DXSound(SoundPath + @"269-4.wav", SoundType.Monster),
            [SoundIndex.EarwigDie] = new DXSound(SoundPath + @"269-5.wav", SoundType.Monster),

            [SoundIndex.IronLanceAttack] = new DXSound(SoundPath + @"270-2.wav", SoundType.Monster),
            [SoundIndex.IronLanceStruck] = new DXSound(SoundPath + @"270-4.wav", SoundType.Monster),
            [SoundIndex.IronLanceDie] = new DXSound(SoundPath + @"270-5.wav", SoundType.Monster),

            [SoundIndex.LordNiJaeAttack] = new DXSound(SoundPath + @"267-2.wav", SoundType.Monster),
            [SoundIndex.LordNiJaeStruck] = new DXSound(SoundPath + @"267-4.wav", SoundType.Monster),
            [SoundIndex.LordNiJaeDie] = new DXSound(SoundPath + @"267-5.wav", SoundType.Monster),

            [SoundIndex.RottingGhoulAttack] = new DXSound(SoundPath + @"318-2.wav", SoundType.Monster),
            [SoundIndex.RottingGhoulStruck] = new DXSound(SoundPath + @"318-4.wav", SoundType.Monster),
            [SoundIndex.RottingGhoulDie] = new DXSound(SoundPath + @"318-5.wav", SoundType.Monster),

            [SoundIndex.DecayingGhoulAttack] = new DXSound(SoundPath + @"312-2.wav", SoundType.Monster),
            [SoundIndex.DecayingGhoulStruck] = new DXSound(SoundPath + @"312-4.wav", SoundType.Monster),
            [SoundIndex.DecayingGhoulDie] = new DXSound(SoundPath + @"312-5.wav", SoundType.Monster),

            [SoundIndex.BloodThirstyGhoulAttack] = new DXSound(SoundPath + @"242-2.wav", SoundType.Monster),
            [SoundIndex.BloodThirstyGhoulStruck] = new DXSound(SoundPath + @"242-4.wav", SoundType.Monster),
            [SoundIndex.BloodThirstyGhoulDie] = new DXSound(SoundPath + @"242-5.wav", SoundType.Monster),


            [SoundIndex.SpinedDarkLizardAttack] = new DXSound(SoundPath + @"246-2.wav", SoundType.Monster),
            [SoundIndex.SpinedDarkLizardStruck] = new DXSound(SoundPath + @"246-4.wav", SoundType.Monster),
            [SoundIndex.SpinedDarkLizardDie] = new DXSound(SoundPath + @"246-5.wav", SoundType.Monster),

            [SoundIndex.UmaInfidelAttack] = new DXSound(SoundPath + @"242-2.wav", SoundType.Monster),
            [SoundIndex.UmaInfidelStruck] = new DXSound(SoundPath + @"242-4.wav", SoundType.Monster),
            [SoundIndex.UmaInfidelDie] = new DXSound(SoundPath + @"242-5.wav", SoundType.Monster),

            [SoundIndex.UmaFlameThrowerAttack] = new DXSound(SoundPath + @"243-2.wav", SoundType.Monster),
            [SoundIndex.UmaFlameThrowerStruck] = new DXSound(SoundPath + @"243-4.wav", SoundType.Monster),
            [SoundIndex.UmaFlameThrowerDie] = new DXSound(SoundPath + @"243-5.wav", SoundType.Monster),

            [SoundIndex.UmaAnguisherAttack] = new DXSound(SoundPath + @"244-2.wav", SoundType.Monster),
            [SoundIndex.UmaAnguisherStruck] = new DXSound(SoundPath + @"244-4.wav", SoundType.Monster),
            [SoundIndex.UmaAnguisherDie] = new DXSound(SoundPath + @"244-5.wav", SoundType.Monster),

            [SoundIndex.UmaKingAttack] = new DXSound(SoundPath + @"245-2.wav", SoundType.Monster),
            [SoundIndex.UmaKingStruck] = new DXSound(SoundPath + @"245-4.wav", SoundType.Monster),
            [SoundIndex.UmaKingDie] = new DXSound(SoundPath + @"245-5.wav", SoundType.Monster),


            [SoundIndex.SpiderBatAttack] = new DXSound(SoundPath + @"297-2.wav", SoundType.Monster),
            [SoundIndex.SpiderBatStruck] = new DXSound(SoundPath + @"297-4.wav", SoundType.Monster),
            [SoundIndex.SpiderBatDie] = new DXSound(SoundPath + @"297-5.wav", SoundType.Monster),

            [SoundIndex.ArachnidGazerStruck] = new DXSound(SoundPath + @"304-4.wav", SoundType.Monster),
            [SoundIndex.ArachnidGazerDie] = new DXSound(SoundPath + @"304-5.wav", SoundType.Monster),

            [SoundIndex.LarvaAttack] = new DXSound(SoundPath + @"303-2.wav", SoundType.Monster),
            [SoundIndex.LarvaStruck] = new DXSound(SoundPath + @"303-4.wav", SoundType.Monster),

            [SoundIndex.RedMoonGuardianAttack] = new DXSound(SoundPath + @"305-2.wav", SoundType.Monster),
            [SoundIndex.RedMoonGuardianStruck] = new DXSound(SoundPath + @"305-4.wav", SoundType.Monster),
            [SoundIndex.RedMoonGuardianDie] = new DXSound(SoundPath + @"305-5.wav", SoundType.Monster),

            [SoundIndex.RedMoonProtectorAttack] = new DXSound(SoundPath + @"306-2.wav", SoundType.Monster),
            [SoundIndex.RedMoonProtectorStruck] = new DXSound(SoundPath + @"306-4.wav", SoundType.Monster),
            [SoundIndex.RedMoonProtectorDie] = new DXSound(SoundPath + @"306-5.wav", SoundType.Monster),

            [SoundIndex.VenomousArachnidAttack] = new DXSound(SoundPath + @"307-2.wav", SoundType.Monster),
            [SoundIndex.VenomousArachnidStruck] = new DXSound(SoundPath + @"307-4.wav", SoundType.Monster),
            [SoundIndex.VenomousArachnidDie] = new DXSound(SoundPath + @"307-5.wav", SoundType.Monster),

            [SoundIndex.DarkArachnidAttack] = new DXSound(SoundPath + @"308-2.wav", SoundType.Monster),
            [SoundIndex.DarkArachnidStruck] = new DXSound(SoundPath + @"308-4.wav", SoundType.Monster),
            [SoundIndex.DarkArachnidDie] = new DXSound(SoundPath + @"308-5.wav", SoundType.Monster),

            [SoundIndex.RedMoonTheFallenAttack] = new DXSound(SoundPath + @"302-2.wav", SoundType.Monster),
            [SoundIndex.RedMoonTheFallenStruck] = new DXSound(SoundPath + @"302-4.wav", SoundType.Monster),
            [SoundIndex.RedMoonTheFallenDie] = new DXSound(SoundPath + @"302-5.wav", SoundType.Monster),


            [SoundIndex.ViciousRatAttack] = new DXSound(SoundPath + @"281-2.wav", SoundType.Monster),
            [SoundIndex.ViciousRatStruck] = new DXSound(SoundPath + @"281-4.wav", SoundType.Monster),
            [SoundIndex.ViciousRatDie] = new DXSound(SoundPath + @"281-5.wav", SoundType.Monster),

            [SoundIndex.ZumaSharpShooterAttack] = new DXSound(SoundPath + @"282-2.wav", SoundType.Monster),
            [SoundIndex.ZumaSharpShooterStruck] = new DXSound(SoundPath + @"282-4.wav", SoundType.Monster),
            [SoundIndex.ZumaSharpShooterDie] = new DXSound(SoundPath + @"282-5.wav", SoundType.Monster),

            [SoundIndex.ZumaFanaticAttack] = new DXSound(SoundPath + @"283-2.wav", SoundType.Monster),
            [SoundIndex.ZumaFanaticStruck] = new DXSound(SoundPath + @"283-4.wav", SoundType.Monster),
            [SoundIndex.ZumaFanaticDie] = new DXSound(SoundPath + @"283-5.wav", SoundType.Monster),

            [SoundIndex.ZumaGuardianAttack] = new DXSound(SoundPath + @"284-2.wav", SoundType.Monster),
            [SoundIndex.ZumaGuardianStruck] = new DXSound(SoundPath + @"284-4.wav", SoundType.Monster),
            [SoundIndex.ZumaGuardianDie] = new DXSound(SoundPath + @"284-5.wav", SoundType.Monster),

            [SoundIndex.ZumaKingAppear] = new DXSound(SoundPath + @"285-0.wav", SoundType.Monster),
            [SoundIndex.ZumaKingAttack] = new DXSound(SoundPath + @"285-2.wav", SoundType.Monster),
            [SoundIndex.ZumaKingStruck] = new DXSound(SoundPath + @"285-4.wav", SoundType.Monster),
            [SoundIndex.ZumaKingDie] = new DXSound(SoundPath + @"285-5.wav", SoundType.Monster),


            [SoundIndex.EvilFanaticAttack] = new DXSound(SoundPath + @"335-2.wav", SoundType.Monster),
            [SoundIndex.EvilFanaticStruck] = new DXSound(SoundPath + @"335-4.wav", SoundType.Monster),
            [SoundIndex.EvilFanaticDie] = new DXSound(SoundPath + @"335-5.wav", SoundType.Monster),

            [SoundIndex.MonkeyAttack] = new DXSound(SoundPath + @"332-2.wav", SoundType.Monster),
            [SoundIndex.MonkeyStruck] = new DXSound(SoundPath + @"332-4.wav", SoundType.Monster),
            [SoundIndex.MonkeyDie] = new DXSound(SoundPath + @"332-5.wav", SoundType.Monster),

            [SoundIndex.EvilElephantAttack] = new DXSound(SoundPath + @"336-2.wav", SoundType.Monster),
            [SoundIndex.EvilElephantStruck] = new DXSound(SoundPath + @"336-4.wav", SoundType.Monster),
            [SoundIndex.EvilElephantDie] = new DXSound(SoundPath + @"336-5.wav", SoundType.Monster),

            [SoundIndex.CannibalFanaticAttack] = new DXSound(SoundPath + @"334-2.wav", SoundType.Monster),
            [SoundIndex.CannibalFanaticStruck] = new DXSound(SoundPath + @"334-4.wav", SoundType.Monster),
            [SoundIndex.CannibalFanaticDie] = new DXSound(SoundPath + @"334-5.wav", SoundType.Monster),


            [SoundIndex.SpikedBeetleAttack] = new DXSound(SoundPath + @"264-2.wav", SoundType.Monster),
            [SoundIndex.SpikedBeetleStruck] = new DXSound(SoundPath + @"264-4.wav", SoundType.Monster),
            [SoundIndex.SpikedBeetleDie] = new DXSound(SoundPath + @"264-5.wav", SoundType.Monster),

            [SoundIndex.NumaGruntAttack] = new DXSound(SoundPath + @"309-2.wav", SoundType.Monster),
            [SoundIndex.NumaGruntStruck] = new DXSound(SoundPath + @"309-4.wav", SoundType.Monster),
            [SoundIndex.NumaGruntDie] = new DXSound(SoundPath + @"309-5.wav", SoundType.Monster),

            [SoundIndex.NumaMageAttack] = new DXSound(SoundPath + @"213-2.wav", SoundType.Monster),
            [SoundIndex.NumaMageStruck] = new DXSound(SoundPath + @"213-4.wav", SoundType.Monster),
            [SoundIndex.NumaMageDie] = new DXSound(SoundPath + @"213-5.wav", SoundType.Monster),

            [SoundIndex.NumaEliteAttack] = new DXSound(SoundPath + @"217-2.wav", SoundType.Monster),
            [SoundIndex.NumaEliteStruck] = new DXSound(SoundPath + @"217-4.wav", SoundType.Monster),
            [SoundIndex.NumaEliteDie] = new DXSound(SoundPath + @"217-5.wav", SoundType.Monster),

            [SoundIndex.SandSharkAttack] = new DXSound(SoundPath + @"342-2.wav", SoundType.Monster),
            [SoundIndex.SandSharkStruck] = new DXSound(SoundPath + @"342-4.wav", SoundType.Monster),
            [SoundIndex.SandSharkDie] = new DXSound(SoundPath + @"342-5.wav", SoundType.Monster),

            [SoundIndex.StoneGolemAppear] = new DXSound(SoundPath + @"204-0.wav", SoundType.Monster),
            [SoundIndex.StoneGolemAttack] = new DXSound(SoundPath + @"204-2.wav", SoundType.Monster),
            [SoundIndex.StoneGolemStruck] = new DXSound(SoundPath + @"204-4.wav", SoundType.Monster),
            [SoundIndex.StoneGolemDie] = new DXSound(SoundPath + @"204-5.wav", SoundType.Monster),

            [SoundIndex.WindfurySorceressAttack] = new DXSound(SoundPath + @"344-2.wav", SoundType.Monster),
            [SoundIndex.WindfurySorceressStruck] = new DXSound(SoundPath + @"344-4.wav", SoundType.Monster),
            [SoundIndex.WindfurySorceressDie] = new DXSound(SoundPath + @"344-5.wav", SoundType.Monster),

            [SoundIndex.CursedCactusAttack] = new DXSound(SoundPath + @"294-2.wav", SoundType.Monster),
            [SoundIndex.CursedCactusStruck] = new DXSound(SoundPath + @"294-4.wav", SoundType.Monster),
            [SoundIndex.CursedCactusDie] = new DXSound(SoundPath + @"294-5.wav", SoundType.Monster),

            [SoundIndex.RagingLizardAttack] = new DXSound(SoundPath + @"233-2.wav", SoundType.Monster),
            [SoundIndex.RagingLizardStruck] = new DXSound(SoundPath + @"233-4.wav", SoundType.Monster),
            [SoundIndex.RagingLizardDie] = new DXSound(SoundPath + @"233-5.wav", SoundType.Monster),

            [SoundIndex.SawToothLizardAttack] = new DXSound(SoundPath + @"212-2.wav", SoundType.Monster),
            [SoundIndex.SawToothLizardStruck] = new DXSound(SoundPath + @"212-4.wav", SoundType.Monster),
            [SoundIndex.SawToothLizardDie] = new DXSound(SoundPath + @"212-5.wav", SoundType.Monster),

            [SoundIndex.MutantLizardAttack] = new DXSound(SoundPath + @"234-2.wav", SoundType.Monster),
            [SoundIndex.MutantLizardStruck] = new DXSound(SoundPath + @"234-4.wav", SoundType.Monster),
            [SoundIndex.MutantLizardDie] = new DXSound(SoundPath + @"234-5.wav", SoundType.Monster),

            [SoundIndex.VenomSpitterAttack] = new DXSound(SoundPath + @"235-2.wav", SoundType.Monster),
            [SoundIndex.VenomSpitterStruck] = new DXSound(SoundPath + @"235-4.wav", SoundType.Monster),
            [SoundIndex.VenomSpitterDie] = new DXSound(SoundPath + @"235-5.wav", SoundType.Monster),

            [SoundIndex.SonicLizardAttack] = new DXSound(SoundPath + @"325-2.wav", SoundType.Monster),
            [SoundIndex.SonicLizardStruck] = new DXSound(SoundPath + @"325-4.wav", SoundType.Monster),
            [SoundIndex.SonicLizardDie] = new DXSound(SoundPath + @"325-5.wav", SoundType.Monster),

            [SoundIndex.GiantLizardAttack] = new DXSound(SoundPath + @"244-2.wav", SoundType.Monster),
            [SoundIndex.GiantLizardStruck] = new DXSound(SoundPath + @"244-4.wav", SoundType.Monster),
            [SoundIndex.GiantLizardDie] = new DXSound(SoundPath + @"244-5.wav", SoundType.Monster),

            [SoundIndex.CrazedLizardAttack] = new DXSound(SoundPath + @"335-2.wav", SoundType.Monster),
            [SoundIndex.CrazedLizardStruck] = new DXSound(SoundPath + @"335-4.wav", SoundType.Monster),
            [SoundIndex.CrazedLizardDie] = new DXSound(SoundPath + @"335-5.wav", SoundType.Monster),

            [SoundIndex.TaintedTerrorAttack] = new DXSound(SoundPath + @"361-2.wav", SoundType.Monster),
            [SoundIndex.TaintedTerrorStruck] = new DXSound(SoundPath + @"361-4.wav", SoundType.Monster),
            [SoundIndex.TaintedTerrorDie] = new DXSound(SoundPath + @"361-5.wav", SoundType.Monster),
            [SoundIndex.TaintedTerrorAttack2] = new DXSound(SoundPath + @"361-8.wav", SoundType.Monster),

            [SoundIndex.DeathLordJichonAttack] = new DXSound(SoundPath + @"362-2.wav", SoundType.Monster),
            [SoundIndex.DeathLordJichonStruck] = new DXSound(SoundPath + @"362-4.wav", SoundType.Monster),
            [SoundIndex.DeathLordJichonDie] = new DXSound(SoundPath + @"362-5.wav", SoundType.Monster),
            [SoundIndex.DeathLordJichonAttack2] = new DXSound(SoundPath + @"M25-1.wav", SoundType.Monster),
            [SoundIndex.DeathLordJichonAttack3] = new DXSound(SoundPath + @"362-8.wav", SoundType.Monster),



            [SoundIndex.MinotaurAttack] = new DXSound(SoundPath + @"317-2.wav", SoundType.Monster),
            [SoundIndex.MinotaurStruck] = new DXSound(SoundPath + @"317-4.wav", SoundType.Monster),
            [SoundIndex.MinotaurDie] = new DXSound(SoundPath + @"317-5.wav", SoundType.Monster),

            [SoundIndex.FrostMinotaurAttack] = new DXSound(SoundPath + @"314-2.wav", SoundType.Monster),
            [SoundIndex.FrostMinotaurStruck] = new DXSound(SoundPath + @"314-4.wav", SoundType.Monster),
            [SoundIndex.FrostMinotaurDie] = new DXSound(SoundPath + @"314-5.wav", SoundType.Monster),

            [SoundIndex.BanyaLeftGuardAttack] = new DXSound(SoundPath + @"310-2.wav", SoundType.Monster),
            [SoundIndex.BanyaLeftGuardStruck] = new DXSound(SoundPath + @"310-4.wav", SoundType.Monster),
            [SoundIndex.BanyaLeftGuardDie] = new DXSound(SoundPath + @"310-5.wav", SoundType.Monster),

            [SoundIndex.EmperorSaWooAttack] = new DXSound(SoundPath + @"335-2.wav", SoundType.Monster),
            [SoundIndex.EmperorSaWooStruck] = new DXSound(SoundPath + @"335-4.wav", SoundType.Monster),
            [SoundIndex.EmperorSaWooDie] = new DXSound(SoundPath + @"335-5.wav", SoundType.Monster),



            [SoundIndex.BoneArcherAttack] = new DXSound(SoundPath + @"322-2.wav", SoundType.Monster),
            [SoundIndex.BoneArcherStruck] = new DXSound(SoundPath + @"322-4.wav", SoundType.Monster),
            [SoundIndex.BoneArcherDie] = new DXSound(SoundPath + @"322-5.wav", SoundType.Monster),

            [SoundIndex.BoneCaptainAttack] = new DXSound(SoundPath + @"320-2.wav", SoundType.Monster),
            [SoundIndex.BoneCaptainStruck] = new DXSound(SoundPath + @"320-4.wav", SoundType.Monster),
            [SoundIndex.BoneCaptainDie] = new DXSound(SoundPath + @"320-5.wav", SoundType.Monster),

            [SoundIndex.ArchLichTaeduAttack] = new DXSound(SoundPath + @"321-2.wav", SoundType.Monster),
            [SoundIndex.ArchLichTaeduStruck] = new DXSound(SoundPath + @"321-4.wav", SoundType.Monster),
            [SoundIndex.ArchLichTaeduDie] = new DXSound(SoundPath + @"321-5.wav", SoundType.Monster),



            [SoundIndex.WedgeMothLarvaAttack] = new DXSound(SoundPath + @"273-2.wav", SoundType.Monster),
            [SoundIndex.WedgeMothLarvaStruck] = new DXSound(SoundPath + @"273-4.wav", SoundType.Monster),
            [SoundIndex.WedgeMothLarvaDie] = new DXSound(SoundPath + @"273-5.wav", SoundType.Monster),

            [SoundIndex.LesserWedgeMothAttack] = new DXSound(SoundPath + @"274-2.wav", SoundType.Monster),
            [SoundIndex.LesserWedgeMothStruck] = new DXSound(SoundPath + @"274-4.wav", SoundType.Monster),
            [SoundIndex.LesserWedgeMothDie] = new DXSound(SoundPath + @"274-5.wav", SoundType.Monster),

            [SoundIndex.WedgeMothAttack] = new DXSound(SoundPath + @"275-2.wav", SoundType.Monster),
            [SoundIndex.WedgeMothStruck] = new DXSound(SoundPath + @"275-4.wav", SoundType.Monster),
            [SoundIndex.WedgeMothDie] = new DXSound(SoundPath + @"275-5.wav", SoundType.Monster),

            [SoundIndex.RedBoarAttack] = new DXSound(SoundPath + @"277-2.wav", SoundType.Monster),
            [SoundIndex.RedBoarStruck] = new DXSound(SoundPath + @"277-4.wav", SoundType.Monster),
            [SoundIndex.RedBoarDie] = new DXSound(SoundPath + @"277-5.wav", SoundType.Monster),

            [SoundIndex.ClawSerpentAttack] = new DXSound(SoundPath + @"279-2.wav", SoundType.Monster),
            [SoundIndex.ClawSerpentStruck] = new DXSound(SoundPath + @"279-4.wav", SoundType.Monster),
            [SoundIndex.ClawSerpentDie] = new DXSound(SoundPath + @"279-5.wav", SoundType.Monster),

            [SoundIndex.BlackBoarAttack] = new DXSound(SoundPath + @"277-2.wav", SoundType.Monster),
            [SoundIndex.BlackBoarStruck] = new DXSound(SoundPath + @"277-4.wav", SoundType.Monster),
            [SoundIndex.BlackBoarDie] = new DXSound(SoundPath + @"277-5.wav", SoundType.Monster),

            [SoundIndex.TuskLordAttack] = new DXSound(SoundPath + @"277-2.wav", SoundType.Monster),
            [SoundIndex.TuskLordStruck] = new DXSound(SoundPath + @"277-4.wav", SoundType.Monster),
            [SoundIndex.TuskLordDie] = new DXSound(SoundPath + @"277-5.wav", SoundType.Monster),

            [SoundIndex.RazorTuskAttack] = new DXSound(SoundPath + @"328-2.wav", SoundType.Monster),
            [SoundIndex.RazorTuskStruck] = new DXSound(SoundPath + @"328-4.wav", SoundType.Monster),
            [SoundIndex.RazorTuskDie] = new DXSound(SoundPath + @"328-5.wav", SoundType.Monster),

            [SoundIndex.PinkGoddessAttack] = new DXSound(SoundPath + @"340-2.wav", SoundType.Monster),
            [SoundIndex.PinkGoddessStruck] = new DXSound(SoundPath + @"340-4.wav", SoundType.Monster),
            [SoundIndex.PinkGoddessDie] = new DXSound(SoundPath + @"340-5.wav", SoundType.Monster),

            [SoundIndex.GreenGoddessAttack] = new DXSound(SoundPath + @"340-2.wav", SoundType.Monster),
            [SoundIndex.GreenGoddessStruck] = new DXSound(SoundPath + @"340-4.wav", SoundType.Monster),
            [SoundIndex.GreenGoddessDie] = new DXSound(SoundPath + @"340-5.wav", SoundType.Monster),

            [SoundIndex.MutantCaptainAttack] = new DXSound(SoundPath + @"339-2.wav", SoundType.Monster),
            [SoundIndex.MutantCaptainStruck] = new DXSound(SoundPath + @"339-4.wav", SoundType.Monster),
            [SoundIndex.MutantCaptainDie] = new DXSound(SoundPath + @"339-5.wav", SoundType.Monster),

            [SoundIndex.StoneGriffinAttack] = new DXSound(SoundPath + @"337-2.wav", SoundType.Monster),
            [SoundIndex.StoneGriffinStruck] = new DXSound(SoundPath + @"337-4.wav", SoundType.Monster),
            [SoundIndex.StoneGriffinDie] = new DXSound(SoundPath + @"337-5.wav", SoundType.Monster),

            [SoundIndex.FlameGriffinAttack] = new DXSound(SoundPath + @"337-2.wav", SoundType.Monster),
            [SoundIndex.FlameGriffinStruck] = new DXSound(SoundPath + @"337-4.wav", SoundType.Monster),
            [SoundIndex.FlameGriffinDie] = new DXSound(SoundPath + @"337-5.wav", SoundType.Monster),

            [SoundIndex.WhiteBoneAttack] = new DXSound(SoundPath + @"63.wav", SoundType.Monster),
            [SoundIndex.WhiteBoneStruck] = new DXSound(SoundPath + @"232-4.wav", SoundType.Monster),
            [SoundIndex.WhiteBoneDie] = new DXSound(SoundPath + @"256-5.wav", SoundType.Monster),

            [SoundIndex.ShinsuSmallStruck] = new DXSound(SoundPath + @"289-4.wav", SoundType.Monster),
            [SoundIndex.ShinsuSmallDie] = new DXSound(SoundPath + @"289-5.wav", SoundType.Monster),

            [SoundIndex.ShinsuBigAttack] = new DXSound(SoundPath + @"290-2.wav", SoundType.Monster),
            [SoundIndex.ShinsuBigStruck] = new DXSound(SoundPath + @"290-4.wav", SoundType.Monster),
            [SoundIndex.ShinsuBigDie] = new DXSound(SoundPath + @"290-5.wav", SoundType.Monster),

            [SoundIndex.ShinsuShow] = new DXSound(SoundPath + @"290-0.wav", SoundType.Monster),

            [SoundIndex.CorpseStalkerAttack] = new DXSound(SoundPath + @"212-2.wav", SoundType.Monster),
            [SoundIndex.CorpseStalkerStruck] = new DXSound(SoundPath + @"212-4.wav", SoundType.Monster),
            [SoundIndex.CorpseStalkerDie] = new DXSound(SoundPath + @"212-5.wav", SoundType.Monster),

            [SoundIndex.LightArmedSoldierAttack] = new DXSound(SoundPath + @"206-2.wav", SoundType.Monster),
            [SoundIndex.LightArmedSoldierStruck] = new DXSound(SoundPath + @"206-4.wav", SoundType.Monster),
            [SoundIndex.LightArmedSoldierDie] = new DXSound(SoundPath + @"206-5.wav", SoundType.Monster),

            [SoundIndex.CorrosivePoisonSpitterAttack] = new DXSound(SoundPath + @"293-2.wav", SoundType.Monster),
            [SoundIndex.CorrosivePoisonSpitterStruck] = new DXSound(SoundPath + @"293-4.wav", SoundType.Monster),
            [SoundIndex.CorrosivePoisonSpitterDie] = new DXSound(SoundPath + @"293-5.wav", SoundType.Monster),

            [SoundIndex.PhantomSoldierAttack] = new DXSound(SoundPath + @"347-2.wav", SoundType.Monster),
            [SoundIndex.PhantomSoldierStruck] = new DXSound(SoundPath + @"347-4.wav", SoundType.Monster),
            [SoundIndex.PhantomSoldierDie] = new DXSound(SoundPath + @"347-5.wav", SoundType.Monster),

            [SoundIndex.MutatedOctopusAttack] = new DXSound(SoundPath + @"202-2.wav", SoundType.Monster),
            [SoundIndex.MutatedOctopusStruck] = new DXSound(SoundPath + @"202-4.wav", SoundType.Monster),
            [SoundIndex.MutatedOctopusDie] = new DXSound(SoundPath + @"202-5.wav", SoundType.Monster),

            [SoundIndex.AquaLizardAttack] = new DXSound(SoundPath + @"345-2.wav", SoundType.Monster),
            [SoundIndex.AquaLizardStruck] = new DXSound(SoundPath + @"345-4.wav", SoundType.Monster),
            [SoundIndex.AquaLizardDie] = new DXSound(SoundPath + @"345-5.wav", SoundType.Monster),

            [SoundIndex.CrimsonNecromancerAttack] = new DXSound(SoundPath + @"346-2.wav", SoundType.Monster),
            [SoundIndex.CrimsonNecromancerStruck] = new DXSound(SoundPath + @"346-4.wav", SoundType.Monster),
            [SoundIndex.CrimsonNecromancerDie] = new DXSound(SoundPath + @"346-5.wav", SoundType.Monster),

            [SoundIndex.ChaosKnightAttack] = new DXSound(SoundPath + @"210-2.wav", SoundType.Monster),
            //[SoundIndex.ChaosKnightStruck] = new DXSound(SoundPath + @"210-4.wav", SoundType.Monster),
            [SoundIndex.ChaosKnightDie] = new DXSound(SoundPath + @"210-5.wav", SoundType.Monster),

            [SoundIndex.PachontheChaosbringerAttack] = new DXSound(SoundPath + @"343-2.wav", SoundType.Monster),
            [SoundIndex.PachontheChaosbringerStruck] = new DXSound(SoundPath + @"343-4.wav", SoundType.Monster),
            [SoundIndex.PachontheChaosbringerDie] = new DXSound(SoundPath + @"343-5.wav", SoundType.Monster),


            [SoundIndex.NumaCavalryAttack] = new DXSound(SoundPath + @"355-2.wav", SoundType.Monster),
            [SoundIndex.NumaCavalryStruck] = new DXSound(SoundPath + @"355-4.wav", SoundType.Monster),
            [SoundIndex.NumaCavalryDie] = new DXSound(SoundPath + @"355-5.wav", SoundType.Monster),

            [SoundIndex.NumaHighMageAttack] = new DXSound(SoundPath + @"359-2.wav", SoundType.Monster),
            [SoundIndex.NumaHighMageStruck] = new DXSound(SoundPath + @"359-4.wav", SoundType.Monster),
            [SoundIndex.NumaHighMageDie] = new DXSound(SoundPath + @"359-5.wav", SoundType.Monster),

            [SoundIndex.NumaStoneThrowerAttack] = new DXSound(SoundPath + @"358-2.wav", SoundType.Monster),
            [SoundIndex.NumaStoneThrowerStruck] = new DXSound(SoundPath + @"358-4.wav", SoundType.Monster),
            [SoundIndex.NumaStoneThrowerDie] = new DXSound(SoundPath + @"358-5.wav", SoundType.Monster),

            [SoundIndex.NumaRoyalGuardAttack] = new DXSound(SoundPath + @"357-2.wav", SoundType.Monster),
            [SoundIndex.NumaRoyalGuardStruck] = new DXSound(SoundPath + @"357-4.wav", SoundType.Monster),
            [SoundIndex.NumaRoyalGuardDie] = new DXSound(SoundPath + @"357-5.wav", SoundType.Monster),

            [SoundIndex.NumaArmoredSoldierAttack] = new DXSound(SoundPath + @"356-2.wav", SoundType.Monster),
            [SoundIndex.NumaArmoredSoldierStruck] = new DXSound(SoundPath + @"356-4.wav", SoundType.Monster),
            [SoundIndex.NumaArmoredSoldierDie] = new DXSound(SoundPath + @"356-5.wav", SoundType.Monster),




            [SoundIndex.IcyRangerAttack] = new DXSound(SoundPath + @"375-2.wav", SoundType.Monster),
            [SoundIndex.IcyRangerStruck] = new DXSound(SoundPath + @"375-4.wav", SoundType.Monster),
            [SoundIndex.IcyRangerDie] = new DXSound(SoundPath + @"375-5.wav", SoundType.Monster),

            [SoundIndex.IcyGoddessAttack] = new DXSound(SoundPath + @"374-2.wav", SoundType.Monster),
            [SoundIndex.IcyGoddessStruck] = new DXSound(SoundPath + @"374-4.wav", SoundType.Monster),
            [SoundIndex.IcyGoddessDie] = new DXSound(SoundPath + @"374-5.wav", SoundType.Monster),

            [SoundIndex.IcySpiritWarriorAttack] = new DXSound(SoundPath + @"378-2.wav", SoundType.Monster),
            [SoundIndex.IcySpiritWarriorStruck] = new DXSound(SoundPath + @"378-4.wav", SoundType.Monster),
            [SoundIndex.IcySpiritWarriorDie] = new DXSound(SoundPath + @"378-5.wav", SoundType.Monster),

            [SoundIndex.GhostKnightAttack] = new DXSound(SoundPath + @"373-2.wav", SoundType.Monster),
            [SoundIndex.GhostKnightStruck] = new DXSound(SoundPath + @"373-4.wav", SoundType.Monster),
            [SoundIndex.GhostKnightDie] = new DXSound(SoundPath + @"373-5.wav", SoundType.Monster),

            [SoundIndex.IcySpiritSpearmanAttack] = new DXSound(SoundPath + @"376-2.wav", SoundType.Monster),
            [SoundIndex.IcySpiritSpearmanStruck] = new DXSound(SoundPath + @"376-4.wav", SoundType.Monster),
            [SoundIndex.IcySpiritSpearmanDie] = new DXSound(SoundPath + @"376-5.wav", SoundType.Monster),

            [SoundIndex.WerewolfAttack] = new DXSound(SoundPath + @"372-2.wav", SoundType.Monster),
            [SoundIndex.WerewolfStruck] = new DXSound(SoundPath + @"372-4.wav", SoundType.Monster),
            [SoundIndex.WerewolfDie] = new DXSound(SoundPath + @"372-5.wav", SoundType.Monster),

            [SoundIndex.WhitefangAttack] = new DXSound(SoundPath + @"371-2.wav", SoundType.Monster),
            [SoundIndex.WhitefangStruck] = new DXSound(SoundPath + @"371-4.wav", SoundType.Monster),
            [SoundIndex.WhitefangDie] = new DXSound(SoundPath + @"371-5.wav", SoundType.Monster),

            [SoundIndex.IcySpiritSoliderAttack] = new DXSound(SoundPath + @"377-2.wav", SoundType.Monster),
            [SoundIndex.IcySpiritSoliderStruck] = new DXSound(SoundPath + @"377-4.wav", SoundType.Monster),
            [SoundIndex.IcySpiritSoliderDie] = new DXSound(SoundPath + @"377-5.wav", SoundType.Monster),

            [SoundIndex.WildBoarAttack] = new DXSound(SoundPath + @"328-2.wav", SoundType.Monster),
            [SoundIndex.WildBoarStruck] = new DXSound(SoundPath + @"328-4.wav", SoundType.Monster),
            [SoundIndex.WildBoarDie] = new DXSound(SoundPath + @"338-5.wav", SoundType.Monster),

            [SoundIndex.FrostLordHwaAttack] = new DXSound(SoundPath + @"379-2.wav", SoundType.Monster),
            [SoundIndex.FrostLordHwaStruck] = new DXSound(SoundPath + @"379-4.wav", SoundType.Monster),
            [SoundIndex.FrostLordHwaDie] = new DXSound(SoundPath + @"379-5.wav", SoundType.Monster),

            [SoundIndex.JinchonDevilAttack] = new DXSound(SoundPath + @"341-2.wav", SoundType.Monster),
            [SoundIndex.JinchonDevilAttack2] = new DXSound(SoundPath + @"341-7.wav", SoundType.Monster),
            [SoundIndex.JinchonDevilAttack3] = new DXSound(SoundPath + @"341-8.wav", SoundType.Monster),
            [SoundIndex.JinchonDevilStruck] = new DXSound(SoundPath + @"341-4.wav", SoundType.Monster),
            [SoundIndex.JinchonDevilDie] = new DXSound(SoundPath + @"341-5.wav", SoundType.Monster),


            [SoundIndex.EscortCommanderAttack] = new DXSound(SoundPath + @"381-2.wav", SoundType.Monster),
            [SoundIndex.EscortCommanderStruck] = new DXSound(SoundPath + @"381-4.wav", SoundType.Monster),
            [SoundIndex.EscortCommanderDie] = new DXSound(SoundPath + @"381-5.wav", SoundType.Monster),
            
            [SoundIndex.FieryDancerAttack] = new DXSound(SoundPath + @"383-2.wav", SoundType.Monster),
            [SoundIndex.FieryDancerStruck] = new DXSound(SoundPath + @"383-4.wav", SoundType.Monster),
            [SoundIndex.FieryDancerDie] = new DXSound(SoundPath + @"383-5.wav", SoundType.Monster),

            [SoundIndex.EmeraldDancerAttack] = new DXSound(SoundPath + @"384-2.wav", SoundType.Monster),
            [SoundIndex.EmeraldDancerStruck] = new DXSound(SoundPath + @"384-4.wav", SoundType.Monster),
            [SoundIndex.EmeraldDancerDie] = new DXSound(SoundPath + @"384-5.wav", SoundType.Monster),

            [SoundIndex.QueenOfDawnAttack] = new DXSound(SoundPath + @"382-2.wav", SoundType.Monster),
            [SoundIndex.QueenOfDawnStruck] = new DXSound(SoundPath + @"382-4.wav", SoundType.Monster),
            [SoundIndex.QueenOfDawnDie] = new DXSound(SoundPath + @"382-5.wav", SoundType.Monster),

            [SoundIndex.OYoungBeastAttack] = new DXSound(SoundPath + @"388-2.wav", SoundType.Monster),
            [SoundIndex.OYoungBeastStruck] = new DXSound(SoundPath + @"388-4.wav", SoundType.Monster),
            [SoundIndex.OYoungBeastDie] = new DXSound(SoundPath + @"388-5.wav", SoundType.Monster),

            [SoundIndex.YumgonWitchAttack] = new DXSound(SoundPath + @"391-2.wav", SoundType.Monster),
            [SoundIndex.YumgonWitchStruck] = new DXSound(SoundPath + @"391-4.wav", SoundType.Monster),
            [SoundIndex.YumgonWitchDie] = new DXSound(SoundPath + @"391-5.wav", SoundType.Monster),

            [SoundIndex.MaWarlordAttack] = new DXSound(SoundPath + @"389-2.wav", SoundType.Monster),
            [SoundIndex.MaWarlordStruck] = new DXSound(SoundPath + @"389-4.wav", SoundType.Monster),
            [SoundIndex.MaWarlordDie] = new DXSound(SoundPath + @"389-5.wav", SoundType.Monster),

            [SoundIndex.JinhwanSpiritAttack] = new DXSound(SoundPath + @"392-2.wav", SoundType.Monster),
            [SoundIndex.JinhwanSpiritStruck] = new DXSound(SoundPath + @"392-4.wav", SoundType.Monster),
            [SoundIndex.JinhwanSpiritDie] = new DXSound(SoundPath + @"392-5.wav", SoundType.Monster),

            [SoundIndex.JinhwanGuardianAttack] = new DXSound(SoundPath + @"393-2.wav", SoundType.Monster),
            [SoundIndex.JinhwanGuardianStruck] = new DXSound(SoundPath + @"393-4.wav", SoundType.Monster),
            [SoundIndex.JinhwanGuardianDie] = new DXSound(SoundPath + @"393-5.wav", SoundType.Monster),

            [SoundIndex.YumgonGeneralAttack] = new DXSound(SoundPath + @"390-2.wav", SoundType.Monster),
            [SoundIndex.YumgonGeneralStruck] = new DXSound(SoundPath + @"390-4.wav", SoundType.Monster),
            [SoundIndex.YumgonGeneralDie] = new DXSound(SoundPath + @"390-5.wav", SoundType.Monster),

            [SoundIndex.ChiwooGeneralAttack] = new DXSound(SoundPath + @"385-2.wav", SoundType.Monster),
            [SoundIndex.ChiwooGeneralStruck] = new DXSound(SoundPath + @"385-4.wav", SoundType.Monster),
            [SoundIndex.ChiwooGeneralDie] = new DXSound(SoundPath + @"385-5.wav", SoundType.Monster),

            [SoundIndex.DragonQueenAttack] = new DXSound(SoundPath + @"387-2.wav", SoundType.Monster),
            [SoundIndex.DragonQueenStruck] = new DXSound(SoundPath + @"387-4.wav", SoundType.Monster),
            [SoundIndex.DragonQueenDie] = new DXSound(SoundPath + @"387-5.wav", SoundType.Monster),

            [SoundIndex.DragonLordAttack] = new DXSound(SoundPath + @"386-2.wav", SoundType.Monster),
            [SoundIndex.DragonLordStruck] = new DXSound(SoundPath + @"386-4.wav", SoundType.Monster),
            [SoundIndex.DragonLordDie] = new DXSound(SoundPath + @"386-5.wav", SoundType.Monster),

            [SoundIndex.FerociousIceTigerAttack] = new DXSound(SoundPath + @"201-2.wav", SoundType.Monster),
            [SoundIndex.FerociousIceTigerStruck] = new DXSound(SoundPath + @"201-4.wav", SoundType.Monster),
            [SoundIndex.FerociousIceTigerDie] = new DXSound(SoundPath + @"201-5.wav", SoundType.Monster),


            [SoundIndex.SamaFireGuardianAttack] = new DXSound(SoundPath + @"400-2.wav", SoundType.Monster),
            [SoundIndex.SamaFireGuardianStruck] = new DXSound(SoundPath + @"400-4.wav", SoundType.Monster),
            [SoundIndex.SamaFireGuardianDie] = new DXSound(SoundPath + @"400-5.wav", SoundType.Monster),
            
            [SoundIndex.SamaIceGuardianAttack] = new DXSound(SoundPath + @"398-2.wav", SoundType.Monster),
            [SoundIndex.SamaIceGuardianStruck] = new DXSound(SoundPath + @"398-4.wav", SoundType.Monster),
            [SoundIndex.SamaIceGuardianDie] = new DXSound(SoundPath + @"398-5.wav", SoundType.Monster),

            [SoundIndex.SamaLightningGuardianAttack] = new DXSound(SoundPath + @"397-2.wav", SoundType.Monster),
            [SoundIndex.SamaLightningGuardianStruck] = new DXSound(SoundPath + @"397-4.wav", SoundType.Monster),
            [SoundIndex.SamaLightningGuardianDie] = new DXSound(SoundPath + @"397-5.wav", SoundType.Monster),

            [SoundIndex.SamaWindGuardianAttack] = new DXSound(SoundPath + @"399-2.wav", SoundType.Monster),
            [SoundIndex.SamaWindGuardianStruck] = new DXSound(SoundPath + @"399-4.wav", SoundType.Monster),
            [SoundIndex.SamaWindGuardianDie] = new DXSound(SoundPath + @"399-5.wav", SoundType.Monster),

            [SoundIndex.PhoenixAttack] = new DXSound(SoundPath + @"402-2.wav", SoundType.Monster),
            [SoundIndex.PhoenixStruck] = new DXSound(SoundPath + @"402-4.wav", SoundType.Monster),
            [SoundIndex.PhoenixDie] = new DXSound(SoundPath + @"402-5.wav", SoundType.Monster),

            [SoundIndex.BlackTortoiseAttack] = new DXSound(SoundPath + @"404-2.wav", SoundType.Monster),
            [SoundIndex.BlackTortoiseStruck] = new DXSound(SoundPath + @"404-4.wav", SoundType.Monster),
            [SoundIndex.BlackTortoiseDie] = new DXSound(SoundPath + @"404-5.wav", SoundType.Monster),

            [SoundIndex.BlueDragonAttack] = new DXSound(SoundPath + @"403-2.wav", SoundType.Monster),
            [SoundIndex.BlueDragonStruck] = new DXSound(SoundPath + @"403-4.wav", SoundType.Monster),
            [SoundIndex.BlueDragonDie] = new DXSound(SoundPath + @"403-5.wav", SoundType.Monster),


            [SoundIndex.Terracotta1Attack] = new DXSound(SoundPath + @"m414-2.wav", SoundType.Monster),
            [SoundIndex.Terracotta1Struck] = new DXSound(SoundPath + @"m414-4.wav", SoundType.Monster),
            [SoundIndex.Terracotta1Die] = new DXSound(SoundPath + @"m414-5.wav", SoundType.Monster),

            [SoundIndex.Terracotta2Attack] = new DXSound(SoundPath + @"m415-2.wav", SoundType.Monster),
            [SoundIndex.Terracotta2Struck] = new DXSound(SoundPath + @"m415-4.wav", SoundType.Monster),
            [SoundIndex.Terracotta2Die] = new DXSound(SoundPath + @"m415-5.wav", SoundType.Monster),

            [SoundIndex.Terracotta3Attack] = new DXSound(SoundPath + @"m416-2.wav", SoundType.Monster),
            [SoundIndex.Terracotta3Struck] = new DXSound(SoundPath + @"m416-4.wav", SoundType.Monster),
            [SoundIndex.Terracotta3Die] = new DXSound(SoundPath + @"m416-5.wav", SoundType.Monster),

            [SoundIndex.Terracotta4Attack] = new DXSound(SoundPath + @"m417-2.wav", SoundType.Monster),
            [SoundIndex.Terracotta4Struck] = new DXSound(SoundPath + @"m417-4.wav", SoundType.Monster),
            [SoundIndex.Terracotta4Die] = new DXSound(SoundPath + @"m417-5.wav", SoundType.Monster),

            [SoundIndex.TerracottaSubAttack] = new DXSound(SoundPath + @"m418-2.wav", SoundType.Monster),
            [SoundIndex.TerracottaSubAttack2] = new DXSound(SoundPath + @"m418-7.wav", SoundType.Monster),
            [SoundIndex.TerracottaSubStruck] = new DXSound(SoundPath + @"m418-4.wav", SoundType.Monster),
            [SoundIndex.TerracottaSubDie] = new DXSound(SoundPath + @"m414-5.wav", SoundType.Monster),

            [SoundIndex.TerracottaBossAttack] = new DXSound(SoundPath + @"m419-2.wav", SoundType.Monster),
            [SoundIndex.TerracottaBossAttack2] = new DXSound(SoundPath + @"m419-7.wav", SoundType.Monster),
            [SoundIndex.TerracottaBossStruck] = new DXSound(SoundPath + @"m419-4.wav", SoundType.Monster),
            [SoundIndex.TerracottaBossDie] = new DXSound(SoundPath + @"m419-5.wav", SoundType.Monster),

            #endregion
        };

        #endregion

        public static void Create()
        {
            try
            {
                Device = new DirectSound();
                Device.SetCooperativeLevel(CEnvir.Target.Handle, CooperativeLevel.Normal);
                AdjustVolume();
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
                Error = true;
            }
        }

        public static void Play(SoundIndex index)
        {
            if (index == SoundIndex.None || Error) return;

            DXSound sound;

            if (SoundList.TryGetValue(index, out sound))
            {
                sound.Play();
                return;
            }
        }
        public static void Stop(SoundIndex index)
        {
            DXSound sound;

            if (!SoundList.TryGetValue(index, out sound)) return;

            sound.Stop();
        }
        public static void StopAllSounds()
        {
            for (int i = DXManager.SoundList.Count - 1; i >= 0; i--)
                DXManager.SoundList[i].Stop();
        }
        public static void AdjustVolume()
        {
            foreach (KeyValuePair<SoundIndex, DXSound> pair in SoundList)
                pair.Value.SetVolume();
        }
        public static void UpdateFlags()
        {
            for (int i = DXManager.SoundList.Count - 1; i >= 0; i--)
                DXManager.SoundList[i].UpdateFlags();
        }

        public static int GetVolume(SoundType type)
        {
            int volume;

            switch (type)
            {
                case SoundType.System:
                    volume = Math.Max(0, Math.Min(100, Config.SystemVolume));
                    break;
                case SoundType.Music:
                    volume = Math.Max(0, Math.Min(100, Config.MusicVolume));
                    break;
                case SoundType.Player:
                    volume = Math.Max(0, Math.Min(100, Config.PlayerVolume));
                    break;
                case SoundType.Monster:
                    volume = Math.Max(0, Math.Min(100, Config.MonsterVolume));
                    break;
                case SoundType.Magic:
                    volume = Math.Max(0, Math.Min(100, Config.MagicVolume));
                    break;
                default:
                    volume = 0;
                    break;
            }

            if (volume == 0)
                return -10000;

            return (int)Math.Floor(2000D * Math.Log10(volume / 100D) + 0.5D);
        }

        public static void Unload()
        {
            for (int i = DXManager.SoundList.Count - 1; i >= 0; i--)
                DXManager.SoundList[i].DisposeSoundBuffer();

            if (Device != null)
            {
                if (!Device.Disposed)
                    Device.Dispose();

                Device = null;
            }


        }
    }

    
    public enum SoundType
    {
        None,

        System,
        Music,
        Magic,
        Monster,
        Player,
    }
}

