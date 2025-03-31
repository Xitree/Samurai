using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 添加对场景管理的支持

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar; // 进度条Slider组件
    public TextMeshProUGUI progressText; // 百分比显示的Text组件

    void Start()
    {
        // 初始化进度条为0
        SetProgress(0);
        progressBar.gameObject.SetActive(true);
        progressText.gameObject.SetActive(true);
        // 开始真正的资源加载
        StartCoroutine(LoadSceneAsync());
    }
    

    // 设置进度条的值
    private void SetProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
        if (progressText != null)
        {
            progressText.text = Mathf.RoundToInt(progress * 100) + "%"; // 更新百分比显示
        }
    }

    // 真正的资源加载协程
    IEnumerator LoadSceneAsync() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1); // 替换为你的场景名称
        asyncLoad.allowSceneActivation = false; // 禁止自动激活场景

        float simulatedProgress = 0f;
        while (!asyncLoad.isDone)
        {
            if (simulatedProgress >= 0.9f)
            {
                SetProgress(1f); // 确保进度条最终达到100%
                //再等个0.5秒再激活场景
                yield return new WaitForSeconds(0.5f);
                
                asyncLoad.allowSceneActivation = true; // 允许激活场景
                progressBar.gameObject.SetActive(false);
                progressText.gameObject.SetActive(false);
                break;
            }
            else
            {
                SetProgress(simulatedProgress); // 更新进度条的值
            }

            // 计算延迟时间
            float delay = Mathf.Clamp01((asyncLoad.progress - simulatedProgress) * 0.05f);
            yield return new WaitForSeconds(delay); // 根据实际进度与模拟进度的差值调整延迟时间
            if (asyncLoad.progress - simulatedProgress < 0.01f)
                simulatedProgress += asyncLoad.progress;
            simulatedProgress += delay;
            
        }
    }
}