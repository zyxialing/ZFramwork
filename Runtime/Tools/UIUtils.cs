using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    /// <param name="path"></param>
    /// <param name="isUI">isUI ÊÇ·ñÊÇUIÂ·¾¶</param>
    public static void SetSprite(Image image,string path,bool isUI = true)
    {
        image.sprite = ResHanderManager.Instance.GetSprite(path,isUI);
    }

    public static Sprite GetSprite(string path, bool isUI = true)
    {
        return ResHanderManager.Instance.GetSprite(path, isUI);
    }

    public static void Release(string path)
    {
        ResHanderManager.Instance.ReleaseRes(path);
    }
}
