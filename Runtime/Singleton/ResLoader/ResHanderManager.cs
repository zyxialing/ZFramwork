using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResHanderManager : Singleton<ResHanderManager>
{
    private Dictionary<string, AudioHander> _dicAudios;
    private Dictionary<string, GameObjectHander> _dicObjects;
    private Dictionary<string, TextHander> _dicTexts;
    private ResHanderMono _resHanderMono;
    private ResHanderManager()
    {
        _resHanderMono = new GameObject("ResHanderManager").AddComponent<ResHanderMono>();

        UnityEngine.Object.DontDestroyOnLoad(_resHanderMono);
        _dicObjects = new Dictionary<string, GameObjectHander>();
        _dicAudios = new Dictionary<string, AudioHander>();
        _dicTexts = new Dictionary<string, TextHander>();
  
        //////////////////////////////////////////////////////////////////
     
    }

    public void Init(Action<float> LoadProgress)
    {
        _resHanderMono.InitRes(this, LoadProgress);
    }

    public AudioClip GetAudio(string name)
    {
        if (_dicAudios.ContainsKey(name))
        {
            return _dicAudios[name].audioClip;
        }
        else
        {
            return null;
        }
    }

    public GameObject GetGameObject(string name)
    {
        if (_dicObjects.ContainsKey(name))
        {
            return _dicObjects[name].gameObject;
        }
        else
        {
            return null;
        }
    }

    public TextAsset GetTextAsset(string name)
    {
        if (_dicTexts.ContainsKey(name))
        {
            return _dicTexts[name].textAsset;
        }
        else
        {
            return null;
        }
    }


    #region 资源周期管理
    public void PreLoadNormalAudio(List<string> audioPaths, Action LoadCompleted)
    {
        _resHanderMono.InitNormalAudio(audioPaths, LoadCompleted);
    }

    public void ReleasePreLoadNormalAudio(List<string> audioPaths)
    {
        for (int i = 0; i < audioPaths.Count; i++)
        {
            if (_dicAudios.ContainsKey(audioPaths[i]))
            {
                Addressables.Release(_dicAudios[audioPaths[i]].audioHander);
                _dicAudios.Remove(audioPaths[i]);
            }
        }
    }


    public void PreLoadGameObject(List<string> gameObjectPaths,Action LoadCompleted)
    {
        _resHanderMono.InitGameObject(gameObjectPaths, LoadCompleted);
    }

    public void ReleasePreLoadGameObject(List<string> gameObjectPaths)
    {
        for (int i = 0; i < gameObjectPaths.Count; i++)
        {
            if (_dicAudios.ContainsKey(gameObjectPaths[i]))
            {
                Addressables.Release(_dicObjects[gameObjectPaths[i]].gameObjectHander);
                _dicAudios.Remove(gameObjectPaths[i]);
            }
        }
    }

    public GameObject PreAddGameObject(string path,AsyncOperationHandle gameObjectHander)
    {
        GameObject gameObj = gameObjectHander.Result as GameObject;
        if (!_dicObjects.ContainsKey(path))
        {
            _dicObjects.Add(path, new GameObjectHander(gameObjectHander, gameObj));
            return gameObj;
        }
        else
        {
            Debug.LogError("严重错误");
            return null;
        }
    }

    public void ReleaseAddGameObject(string path)
    {
        if (_dicAudios.ContainsKey(path))
        {
            Addressables.Release(_dicObjects[path].gameObjectHander);
            _dicAudios.Remove(path);
        }
    }

    public void ReleaseAddGameObjects(List<string> paths)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            if (_dicObjects.ContainsKey(paths[i]))
            {
                Addressables.Release(_dicObjects[paths[i]].gameObjectHander);
                _dicObjects.Remove(paths[i]);
            }
        }

    }

    public TextAsset PreAddTextAsset(string path, AsyncOperationHandle textAssetHander)
    {
        TextAsset gameObj = textAssetHander.Result as TextAsset;
        if (!_dicTexts.ContainsKey(path))
        {
            _dicTexts.Add(path, new TextHander(textAssetHander, gameObj));
            return gameObj;
        }
        else
        {
            Debug.LogError("严重错误");
            return null;
        }
    }

    #endregion





    public class ResHanderMono : MonoBehaviour
    {
        ResHanderManager _resHanderManager;
        public void InitRes(ResHanderManager resHanderManager, Action<float> LoadProgress)
        {
            _resHanderManager = resHanderManager;
            StartCoroutine(InitAllRes(LoadProgress));
        }
        private IEnumerator InitAllRes(Action<float> LoadProgress)
        {
            yield return InitCommonAudioRes(null);
            for (int i = 0; i < 10; i++)
            {
                LoadProgress?.Invoke(i * 0.1f);
                yield return new WaitForSeconds(0.19f);
            }
            ///总加载都在这里
            LoadProgress?.Invoke(1);
        }

        public Coroutine InitNormalAudio(List<string> audioPaths,Action LoadCompleted)
        {
            return StartCoroutine(InitNormalAudioRes(audioPaths,LoadCompleted));
        }

        public Coroutine InitGameObject(List<string> gameObjectPaths,Action LoadCompleted)
        {
            return StartCoroutine(InitGameObjectRes(gameObjectPaths, LoadCompleted));
        }

        #region 初始化公用资源

        /// <summary>
        /// 公用音效资源
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitCommonAudioRes(Action LoadCompleted)
        {
            AdressablePath adressablePath = Resources.Load<AdressablePath>(typeof(AdressablePath).ToString());
            int audioCount = 0;
            foreach (var item in adressablePath.commonAudioPaths)
            {
                 ResLoader.Instance.GetAudioClip(item, (audioClipHander) =>
                {
                    audioCount++;
                    AudioClip audioClip = audioClipHander.Result as AudioClip;
                    _resHanderManager._dicAudios.Add(audioClip.name,new AudioHander(audioClipHander,audioClip));
                });
            }
            while (adressablePath.commonAudioPaths.Count!= audioCount)
            {
                yield return null;
            }
            LoadCompleted?.Invoke();
        }

        #endregion

        #region 预加载资源

        /// <summary>
        /// 普通音效
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitNormalAudioRes(List<string> audioPaths, Action LoadCompleted)
        {
            int audioCount = 0;
            foreach (var item in audioPaths)
            {
                ResLoader.Instance.GetAudioClip(item, (audioClipHander) =>
                {
                    audioCount++;
                    AudioClip audioClip = audioClipHander.Result as AudioClip;
                    if (_resHanderManager._dicAudios.ContainsKey(audioClip.name))
                    {
                        Debug.LogError("音效重复！");
                        return;
                    }
                    _resHanderManager._dicAudios.Add(item, new AudioHander(audioClipHander, audioClip));
                });
            }
            while (audioPaths.Count != audioCount)
            {
                yield return null;
            }
            LoadCompleted?.Invoke();
        }

        private IEnumerator InitGameObjectRes(List<string> gameObjectPaths, Action LoadCompleted)
        {
            int gameObjectCount = 0;
            foreach (var item in gameObjectPaths)
            {
                ResLoader.Instance.GetGamePrefab(item, (gameObjectHander) =>
                {
                    gameObjectCount++;
                    GameObject gameObj = gameObjectHander.Result as GameObject;
                    if (_resHanderManager._dicObjects.ContainsKey(gameObj.name))
                    {
                        Debug.LogError("GameObject重复！"+gameObj.name);
                        return;
                    }
                    _resHanderManager._dicObjects.Add(item, new GameObjectHander(gameObjectHander, gameObj));
                });
            }
            while (gameObjectPaths.Count != gameObjectCount)
            {
                yield return null;
            }
            LoadCompleted?.Invoke();
        }

        #endregion
    }
    struct AudioHander
    {
        public AsyncOperationHandle audioHander;
        public AudioClip audioClip;

        public AudioHander(AsyncOperationHandle hander,AudioClip audioClip)
        {
            this.audioHander = hander;
            this.audioClip = audioClip;
        }


    }

    struct GameObjectHander
    {
        public AsyncOperationHandle gameObjectHander;
        public GameObject gameObject;

        public GameObjectHander(AsyncOperationHandle hander, GameObject gameObject)
        {
            this.gameObjectHander = hander;
            this.gameObject = gameObject;
        }
    }

    struct TextHander
    {
        public AsyncOperationHandle textHander;
        public TextAsset textAsset;

        public TextHander(AsyncOperationHandle hander, TextAsset textAsset)
        {
            this.textHander = hander;
            this.textAsset = textAsset;
        }
    }
}
