namespace _NueExtras.NLocalizationSystem
{
    public struct NLocalParams
    {
        public static NLocalParams CreateInstance()
        {
            return new NLocalParams();
        }

        public NLocal_Field_String NLocalField;
        public string DefaultValue;
        public string Identifier;
        public NTables NTable;
        public bool IncludeBaseID;
        public bool CreateEntry;
        public NLocalParams(NLocal_Field_String nLocalField, string defaultValue, string identifier)
        {
            NLocalField = nLocalField;
            DefaultValue = defaultValue;
            Identifier = identifier;
            NTable = NTables.NTable;
            IncludeBaseID = true;
            CreateEntry = true;
        }
            
        public NLocalParams(NLocal_Field_String nLocalField, string defaultValue, string identifier,NTables table,bool includeBaseID, bool createEntry)
        {
            NLocalField = nLocalField;
            DefaultValue = defaultValue;
            Identifier = identifier;
            NTable = table;
            IncludeBaseID = includeBaseID;
            CreateEntry = createEntry;
        }
    }
}