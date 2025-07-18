using Pathfinding;
using StateMachine;
using StateMachine.States;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    [RequireComponent(typeof(PathNodeAgent))]
    public class Miner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int miningRate = 1;
        [SerializeField] private int carryingCapacity = 20;
        [SerializeField] private float scanInterval = 2f;
        [SerializeField] private float depositTime = 1f;
        
        private IdleState _idleState = new();
        private GoingToMineState _goingToMineState = new();
        private MiningState _miningState = new();
        private GoingToUnloadState _goingToUnloadState = new();
        private UnloadingState _unloadingState = new();
        
        public UnityEvent OnMineFound { get; private set; } = new();
        public UnityEvent OnMineReached { get; private set; } = new();
        public UnityEvent OnOreMined { get; private set; } = new();
        public UnityEvent OnDepositReached { get; private set; } = new();
        public UnityEvent OnOreDeposited { get; private set; } = new();
        public UnityEvent OnMineDepleted { get; private set; } = new();

        public int CurrentOre { get; private set; }
        public Mine CurrentMine { get; private set; }
        public Deposit DepositTarget { get; set; }
        
        private PathNodeAgent _pathNodeAgent;
        private FiniteStateMachine<Miner> _fsm;

        private void Awake()
        {
            // Agent setup
            _pathNodeAgent = GetComponent<PathNodeAgent>();
            _idleState.Initialize(this);
            _goingToMineState.Initialize(this);
            _miningState.Initialize(this);
            _goingToUnloadState.Initialize(this);
            _unloadingState.Initialize(this);
            
            // Assign gameplay settings
            CurrentOre = 0;
            CurrentMine = null;
            DepositTarget = null;
        }

        private void Start()
        {
            FsmState<Miner>[] states = { _idleState, _goingToMineState, _miningState, _goingToUnloadState, _unloadingState };
            UnityEvent[] events = { OnMineFound, OnMineReached, OnOreMined, OnDepositReached, OnOreDeposited, OnMineDepleted };
            
            _fsm = new FiniteStateMachine<Miner>(states, events, _idleState);

            _fsm.ConfigureTransition(_idleState, _goingToMineState, OnMineFound);
            _fsm.ConfigureTransition(_goingToMineState, _miningState, OnMineReached);
            _fsm.ConfigureTransition(_miningState, _goingToUnloadState, OnOreMined);
            _fsm.ConfigureTransition(_goingToUnloadState, _unloadingState, OnDepositReached);
            _fsm.ConfigureTransition(_unloadingState, _goingToMineState, OnOreDeposited); // If mine can still be mined -> Unload > Return to mine 
            _fsm.ConfigureTransition(_unloadingState, _idleState, OnMineDepleted); // If mine cannot be mined anymore -> Unload > Idle
        }

        private void Update()
        {
            _fsm.Update();
        }

        public float GetScanInterval() => scanInterval;

        public void AssignMine(Mine mine)
        {
            if (CurrentMine == mine) 
                return;
            
            CurrentMine = mine;
        }
        
        public void AssignDeposit(Deposit deposit) => DepositTarget = deposit;

        public void SetDirection(Vector3 direction)
        {
            _pathNodeAgent.Destination = direction;
        }

        public bool HasReachedDestination() => _pathNodeAgent.HasReachedDestination;

        public void MineOre()
        {
            // We can't mine more than we can carry, so adjust the amount if needed
            var maxAmountToMine = Mathf.Min(carryingCapacity - CurrentOre, miningRate);
            
            var minedOre = CurrentMine.MineOre(maxAmountToMine);
            CurrentOre += minedOre;
        }
        
        public bool HasReachedMaxCapacity() => CurrentOre >= carryingCapacity;
        
        public float GetDepositTime() => depositTime;

        public void ResetCurrentOre() => CurrentOre = 0;
    }
}
