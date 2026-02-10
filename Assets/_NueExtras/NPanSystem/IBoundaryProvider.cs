using UnityEngine;

namespace _NueExtras.NPanSystem
{
    /// <summary>
    /// Interface for objects that can provide boundary information to NPanSystem
    /// </summary>
    public interface IBoundaryProvider
    {
        /// <summary>
        /// Get the bounds of all relevant objects
        /// </summary>
        Bounds GetBounds();
    }
}

