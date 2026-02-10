using System;
using System.Collections.Generic;
using System.Text;
using _NueCore.Common.NueLogger;
using _NueCore.Common.Utility;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace _NueExtras.NLocalizationSystem
{
    //Main component for localizing TMP_Text
    [RequireComponent(typeof(TMP_Text),typeof(LocalizeStringEvent))]
    public class NLocal_Text : MonoBehaviour
    {
        [SerializeField,ReadOnly] private LocalizeStringEvent localizedStringEvent;
        [SerializeField,ReadOnly] private TMP_Text tmp;
        [SerializeField,ReadOnly] private string defaultString = String.Empty;
        
        #region Cache
        public Func<string,string> ConvertFunc { get; private set; }
        public Action OnUpdateAction { get; set; }
        public Action<Locale> OnLocaleChangedAction { get; set; }
        private TMP_FontAsset _baseFont;
        
        private TMP_Text TMP
        {
            get
            {
                if (tmp == null)
                    tmp = GetComponent<TMP_Text>();
                return tmp;
            }
        }
        public LocalizeStringEvent LocalizedStringEvent
        {
            get
            {
                if (localizedStringEvent == null)
                    localizedStringEvent = GetComponent<LocalizeStringEvent>();
                return localizedStringEvent;
            }
        }
        #endregion

        #region Setup

        private void Awake()
        {
            _baseFont = TMP.font;
        }

        private void OnEnable()
        {
            NLocalizationManager.Register(this);
            LocalizedStringEvent.OnUpdateString.AddListener(UpdateText);
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            OnLocaleChanged(LocalizationSettings.SelectedLocale);
            UpdateText(LocalizedStringEvent.StringReference.GetLocalizedString());
        }

        private void OnDisable()
        {
            NLocalizationManager.Unregister(this);
            LocalizedStringEvent.OnUpdateString.RemoveListener(UpdateText);
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }
        
        #endregion

        #region Methods

       
        private void OnLocaleChanged(Locale locale)
        {
            if (NLocalizationManager.Instance)
            {
                if (!NLocalizationManager.Instance.TryCheckLocalizedFont(locale,TMP))
                    TMP.font = _baseFont;
            }
            OnLocaleChangedAction?.Invoke(locale);
        }

        public void AddConverter(Func<string, string> converter)
        {
            ConvertFunc += converter;
        }
        private void UpdateText(string context)
        {
            var text = context;
            if (ConvertFunc != null)
                text = ConvertFunc.Invoke(text);
            text = text.NApplyLocal();
            TMP.SetText(text);
            OnUpdateAction?.Invoke();
        }
        public void ChangeDefault(string value)
        {
            defaultString = value;
        }
        #endregion

        #region Editor

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            LocalizedStringEvent.OnUpdateString.RemoveListener(UpdateEditor);
            LocalizedStringEvent.OnUpdateString.AddListener(UpdateEditor);
            UpdateEditor("");
            if (StringHelper.IsNull(defaultString))
                ChangeDefault(TMP.text);
        }

        private void UpdateEditor(string str)
        {
            if (LocalizedStringEvent.StringReference.IsEmpty)
            {
                TMP.SetText(str);
                return;
            }
            var tRef = LocalizedStringEvent.StringReference;
            if (tRef != null)
                TMP.SetText(tRef.GetLocalizedString());
        }

       
        [Button(ButtonSizes.Gigantic),GUIColor(1,0.5f,1f)]
        private void CreateEntry(string id,NTables table = NTables.NTable,bool includeSelfID = true,bool includePath = true)
        {
            if (!LocalizedStringEvent.StringReference.IsEmpty)
            {
                "Entry has key!".NLog(Color.red);
                return;
            }
            var p = transform.parent;
            var pathStr = new StringBuilder();
            var tList = new List<Transform>();
            tList.Add(p);
            while (p != null )
            {
                tList.Add(p);
                p = p.parent;
            }

            var x =tList.Find(x => x.name == "Prefab Mode in Context");
            if (x)
                tList.Remove(x);
            tList.Reverse();
            if (includePath)
            {
                tList.Add(transform);
                for (int i = 0; i < tList.Count; i++)
                {
                    var item = tList[i];
                    pathStr.Append(item.name);
                    if (i < tList.Count - 1)
                    {
                        pathStr.Append("/");
                    }
                }
            }
           

            //pathStr.Append(" (").Append(TMP.text).Append(")");
            if (!StringHelper.IsNull(id))
            {
                pathStr.Append("(").Append(id).Append(")");
            }
            var identifier = pathStr.ToString();
            if (StringHelper.IsNull(identifier))
            {
                identifier = "NLOCAL_TEXT";
            }

            if (StringHelper.IsNull(defaultString))
                ChangeDefault(TMP.text);
            NLocalStatic.TryCreateEntry(localizedStringEvent.StringReference,
                TMP.text,
                identifier,
                table.GetTableName(),
                includeSelfID);
        }
        
        [Button(ButtonSizes.Medium),GUIColor(0.5f,0.5f,1f)]
        private void SetTableDefault()
        {
            if (LocalizedStringEvent == null)
                return;
            NLocalStatic.SetTableDefault(LocalizedStringEvent.StringReference, TMP.text);
        }
        [Button(ButtonSizes.Small),GUIColor(0.5f,0.25f,0.3f)]
        private void ChangeDefaultEditor(bool apply, string value)
        {
            if (!apply)
            {
                return;
            }
            ChangeDefault(value);
        }

#endif

        #endregion
    }
}