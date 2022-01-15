using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class TextAssetUtils
{
    public static TextAsset GetTextAsset(string path)
    {
        return ResHanderManager.Instance.GetTextAsset(path);
    }

    public static void SafeGetTextAsset(string path, Action<TextAsset> callBack)
    {
        var obj = ResHanderManager.Instance.GetTextAsset(path);
        if (obj == null)
        {
            ResLoader.Instance.GetTextAsset(path, (hander) => {
                ResHanderManager.Instance.PreAddTextAsset(path,hander);
                TextAsset tempObj = TextAssetUtils.GetTextAsset(path);
                callBack?.Invoke(tempObj);
               
            });
        }
        else
        {
            callBack?.Invoke(obj);
        }
    }


}
