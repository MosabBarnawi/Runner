namespace BarnoGames.Utilities.DeveloperConsole.Commands
{
    public interface IConsoleCommand
    {
        string CommandWord { get; }
        string [] AltCommandsWord { get; }
        string CommandDescription { get; }

        string Process( string [] args );
    }

}
