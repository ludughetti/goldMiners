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
        
        void Start()
        {
            if (_pathNodes == null)
                GeneratePath();
            
            _openNodes = new List<PathNode>();
            _closedNodes = new List<PathNode>();
        }

        void OnDrawGizmos()
        {
            if (_pathNodes == null)
                return;

            Gizmos.color = Color.blue;

            foreach (PathNode pathNode in _pathNodes)
            {
                foreach (PathNode adjacentNode in pathNode.AdjacentNodes)
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

            float closestSqrDistance = float.MaxValue;

            foreach (PathNode pathNode in _pathNodes)
            {
                float sqrDistance = (pathNode.Position - position).sqrMagnitude;

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestNode = pathNode;
                }
            }

            return closestNode;
        }

        private PathNode GetNextOpenNode()
        {
            if (_openNodes.Count == 0)
                return null;

            PathNode openNode = null;

            switch (pathfindingStrategy)
            {
                case PathfindingStrategy.BreadthFirst:
                    openNode = _openNodes[0];
                    break;
                
                case PathfindingStrategy.DepthFirst:
                    openNode = _openNodes[^1];
                    break;
            }

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

        private void OpenAdjacentNodes(PathNode node)
        {
            foreach (PathNode pathNode in node.AdjacentNodes)
            {
                if (pathNode.CurrentState != PathNode.State.Unreviewed)
                    continue;

                pathNode.Parent = node;

                OpenNode(pathNode);
            }
        }

        private void ResetNodes()
        {
            foreach (PathNode pathNode in _pathNodes)
            {
                if (pathNode.CurrentState == PathNode.State.Unreviewed)
                    continue;

                pathNode.CurrentState = PathNode.State.Unreviewed;
                pathNode.Parent = null;
            }

            _openNodes.Clear();
            _closedNodes.Clear();
        }

        private Stack<PathNode> GeneratePath(PathNode originNode, PathNode destinationNode)
        {
            Stack<PathNode> path = new Stack<PathNode>();
            PathNode currentNode = destinationNode;

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

            PathNode originNode = FindClosestNode(origin);
            PathNode destinationNode = FindClosestNode(destination);

            OpenNode(originNode);

            while (_openNodes.Count > 0 && path == null)
            {
                PathNode openNode = GetNextOpenNode();

                if (openNode == destinationNode)
                    path = GeneratePath(originNode, destinationNode);
                else
                    OpenAdjacentNodes(openNode);

                CloseNode(openNode);
            }

            ResetNodes();

            return path;
        }
    }
}