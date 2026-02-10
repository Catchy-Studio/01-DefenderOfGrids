using _NueCore.Common.ReactiveUtils;

namespace _NueExtras.TokenSystem
{
    public abstract class TokenREvents 
    {
        public class TokenValueChangedREvent : REvent
        {
            public int Delta { get; private set; }
            public TokenValueChangedREvent(int delta)
            {
                Delta = delta;
            }
        }
        
        public class TokenUnlockedREvent : REvent
        {
            public ITokenData Data { get; private set; }
            public TokenUnlockedREvent(ITokenData data)
            {
                Data = data;
            }
        }
       
    }
}