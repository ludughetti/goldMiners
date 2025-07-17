using Pathfinding;
using UnityEngine;

namespace Terrains
{
    public class TerrainMud: MonoBehaviour
    {
        [SerializeField] private float speedReductionPerc = 0.5f;

        private void OnTriggerEnter (Collider other)
        {
            var agent = other.gameObject.GetComponentInParent<PathNodeAgent>();

            if (agent)
                agent.MovementSpeed *= speedReductionPerc;
        }

        private void OnTriggerExit (Collider other)
        {
            var agent = other.gameObject.GetComponentInParent<PathNodeAgent>();

            if (agent)
                agent.MovementSpeed /= speedReductionPerc;
        }
    }
}
