using UnityEngine;

namespace __Project.Systems.RunSystem
{
    public enum RunState
    {
        Idle =0,
        Shop =1,
        Placement =2,
        Fail = 3,
        Game =4,
        Win =5,
        Transition =6
    }

    public static class RunStateExtensions
    {
        public static Color GetColor(this RunState state)
        {
            return state switch
            {
                RunState.Idle => Color.white,
                RunState.Shop => Color.cyan,
                RunState.Placement => Color.yellow,
                RunState.Fail => Color.red,
                RunState.Game => Color.green,
                RunState.Win => Color.magenta,
                RunState.Transition => Color.blue,
                _ => Color.white
            };
        }
    }
}