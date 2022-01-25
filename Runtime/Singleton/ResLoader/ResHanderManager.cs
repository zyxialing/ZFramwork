using BehaviorDesigner.Runtime;
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
    private Dictionary<string, AIHander> _dicAis;
    private ResHanderMono _resHanderMono;
    private ResHanderManager()
    {
        _resHanderMono = new GameObject("ResHanderManager").AddComponent<ResHanderMono>();

        UnityEngine.Object.DontDestroyOnLoad(_resHanderMono);
        _dicObjects = new Dictionary<string, GameObjectHander>();
        _dicAudios = new Dictionary<string, AudioHander>();
        _dicTexts = new Dictionary<string, TextHander>();
        _dicAis = new Dictionary<string, AIHander>();
        //////////////////////////////////////////////////////////////////

    }

    public ResHanderMono Init()
    {
       return _resHanderMono;
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

    public ExternalBehavior GetAI(string name)
    {
        if (_dicAis.ContainsKey(name))
        {
            return _dicAis[name].behavior;
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
    public ExternalBehavior PreLoadAI(string path, AsyncOperationHandle aiHander)
    {
        ExternalBehavior gameObj = aiHander.Result as ExternalBehavior;
        if (!_dicAudios.ContainsKey(path))
        {
            _dicAis.Add(path, new AIHander(aiHander, gameObj));
            return gameObj;
        }
        else
        {
            Debug.LogError("严重错误");
            return null;
        }
    }

    public AudioClip PreLoadAudio(string path, AsyncOperationHandle audioHander)
    {
        AudioClip gameObj = audioHander.Result as AudioClip;
        if (!_dicAudios.ContainsKey(path))
        {
            _dicAudios.Add(path, new AudioHander(audioHander, gameObj));
            return gameObj;
        }
        else
        {
            Debug.LogError("严重错误");
            return null;
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
        public IEnumerator InitCommonAudioRes()
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
        }

        public IEnumerator InitCommonTextAssetRes()
        {
            AdressablePath adressablePath = Resources.Load<AdressablePath>(typeof(AdressablePath).ToString());
            int audioCount = 0;
            foreach (var item in adressablePath.commonAudioPaths)
            {
                ResLoader.Instance.GetAudioClip(item, (audioClipHander) =>
                {
                    audioCount++;
                    AudioClip audioClip = audioClipHander.Result as AudioClip;
                    _resHanderManager._dicAudios.Add(audioClip.name, new AudioHander(audioClipHander, audioClip));
                });
            }
            while (adressablePath.commonAudioPaths.Count != audioCount)
            {
                yield return null;
            }
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
    struct AIHander
    {
        public AsyncOperationHandle behaviorHander;
        public ExternalBehavior behavior;

        public AIHander(AsyncOperationHandle hander, ExternalBehavior behavior)
        {
            this.behaviorHander = hander;
            this.behavior = behavior;
        }
    }
}
