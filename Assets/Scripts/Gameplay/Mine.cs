using UnityEngine;

namespace Gameplay
{
    public class Mine : MonoBehaviour
    {
        [SerializeField]
        private int oreAmount = 100;
        public bool IsOccupied { get; private set; }

        public int MineOre(int requested)
        {
            var mined = Mathf.Min(oreAmount, requested);
            oreAmount -= mined;
            
            return mined;
        }

        public void SetOccupied(bool occupied) => IsOccupied = occupied;

        public bool IsDepleted => oreAmount <= 0;
    }
}
