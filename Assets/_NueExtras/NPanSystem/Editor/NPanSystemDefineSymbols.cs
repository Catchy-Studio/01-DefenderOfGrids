using UnityEditor;
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
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

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

                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                Debug.Log($"NPanSystem: Added {define} scripting define symbol for Cinemachine 3.0+ support");
            }
        }
    }
}

