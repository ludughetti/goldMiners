using Gameplay;
using UnityEngine;

namespace StateMachine.States
{
    public class GoingToUnloadState : FsmState<Miner>
    {
        private Deposit[] _cachedDeposits;
        
        protected override void OnInitialize()
        {
            _cachedDeposits = Object.FindObjectsByType<Deposit>(FindObjectsSortMode.None);
        }

        public override void Enter()
        {
            var deposit = FindNearestDeposit();
            
            if (deposit == null)
            {
                Debug.Log("[GoingToUnloadState] No deposit assigned! Falling back to next state...");
                Owner.ResetCurrentOre();
                Owner.OnDepositReached?.Invoke();
                return;
            }

            Owner.AssignDeposit(deposit);
            Owner.SetDirection(Owner.DepositTarget.transform.position);
        }

        public override void Update()
        {
            if (Owner.HasReachedDestination())
                Owner.OnDepositReached?.Invoke();
        }

        public override void Exit() { }
        
        private Deposit FindNearestDeposit()
        {
            var closestDist = float.MaxValue;
            Deposit closest = null;

            foreach (var deposit in _cachedDeposits)
            {
                var dist = (deposit.transform.position - Owner.transform.position).sqrMagnitude;

                // If it's not the closest available, continue
                if (!(dist < closestDist)) 
                    continue;
                
                closestDist = dist;
                closest = deposit;
            }

            return closest;
        }
    }
}
