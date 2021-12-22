using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class ResLoader : Singleton<ResLoader>
{
    private ResLoader()
    {
       
    }

    public AsyncOperationHandle GetAudioClip(string path, Action<AsyncOperationHandle> callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadAssetAsync<AudioClip>(AdressablePath.Instance.audio_path + path);
        hander.Completed += obj =>
        {
            callBack?.Invoke(obj);
        };
        return hander;
    }

    public AsyncOperationHandle GetMaterial(string path, Action<Material> callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadAssetAsync<Material>(AdressablePath.Instance.material_path + path);
        hander.Completed += obj =>
        {
            callBack?.Invoke(obj.Result as Material);
        };
        return hander;
    }

    public AsyncOperationHandle GetGamePrefab(string path, Action<AsyncOperationHandle> callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadAssetAsync<GameObject>(AdressablePath.Instance.prefab_game_path + path);
        hander.Completed += obj =>
        {
            callBack?.Invoke(obj);
        };
        return hander;
    }

    public AsyncOperationHandle GetUIPrefab(string path, Action<AsyncOperationHandle> callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadAssetAsync<GameObject>(AdressablePath.Instance.prefab_ui_path + path);
        hander.Completed += obj =>
        {
            callBack?.Invoke(obj);
        };
        return hander;
    }

    public AsyncOperationHandle GetScene(string path, Action callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadSceneAsync(AdressablePath.Instance.scene_path + path);
        hander.Completed += obj =>
        {
            callBack?.Invoke();
        };
        return hander;
    }

    public AsyncOperationHandle GetGameSprite(string path, Action<Sprite> callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadAssetAsync<Sprite>(AdressablePath.Instance.sprite_game_path + path);
        hander.Completed += obj =>
        {
            callBack.Invoke(obj.Result as Sprite);
        };
        return hander;
    }

    public AsyncOperationHandle GetUISprite(string path, Action<Sprite> callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadAssetAsync<Sprite>(AdressablePath.Instance.sprite_ui_path + path);
        hander.Completed += obj =>
        {
            callBack.Invoke(obj.Result as Sprite);
        };
        return hander;
    }

    public AsyncOperationHandle GetGameTexture(string path, Action<Texture> callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadAssetAsync<Texture>(AdressablePath.Instance.texture_game_path + path);
        hander.Completed += obj =>
        {
            callBack.Invoke(obj.Result as Texture);
        };
        return hander;
    }

    public AsyncOperationHandle GetUITexture(string path, Action<Texture> callBack)
    {
        AsyncOperationHandle hander = Addressables.LoadAssetAsync<Texture>(AdressablePath.Instance.texture_ui_path + path);
        hander.Completed += obj =>
        {
            callBack.Invoke(obj.Result as Texture);
        };
        return hander;
    }

}
