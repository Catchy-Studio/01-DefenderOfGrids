using System;

namespace _NueExtras.RaritySystem
{
    [Flags]
    public enum NTags
    {
        None =0,
        STAT = 1<<0,
        NEW_CONTENT = 1<<1,
        CORE = 1<<2,
        STAGE_1 = 1<<3,
        STAGE_2 = 1<<4,
        STAGE_3 = 1<<5
    }
}