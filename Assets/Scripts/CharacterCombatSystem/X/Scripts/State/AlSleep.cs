using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AISleep", menuName = "StateMachine/State/AISleep")]
public class AlSleep : StateActionSO
{
    public override void OnUpdate() {
        Debug.Log("默认状态");
    }
}
