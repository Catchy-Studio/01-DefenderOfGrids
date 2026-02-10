using System.Collections.Generic;
using _NueCore.Common.Utility;
using UnityEngine;

namespace NueGames.NTooltip
{
    public struct NTooltipInfo
    {
        public string CustomTooltipID { get; set; }
        public NTooltipTypes NTooltipType { get; set; }
        public NTooltipLayout Layout { get; set; }
        public string Source { get; set; }
        public GameObject SourceGo { get; set; }
        public bool Is3D { get; set; }
        public bool BlockTooltip { get; set; }
        public Transform FollowTarget { get; set; }
        public Vector3 FollowOffset { get; set; }
        public int PreferredWidth { get; set; }
        public int PreferredHeight { get; set; }
        public KVD<string,Transform> TransformVariables { get; set; }
        public KVD<string,string> StringVariables { get; set; }
        public KVD<string,Sprite> SpriteVariables { get; set; }

        #region Methods
        public string GetID()
        {
            return StringHelper.IsNull(CustomTooltipID) ? NTooltipType.ToString() : CustomTooltipID;
        }

        public string GetStringVariable(string key)
        {
            if (StringVariables == null) return null;
            foreach (var variable in StringVariables)
            {
                if (variable.Key == key)
                {
                    return variable.Value;
                }
            }
            return null;
        }
        
        public Sprite GetSpriteVariable(string key)
        {
            if (SpriteVariables == null) return null;
            foreach (var variable in SpriteVariables)
            {
                if (variable.Key == key)
                {
                    return variable.Value;
                }
            }
            return null;
        }

        public Transform GetTransformVariable(string key)
        {
            if (TransformVariables == null) return null;
            foreach (var variable in TransformVariables)
            {
                if (variable.Key == key)
                {
                    return variable.Value;
                }
            }
            return null;
        }
        
        

        public void SetStringVariable(string key, string text)
        {
            StringVariables ??= new KVD<string, string>();

            if (StringVariables.ContainsKey(key))
                StringVariables[key] = text;
            else
                StringVariables.Add(key, text);
        }
        
        public void SetSpriteVariable(string key, Sprite sprite)
        {
            SpriteVariables ??= new KVD<string, Sprite>();

            if (SpriteVariables.ContainsKey(key))
                SpriteVariables[key] = sprite;
            else
                SpriteVariables.Add(key, sprite);
        }
        

        #endregion

        public void SetTransforms(List<NTooltipField_TMP> tmpFieldList)
        {
            //TransformVariables.Clear();
            TransformVariables ??= new KVD<string, Transform>();
            foreach (var tmpField in tmpFieldList)
            {
                if (!TransformVariables.ContainsKey(tmpField.Key))
                    TransformVariables.Add(tmpField.Key, tmpField.Field.transform);
            }
        }
    }
}