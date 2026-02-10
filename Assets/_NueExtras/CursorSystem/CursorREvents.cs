using _NueCore.Common.ReactiveUtils;

namespace _NueExtras.CursorSystem
{
    public abstract class CursorREvents
    {
        public class CursorChangedREvent : REvent
        {
            public CursorType CursorType { get; private set; }
            public CursorChangedREvent(CursorType cursorType)
            {
                CursorType = cursorType;
            }
        }
    }
}