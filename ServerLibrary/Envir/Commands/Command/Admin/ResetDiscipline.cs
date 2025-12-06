using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class ResetDiscipline : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "RESETDISCIPLINE";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            CharacterInfo targetCharacter = player.Character;
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException(); //RESETDISCIPLINE John
            if (vals.Length > 2)
            {
                targetCharacter = SEnvir.GetCharacter(vals[1]);
                if (targetCharacter == null)
                    throw new UserCommandException(string.Format("Could not find character: {0}", vals[1]));
            }

            if (targetCharacter.Discipline != null)
            {
                foreach (var magic in targetCharacter.Discipline.Magics)
                {
                    targetCharacter.Player?.MagicObjects.Remove(magic.Info.Magic);
                }

                targetCharacter.Discipline.Delete();
                targetCharacter.Discipline = null;

                targetCharacter.Player?.Enqueue(new S.DisciplineUpdate { Discipline = null });
                targetCharacter.Player?.RefreshStats();
            }
        }
    }
}
