using System;
using System.Collections.Generic;
using System.Text;
using _NueCore.SaveSystem;

namespace _NueExtras.TokenSystem
{
    public class TokenSave : NBaseSave
    {
        #region Setup
        private static readonly StringBuilder Str = new StringBuilder();
        protected override string GetSavePath()
        {
            Str.Clear();
            Str.Append("Token_Global");
            return Str.ToString();
        }
        public override void Save()
        {
            NSaver.SaveData<TokenSave>();
        }
        public override void Load()
        {
            NSaver.GetSaveData<TokenSave>();
        }
        public override void ResetSave()
        {
            NSaver.ResetSave<TokenSave>();
        }

        public override SaveTypes GetSaveType()
        {
            return SaveTypes.Global;
        }

        #endregion

        public int TotalExp;
        public int LastExp;
        public int CollectedExp;
        public int TotalCollectedToken;
        public int AvailableToken;

        public List<TokenInfo> Tokens = new List<TokenInfo>();

        public TokenInfo GetTokenInfo(string id)
        {
            var token = Tokens.Find(t => t.id == id);
            if (token == null)
            {
               token = new TokenInfo { id = id, isUnlocked = false };
               Tokens.Add(token);
            }
            return token;
        }
        public void UnlockToken(string id)
        {
            var token = GetTokenInfo(id);
            token.isUnlocked = true;
        }
        
        public bool IsTokenUnlocked(string id)
        {
            var token = GetTokenInfo(id);
            return token.isUnlocked;
        }
        
        
        
        [Serializable]
        public class TokenInfo
        {
            public string id;
            public bool isUnlocked;
        }
    }
}