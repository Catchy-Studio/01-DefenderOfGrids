using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace _NueExtras.NPanSystem.Editor
{
    /// <summary>
    /// Automatically adds MM_CINEMACHINE3 define symbol for Cinemachine 3.0+ support
    /// </summary>
    [InitializeOnLoad]
    public static class NPanSystemDefineSymbols
    {
        private const string CINEMACHINE3_DEFINE = "MM_CINEMACHINE3";

        static NPanSystemDefineSymbols()
        {
            // Check if Unity.Cinemachine exists (Cinemachine 3.0+)
            var cinemachine3Exists = System.Type.GetType("Unity.Cinemachine.CinemachineVirtualCameraBase, Unity.Cinemachine") != null;

            if (cinemachine3Exists)
            {
                AddDefineSymbol(CINEMACHINE3_DEFINE);
            }
        }

        private static void AddDefineSymbol(string define)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
#if UNITY_2023_1_OR_NEWER
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
#else
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
#endif

            if (!defines.Contains(define))
            {
                if (string.IsNullOrEmpty(defines))
                {
                    defines = define;
                }
                else
                {
                    defines += ";" + define;
                }

#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(namedTarget, defines);
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
#endif
                Debug.Log($"NPanSystem: Added {define} scripting define symbol for Cinemachine 3.0+ support");
            }
        }
    }
}

