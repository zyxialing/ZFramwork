using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpManager
{
    public static void JumpPanel<T>(params object[] args) where T: BasePanel
    {
        UIManager.Instance.OpenPanel<T>(args);
    }

    public static void GomeHome(Action callBack)
    {
        UIManager.Instance.CloseAll();
        callBack?.Invoke();
    }
    /// <summary>
    /// 栈堆关闭最上层
    /// </summary>
    /// <param name="panelLayer"></param>
    public static void Close(PanelLayer panelLayer)
    {
        UIManager.Instance.ClosePanel(panelLayer);
    }
}
