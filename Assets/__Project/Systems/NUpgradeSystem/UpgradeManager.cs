using System.Collections.Generic;
using __Project.Systems.NUpgradeSystem._SkillTree;
using _NueCore.Common.ReactiveUtils;
using _NueCore.ManagerSystem.Core;
using _NueCore.SettingsSystem;
using _NueExtras.NShaders;
using UniRx;
using UnityEngine;


namespace __Project.Systems.NUpgradeSystem
{
    public class UpgradeManager : NManagerBase
    {
        [SerializeField] private UpgradeHUD upgradeHud;
        [SerializeField] private List<SkillTreeController> skillTreeControllerList;

        public override void NAwake()
        {
            base.NAwake();
            RBuss.OnEvent<SettingsREvents.SettingsChangedREvent>().Subscribe(ev =>
            {
                ShaderStatic.SetRendererFeatureActive("CRT",SettingsStatic.IsCRTOn());
            }).AddTo(gameObject);
            
        }

        public override void NStart()
        {
            base.NStart();
            ShaderStatic.SetRendererFeatureActive("CRT",SettingsStatic.IsCRTOn());
            upgradeHud.Build();
            foreach (var skillTreeController in skillTreeControllerList)
            {
                skillTreeController.Build();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ShaderStatic.SetRendererFeatureActive("CRT",false);


        }
    }
}