using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PrefabUtils
{
    public static GameObject Instance(string path)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if(obj == null)
        {
            Debug.LogError("缓存中不存在:" + path);
            return null;
        }
        else
        {
          return GameObject.Instantiate(obj);
        }
    }
    public static GameObject Instance(string path,Transform parent)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if (obj == null)
        {
            Debug.LogError("缓存中不存在:" + path);
            return null;
        }
        else
        {
            return GameObject.Instantiate(obj,parent);
        }
    }
    public static GameObject Instance(string path,Vector3 position,Quaternion rotation, Transform parent)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if (obj == null)
        {
            Debug.LogError("缓存中不存在:" + path);
            return null;
        }
        else
        {
            return GameObject.Instantiate(obj, position,rotation,parent);
        }
    }
    public static GameObject Instance(string path,Transform parent,bool instantiateInWorldSpace)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if (obj == null)
        {
            Debug.LogError("缓存中不存在:" + path);
            return null;
        }
        else
        {
            return GameObject.Instantiate(obj,parent,instantiateInWorldSpace);
        }
    }


    public static GameObject SafeInstance(string path,Action<GameObject> callBack)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if (obj == null)
        {
            ResLoader.Instance.GetGamePrefab(path,(hander)=> {
                ResHanderManager.Instance.PreAddGameObject(path,hander);
                GameObject tempObj = PrefabUtils.Instance(path);
                callBack?.Invoke(tempObj);
            });
            return null;
        }
        else
        {
            return GameObject.Instantiate(obj);
        }
    }
    public static GameObject SafeInstance(string path, Transform parent, Action<GameObject> callBack)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if (obj == null)
        {
            ResLoader.Instance.GetGamePrefab(path, (hander) => {
                ResHanderManager.Instance.PreAddGameObject(path, hander);
                GameObject tempObj = PrefabUtils.Instance(path, parent);
                callBack?.Invoke(tempObj);
            });
            return null;
        }
        else
        {
            return GameObject.Instantiate(obj, parent);
        }
    }
    public static GameObject SafeInstance(string path, Vector3 position, Quaternion rotation, Transform parent, Action<GameObject> callBack)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if (obj == null)
        {
            ResLoader.Instance.GetGamePrefab(path, (hander) => {
                ResHanderManager.Instance.PreAddGameObject(path,hander);
                GameObject tempObj = PrefabUtils.Instance(path, position, rotation, parent);
                callBack?.Invoke(tempObj);
            });
            return null;
        }
        else
        {
            return GameObject.Instantiate(obj, position, rotation, parent);
        }
    }
    public static GameObject SafeInstance(string path, Transform parent, bool instantiateInWorldSpace, Action<GameObject> callBack)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if (obj == null)
        {
            ResLoader.Instance.GetGamePrefab(path, (hander) => {
                ResHanderManager.Instance.PreAddGameObject(path,hander);
                GameObject tempObj = PrefabUtils.Instance(path, parent, instantiateInWorldSpace);
                callBack?.Invoke(tempObj);

            });
            return null;
        }
        else
        {
            return GameObject.Instantiate(obj, parent, instantiateInWorldSpace);
        }
    }

    public static GameObject InstanceUI(string path)
    {
        return PrefabUtils.Instance(path);
    }

    public static GameObject SafeInstanceUI(string path, Action<GameObject> callBack)
    {
        var obj = ResHanderManager.Instance.GetGameObject(path);
        if (obj == null)
        {
            ResLoader.Instance.GetUIPrefab(path, (hander) => {
                ResHanderManager.Instance.PreAddGameObject(path,hander);
                GameObject tempObj = PrefabUtils.Instance(path);
                callBack?.Invoke(tempObj);
               
            });
            return null;
        }
        else
        {
            return GameObject.Instantiate(obj);
        }
    }


}
