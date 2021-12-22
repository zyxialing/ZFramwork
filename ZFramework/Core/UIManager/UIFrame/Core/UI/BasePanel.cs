using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BasePanel : MonoBehaviour {


    public static BasePanel basePanel;

    public string adressPath;
    //
    public GameObject panel;
    //层级
    public PanelLayer panelLayer;
    //
    public Dictionary<string, Transform> uiTransforms;
    //面板参数
    public object[] args;

    //面板的生命周期
    #region 
    //若面板退出时，不销毁则该初始化，只会运行一次
    public virtual void Init(params object[] args)
    {
        this.args = args;
        basePanel = this;
        uiTransforms = new Dictionary<string, Transform>();
    }

    public virtual void AutoInit()
    {

    }

    /// <summary>
    /// 开始启动面板前
    /// </summary>
    public virtual void OnShowing()
    {
      
    }
    /// <summary>
    /// 显示面板后
    /// </summary>
    public virtual void OnShowed()
    {

    }
    /// <summary>
    /// 关闭面板前
    /// </summary>
    public virtual void OnClosing()
    {

    }
    /// <summary>
    /// 关闭面板后
    /// </summary>
    public virtual void OnClosed()
    {

    }

    #endregion

    protected virtual void Close()
    {
        UIManager.Instance.ClosePanel();
    }

}
