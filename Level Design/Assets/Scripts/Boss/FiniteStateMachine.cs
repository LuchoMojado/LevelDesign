using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    State _currentState = null;

    Dictionary<Boss.BossStates, State> _allStates = new Dictionary<Boss.BossStates, State>();
    public void Update()
    {
        _currentState?.OnUpdate();
    }

    public void AddState(Boss.BossStates name, State state)
    {
        if (!_allStates.ContainsKey(name))
            _allStates.Add(name, state);
        else
            _allStates[name] = state;

        state.fsm = this;
    }

    public void ChangeState(Boss.BossStates state)
    {
        _currentState?.OnExit();
        if (_allStates.ContainsKey(state))
            _currentState = _allStates[state];
        _currentState.OnEnter();
    }
}
