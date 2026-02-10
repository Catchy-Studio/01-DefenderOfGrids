using System;

namespace _NueExtras.NLocalizationSystem
{
    public enum NTables
    {
        NTable =0,
        GameTable =1,
        MenuTable =2,
        KWTable =3,
        RelicTable = 4,
        ShopTable =5,
        SettingsTable =6,
        NStatTable =7
    }

    public static class NTableUtils
    {
        public static string GetTableName(this NTables table)
        {
            return table switch
            {
                NTables.NTable => "NTable",
                NTables.GameTable => "GameTable",
                NTables.MenuTable => "MenuTable",
                NTables.KWTable => "KWTable",
                NTables.RelicTable => "RelicTable",
                NTables.ShopTable => "ShopTable",
                NTables.SettingsTable => "SettingsTable",
                NTables.NStatTable => "NStatTable",
                _ => throw new ArgumentOutOfRangeException(nameof(table), table, null)
            };
        }
    }
}