namespace Server.Envir.Commands.Command
{
    public interface IParameterizedCommand : ICommand
    {
        int PARAMS_LENGTH { get; }
    }
}
