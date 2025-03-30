using System.Collections;
using System.Collections.Generic;
using UGG.Health;
using UGG.Move;
using UnityEngine;

public abstract class StateActionSO : ScriptableObject{
    [SerializeField] protected int statePriority; //状态优先级

    //引用
    protected Animator _animator;
    protected AICombatSystem _combatSystem;
    protected AIMovement _movement;
    protected AIHealthSystem _healthSystem;
    protected Transform self;
    
    //animationID
    protected int animationMoveID = Animator.StringToHash("AnimationMove");
    protected int movementID = Animator.StringToHash("Movement");
    protected int lAtkID = Animator.StringToHash("LAtk");
    protected int runID = Animator.StringToHash("Run");
    protected int horizontalID = Animator.StringToHash("Horizontal");
    protected int verticalID = Animator.StringToHash("Vertical");

    public void InitState(StateMachineSystem stateMachineSystem) {
        _animator = stateMachineSystem.GetComponentInChildren<Animator>();
        _combatSystem = stateMachineSystem.GetComponentInChildren<AICombatSystem>();
        _movement = stateMachineSystem.GetComponent<AIMovement>();
        _healthSystem = stateMachineSystem.GetComponent<AIHealthSystem>();
        self = stateMachineSystem.transform;
    }

    public virtual void OnEnter() {
        
    }

    public abstract void OnUpdate();

    public virtual void OnExit() {
    }

    /// <summary>
    /// 获取状态优先级
    /// </summary>
    /// <returns></returns>
    public int GetStatePriority() => statePriority;
}