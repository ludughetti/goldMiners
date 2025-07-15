using Pathfinding;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(PathNodeAgent))]
    public class PlayerClickMovement : MonoBehaviour
    {
        private Camera _mainCamera;
        private PathNodeAgent _agent;

        void Awake()
        {
            _agent = GetComponent<PathNodeAgent>();
            _mainCamera = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, maxDistance: float.MaxValue))
                    _agent.Destination = raycastHit.point;
            }
        }
    }
}
