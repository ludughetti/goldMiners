using System;
using System.Collections.Generic;
using Common;
using Enums;
using UnityEngine;

namespace Pathfinding
{
    public class PathfindingManager : MonoBehaviourSingleton<PathfindingManager>
    {
        [SerializeField] private PathGenerator pathGenerator = new PathGenerator();
        [SerializeField] private PathfindingStrategy pathfindingStrategy = PathfindingStrategy.BreadthFirst;

        private List<PathNode> _pathNodes;
        private List<PathNode> _openNodes;
        private List<PathNode> _closedNodes;

        private void Start()
        {
            if (_pathNodes == null)
                GeneratePath();
            
            _openNodes = new List<PathNode>();
            _closedNodes = new List<PathNode>();
        }

        private void OnDrawGizmos()
        {
            if (_pathNodes == null)
                return;

            Gizmos.color = Color.blue;

            foreach (var pathNode in _pathNodes)
            {
                foreach (var adjacentNode in pathNode.AdjacentNodes)
                    Gizmos.DrawLine(pathNode.Position, adjacentNode.Position);
            }           
        }

        
        [ContextMenu("Generate Path")]
        private void GeneratePath()
        {
            _pathNodes = pathGenerator.GenerateNodes();
        }

        private PathNode FindClosestNode(Vector3 position)
        {
            PathNode closestNode = null;

            var closestSqrDistance = float.MaxValue;

            foreach (var pathNode in _pathNodes)
            {
                var sqrDistance = (pathNode.Position - position).sqrMagnitude;

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestNode = pathNode;
                }
            }

            return closestNode;
        }

        private PathNode GetNextOpenNode(PathNode destinationNode)
        {
            if (_openNodes.Count == 0)
                return null;

            var openNode = pathfindingStrategy switch
            {
                PathfindingStrategy.BreadthFirst => GetNextOpenNodeBreadthFirst(),
                PathfindingStrategy.DepthFirst => GetNextOpenNodeDepthFirst(),
                PathfindingStrategy.Dijkstra => GetNextOpenNodeDijkstra(),
                PathfindingStrategy.AStar => GetNextOpenNodeAStar(destinationNode),
                _ => null
            };

            return openNode;
        }

        private void OpenNode(PathNode node)
        {
            if (_openNodes.Contains(node))
                return;

            node.CurrentState = PathNode.State.Open;
            _openNodes.Add(node);
        }

        private void CloseNode(PathNode node)
        {
            if (!_openNodes.Contains(node))
                return;

            node.CurrentState = PathNode.State.Closed;
            _openNodes.Remove(node);
            _closedNodes.Add(node);
        }

        private void OpenAdjacentNodes(PathNode parentNode)
        {
            foreach (var pathNode in parentNode.AdjacentNodes)
            {
                if (pathNode.CurrentState != PathNode.State.Unreviewed)
                    continue;

                pathNode.Parent = parentNode;

                switch (pathfindingStrategy)
                {
                    case PathfindingStrategy.Dijkstra:
                    case PathfindingStrategy.AStar:
                        var sqrDistance = (parentNode.Position - pathNode.Position).sqrMagnitude;
                        pathNode.AccumulatedCost = parentNode.AccumulatedCost + sqrDistance * pathNode.CostMultiplier;
                        break;
                }

                OpenNode(pathNode);
            }
        }

        private void ResetNodes()
        {
            foreach (var pathNode in _pathNodes)
            {
                if (pathNode.CurrentState == PathNode.State.Unreviewed)
                    continue;

                pathNode.CurrentState = PathNode.State.Unreviewed;
                pathNode.Parent = null;
                pathNode.AccumulatedCost = 0f;
            }

            _openNodes.Clear();
            _closedNodes.Clear();
        }
        
        private PathNode GetNextOpenNodeBreadthFirst () => _openNodes[0];
        
        private PathNode GetNextOpenNodeDepthFirst () => _openNodes[^1];
        
        private PathNode GetNextOpenNodeDijkstra ()
        {
            var openNode = _openNodes[0];

            foreach (var pathNode in _openNodes)
            {
                if (pathNode.AccumulatedCost < openNode.AccumulatedCost)
                    openNode = pathNode;
            }
            
            return openNode;
        }
        
        private PathNode GetNextOpenNodeAStar (PathNode destinationNode)
        {
            var openNode = _openNodes[0];

            var closestSqrDistance = (destinationNode.Position - openNode.Position).sqrMagnitude;

            foreach (var pathNode in _openNodes)
            {
                var sqrDistance = (destinationNode.Position - pathNode.Position).sqrMagnitude;

                if (pathNode.AccumulatedCost <= openNode.AccumulatedCost && sqrDistance < closestSqrDistance)
                {
                    openNode = pathNode;
                    closestSqrDistance = sqrDistance;
                }
            }
            
            return openNode;
        }

        private Stack<PathNode> GeneratePath(PathNode destinationNode)
        {
            var path = new Stack<PathNode>();
            var currentNode = destinationNode;

            while(currentNode != null)
            {
                path.Push(currentNode);
                currentNode = currentNode.Parent;
            }

            return path;
        }

        public Stack<PathNode> CreatePath(Vector3 origin, Vector3 destination)
        {
            Stack<PathNode> path = null;

            var originNode = FindClosestNode(origin);
            var destinationNode = FindClosestNode(destination);

            OpenNode(originNode);

            while (_openNodes.Count > 0 && path == null)
            {
                var openNode = GetNextOpenNode(destinationNode);

                if (openNode == destinationNode)
                    path = GeneratePath(destinationNode);
                else
                    OpenAdjacentNodes(openNode);

                CloseNode(openNode);
            }

            ResetNodes();

            return path;
        }
    }
}