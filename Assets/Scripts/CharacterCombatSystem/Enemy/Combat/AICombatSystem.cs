using System;
using System.Collections;
using System.Collections.Generic;
using UGG.Combat;
using UGG.Move;
using UnityEngine;

public class AICombatSystem : CharacterCombatSystemBase{
    [SerializeField, Header("范围检测")] private Transform detectionCenter;
    [SerializeField] private float detectionRang;

    [SerializeField] private LayerMask whatIsEnemy;

    [SerializeField] private LayerMask whatIsBos;

    //Speed
    [SerializeField, Header("攻击移动速度倍率"), Range(.1f, 10f)]
    private float attackMoveMult;

    [SerializeField, Header("各种技能移动速度倍率")]
    private float abilityMoveMult;
    //缓存
    private Collider[] detectionedTarget = new Collider[1];

    [SerializeField, Header("目标")] private Transform currentTarget;
    
    
    //AnimationID
    private int lockOnID = Animator.StringToHash("LockOn");

    
    [SerializeField,Header("技能搭配")] private List<CombatAblityBase> abilites = new List<CombatAblityBase>();
    private Dictionary<int, CombatAblityBase> ablityIdDic = new Dictionary<int, CombatAblityBase>();

    private void Start() {
        InitAllAbility();
    }

    private void Update() {
        AIView();
        
        LockOnTarget();
        
    }

    //AI发现目标
    private void AIView() {
        int targetCount =
            Physics.OverlapSphereNonAlloc(detectionCenter.position, detectionRang, detectionedTarget, whatIsEnemy);
        if (targetCount > 0) {
            //射线是否打到墙壁
            if (!Physics.Raycast(transform.root.position + transform.root.up * .5f,
                    detectionedTarget[0].transform.position - transform.root.position
                    , out RaycastHit hit, detectionRang, whatIsBos)) {
                //考虑视野
                // currentTarget = Vector3.Dot((detectionedTarget[0].transform.position - transform.root.position).normalized,
                //     transform.root.forward) > 0.35f ? detectionedTarget[0].transform : null;
                //不考虑
                currentTarget = detectionedTarget[0].transform;    
            }
        }
    }

    #region EditorView

    protected override void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRang);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(attackDetectionCenter.position, attackDetectionRang);
    }

    #endregion
    

    //获取当前目标
    public Transform GetCurrentTarget() {
        if (currentTarget == null) {
            return null;
        }

        return currentTarget;
    }
    //获取当前目标与自己的距离
    public float GetCurrentTargetDistance()=> Vector3.Distance(transform.root.position, currentTarget.position);
    //获取当前目标的方向
    public Vector3 GetDirectionForTarget() {
        if (!currentTarget) {
            return transform.forward;
        }
        return (currentTarget.position - transform.root.position).normalized;
    }
    //锁定目标
    private void LockOnTarget() {
        
        if (_animator.CheckAnimationTag("Motion")||_animator.CheckAnimationTag("Attack") && currentTarget != null) {
            _animator.SetFloat(lockOnID,1f);
            transform.root.rotation = transform.LockOnTarget(currentTarget,transform.root,4.5f);
        } else {
            _animator.SetFloat(lockOnID, 0f);
        }

        if (_animator.CheckAnimationTag("Ability")&&_abilityAnimationEvent.IsLockOnTarget) {
            transform.root.rotation = transform.LockOnTarget(currentTarget,transform.root,100f);
            _animator.SetFloat(lockOnID, 1f);
        }
        
    }
    
    

    public float GetAnimationMove() => _animator.GetFloat(animationMoveID);

    #region 技能

    private void InitAllAbility() {
        if(abilites.Count==0) return;
        foreach (var ab in abilites) {
            ab.InitAbility(_animator,this, _characterMovementBase);
            if (!ab.GetAbilityIsDone()) {
                //如果当前技能不可用，重置
                ab.ResetAbility();
            }
            ablityIdDic.Add(ab.GetAbilityID(),ab);
        }
    }
    
    //得到其中一个可用的技能
    public CombatAblityBase GetAnDoneAbility() {
        foreach (var ab in abilites) {
            if (ab.GetAbilityIsDone())
                return ab;
        }

        return null;
    }
    
    /// <summary>
    /// 由名称得技能 - 效率低
    /// </summary>
    /// <param name="abilityName"></param>
    /// <returns></returns>
    public CombatAblityBase GetAbilityByName(string abilityName) {
        foreach (var ab in abilites) {
            if (ab.GetAbilityName().Equals(abilityName))
                return ab;
        }
        return null;
    }
    //由ID获得技能
    public CombatAblityBase GetAbilityByID(int abilityID) {
        return ablityIdDic.GetValueOrDefault(abilityID);
    }
    //得到技能移动速度倍率
    public float GetAbilityMoveMult()=> abilityMoveMult;
    //SET技能移动速度倍率
    public float SetAbilityMoveMult(float mult)=>abilityMoveMult = mult;
    #endregion
}