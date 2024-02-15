using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;

namespace Server.Views
{
    public partial class MonsterInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MonsterInfoView()
        {
            InitializeComponent();

            MonsterInfoGridControl.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;

            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            MonsterImageComboBox.Items.AddEnum<MonsterImage>();
            StatComboBox.Items.AddEnum<Stat>();

            CheckNeedMonsterUpdate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MonsterInfoGridView);
            SMain.SetUpView(MonsterInfoStatsGridView);
            SMain.SetUpView(DropsGridView);
            SMain.SetUpView(RespawnsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<MonsterInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<MonsterInfo>(MonsterInfoGridView);
        }

        private void UpdateMonsterImageButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var monsterInfos = SMain.Session.GetCollection<MonsterInfo>().Binding;

            foreach (var info in monsterInfos)
            {
                var oldInt = (int)info.Image;

                var oldEnum = (OldMonsterImage)oldInt;

                var newParsed = Enum.TryParse(oldEnum.ToString(), out MonsterImage newImage);

                if (newParsed && oldEnum.ToString() == newImage.ToString())
                {
                    info.Image = newImage;
                }
            }

            CheckNeedMonsterUpdate();
            Refresh();
        }

        private void CheckNeedMonsterUpdate()
        {
            var monsterInfos = SMain.Session.GetCollection<MonsterInfo>().Binding;

            foreach (var item in monsterInfos)
            {
                if ((int)item.Image < 10 && item.Image != MonsterImage.None)
                {
                    UpdateMonsterImageButton.Enabled = true;
                    return;
                }
            }

            UpdateMonsterImageButton.Enabled = false;
        }
    }

    public enum OldMonsterImage
    {
        None,

        Guard,

        Chicken,
        Pig,
        Deer,
        Cow,
        Sheep,
        ClawCat,
        Wolf,
        ForestYeti,
        ChestnutTree,
        CarnivorousPlant,
        Oma,
        TigerSnake,
        SpittingSpider,
        Scarecrow,
        OmaHero,

        CaveBat,
        Scorpion,
        Skeleton,
        SkeletonAxeMan,
        SkeletonAxeThrower,
        SkeletonWarrior,
        SkeletonLord,

        CaveMaggot,
        GhostSorcerer,
        GhostMage,
        VoraciousGhost,
        DevouringGhost,
        CorpseRaisingGhost,
        GhoulChampion,

        ArmoredAnt,
        AntSoldier,
        AntHealer,
        AntNeedler,

        ShellNipper,
        Beetle,
        VisceralWorm,

        MutantFlea,
        PoisonousMutantFlea,
        BlasterMutantFlea,

        WasHatchling,
        Centipede,
        ButterflyWorm,
        MutantMaggot,
        Earwig,
        IronLance,
        LordNiJae,

        RottingGhoul,
        DecayingGhoul,
        BloodThirstyGhoul,

        SpinedDarkLizard,
        UmaInfidel,
        UmaFlameThrower,
        UmaAnguisher,
        UmaKing,

        SpiderBat,
        ArachnidGazer,
        Larva,
        RedMoonGuardian,
        RedMoonProtector,
        VenomousArachnid,
        DarkArachnid,
        RedMoonTheFallen,

        ZumaSharpShooter,
        ZumaFanatic,
        ZumaGuardian,
        ViciousRat,
        ZumaKing,

        EvilFanatic,
        Monkey,
        EvilElephant,
        CannibalFanatic,

        SpikedBeetle,
        NumaGrunt,
        NumaMage,
        NumaElite,
        SandShark,
        StoneGolem,
        WindfurySorceress,
        CursedCactus,
        NetherWorldGate,

        RagingLizard,
        SawToothLizard,
        MutantLizard,
        VenomSpitter,
        SonicLizard,
        GiantLizard,
        CrazedLizard,
        TaintedTerror,
        DeathLordJichon,

        Minotaur,
        FrostMinotaur,
        ShockMinotaur,
        FlameMinotaur,
        FuryMinotaur,
        BanyaLeftGuard,
        BanyaRightGuard,
        EmperorSaWoo,

        BoneArcher,
        BoneBladesman,
        BoneCaptain,
        BoneSoldier,
        ArchLichTaedu,

        WedgeMothLarva,
        LesserWedgeMoth,
        WedgeMoth,
        RedBoar,
        ClawSerpent,
        BlackBoar,
        TuskLord,
        RazorTusk,

        PinkGoddess,
        GreenGoddess,
        MutantCaptain,
        StoneGriffin,
        FlameGriffin,

        WhiteBone,
        Shinsu,
        InfernalSoldier,
        InfernalGuardian,
        InfernalWarrior,

        CorpseStalker,
        LightArmedSoldier,
        CorrosivePoisonSpitter,
        PhantomSoldier,
        MutatedOctopus,
        AquaLizard,
        Stomper,
        CrimsonNecromancer,
        ChaosKnight,
        PachonTheChaosBringer,

        NumaCavalry,
        NumaHighMage,
        NumaStoneThrower,
        NumaRoyalGuard,
        NumaArmoredSoldier,

        IcyRanger,
        IcyGoddess,
        IcySpiritWarrior,
        IcySpiritGeneral,
        GhostKnight,
        IcySpiritSpearman,
        Werewolf,
        Whitefang,
        IcySpiritSolider,
        WildBoar,
        JinamStoneGate,
        FrostLordHwa,

        Companion_Pig,
        Companion_TuskLord,
        Companion_SkeletonLord,
        Companion_Griffin,
        Companion_Dragon,
        Companion_Donkey,
        Companion_Sheep,
        Companion_BanyoLordGuzak,
        Companion_Panda,
        Companion_Rabbit,

        JinchonDevil,
        OmaWarlord,

        EscortCommander,
        FieryDancer,
        EmeraldDancer,
        QueenOfDawn,

        OYoungBeast,
        YumgonWitch,
        MaWarlord,
        JinhwanSpirit,
        JinhwanGuardian,
        YumgonGeneral,
        ChiwooGeneral,
        DragonQueen,
        DragonLord,

        FerociousIceTiger,

        SamaFireGuardian,
        SamaIceGuardian,
        SamaLightningGuardian,
        SamaWindGuardian,
        Phoenix,
        BlackTortoise,
        BlueDragon,
        WhiteTiger,
        SamaCursedBladesman,
        SamaCursedSlave,
        SamaCursedFlameMage,
        SamaProphet,
        SamaSorcerer,
        EnshrinementBox,
        BloodStone,

        OrangeTiger,
        RegularTiger,
        RedTiger,
        SnowTiger,
        BlackTiger,
        BigBlackTiger,
        BigWhiteTiger,
        OrangeBossTiger,
        BigBossTiger,
        WildMonkey,
        FrostYeti,

        EvilSnake,
        Salamander,
        SandGolem,
        SDMob4,
        SDMob5,
        SDMob6,
        SDMob7,
        OmaMage,
        SDMob9,
        SDMob10,
        SDMob11,
        SDMob12,
        SDMob13,
        SDMob14,
        CrystalGolem,
        DustDevil,
        TwinTailScorpion,
        BloodyMole,
        SDMob19,
        SDMob20,
        SDMob21,
        SDMob22,
        SDMob23,
        SDMob24,
        SDMob25,
        GangSpider,
        VenomSpider,
        SDMob26,

        LobsterLord,
        LobsterSpawn,

        NewMob1,
        NewMob2,
        NewMob3,
        NewMob4,
        NewMob5,
        NewMob6,
        NewMob7,
        NewMob8,
        NewMob9,
        NewMob10,

        MonasteryMon0,
        MonasteryMon1,
        MonasteryMon2,
        MonasteryMon3,
        MonasteryMon4,
        MonasteryMon5,
        MonasteryMon6,

        CastleFlag,

        Tornado
    }
}