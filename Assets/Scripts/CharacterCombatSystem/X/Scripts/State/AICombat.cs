using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AICombat", menuName = "StateMachine/State/AICombat")]
public class AICombat : StateActionSO
{
    //AI战斗状态，各自战斗相关的逻辑就会在这个状态来处理
    [SerializeField] private CombatAblityBase currentAbility;
    //随机左右监视
    private int randomHorizontal;
    
    

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        //AI行为
        AICombatAction();
        //技能动画位移
        UpdateAnimationMotion();
    }

    public override void OnExit()
    {

    }

    /// <summary>
    /// AIAction
    /// </summary>
    private void AICombatAction() {
        if (currentAbility == null) {
            //没有技能，监视敌人
            NoCombatMove();
            GetAbility();
        } else {
            //如果有技能，且上一个技能状态已经结束
            if (!_animator.CheckAnimationTag("Ability")) {
                currentAbility.InvokeAbility();
                
                if(!currentAbility.GetAbilityIsDone())
                    currentAbility = null;
            }
            
        }
        
    }
    /// <summary>
    /// 获得可用技能
    /// </summary>
    private void GetAbility() {
        if (currentAbility == null) {
            currentAbility = _combatSystem.GetAnDoneAbility();
            
        }
    }

    /// <summary>
    /// 技能动画位移
    /// </summary>
    private void UpdateAnimationMotion() {
        if (_animator.CheckAnimationTag("Ability")) {
            float moveSpeed = _combatSystem.GetAnimationMove() * _combatSystem.GetAbilityMoveMult();
            
            _movement.CharacterMoveInterface(_combatSystem.transform.root.forward,moveSpeed, true);
            
        }
    }
    //徘徊
    private void NoCombatMove()
    {
        //没有目标
        if(!_combatSystem.GetCurrentTarget())
            return;
        
        if (_animator.CheckAnimationTag("Motion"))
        {
            //保持距离
            if (_combatSystem.GetCurrentTargetDistance() < 2f + .1f )
            {
                _movement.CharacterMoveInterface(-_combatSystem.GetDirectionForTarget(), 1.4f, true);
                _animator.SetFloat(verticalID, -1f, 0.25f, Time.deltaTime);
                _animator.SetFloat(horizontalID, 0f, 0.25f, Time.deltaTime);
                
                randomHorizontal = GetRandomHorizontal();
                //距离过近
                if (_combatSystem.GetCurrentTargetDistance() < 1.5 + .05f)
                {
                    if (!_animator.CheckAnimationTag("Hit") || !_animator.CheckAnimationTag("Defen"))
                    {
                        //自卫攻击
                        _animator.Play("Attack_0",0,0f);
                        randomHorizontal = GetRandomHorizontal();
                    }
                }
            }
            else if (_combatSystem.GetCurrentTargetDistance() > 2f + .1f && _combatSystem.GetCurrentTargetDistance()< 6.1 + .5f)
            {
                _movement.CharacterMoveInterface(_movement.transform.right * ((randomHorizontal == 0) ? 1 : randomHorizontal), 1.4f, true);
                _animator.SetFloat(verticalID, 0f,0.25f, Time.deltaTime);
                _animator.SetFloat(horizontalID, ((randomHorizontal == 0) ? 1 : randomHorizontal), 0.25f, Time.deltaTime);
            }
            else if (_combatSystem.GetCurrentTargetDistance() > 6.1 + .5f)
            {
                _movement.CharacterMoveInterface(_movement.transform.forward, 1.4f, true);
                _animator.SetFloat(verticalID, 1f, 0.25f, Time.deltaTime);
                _animator.SetFloat(horizontalID, 0f, 0.25f, Time.deltaTime);
              
            }
           
            
            
        }
        else
        {
            _animator.SetFloat(verticalID, 0f);
            _animator.SetFloat(horizontalID, 0f);
            _animator.SetFloat(runID, 0f);
        }
    }

    private int GetRandomHorizontal() => Random.Range(-1, 2);
}
