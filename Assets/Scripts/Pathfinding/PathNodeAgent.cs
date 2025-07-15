using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class PathNodeAgent : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 2f;
        [SerializeField] private float rotateSpeed = 45f;

        private Stack<PathNode> _currentPath;
        private PathNode _targetNode;
        private Vector3? _destination;

        public bool HasReachedDestination { get; private set; } = false;
        public Vector3? Destination 
        { 
            get => _destination;
            set
            {
                _destination = value;

                if (_destination != null)
                {
                    _currentPath = PathfindingManager.Instance.CreatePath(transform.position, _destination.Value);
                    _targetNode = _currentPath.Pop();
                    HasReachedDestination = false;
                }
            }
        }


        private void Update()
        {
            if (Destination == null || HasReachedDestination)
                return;

            Vector3 targetPosition = _targetNode.Position;
            Vector3 diff = targetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(diff.normalized, transform.up);
            float maxDistanceDelta = movementSpeed * Time.deltaTime;
            float maxDegreesDelta = rotateSpeed * Time.deltaTime;

            Vector3 updatedPosition = Vector3.MoveTowards(transform.position, targetPosition, maxDistanceDelta);
            Quaternion updatedRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta);

            transform.SetPositionAndRotation(updatedPosition, updatedRotation);

            if (diff.sqrMagnitude <= Mathf.Epsilon * Mathf.Epsilon)
            {
                if (_currentPath.Count > 0)
                    _targetNode = _currentPath.Pop();
                else
                    HasReachedDestination = true;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_targetNode == null)
                return;

            float lineThickness = 5f;
            UnityEditor.Handles.color = Color.red;

            UnityEditor.Handles.DrawLine(transform.position, _targetNode.Position, lineThickness);

            if (_currentPath != null && _currentPath.Count > 0)
            {
                PathNode[] nodes = _currentPath.ToArray();

                UnityEditor.Handles.DrawLine(_targetNode.Position, nodes[0].Position, lineThickness);

                for (int i = 0; i < nodes.Length - 1; i++)
                    UnityEditor.Handles.DrawLine(nodes[i].Position, nodes[i + 1].Position, lineThickness);
            }
        }
#endif
    }
}