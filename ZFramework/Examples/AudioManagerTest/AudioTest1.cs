using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AudioTest1 : MonoBehaviour
{
    public List<string> audioPaths;
    void Start()
    {
        ResHanderManager.Instance.PreLoadNormalAudio(audioPaths, () => { ZLogUtil.Log("普通音效加载完成"); });
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResHanderManager.Instance.ReleasePreLoadNormalAudio(audioPaths);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ResHanderManager.Instance.PreLoadNormalAudio(audioPaths, () => { ZLogUtil.Log("普通音效加载完成"); });
        }
    }
}
