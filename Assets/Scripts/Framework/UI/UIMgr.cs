using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 层级枚举
/// </summary>
public enum E_UILayer{
    /// <summary>
    /// 最底层
    /// </summary>
    Bot,

    /// <summary>
    /// 中层
    /// </summary>
    Mid,

    /// <summary>
    /// 高层
    /// </summary>
    Top,

    /// <summary>
    /// 系统层 最高层
    /// </summary>
    System,
}

/// <summary>
/// 管理所有UI面板的管理器
/// 注意：面板预设体名要和面板类名一致！！！！！
/// </summary>
public class UIMgr : BaseManager<UIMgr>{
    /// <summary>
    /// 主要用于里式替换原则 在字典中 用父类容器装载子类对象
    /// </summary>
    // private abstract class BasePanelInfo { }
    //
    // /// <summary>
    // /// 用于存储面板信息 和加载完成的回调函数的
    // /// </summary>
    // /// <typeparam name="T">面板的类型</typeparam>
    // private class PanelInfo<T> : BasePanelInfo where T:BasePanel
    // {
    //     public T panel;
    //     public UnityAction<T> callBack;
    //     public bool isHide;
    //
    //     public PanelInfo(UnityAction<T> callBack)
    //     {
    //         this.callBack += callBack;
    //     }
    // }
    private Camera uiCamera;

    private Canvas uiCanvas;
    private EventSystem uiEventSystem;

    // //层级父对象
    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    /// <summary>
    /// 用于存储所有的面板对象
    /// </summary>
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private UIMgr() {
        //删除已有的canvas和event system
        GameObject.Destroy(GameObject.Find("EventSystem"));
        GameObject.Destroy(GameObject.Find("Canvas"));
        
        //动态创建唯一的Canvas和EventSystem（摄像机）
        uiCamera = GameObject.Instantiate(Resources.Load<GameObject>("Camera/UICamera")).GetComponent<Camera>();
        uiCamera.name = "UICamera";
        //ui摄像机过场景不移除 专门用来渲染UI面板
        GameObject.DontDestroyOnLoad(uiCamera.gameObject);

        //动态创建Canvas
        uiCanvas = GameObject.Instantiate(Resources.Load<GameObject>("Camera/Canvas")).GetComponent<Canvas>();
        uiCanvas.name = "Canvas";
        //设置使用的UI摄像机
        uiCanvas.worldCamera = uiCamera;
        //过场景不移除
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

        //找到层级父对象
        bottomLayer = uiCanvas.transform.Find("Bot");
        middleLayer = uiCanvas.transform.Find("Mid");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");
        InitLayer();

        //动态创建EventSystem
        uiEventSystem = GameObject.Instantiate(Resources.Load<GameObject>("Camera/EventSystem"))
            .GetComponent<EventSystem>();
        uiEventSystem.name = "EventSystem";
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
    }

    private void InitLayer() {
        foreach (var layerType in Enum.GetValues(typeof(E_UILayer))) {
            switch (layerType) {
                case E_UILayer.Bot:
                    if (bottomLayer == null) {
                        GameObject empty = new GameObject("Bot" ,typeof(RectTransform));
                        bottomLayer = GameObject.Instantiate(empty, uiCanvas.transform).transform;
                        bottomLayer.name = "Bot";
                        //设置ui strecth
                        RectTransform re = bottomLayer.GetComponent<RectTransform>();
                        InitRectTransform(re);
                    }
                    break;
                case E_UILayer.Mid:
                    if (middleLayer == null) {
                        GameObject empty = new GameObject("Mid", typeof(RectTransform));
                        middleLayer = GameObject.Instantiate(empty, uiCanvas.transform).transform;
                        middleLayer.name = "Mid";
                        
                        RectTransform re = middleLayer.GetComponent<RectTransform>();
                        InitRectTransform(re);
                    }
                    break;
                case E_UILayer.Top:
                    if (topLayer == null) {
                        GameObject empty = new GameObject("Top" ,typeof(RectTransform));
                        topLayer = GameObject.Instantiate(empty, uiCanvas.transform).transform;
                        topLayer.name = "Top";
                        
                        RectTransform re = topLayer.GetComponent<RectTransform>();
                        InitRectTransform(re);
                    }
                    break;
                case E_UILayer.System:
                    if (systemLayer == null) {
                        GameObject empty = new GameObject("System" ,typeof(RectTransform));
                        systemLayer = GameObject.Instantiate(empty, uiCanvas.transform).transform;
                        systemLayer.name = "System";
                        
                        RectTransform re = systemLayer.GetComponent<RectTransform>();
                        InitRectTransform(re);
                    }
                    break;
            }
        }

        void InitRectTransform(RectTransform re) {
            re.anchorMin = new Vector2(0, 0);
            re.anchorMax = new Vector2(1, 1);
            re.sizeDelta = new Vector2(0, 0);
            re.offsetMin = new Vector2(0, 0);
            re.offsetMax = new Vector2(0, 0);
        }
    }
    /// <summary>
    /// 获取对应层级的父对象
    /// </summary>
    /// <param name="layer">层级枚举值</param>
    /// <returns></returns>
    public Transform GetLayerFather(E_UILayer layer) {
        switch (layer) {
            case E_UILayer.Bot:
                return bottomLayer;
            case E_UILayer.Mid:
                return middleLayer;
            case E_UILayer.Top:
                return topLayer;
            case E_UILayer.System:
                return systemLayer;
            default:
                return null;
        }
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    /// <param name="layer">面板显示的层级</param>
    /// <param name="callBack">由于可能是异步加载 因此通过委托回调的形式 将加载完成的面板传递出去进行使用</param>
    /// <param name="isSync">是否采用同步加载 默认为false</param>
    public T ShowPanel<T>(E_UILayer layer = E_UILayer.Top) where T : BasePanel {
        //获取面板名 预设体名必须和面板类名一致 
        string panelName = typeof(T).Name;
        //存在面板
        if (panelDic.ContainsKey(panelName)) {
            return panelDic[panelName] as T;
        }
        // Debug.Log("创建面板");
        //层级的处理
        Transform father = GetLayerFather(layer);
        //避免没有按指定规则传递层级参数 避免为空
        if (father == null)
            father = middleLayer;
        //将面板预设体创建到对应父对象下 并且保持原本的缩放大小
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName), father, false);

        //获取对应UI组件返回出去
        T panel = panelObj.GetComponent<T>();
        panelDic.Add(panelName,panel);
        //显示面板时执行的默认方法
        panel.ShowMe();
        return panel;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void HidePanel<T>() where T : BasePanel {
        string panelName = typeof(T).Name;
        //判断当前显示的面板 有没有该名字的面板
        if( panelDic.ContainsKey(panelName) )
        {
            //删除面板
            GameObject.Destroy(panelDic[panelName].gameObject);
            //删除面板后 从 字典中移除
            panelDic.Remove(panelName);
        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    public T GetPanel<T>() where T : BasePanel {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;

        //如果没有 直接返回空
        return null;
    }


    /// <summary>
    /// 为控件添加自定义事件
    /// </summary>
    /// <param name="control">对应的控件</param>
    /// <param name="type">事件的类型</param>
    /// <param name="callBack">响应的函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type,
        UnityAction<BaseEventData> callBack) {
        //这种逻辑主要是用于保证 控件上只会挂载一个EventTrigger
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}