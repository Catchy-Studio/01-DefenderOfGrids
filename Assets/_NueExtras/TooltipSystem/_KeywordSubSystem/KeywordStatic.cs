using System;
using System.Collections.Generic;
using _NueCore.Common.NueLogger;
using _NueCore.Common.Utility;
using _NueExtras.RaritySystem;
using UnityEngine;

namespace NueGames.NTooltip._Keyword
{
    public static class KeywordStatic
    {
        public const string IDENTIFIER_START = "<";
        public const string IDENTIFIER_END = ">";
        public const string DESC = "DESC";
        public static KeywordCatalog Catalog { get; private set; }

        public static void SetKeywordCatalog(KeywordCatalog catalog)
        {
            Catalog = catalog;
        }

        public static void AddKeywordTooltip(NTooltipInfo baseInfo,List<KeywordData> keywords)
        {
            if (keywords == null || keywords.Count == 0 || Catalog == null)
                return;

            foreach (var keyword in keywords)
            {
                if (keyword.IgnoreTooltip)
                    continue;
                var newInfo = keyword.GetTooltipInfo();
                newInfo.FollowTarget = baseInfo.FollowTarget;
                newInfo.Is3D = baseInfo.Is3D;
                newInfo.Source = baseInfo.Source + keyword.GetKeywordID();
                newInfo.SourceGo = baseInfo.SourceGo;
                var titleNew= newInfo.GetStringVariable(NTooltipKeys.Title);
                var descNew = newInfo.GetStringVariable(NTooltipKeys.Description);
                titleNew =ConvertKeywords(titleNew);
                newInfo.SetStringVariable(NTooltipKeys.Title, titleNew);
                if (!StringHelper.IsNull(descNew))
                    descNew = ConvertKeywords(descNew);
                newInfo.SetStringVariable(NTooltipKeys.Description, descNew);
                ShowTooltip(newInfo);
            }
        }

        public static void SetTooltip(NTooltipInfo baseInfo)
        {
            var title= baseInfo.GetStringVariable(NTooltipKeys.Title);
            var desc = baseInfo.GetStringVariable(NTooltipKeys.Description);
            List<KeywordData> keywords = ExtractKeywords(desc);
            ExtractKeywords(title,ref keywords);
            title =ConvertKeywords(title);
            desc = ConvertKeywords(desc);
            baseInfo.SetStringVariable(NTooltipKeys.Title, title);
            baseInfo.SetStringVariable(NTooltipKeys.Description, desc);
            AddKeywordTooltip(baseInfo,keywords);
        }

        private static void ShowTooltip(NTooltipInfo newInfo)
        {
            if (NTooltipManager.Instance.ShownSourceList.Contains(newInfo.Source)) 
                return;
            if (newInfo.BlockTooltip)
            {
                NTooltipManager.Instance.HideTooltip();
                return;
            }
            NTooltipManager.Instance.ShownSourceList.Add(newInfo.Source + "_" + $"{NTooltipManager.Instance.ShownSourceList.Count}");
            NTooltipManager.Instance.ShowTooltip(newInfo);
        }

        public static List<KeywordData> ExtractKeywords(string fullText)
        {
            List<KeywordData> keywords = new List<KeywordData>();
            return ExtractKeywords(fullText, ref keywords);
        }
        
        public static List<KeywordData> ExtractKeywords(string fullText,ref List<KeywordData> keywords)
        {
            if (string.IsNullOrEmpty(fullText) || Catalog == null)
                return keywords;
                        
            foreach (var keyword in Catalog.KeywordList)
            {
                if (keyword == null)
                    continue;
                            
                string keywordText = IDENTIFIER_START + keyword.GetKeywordID() + IDENTIFIER_END;
                if (fullText.Contains(keywordText) && !keywords.Contains(keyword))
                    keywords.Add(keyword);
            }
                        
            return keywords;
        }

        public static string ApplyKeywords(this string fullText)
        {
            if (string.IsNullOrEmpty(fullText) || Catalog == null)
                return fullText;

            fullText = ConvertKeywords(fullText);
            return fullText;
        }
        
        public static string ApplyKeywords(this string fullText,List<KeywordData> keywords)
        {
            if (string.IsNullOrEmpty(fullText) || Catalog == null)
                return fullText;

            fullText = ConvertKeywords(fullText);
            return fullText;
        }

        private static string ConvertKeywords(string fullText,List<KeywordData> keywords)
        {
            if (keywords == null || keywords.Count == 0)
                return fullText;

            string convertedText = fullText;
            foreach (var keyword in keywords)
            {
                if (keyword == null)
                    continue;

                string keywordText = IDENTIFIER_START + keyword.GetKeywordID() + IDENTIFIER_END;
                convertedText = convertedText.Replace(keywordText, keyword.GetKeywordName());
                var descText = IDENTIFIER_START + DESC + IDENTIFIER_END;
                convertedText = convertedText.Replace(descText, keyword.GetKeywordDescription());
            }
            return convertedText;
        }
        public static string ConvertKeywords(string fullText)
        {
            return ConvertKeywords(fullText, Catalog.KeywordList);
        }
        
        public static string GetRarityKeyword(this NRarity rarity)
        {
            return rarity switch
            {
                NRarity.Common => "<COMMON>",
                NRarity.Uncommon => "<UNCOMMON>",
                NRarity.Rare => "<RARE>",
                NRarity.Epic => "<EPIC>",
                NRarity.Legendary => "<LEGENDARY>",
                NRarity.Cursed => "<CURSED>",
                _ => "<COMMON>"
            };
        }

    }
}