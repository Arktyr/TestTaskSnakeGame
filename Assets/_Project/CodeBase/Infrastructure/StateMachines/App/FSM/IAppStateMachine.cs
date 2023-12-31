using _Project.CodeBase.Infrastructure.StateMachines.States;

namespace _Project.CodeBase.Infrastructure.StateMachines.App.FSM
{
    public interface IAppStateMachine
    {
        void Enter<TState>() where TState : IState;
        void Add<TState>(TState state) where TState : IExitableState;
    }
}