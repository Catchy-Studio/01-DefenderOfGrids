using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Editor
{
    [CustomEditor(typeof(SkillData))]
    public class SkillDataEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}

