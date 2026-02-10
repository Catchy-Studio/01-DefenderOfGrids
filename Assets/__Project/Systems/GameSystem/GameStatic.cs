using _NueCore.Common.ReactiveUtils;

namespace __Project.Systems.GameSystem
{
    public static class GameStatic
    {
        public static void Win()
        {
            RBuss.Publish(new GameREvents.WinREvent());
        }
        
        public static void Lose()
        {
            RBuss.Publish(new GameREvents.LoseREvent());
        }
    }
}