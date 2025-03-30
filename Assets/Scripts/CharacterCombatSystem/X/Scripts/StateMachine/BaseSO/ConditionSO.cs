using System.Collections;
using System.Collections.Generic;
using UGG.Health;
using UGG.Move;
using UnityEngine;


public abstract class ConditionSO : ScriptableObject
{
    [SerializeField] protected int priority;//条件优先级
    //引用
    protected AICombatSystem _combatSystem;
    protected AIMovement _movement;
    protected AIHealthSystem _healthSystem;
    protected Animator animator;
    
    public void InitCondition(StateMachineSystem stateMachineSystem) {
        _combatSystem = stateMachineSystem.GetComponentInChildren<AICombatSystem>();
        _movement = stateMachineSystem.GetComponent<AIMovement>();
        _healthSystem = stateMachineSystem.GetComponent<AIHealthSystem>();
        animator = stateMachineSystem.GetComponentInChildren<Animator>();
    }
    
    
    public abstract bool ConditionSetUp();//条件是否成立

    /// <summary>
    /// 获取当前条件的优先级
    /// </summary>
    /// <returns></returns>
    public int GetConditionPriority() => priority;

    
}
