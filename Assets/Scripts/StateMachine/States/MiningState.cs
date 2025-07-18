using System;
using Gameplay;
using UnityEngine;

namespace StateMachine.States
{
    [Serializable]
    public class MiningState : FsmState<Miner>
    {
        [SerializeField] private int miningRate = 5;
        [SerializeField] private int carryingCapacity = 20;
        
        protected override void OnInitialize() { }

        public override void Enter()
        {
            if (Owner.CurrentMine == null || Owner.CurrentMine.IsDepleted)
            {
                Debug.Log("[MiningState] No mine assigned! Falling back to next state...");
                Owner.OnOreMined?.Invoke();
            }
        }

        public override void Update()
        {
            MineOre();

            if(!CanKeepMining())
                Owner.OnOreMined?.Invoke();
        }

        public override void Exit() { }
        
        private void MineOre()
        {
            // We can't mine more than we can carry, so adjust the amount if needed
            var maxAmountToMine = Mathf.Min(carryingCapacity - Owner.CurrentOre, miningRate);
            var minedOre = Owner.CurrentMine.MineOre(maxAmountToMine);
            
            Owner.AddOre(minedOre);
        }

        private bool CanKeepMining()
        {
            // If mine is not depleted and we still have room to carry
            return !Owner.CurrentMine.IsDepleted && Owner.CurrentOre < carryingCapacity;
        }
    }
}
