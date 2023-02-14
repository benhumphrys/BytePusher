using static System.Console;

WriteLine("BytePusher VM implementation in MonoGame.");

if (args.Length != 1)
{
    WriteLine("Usage:\r\n");
    WriteLine("BytePusherMonoGameWin.exe <bp file>");
    return;
}

using var game = new BytePusherMonoGameWin.BytePusherGame(args[0]);
game.Run();
