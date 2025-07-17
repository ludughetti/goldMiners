namespace StateMachine
{
    public abstract class FsmState<T>
    {
        protected T Owner;

        public void Initialize(T owner)
        {
            this.Owner = owner;
            OnInitialize();
        }

        protected abstract void OnInitialize();
        
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();
    }
}
