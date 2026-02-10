using _NueCore.Common.ReactiveUtils;

namespace _NueExtras.CursorSystem
{
    public static class CursorStatic
    {
        public static void ChangeCursor(CursorType cursorType)
        {
            RBuss.Publish(new CursorREvents.CursorChangedREvent(cursorType));
        }
    }
}