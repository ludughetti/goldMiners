using Gameplay;
using UnityEngine;

namespace StateMachine.States
{
    public class MiningState : FsmState<Miner>
    {
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
            Owner.MineOre();

            if(Owner.CurrentMine.IsDepleted || Owner.HasReachedMaxCapacity())
                Owner.OnOreMined?.Invoke();
        }

        public override void Exit() { }
    }
}
