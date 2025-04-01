using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTip : MonoBehaviour{
    

    // Update is called once per frame
    void Update()
    {
        // 新增: 获取主摄像机的前向向量
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0; // 忽略 Y 轴，只在 XZ 平面上朝向摄像机
        transform.rotation = Quaternion.LookRotation(-cameraForward);
    }
}
