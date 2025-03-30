using System.Collections;
using System.Collections.Generic;
using UGG.Combat;
using UnityEngine;

public class ActiveAttackInput : StateMachineBehaviour
{
    private static readonly int lAtkID = Animator.StringToHash("LAtk");
    
    private PlayerCombatSystem _combatSystem;
    private CharacterInputSystem _inputSystem;
    [SerializeField,Header("最大攻击时间间隔")] private float maxApplyAttackTime;

    private float _currentApplyTime;
    //预输入
    private bool _preInput;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //获取玩家攻击系统脚本
        if (_combatSystem==null) {
            _combatSystem=animator.GetComponent<PlayerCombatSystem>();
        }
        //获取玩家输入系统脚本
        if (_inputSystem == null) {
            _inputSystem = animator.transform.root.GetComponent<CharacterInputSystem>();
        }
        
        _currentApplyTime = maxApplyAttackTime;
        _preInput = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        //如果当前是不允许输入攻击信号，那么我们再去计时
        if (!_combatSystem.GetApplyAttackInput()) {
            if (_currentApplyTime > 0) {
                _currentApplyTime -= Time.deltaTime;
                //当计时未结束，如果按了左键设置允许输入攻击信号
                if (_inputSystem.playerLAtk) {
                    _preInput = true;
                }
            }
            if(_currentApplyTime<=0){
                //说明有攻击欲望
                if (_preInput) {
                    animator.SetTrigger(lAtkID);
                }
                    
                
                _combatSystem.SetApplyAttackInput(true);
            }
            
        }
        
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
