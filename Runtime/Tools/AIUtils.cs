using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AIUtils
{
    public static ExternalBehavior GetExternalBehavior(string path)
    {
        return ResHanderManager.Instance.GetAI(path);
    }

    public static void SafeGetExternalBehavior(string path, Action<ExternalBehavior> callBack)
    {
        var obj = ResHanderManager.Instance.GetAI(path);
        if (obj == null)
        {
            ResLoader.Instance.GetAI(path, (hander) => {
                ResHanderManager.Instance.PreLoadAI(path,hander);
                ExternalBehavior tempObj = AIUtils.GetExternalBehavior(path);
                callBack?.Invoke(tempObj);
               
            });
        }
        else
        {
            callBack?.Invoke(obj);
        }
    }


}
