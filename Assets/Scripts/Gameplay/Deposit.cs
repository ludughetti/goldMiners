using UnityEngine;

namespace Gameplay
{
    public class Deposit : MonoBehaviour
    {
        public int TotalOre { get; private set; }

        private void Start()
        {
            TotalOre = 0;
        }

        public void DepositOre(int amount)
        {
            TotalOre += amount;
        }
    }
}
