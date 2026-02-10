using System;

namespace NueGames.NTooltip._Keyword
{
    [Flags]
    public enum NKeywords
    {
        None = 0,
        Title = 1 << 0,
        Description = 1 << 1,
        Author = 1 << 2,
        Publisher = 1 << 3,
    }
}