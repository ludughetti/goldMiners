using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    [Serializable]
    public class PathGenerator
    {
        [Serializable]
        private struct TerrainLayer
        {
            public string tag;
            public float costMultiplier;
        }
        
        [SerializeField] private Transform lowerLeftLimit;
        [SerializeField] private Transform upperRightLimit;
        [SerializeField, Range(0.1f, 10f)] private float nodesSeparation;
        [SerializeField, Range(0.1f, 1f)] private float clearanceRadius = 0.5f;
        [SerializeField, Range(0.1f, 10f)] private float raycastHeight = 3;
        [SerializeField] private TerrainLayer[] terrainLayers;
        [SerializeField] private LayerMask ignoreLayers;
        
        private bool AreNodesAdjacent(PathNode first, PathNode second, float maxSqrDistance)
        {
            if (first == second) return false;

            // Check distance between nodes
            var diff = first.Position - second.Position;
            if (diff.sqrMagnitude > maxSqrDistance) return false;

            // Check if there's a clear line between nodes
            // to avoid mesh overlap in the middle of a segment
            var start = first.Position + Vector3.up * raycastHeight;
            var end = second.Position + Vector3.up * raycastHeight;
            
            return !Physics.CheckCapsule(start, end, clearanceRadius, ~ignoreLayers);
        }

        public List<PathNode> GenerateNodes()
        {
            var nodes = new List<PathNode>();

            var separationSqr = nodesSeparation * nodesSeparation;
            var maxSqrDistance = separationSqr + separationSqr;

            var startX = lowerLeftLimit.position.x;
            var endX = upperRightLimit.position.x;
            var startZ = lowerLeftLimit.position.z;
            var endZ = upperRightLimit.position.z;

            for (var x = startX; x <= endX; x += nodesSeparation)
            {
                for (var z = startZ; z <= endZ; z += nodesSeparation)
                {
                    var rayOrigin = new Vector3(x, raycastHeight, z);
                    if (!Physics.Raycast(rayOrigin, Vector3.down, out var hitInfo, raycastHeight, ~ignoreLayers))
                        continue;
                    
                    foreach (var terrainLayer in terrainLayers)
                    {
                        // Terrain check
                        if (!hitInfo.collider.CompareTag(terrainLayer.tag)) 
                            continue;
                        
                        // Depth check: ensures there's enough clearance for the agent
                        if (Physics.CheckSphere(rayOrigin, clearanceRadius, ~ignoreLayers)) 
                            continue;
                            
                        var pathNode = new PathNode()
                        {
                            Position = hitInfo.point,
                            CostMultiplier = terrainLayer.costMultiplier,
                            AccumulatedCost = 0f
                        };

                        nodes.Add(pathNode);
                        break;
                    }
                }
            }

            foreach (var node in nodes)
                node.AdjacentNodes = nodes.FindAll(n => AreNodesAdjacent(node, n, maxSqrDistance));

            return nodes;
        }
    }
}