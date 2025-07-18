using Gameplay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StateMachine.States
{
    public class IdleState : FsmState<Miner>
    {
        private float _scanInterval;
        private float _scanTimer;
        private Mine[] _cachedMines;

        protected override void OnInitialize()
        {
            _cachedMines = Object.FindObjectsByType<Mine>(FindObjectsSortMode.None);
        }

        public override void Enter()
        {
            _scanInterval = Owner.GetScanInterval();
            _scanTimer = _scanInterval;
        }

        public override void Update()
        {
            _scanTimer -= Time.deltaTime;

            if (!(_scanTimer <= 0f)) 
                return;
            
            _scanTimer = _scanInterval;

            // Try to find a nearby, unoccupied mine
            var closestMine = FindAvailableMine();

            // If there's no available mine, then keep idling
            if (closestMine == null) 
                return;
            
            Owner.AssignMine(closestMine);
            Owner.OnMineFound?.Invoke();
        }

        public override void Exit()
        {
            _scanTimer = 0f;
        }

        private Mine FindAvailableMine()
        {
            var closestDist = float.MaxValue;
            Mine closest = null;

            foreach (var mine in _cachedMines)
            {
                if (mine.IsOccupied || mine.IsDepleted)
                    continue;

                var dist = (mine.transform.position - Owner.transform.position).sqrMagnitude;

                // If it's not the closest available, continue
                if (!(dist < closestDist)) 
                    continue;
                
                closestDist = dist;
                closest = mine;
            }

            if (closest != null)
            {
                Debug.Log($"{Owner.name} found a mine at {closest.transform.position}");
                closest.SetOccupied(true);
            }

            return closest;
        }
    }
}
