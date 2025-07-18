using Gameplay;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text minersText;
        [SerializeField] private TMP_Text depositsText;
        
        private Miner[] _miners;
        private Deposit[] _deposits;

        private void Start()
        {
            _miners = Object.FindObjectsByType<Miner>(FindObjectsSortMode.None);
            _deposits = Object.FindObjectsByType<Deposit>(FindObjectsSortMode.None);
        }

        private void Update()
        {
            minersText.text = "Miners:\n";
            foreach (var miner in _miners)
                minersText.text += $"{miner.name}: {miner.CurrentOre} gold\n";

            depositsText.text = "Deposits:\n";
            foreach (var deposit in _deposits)
                depositsText.text += $"{deposit.name}: {deposit.TotalOre} gold\n";
        }
    }
}
