using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTest : MonoBehaviour
{
    public string name;
    public string name2;
    public string name3;
    private string curName;
    public void Bgm()
    {
        if (curName != name)
        {
            curName = name;
            AudioManager.Instance.PlayBgmSound(name);
        }
        else
        {
            curName = name3;
            AudioManager.Instance.PlayBgmSound(name3);
        }
    }
    public void Audio()
    {
        AudioManager.Instance.PlayNewSound(name2);
    }
    public void pauseBgm()
    {
        AudioManager.Instance.PauseBgmSound();
    }
    public void resumeBgm()
    {
        AudioManager.Instance.ResumeBgmSound();
    }
    public void changeScene()
    {
        ResLoader.Instance.GetScene("ExampleMain", null);
    }
}
