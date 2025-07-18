using UnityEngine;
using UnityEngine.Events;

namespace StateMachine
{
    public class FiniteStateMachine<T>
    {
        private DoubleEntryTable<FsmState<T>, UnityEvent, FsmState<T>> _fsmTable;
        private FsmState<T> _currentState;
        
        public FiniteStateMachine(FsmState<T>[] states, UnityEvent[] transitionEvents, FsmState<T> entryState)
        {
            _fsmTable = new DoubleEntryTable<FsmState<T>, UnityEvent, FsmState<T>>(states, transitionEvents);
            _currentState = entryState;

            _currentState.Enter();
        }

        private void OnTriggerTransition(UnityEvent transitionEvent)
        {
            var targetState = _fsmTable[_currentState, transitionEvent];
            Debug.Log($"[StateMachine] Attempting transition: {_currentState} -> {targetState} via {transitionEvent}");

            if (targetState == null) 
                return;
            
            _currentState.Exit();
            targetState.Enter();

            _currentState = targetState;
            Debug.Log($"[StateMachine] Transition completed: {_currentState} -> {targetState} via {transitionEvent}");
        }

        public void ConfigureTransition(FsmState<T> sourceState, FsmState<T> targetState, UnityEvent transitionEvent)
        {
            _fsmTable[sourceState, transitionEvent] = targetState;
            transitionEvent.AddListener(() => OnTriggerTransition(transitionEvent));
        }

        public void Update() => _currentState.Update();
    }
}
