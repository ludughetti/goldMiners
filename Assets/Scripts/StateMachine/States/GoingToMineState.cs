using Gameplay;

namespace StateMachine.States
{
    public class GoingToMineState : FsmState<Miner>
    {
        protected override void OnInitialize() { }

        public override void Enter()
        {
            Owner.SetDirection(Owner.CurrentMine.transform.position);
        }

        public override void Update()
        {
            if (!Owner.HasReachedDestination()) 
                return;

            Owner.OnMineReached?.Invoke();
        }

        public override void Exit() { }
    }
}
