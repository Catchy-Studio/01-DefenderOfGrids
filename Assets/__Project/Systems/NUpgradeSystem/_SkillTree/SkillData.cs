﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using __Project.Systems.NUpgradeSystem._SkillTree.Requirements;
using _NueCore.Common.Extensions;
using _NueCore.Common.Utility;
using _NueCore.NStatSystem;
using _NueCore.SaveSystem;
using _NueCore.SceneSystem;
using _NueExtras.RaritySystem;
using _NueExtras.StockSystem;
using NueGames.NTooltip._Keyword;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
    [InlineEditor(InlineEditorModes.FullEditor)]
    [CreateAssetMenu(fileName = "SkillData", menuName = "N/UpgradeSystem/SkillData", order = 0)]
    public class SkillData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite sprite;
        [SerializeField,TextArea(3,5)] private string title;
        [SerializeField,TextArea(5,10)] private string description;
        [SerializeField] private NRarity rarity;
        [SerializeField] private NTags tags;
        [SerializeField] private NCategory category;
        [SerializeField] private int revealDepth =0;
        [SerializeField] private bool isAutoUnlocked;
        [SerializeField] private bool isPermaLocked;
        [SerializeField] private List<SkillData> targetSkillsToRevealList = new List<SkillData>();
        [SerializeField, BoxGroup("Reveal Conditions"), LabelText("Stat Requirements")] 
        private List<NStatRequirement> revealStatRequirements = new List<NStatRequirement>();
        [SerializeField] private List<SkillTier> skillTierList = new List<SkillTier>();
        [SerializeField] private List<SkillData> requiredSkillList = new List<SkillData>();
        [SerializeField] private bool requireMaxSkills;
        [SerializeField] private UnityEvent purchasedEvent;
        
        
        // Port connection mapping: stores which port each required skill connects to (input port)
        [SerializeField,ReadOnly,HideInInspector] private List<string> requiredSkillPortTypes = new List<string>();
        
        // Stores which output port is used from the required skill
        [SerializeField,ReadOnly,HideInInspector] private List<string> requiredSkillOutputPortTypes = new List<string>();

        public int RevealDepth => revealDepth;

        public bool IsAutoUnlocked => isAutoUnlocked;

        public bool IsPermaLocked => isPermaLocked;

        public UnityEvent PurchasedEvent => purchasedEvent;

        public bool RequireMaxSkills => requireMaxSkills;
        
        #region Methods

        public string GetID()
        {
            return id;
        }

        public string GetTitle()
        {
            return title;
        }

        public NRarity GetRarity()
        {
            return rarity;
        }
        public NTags GetTags()
        {
            return tags;
        }
        
        public NCategory GetCategory()
        {
            return category;
        }

       
        public Sprite GetSprite()
        {
            return sprite;
        }
        public string GetDesc(NStatList tStat = null)
        {
            var ds = description.ApplyKeywords();
            if (tStat != null)
            {
                ds = ds.ConvertNStatStr(tStat);
            }
            return ds;
        }


        public string ConvertCurrentNextStats(string baseText, NStatList statList,bool isMaxed)
        {
            var str = new StringBuilder();
            str.Append(baseText);
            var save = NSaver.GetSaveData<UpgradeSave>();
            foreach (var stat in statList.GetStatList())
            {
                var ss =save.GetSkillTreeStatSave(stat.Key);
                var current = ss.GetTotal();
                str = str.Replace($"#c_{stat.Key}", $"Current: {current.ToString("0.0").Colorize(isMaxed ? Color.yellow : Color.white)}");
               
                var nextValue = ss.GetTotal();
                var c = stat.GetStatCategory();
                if (c is NStatCategory.Flat)
                {
                    nextValue = (ss.flatValue +stat.GetValue());
                    nextValue += ss.percentValue * nextValue / 100f;
                }
                else if (c is NStatCategory.Percent)
                {
                    nextValue = (ss.flatValue);
                    nextValue += (ss.percentValue + stat.GetValue()) * nextValue / 100f;                }
               
                str = str.Replace($"#n_{stat.Key}", isMaxed ? "": $" -> Next: {nextValue.ToString("0.0").Colorize(Color.green)}");
            }
            return str.ToString();
        }
        
        public List<SkillTier> GetSkillTierList()
        {
            return skillTierList;
        }
        
        public List<SkillData> GetRequiredSkillList() 
        {
            return requiredSkillList;
        }
        
        public List<SkillData> GetTargetSkillsToRevealList()
        {
            return targetSkillsToRevealList;
        }

        public List<NStatRequirement> GetRevealStatRequirements()
        {
            return revealStatRequirements;
        }

        public bool AreRevealStatRequirementsSatisfied()
        {
            if (revealStatRequirements == null || revealStatRequirements.Count == 0)
                return true;
            
            foreach (var statReq in revealStatRequirements)
            {
                if (!statReq.IsSatisfied())
                    return false;
            }
            
            return true;
        }

        public bool IsSkillMaxed()
        {
            var save = NSaver.GetSaveData<UpgradeSave>();
            var t = save.GetNodeSave(GetID());
            return t.purchaseCount >= skillTierList.Count;
        }
      

        public bool IsAllRequirementsSatisfied()
        {
            var save = NSaver.GetSaveData<UpgradeSave>();
            foreach (var reqSkill in requiredSkillList)
            {
                if (!save.IsSkillUnlocked(reqSkill))
                    return false;
                if (RequireMaxSkills)
                {
                    if (!reqSkill.IsSkillMaxed())
                        return false;
                }

                
                
            }

            return true;
        }

        #endregion

        #region Ports
        public List<string> GetRequiredSkillPortTypes()
        {
            return requiredSkillPortTypes;
        }


        public void SetPortTypeForRequiredSkill(int index, string portType)
        {
            // Ensure list is large enough
            while (requiredSkillPortTypes.Count <= index)
            {
                requiredSkillPortTypes.Add("TopInput");
            }
            requiredSkillPortTypes[index] = portType;
        }
        
        public string GetPortTypeForRequiredSkill(int index)
        {
            if (index < 0 || index >= requiredSkillPortTypes.Count)
                return "TopInput"; // Default port type
            return requiredSkillPortTypes[index];
        }
        
        public List<string> GetRequiredSkillOutputPortTypes()
        {
            return requiredSkillOutputPortTypes;
        }
        
        public void SetOutputPortTypeForRequiredSkill(int index, string portType)
        {
            // Ensure list is large enough
            while (requiredSkillOutputPortTypes.Count <= index)
            {
                requiredSkillOutputPortTypes.Add("RightOutput");
            }
            requiredSkillOutputPortTypes[index] = portType;
        }
        
        public string GetOutputPortTypeForRequiredSkill(int index)
        {
            if (index < 0 || index >= requiredSkillOutputPortTypes.Count)
                return "RightOutput"; // Default port type
            return requiredSkillOutputPortTypes[index];
        }

        #endregion
        
        #region Editor

#if UNITY_EDITOR
        [HorizontalGroup("Buttons"), Button("🎯 Show Node"), GUIColor(0.4f, 0.8f, 1f)]
        public void ShowNode()
        {
            if (SceneManager.GetActiveScene().name != SceneStatic.UpgradeScene)
            {
                return;
            }
            
            var skillTreeController = FindAnyObjectByType<SkillTreeController>();
            if (skillTreeController == null)
            {
                return;
            }
            
            var tList = skillTreeController.AllNodeList;
            for (int i = 0; i < tList.Count; i++)
            {
                var t = tList[i];
                var dt = t.Data;
                if (dt == this)
                {
                    Selection.activeGameObject = t.gameObject;
                    SceneView.FrameLastActiveSceneView();
                    EditorGUIUtility.PingObject(t.gameObject);
                    break;
                }
            }
        }
        
        //[SerializeField] private bool ignoreRandomize;

        //[HorizontalGroup("RandomButtons"), Button("🎲 Randomize"), GUIColor(1f, 0.6f, 0.4f), HideIf(nameof(ignoreRandomize))]
        private void RandomizeData(bool apply)
        {
            if (!apply)
            {
                return;
            }

            // if (ignoreRandomize)
            // {
            //     return;
            // }
            title = "Skill " + Random.Range(1, 100);
            description = "This is a description for " + title + ". It provides various benefits and enhancements to the player.";
            skillTierList.ForEach(tier => tier.RandomizeData());
        }
        
        [BoxGroup("Tier Builder"), TextArea(3, 10)]
        [SerializeField] private string tierConfigString = "";
        
        [BoxGroup("Tier Builder"), Button("📝 Parse and Build Tiers"), GUIColor(0.3f, 0.8f, 0.9f)]
        private void ParseAndBuildTiers()
        {
            if (string.IsNullOrWhiteSpace(tierConfigString))
            {
                Debug.LogWarning("Tier config string is empty!");
                return;
            }
            
            ParseTierConfiguration(tierConfigString);
            EditorUtility.SetDirty(this);
        }
        
        [BoxGroup("Tier Builder"), Button("📋 Get Tier String"), GUIColor(0.9f, 0.8f, 0.3f)]
        private void GetTierString()
        {
            var result = ConvertTiersToString();
            Debug.Log($"Tier Configuration String:\n{result}");
            GUIUtility.systemCopyBuffer = result;
            Debug.Log("String copied to clipboard!");
        }
        
        private void ParseTierConfiguration(string configString)
        {
            // Clear all old tiers
            skillTierList.Clear();
            
            // Split by semicolon to get each category
            var categories = configString.Split(';');
            
            // Dictionary to hold parsed data: [tierIndex][categoryName] = (value, stockType or statCategory)
            var tierDataMap = new Dictionary<int, Dictionary<string, List<(float value, string type)>>>();
            
            int maxTierCount = 0;
            
            foreach (var category in categories)
            {
                if (string.IsNullOrWhiteSpace(category)) continue;
                
                // Extract category name from parentheses
                var openParen = category.IndexOf('(');
                var closeParen = category.IndexOf(')');
                
                if (openParen == -1 || closeParen == -1) continue;
                
                var categoryName = category.Substring(openParen + 1, closeParen - openParen - 1).Trim();
                var valuesString = category.Substring(closeParen + 1).Trim();
                
                // Split values by /
                var values = valuesString.Split('/');
                
                if (values.Length > maxTierCount)
                    maxTierCount = values.Length;
                
                // Parse each tier value
                for (int tierIndex = 0; tierIndex < values.Length; tierIndex++)
                {
                    var valueStr = values[tierIndex].Trim();
                    if (string.IsNullOrWhiteSpace(valueStr)) continue;
                    
                    // Extract number and suffix
                    var parsedData = ParseValue(valueStr);
                    
                    // Initialize tier data if not exists
                    if (!tierDataMap.ContainsKey(tierIndex))
                        tierDataMap[tierIndex] = new Dictionary<string, List<(float, string)>>();
                    
                    if (!tierDataMap[tierIndex].ContainsKey(categoryName))
                        tierDataMap[tierIndex][categoryName] = new List<(float, string)>();
                    
                    tierDataMap[tierIndex][categoryName].Add(parsedData);
                }
            }
            
            // Create tiers based on parsed data
            for (int tierIndex = 0; tierIndex < maxTierCount; tierIndex++)
            {
                var newTier = new SkillTier();
                newTier.statList = new NStatList();
                
                if (tierDataMap.ContainsKey(tierIndex))
                {
                    var tierData = tierDataMap[tierIndex];
                    
                    foreach (var categoryEntry in tierData)
                    {
                        var categoryName = categoryEntry.Key;
                        var valuesList = categoryEntry.Value;
                        
                        // Check if it's a Price category (Price, Price2, Price3, etc.)
                        if (categoryName.StartsWith("Price"))
                        {
                            // Add required resources
                            foreach (var valueData in valuesList)
                            {
                                var requiredResource = new RequiredResource
                                {
                                    StockType = ConvertToStockType(valueData.type),
                                    Amount = (int)valueData.value
                                };
                                newTier.requiredResourceList.Add(requiredResource);
                            }
                        }
                        else
                        {
                            // It's a stat category
                            foreach (var valueData in valuesList)
                            {
                                var statCategory = ConvertToStatCategory(valueData.type);
                                
                                // Try to convert to NStatEnum
                                if (categoryName.TryConvertToNStatEnum(out NStatEnum statEnum) && statEnum != NStatEnum.None)
                                {
                                    // Use enum-based stat
                                    newTier.statList.AddStat(statEnum, valueData.value, statCategory);
                                }
                                else
                                {
                                    // Use custom stat
                                    newTier.statList.AddStatCustom(categoryName, valueData.value, statCategory);
                                }
                            }
                        }
                    }
                }
                
                skillTierList.Add(newTier);
            }
            
            Debug.Log($"Successfully parsed {maxTierCount} tiers from configuration string.");
        }
        
        
        
        private (float value, string type) ParseValue(string valueStr)
        {
            valueStr = valueStr.Trim();
            
            // Extract numeric part and suffix
            string numericPart = "";
            string suffix = "";
            
            for (int i = 0; i < valueStr.Length; i++)
            {
                char c = valueStr[i];
                if (char.IsDigit(c) || c == '.' || c == '-')
                {
                    numericPart += c;
                }
                else
                {
                    suffix = valueStr.Substring(i).Trim();
                    break;
                }
            }
            
            float value = 0;
            if (!string.IsNullOrEmpty(numericPart))
            {
                float.TryParse(numericPart, out value);
            }
            
            return (value, suffix);
        }
        
        private string ConvertTiersToString()
        {
            if (skillTierList.Count == 0)
            {
                return "";
            }
            
            // Group data by category
            var priceGroups = new Dictionary<int, List<string>>(); // priceIndex -> tier values
            var statGroups = new Dictionary<string, List<string>>(); // statName -> tier values
            
            for (int tierIndex = 0; tierIndex < skillTierList.Count; tierIndex++)
            {
                var tier = skillTierList[tierIndex];
                
                // Process required resources
                for (int priceIndex = 0; priceIndex < tier.requiredResourceList.Count; priceIndex++)
                {
                    var resource = tier.requiredResourceList[priceIndex];
                    if (!priceGroups.ContainsKey(priceIndex))
                        priceGroups[priceIndex] = new List<string>();
                    
                    var suffix = ConvertStockTypeToSuffix(resource.StockType);
                    priceGroups[priceIndex].Add($"{resource.Amount}{suffix}");
                }
                
                // Process stats
                foreach (var stat in tier.statList.GetStatList())
                {
                    var statKey = stat.Key;
                    if (!statGroups.ContainsKey(statKey))
                        statGroups[statKey] = new List<string>();
                    
                    var suffix = ConvertStatCategoryToSuffix(stat.GetStatCategory());
                    var valueStr = stat.GetValue().ToString("0.##");
                    statGroups[statKey].Add($"{valueStr}{suffix}");
                }
            }
            
            // Build the final string
            var resultParts = new List<string>();
            
            // Add price categories
            foreach (var priceGroup in priceGroups.OrderBy(x => x.Key))
            {
                var categoryName = priceGroup.Key == 0 ? "Price" : $"Price{priceGroup.Key + 1}";
                var values = string.Join("/", priceGroup.Value.ToArray());
                resultParts.Add($"({categoryName}){values}");
            }
            
            // Add stat categories
            foreach (var statGroup in statGroups.OrderBy(x => x.Key))
            {
                var values = string.Join("/", statGroup.Value.ToArray());
                resultParts.Add($"({statGroup.Key}){values}");
            }
            
            return string.Join(";", resultParts);
        }
        
        private string ConvertStockTypeToSuffix(StockTypes stockType)
        {
            switch (stockType)
            {
                case StockTypes.Coin: return "c";
                case StockTypes.Emerald: return "e";
                case StockTypes.Gem: return "g";
                default: return "c";
            }
        }
        
        private string ConvertStatCategoryToSuffix(NStatCategory category)
        {
            switch (category)
            {
                case NStatCategory.Flat: return "";
                case NStatCategory.Percent: return "P";
                case NStatCategory.Custom1: return "C";
                case NStatCategory.Custom2: return "CC";
                case NStatCategory.Custom3: return "CCC";
                default: return "";
            }
        }
        
        private StockTypes ConvertToStockType(string suffix)
        {
            suffix = suffix.ToLower();
            
            if (suffix.Contains("c"))
                return StockTypes.Coin;
            else if (suffix.Contains("e"))
                return StockTypes.Emerald;
            else if (suffix.Contains("g"))
                return StockTypes.Gem;
            
            return StockTypes.Coin; // Default
        }
        
        private NStatCategory ConvertToStatCategory(string suffix)
        {
            if (string.IsNullOrEmpty(suffix))
                return NStatCategory.Flat;
            
            suffix = suffix.ToUpper();
            
            if (suffix == "P")
                return NStatCategory.Percent;
            else if (suffix == "C")
                return NStatCategory.Custom1;
            else if (suffix == "CC")
                return NStatCategory.Custom2;
            else if (suffix == "CCC")
                return NStatCategory.Custom3;
            
            return NStatCategory.Flat;
            sprite = SpriteHelper.GetRandomSprite();
        }
        
        public void SetEditor()
        {
            if (skillTierList.Count<=0)
            {
                skillTierList.Add(new SkillTier());
            }
            //Validate();
        }
        
        /// <summary>
        /// Fills the skill data with random values
        /// </summary>
        public void FillRandom(bool apply)
        {
            RandomizeData(apply);
        }
        
        public void RandomizeSprite()
        {
            sprite = SpriteHelper.GetRandomSprite();
        }
#endif


        #endregion

       
    }
}