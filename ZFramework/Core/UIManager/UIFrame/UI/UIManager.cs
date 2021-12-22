using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum PanelLayer
{
    Panel,
    Pop,
    Tips,
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
    private Stack<PanelData> _panelStack;
    private Dictionary<PanelLayer, Transform> layerDict;
    public void Init()
    {
        ////not need write anyting
    }
    private UIManager()
    {
        canvas = new GameObject("UICanvas", new Type[] { typeof(Canvas), typeof(CanvasScaler)}).GetComponent<Canvas>();
        GameObject.DontDestroyOnLoad(canvas.gameObject);
        uiCamera = new GameObject("UICamera", new Type[] { typeof(Camera) }).GetComponent<Camera>();
        Transform eventTrans = new GameObject("EventSystem", new Type[] { typeof(EventSystem),typeof(StandaloneInputModule), typeof(BaseInput) }).transform;

        TransformUtils.TransformWorldNormalize(canvas.gameObject);
        TransformUtils.TransformLocalNormalize(uiCamera.gameObject, canvas.gameObject);
        TransformUtils.TransformLocalNormalize(eventTrans.gameObject, canvas.gameObject);

        CameraUtils.SetUICameraParma(uiCamera);

        InitUICanvas();

        layerDict = new Dictionary<PanelLayer, Transform>();
        _panelStack = new Stack<PanelData>();
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

    public void OpenPanel<T>(params object[] args) where T : BasePanel
    {
        string className = typeof(T).FullName;
        if (IsContainUI(className))
        {
            int panelCount = GetStackCount();
            for (int i = 0; i < panelCount; i++)
            {
                PanelData panelData = _panelStack.Peek();
                if (panelData.name.Equals(className))
                {
                    BasePanel basePanel = panelData.basePanel;
                    basePanel.gameObject.SetActive(true);
                    basePanel.OnShowed();
                    return;
                }
                else
                {
                    ClosePanel();
                }
            }
        }
        else
        {
            BasePanel basePanel = canvas.gameObject.AddComponent(typeof(T)) as BasePanel;
            basePanel.Init(args);
            if (_panelStack.Count > 0)
            {
                BasePanel lastPanel = _panelStack.Peek().basePanel;
                lastPanel.panel.SetActive(false);
                lastPanel.OnClosing();
            }
            _panelStack.Push(new PanelData(className, basePanel));
            string adressPath = basePanel.adressPath;
            GameObject panelObj = PrefabUtils.SafeInstanceUI(adressPath, obj => {
                InitUIPrefab(basePanel,obj);
            });
            if (panelObj != null)
            {
                InitUIPrefab(basePanel, panelObj);
            }
        }
    }

   
    public bool IsContainUI(string name)
    {
            foreach (var item in _panelStack)
            {
                if (item.name.Equals(name))
                {
                    return true;
                }
            }
            return false;
    }

    public int GetStackCount()
    {
        return _panelStack.Count;
    }

    public void ClosePanel()
    {
        if (_panelStack.Count > 0)
        {
          PanelData panelData = _panelStack.Pop();
          BasePanel panel = panelData.basePanel;
          panel.OnClosing();
          panel.OnClosed();
            GameObject.Destroy(panel.panel);
            GameObject.Destroy(panel);
          return;
        }
        ZLogUtil.LogError("没有面板关闭了");
    }

    private void InitUIPrefab(BasePanel basePanel, GameObject obj)
    {
        basePanel.panel = obj;
        Transform panelTrans = basePanel.panel.transform;
        PanelLayer layer = basePanel.panelLayer;
        Transform parent = layerDict[layer];
        panelTrans.SetParent(parent, false);
        basePanel.panel.SetActive(true);
        basePanel.AutoInit();
        basePanel.OnShowing();
        basePanel.OnShowed();
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

