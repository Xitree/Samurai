using System;
using System.Collections;
using System.Collections.Generic;
using UGG.Move;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class CombatAblityBase : ScriptableObject{
    [SerializeField] protected string abilityName;
    [SerializeField] protected int abilityID;
    [SerializeField] protected float abilityCdTime;
    [SerializeField] protected float abilityUseDistance;
    [SerializeField] protected bool abilityIsDone;
    [SerializeField,Header("技能移动速率倍率"),Range(.1f, 10f)] protected float abilityMult;
    
    protected Animator Animator;
    protected AICombatSystem Combat;
    protected CharacterMovementBase Movement;
    
    //animationID
    protected readonly int HorizontalID = Animator.StringToHash("Horizontal");
    protected readonly int VerticalID = Animator.StringToHash("Vertical");
    protected readonly int RunID= Animator.StringToHash("Run");
    protected readonly int AnimationMove = Animator.StringToHash("AnimationMove");
    protected readonly int MovementID = Animator.StringToHash("Movement");

    
    /// <summary>
    /// 外部调用技能
    /// </summary>
    public abstract void InvokeAbility();

    protected void UseAbility() {
        Animator.Play(abilityName,0,0f);
        abilityIsDone = false;
        
        ResetAbility();
    }

    
    //重置技能
    public void ResetAbility() {
        //技能CD
        //去对象池拿一个计时器出来，通过拿出来的计时器去获取它身上的计时器脚本中的创建计时器函数
        //当传入的时间递减为0时，内部会执行一个委托
        GameObjectPoolSystem.Instance.TakeGameObject("Timer").GetComponent<Timer>().CreateTime(abilityCdTime, () => {
            abilityIsDone= true;
        });
    }
    #region 调用接口

    public void InitAbility(Animator animator, AICombatSystem combat, CharacterMovementBase movement) {
        Animator = animator;
        Combat = combat;
        Movement = movement;
    }
    
    public string GetAbilityName()=> abilityName;
    public int GetAbilityID()=> abilityID;
    public bool GetAbilityIsDone()=> abilityIsDone;
    public void SetAbilityDone(bool done) => abilityIsDone = done;

    


    #endregion
}
