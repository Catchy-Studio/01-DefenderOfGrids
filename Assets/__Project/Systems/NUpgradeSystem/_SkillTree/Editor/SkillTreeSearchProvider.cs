using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Editor
{
    public class SkillTreeSearchProvider : ScriptableObject
    {
        public static List<SkillData> FindAllSkillData()
        {
            var guids = AssetDatabase.FindAssets("t:SkillData");
            var result = new List<SkillData>();
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var skill = AssetDatabase.LoadAssetAtPath<SkillData>(path);
                if (skill != null)
                {
                    result.Add(skill);
                }
            }
            
            return result;
        }

        public static List<SkillData> SearchSkills(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
                return FindAllSkillData();

            var allSkills = FindAllSkillData();
            searchQuery = searchQuery.ToLower();

            return allSkills.Where(s => 
                s.GetTitle().ToLower().Contains(searchQuery) ||
                s.GetID().ToLower().Contains(searchQuery) ||
                s.GetDesc().ToLower().Contains(searchQuery)
            ).ToList();
        }

        public static List<SkillData> FindOrphanedSkills(SkillTreeController controller)
        {
            if (controller == null)
                return new List<SkillData>();

            var allSkills = FindAllSkillData();
            var usedSkills = controller.AllNodeList
                .Where(n => n != null && n.Data != null)
                .Select(n => n.Data)
                .ToList();

            return allSkills.Where(s => !usedSkills.Contains(s)).ToList();
        }

        public static List<SkillData> FindCircularDependencies()
        {
            var allSkills = FindAllSkillData();
            var problematicSkills = new List<SkillData>();

            foreach (var skill in allSkills)
            {
                if (HasCircularDependency(skill, new HashSet<SkillData>()))
                {
                    problematicSkills.Add(skill);
                }
            }

            return problematicSkills;
        }

        private static bool HasCircularDependency(SkillData skill, HashSet<SkillData> visited)
        {
            if (visited.Contains(skill))
                return true;

            visited.Add(skill);

            var requirements = skill.GetRequiredSkillList();
            if (requirements != null)
            {
                foreach (var req in requirements)
                {
                    if (req != null && HasCircularDependency(req, new HashSet<SkillData>(visited)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

