using Library;
using Server.DBModels;
using Server.Envir;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DemonicRecovery)]
    public class DemonicRecovery : MagicObject
    {
        protected override Element Element => Element.None;

        public DemonicRecovery(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void Toggle(bool canUse)
        {
            if (!CheckCost() || SEnvir.Now < Magic.Cooldown || Player.Dead || (Player.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Player.Poison & PoisonType.Silenced) == PoisonType.Silenced) return;
            if (Player.Pets.All(x => x.MonsterInfo.Flag != MonsterFlag.InfernalSoldier || x.Dead))
                return;

            MagicConsume();
            MagicCooldown();

            MonsterObject pet = Player.Pets.FirstOrDefault(x => x.MonsterInfo.Flag == MonsterFlag.InfernalSoldier && !x.Dead);

            if (pet == null) return;
            int health = pet.Stats[Stat.Health] * Magic.GetPower() / 100;

            pet.ChangeHP(health);

            Player.LevelMagic(Magic);
        }
    }
}
