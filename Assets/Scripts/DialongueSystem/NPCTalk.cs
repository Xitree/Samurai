using System;
using System.Collections;
using System.Collections.Generic;
using UGG.Move;
using UnityEngine;

public class NPCTalk : MonoBehaviour
{
    [SerializeField]
    private DialogueData dialogueData;
    private Transform _playerTransform; // 玩家的Transform
    private readonly float _interactionRange = 1f; // 交互距离
    private bool _onTalk = false; // 是否正在对话
    
    private void Awake() {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // 假设玩家有一个Tag为"Player"
        
    }

    private void Update() {
        Talking();
    }

    private void Talking() {
        if (Vector3.Distance(_playerTransform.position, transform.position) <= _interactionRange) {
            
            if (!_onTalk) {
                //TODO 显示对话图标
                
            }
            if (CharacterInputSystem.Instance.Interact && !_onTalk) {
                //监听对话结束事件
                EventCenter.Instance.AddEventListener(E_EventType.E_Dialongue_End, () => {
                    _onTalk = false;
                    _playerTransform.GetComponent<PlayerMovementController>().isTalking= false;
                });
                // 触发对话
                TriggerDialogue();
                _onTalk = true;
            }
        } else {
            //TODO 隐藏对话图标
        }
    }
    
    // 触发对话
    private void TriggerDialogue() {
        UIMgr.Instance.ShowPanel<DialoguePanel>(E_UILayer.System);
        EventCenter.Instance.EventTrigger<DialogueData>(E_EventType.E_Dialongue_Start, dialogueData); // 确保在显示对话框后触发事件
    }
}