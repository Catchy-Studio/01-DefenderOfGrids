using _NueCore.Common.ReactiveUtils;
using UnityEngine;

namespace _NueExtras.NColorSystem
{
    public static class NColorStatic
    {
        public static NColorCatalog Catalog { get; private set; }

        public static void SetCatalog(NColorCatalog catalog)
        {
            Catalog = catalog;
            RBuss.Publish(new NColorREvents.NColorCatalogChangedREvent());
        }
        
    }
}