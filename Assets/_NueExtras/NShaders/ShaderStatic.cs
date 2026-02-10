using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _NueExtras.NShaders
{
    public static class ShaderStatic
    {
        public static void SetRendererFeatureActive(string featureName, bool setEnabled)
        {
            // Get the current URP Asset
            var pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipelineAsset == null)
            {
                Debug.LogWarning("URP Asset not found!");
                return;
            }

            // Use reflection to get the renderer data list
            FieldInfo propertyInfo = pipelineAsset.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            var scriptableRendererDataList = (ScriptableRendererData[])propertyInfo?.GetValue(pipelineAsset);

            if (scriptableRendererDataList != null && scriptableRendererDataList.Length > 0)
            {
                // Get the default renderer (index 0)
                var rendererData = scriptableRendererDataList[0];

                foreach (var feature in rendererData.rendererFeatures)
                {
                    if (feature != null && feature.name == featureName)
                    {
                        feature.SetActive(setEnabled);
                        Debug.Log($"Renderer feature '{featureName}' set to {setEnabled}");
                        return;
                    }
                }
                
                Debug.LogWarning($"Renderer feature '{featureName}' not found!");
            }
        }
    }
}