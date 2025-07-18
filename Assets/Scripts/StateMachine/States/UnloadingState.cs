using System;
using Gameplay;
using UnityEngine;

namespace StateMachine.States
{
    [Serializable]
    public class UnloadingState : FsmState<Miner>
    {
        [SerializeField] private float depositTime = 1f;
        private float _unloadTimer;

        protected override void OnInitialize() { }

        public override void Enter()
        {
            if (Owner.DepositTarget == null)
            {
                Debug.Log("[UnloadingState] No deposit assigned! Falling back to next state...");
                Owner.OnOreDeposited?.Invoke();
                return;
            }
            
            _unloadTimer = depositTime;
        }

        public override void Update()
        {
            _unloadTimer -= Time.deltaTime;
            if (_unloadTimer > 0f)
                return;

            // Deposit ore into the central deposit
            Owner.DepositTarget.DepositOre(Owner.CurrentOre);
            Owner.ResetCurrentOre();

            // Decide where to go next based on mine status
            if (Owner.CurrentMine != null && !Owner.CurrentMine.IsDepleted)
            {
                Owner.OnOreDeposited?.Invoke(); // Continue mining
            }
            else
            {
                // Reset mine if present
                if (Owner.CurrentMine != null)
                {
                    Owner.CurrentMine.SetOccupied(false);
                    Owner.AssignMine(null);
                }
                
                Owner.OnMineDepleted?.Invoke(); // Go idle 
            }
        }

        public override void Exit() { }
    }
}
