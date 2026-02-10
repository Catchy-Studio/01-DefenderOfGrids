using System.Collections.Generic;
using _NueExtras.NPanSystem;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
    /// <summary>
    /// Provides boundary information for the skill tree based on all node positions
    /// </summary>
    public class SkillTreeBoundaryProvider : IBoundaryProvider
    {
        private readonly List<SkillNodeBase> _nodes;

        public SkillTreeBoundaryProvider(List<SkillNodeBase> nodes)
        {
            _nodes = nodes;
        }

        public Bounds GetBounds()
        {
            if (_nodes == null || _nodes.Count == 0)
            {
                Debug.LogWarning("SkillTreeBoundaryProvider: No nodes to calculate bounds");
                return new Bounds(Vector3.zero, Vector3.one * 10f);
            }

            // Initialize with first node position
            Vector3 min = _nodes[0].transform.position;
            Vector3 max = _nodes[0].transform.position;

            // Calculate min and max from all nodes
            foreach (var node in _nodes)
            {
                if (node == null) continue;

                Vector3 pos = node.transform.position;
                
                min.x = Mathf.Min(min.x, pos.x);
                min.y = Mathf.Min(min.y, pos.y);
                min.z = Mathf.Min(min.z, pos.z);
                
                max.x = Mathf.Max(max.x, pos.x);
                max.y = Mathf.Max(max.y, pos.y);
                max.z = Mathf.Max(max.z, pos.z);
            }

            Vector3 center = (min + max) / 2f;
            Vector3 size = max - min;

            return new Bounds(center, size);
        }
    }
}

