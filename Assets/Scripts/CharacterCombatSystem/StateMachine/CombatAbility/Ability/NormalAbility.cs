using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NormalAbility", menuName = "Ability/NormalAbility")]
public class NormalAbility : CombatAblityBase{
    
    public override void InvokeAbility() {
        //技能逻辑
        if (Animator.CheckAnimationTag("Motion") && abilityIsDone&& Combat.GetCurrentTarget()) {
            if (Combat.GetCurrentTargetDistance() > abilityUseDistance+0.1f) {
                //当技能被激活时，还没有进入允许释放的距离
                Movement.CharacterMoveInterface(Movement.transform.forward, Animator.GetFloat(RunID)==0?1.4f:3f, true);
                Animator.SetFloat(VerticalID, 1f, 0.25f, Time.deltaTime);
                Animator.SetFloat(HorizontalID, 0f, 0.25f, Time.deltaTime);
                Animator.SetFloat(RunID,1f, 0.25f, Time.deltaTime);
                
            } else {
                
                //当技能被激活,设置当前技能移动倍率
                Combat.SetAbilityMoveMult(abilityMult);
                UseAbility();
                //往后翻滚
                MonoBehaviour coroutineRoll = Animator.GetComponent<MonoBehaviour>();
                coroutineRoll.StartCoroutine(NormalAbilityRollBack());
            }
        }
    }

    
    private IEnumerator NormalAbilityRollBack() {
        // 等待一帧确保动画开始
        yield return null;
        
        // 获取当前攻击动画的时长
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
        float attackAnimLength = stateInfo.length;
        Debug.Log(attackAnimLength);
        // 等待攻击动画播放完毕
        yield return new WaitForSeconds(attackAnimLength*0.6f);
        
        // 触发向后翻滚动画
        if(Animator.CheckAnimationName("Skill_1")||Animator.CheckAnimationName("Skill_2"))
            Animator.Play("Roll_B",0,0f);
    }
}
