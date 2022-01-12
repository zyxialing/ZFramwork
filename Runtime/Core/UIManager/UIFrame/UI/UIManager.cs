using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum PanelLayer
{
    Panel,
    PanelUp,
    Pop,
    Tips,
    Overlay
}

public struct PanelData
{
    public BasePanel basePanel;
    public string name;

    public PanelData(string name,BasePanel basePanel)
    {
        this.basePanel = basePanel;
        this.name = name;
    }
}

/// <summary>
/// 面板管理器只有打开和关闭的功能！！！！！！！
/// </summary>
public class UIManager : Singleton<UIManager>
{
    //画布
    private Canvas canvas;
    private Camera uiCamera;
    private Dictionary<PanelLayer, Stack<BasePanel>> _panelStacks;
    private Dictionary<PanelLayer, Transform> layerDict;
    public void Init()
    {
        ////not need write anyting
    }
    private UIManager()
    {
        canvas = new GameObject("UICanvas", new Type[] { typeof(Canvas), typeof(CanvasScaler)}).GetComponent<Canvas>();
        GameObject.DontDestroyOnLoad(canvas.gameObject);
        uiCamera = CameraUtils.CreateCamer("UICamera");
        Transform eventTrans = new GameObject("EventSystem", new Type[] { typeof(EventSystem),typeof(StandaloneInputModule), typeof(BaseInput) }).transform;

        TransformUtils.TransformWorldNormalize(canvas.gameObject);
        TransformUtils.TransformLocalNormalize(uiCamera.gameObject, canvas.gameObject);
        TransformUtils.TransformLocalNormalize(eventTrans.gameObject, canvas.gameObject);

        CameraUtils.SetUICameraParma(uiCamera);

        InitUICanvas();

        layerDict = new Dictionary<PanelLayer, Transform>();
        _panelStacks = new Dictionary<PanelLayer, Stack<BasePanel>>();
        foreach (PanelLayer pl in Enum.GetValues(typeof(PanelLayer)))
        {
            string name = pl.ToString();
            GameObject obj = new GameObject(name, new Type[] { typeof(Canvas),typeof(GraphicRaycaster) });
            TransformUtils.TransformLocalNormalize(obj, canvas.gameObject);
            RectTransformUtils.SetStretch(obj);
            LayerUtils.SetUILayer(obj);
            layerDict.Add(pl, obj.transform);
        }
    }

    public void OpenPanel<T>(Action callback=null,params object[] args) where T : BasePanel
    {
        string className = typeof(T).FullName;

        BasePanel basePanel = canvas.gameObject.AddComponent(typeof(T)) as BasePanel;
        basePanel.Init(args);
        if (_panelStacks.ContainsKey(basePanel.panelLayer))
        {
            if (_panelStacks[basePanel.panelLayer].Count > 0)
            {
                BasePanel lastPanel = _panelStacks[basePanel.panelLayer].Peek();
                lastPanel.OnHide();
                lastPanel.panel.SetActive(false);
            }
            _panelStacks[basePanel.panelLayer].Push(basePanel);
        }
        else
        {
            Stack<BasePanel> panels = new Stack<BasePanel>();
            panels.Push(basePanel);
            _panelStacks.Add(basePanel.panelLayer, panels);
        }

        string adressPath = basePanel.adressPath;
        GameObject panelObj = PrefabUtils.SafeInstanceUI(adressPath, obj => {
            InitUIPrefab(basePanel,obj,callback);
        });
        if (panelObj != null)
        {
            InitUIPrefab(basePanel, panelObj,callback);
        }
        
    }

    public void ClosePanel(PanelLayer panelLayer)
    {
        if (_panelStacks.ContainsKey(panelLayer))
        {
            if (_panelStacks[panelLayer].Count > 0)
            {
                BasePanel panel = _panelStacks[panelLayer].Pop();
                panel.OnHide();
                panel.OnClosing();
                panel.panel.SetActive(false);
                if (_panelStacks[panelLayer].Count > 0)
                {
                    BasePanel basePanel =  _panelStacks[panelLayer].Peek();
                    basePanel.panel.SetActive(true);
                    basePanel.OnOpen();
                }
                GameObject.Destroy(panel.panel);
                GameObject.Destroy(panel);
                return;
            }
        }
        ZLogUtil.LogError("没有面板关闭了");
    }

    private void InitUIPrefab(BasePanel basePanel, GameObject obj,Action callback)
    {
        basePanel.panel = obj;
        Transform panelTrans = basePanel.panel.transform;
        PanelLayer layer = basePanel.panelLayer;
        Transform parent = layerDict[layer];
        panelTrans.SetParent(parent, false);
        basePanel.panel.SetActive(true);
        basePanel.AutoInit();
        basePanel.CoroInit(callback);

    }

    private void InitUICanvas()
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCamera;
        var scaler = canvas.gameObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution =ZDefine.Portrait ? new Vector2(ZDefine.StandardScreen.y, ZDefine.StandardScreen.x) : ZDefine.StandardScreen; ;
        scaler.matchWidthOrHeight = ZDefine.Portrait ? 0 : 1;
    }

}

