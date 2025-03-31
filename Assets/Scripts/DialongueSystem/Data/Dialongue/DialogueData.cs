using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue Data")]
public class DialogueData : ScriptableObject {
    // 对话节点
    [Header("对话节点")]
    public DialogueNode[] dialogueNodes;
}

[Serializable]
public class DialogueNode{
    [Header("角色名字")]
    public string characterName;
    [Header("角色头像")]
    public Sprite characterIcon;
    [Header("对话内容"),TextArea]
    public string content;
}