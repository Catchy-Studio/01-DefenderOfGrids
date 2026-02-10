using _NueCore.ManagerSystem.Core;
using UnityEngine;

namespace _NueExtras.NColorSystem
{
    public class NColorManager : NManagerBase
    {
        [SerializeField] private NColorCatalog catalog;
        
        public override void NAwake()
        {
            base.NAwake();
            NColorStatic.SetCatalog(catalog);
        }
    }
}