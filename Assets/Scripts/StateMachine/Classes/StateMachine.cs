using System;
using System.Collections.Generic;

public class StateMachine
{
    StateNode _current;
    Dictionary<Type, StateNode> _stateNodes = new();
    HashSet<ITransition> _anyTransitions = new();
    public IState CurrentState => _current?.State;

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
            ChangeState(transition.ToState);
        
        _current.State?.Update();
    }

    public void FixedUpdate()
    {   
        _current.State?.FixedUpdate();
    }

    public void SetState(IState state)
    {
        _current = _stateNodes[state.GetType()];
        _current.State?.OnEnter();
    }

    public void ChangeState(IState newState)
    {
        if (_current != null && _current.State == newState)
            return;
        var previousState = _current?.State;
        var nextState = _stateNodes[newState.GetType()].State;
        previousState?.OnExit();
        _current = _stateNodes[newState.GetType()];
        nextState?.OnEnter();
    }

    ITransition GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.Condition.Evaluate())
                return transition;
        foreach (var transition in _current.Transitions)
            if (transition.Condition.Evaluate())
                return transition;
        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        GetOrAddNote(from).AddTransition(GetOrAddNote(to).State, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        _anyTransitions.Add(new Transition(GetOrAddNote(to).State, condition));
    }

    StateNode GetOrAddNote(IState state)
    {
        var node = _stateNodes.GetValueOrDefault(state.GetType());
        if (node == null)
        {
            node = new StateNode(state);
            _stateNodes.Add(state.GetType(), node);
        }
        return node;
    }


    class StateNode
    {
        public IState State {  get; }
        public HashSet<Transition> Transitions { get; }

        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<Transition>();
        }

        public void AddTransition(IState toState, IPredicate condition)
        {
            Transitions.Add(new Transition(toState, condition));
        }
    }
}