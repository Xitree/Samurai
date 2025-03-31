using System.Collections;
using System.Collections.Generic;
using TMPro;
using UGG.Move;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : BasePanel {
    private PlayerMovementController _playerMovementController;
    
    private Image _characterImage;

    private TextMeshProUGUI _characterName;

    private TextMeshProUGUI _dialogueText;

    // 当前对话数据
    private DialogueData _currentDialogueData;
    //索引
    private int _index;

    // 新增: 跟踪当前对话是否正在逐字显示
    private bool _isTyping;
    private float _typeSpeed = 0.05f;
    protected override void Awake() {
        base.Awake();
        _characterImage = GetControl<Image>("CharacterImage");
        _characterName = GetControl<TextMeshProUGUI>("TexName");
        _dialogueText = GetControl<TextMeshProUGUI>("TexLetter");
        _playerMovementController = GameObject.FindWithTag("Player").GetComponent<PlayerMovementController>();
        
        _index = 0;
    }

    private void OnEnable() {
        EventCenter.Instance.AddEventListener<DialogueData>(E_EventType.E_Dialongue_Start, OnDialogueDataReceived);
    }

    private void OnDisable() {
        EventCenter.Instance.RemoveEventListener<DialogueData>(E_EventType.E_Dialongue_Start, OnDialogueDataReceived);
        //触发对话结束
        EventCenter.Instance.EventTrigger(E_EventType.E_Dialongue_End);
        //清除对话结束事件
        EventCenter.Instance.Claer(E_EventType.E_Dialongue_End);
    }

    private void OnDialogueDataReceived(DialogueData data) {
        _currentDialogueData = data;
        //玩家禁止移动
        _playerMovementController.isTalking = true;
        PlayDialogue();
    }

    private void Update() {
        if (CharacterInputSystem.Instance.Interact) {
            if (_index >= _currentDialogueData.dialogueNodes.Length) {
                if(!_isTyping)
                    HideMe();
                return;
            }
            //在逐字显示时禁用按交流键的逻辑
            
            if (_isTyping) {
                _typeSpeed = 0.01f;
            } else {
                _typeSpeed = 0.05f;
                PlayDialogue();
            }
            
        }
    }

    public override void ShowMe() {
        
    }

    public override void HideMe() {
        UIMgr.Instance.HidePanel<DialoguePanel>();
    }
    //播放对话
    private void PlayDialogue() {
        if (_currentDialogueData == null) {
            return;
        }
        if (_index >= _currentDialogueData.dialogueNodes.Length) {
            return;
        }
        DialogueNode dialogueNode = _currentDialogueData.dialogueNodes[_index];
        _characterImage.sprite = dialogueNode.characterIcon;
        _characterName.text = dialogueNode.characterName;
        StartCoroutine(TypeDialogueText(dialogueNode.content));
        _index++;
    }

    // 新增: 协程方法，逐字显示对话内容
    private IEnumerator TypeDialogueText(string text) {
        _dialogueText.text = string.Empty;
        _isTyping = true;
        foreach (char c in text) {
            _dialogueText.text += c;
            yield return new WaitForSeconds(_typeSpeed); // 调整等待时间以控制打印速度
        }
        _isTyping = false;
    }
}