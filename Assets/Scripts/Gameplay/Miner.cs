using Pathfinding;
using StateMachine;
using StateMachine.States;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gameplay
{
    [RequireComponent(typeof(PathNodeAgent))]
    public class Miner : MonoBehaviour
    {
        [Header("States")]
        [SerializeField] private IdleState idleState = new();
        [SerializeField] private MiningState miningState = new();
        [SerializeField] private UnloadingState unloadingState = new();
        private GoingToMineState _goingToMineState = new();
        private GoingToUnloadState _goingToUnloadState = new();
        
        public UnityEvent OnMineFound { get; } = new();
        public UnityEvent OnMineReached { get; } = new();
        public UnityEvent OnOreMined { get; } = new();
        public UnityEvent OnDepositReached { get; } = new();
        public UnityEvent OnOreDeposited { get; } = new();
        public UnityEvent OnMineDepleted { get; } = new();

        public int CurrentOre { get; private set; }
        public Mine CurrentMine { get; private set; }
        public Deposit DepositTarget { get; private set; }
        
        private PathNodeAgent _pathNodeAgent;
        private FiniteStateMachine<Miner> _fsm;

        private void Awake()
        {
            // Agent setup
            _pathNodeAgent = GetComponent<PathNodeAgent>();
            idleState.Initialize(this);
            _goingToMineState.Initialize(this);
            miningState.Initialize(this);
            _goingToUnloadState.Initialize(this);
            unloadingState.Initialize(this);
            
            // Assign gameplay settings
            CurrentOre = 0;
            CurrentMine = null;
            DepositTarget = null;
        }

        private void Start()
        {
            FsmState<Miner>[] states = { idleState, _goingToMineState, miningState, _goingToUnloadState, unloadingState };
            UnityEvent[] events = { OnMineFound, OnMineReached, OnOreMined, OnDepositReached, OnOreDeposited, OnMineDepleted };
            
            _fsm = new FiniteStateMachine<Miner>(states, events, idleState);

            _fsm.ConfigureTransition(idleState, _goingToMineState, OnMineFound);
            _fsm.ConfigureTransition(_goingToMineState, miningState, OnMineReached);
            _fsm.ConfigureTransition(miningState, _goingToUnloadState, OnOreMined);
            _fsm.ConfigureTransition(_goingToUnloadState, unloadingState, OnDepositReached);
            _fsm.ConfigureTransition(unloadingState, _goingToMineState, OnOreDeposited); // If mine can still be mined -> Unload > Return to mine 
            _fsm.ConfigureTransition(unloadingState, idleState, OnMineDepleted); // If mine cannot be mined anymore -> Unload > Idle
        }

        private void Update()
        {
            _fsm.Update();
        }
        
        public void AssignMine(Mine mine) => CurrentMine = mine;
        public void AssignDeposit(Deposit deposit) => DepositTarget = deposit;
        public void SetDirection(Vector3 direction) =>_pathNodeAgent.Destination = direction;
        public bool HasReachedDestination() => _pathNodeAgent.HasReachedDestination;
        public void AddOre(int minedOre) => CurrentOre += minedOre;
        public void ResetCurrentOre() => CurrentOre = 0;
    }
}
