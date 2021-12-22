using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : Singleton<AudioManager>
{
     private AudioMono audioMono;
     private HashSet<Sound> sounds;
     private Sound bgmSound;
     private AudioManager()
     {
        audioMono = new GameObject("AudioManager").AddComponent<AudioMono>();
        sounds = audioMono.sounds;
        UnityEngine.Object.DontDestroyOnLoad(audioMono);
     }
     
    public Sound PlayNewSound(string soundName,bool interrupts = false,Action<Sound> callback = null)
    {
        Debug.LogError(sounds.Count);
        return audioMono.PlayNewSound(soundName, false, interrupts, callback);
    }

    public void PlayBgmSound(string soundName,Action<Sound> callback = null)
    {
        if (bgmSound == null)
        {
            bgmSound = audioMono.PlayNewSound(soundName, true, false, callback);
        }
        else
        {
            if (bgmSound.name.Equals(soundName))
            {
                return;
            }
            else
            {
                bgmSound.Finish();
                bgmSound = audioMono.PlayNewSound(soundName, true, false, callback);
            }
        }
    }

    public void ResumeBgmSound()
    {
        if (bgmSound != null)
        {
            bgmSound.PlayOrPause(true, false);
        }
    }

    public void PauseBgmSound()
    {
        if (bgmSound != null)
        {
            bgmSound.PlayOrPause(false, false);
        }
     }



    #region audioMono
    public class AudioMono : MonoBehaviour
    {
        public HashSet<Sound> sounds =
        new HashSet<Sound>();

        /// Creates a new sound, registers it, gives it the properties specified, and starts playing it
        public Sound PlayNewSound(string soundName, bool loop = false, bool interrupts = false, Action<Sound> callback = null)
        {
            Sound sound = NewSound(soundName, loop, interrupts, callback);
            sound.playing = true;
            return sound;
        }

        /// Creates a new sound, registers it, and gives it the properties specified
        public Sound NewSound(string soundName, bool loop = false, bool interrupts = false, Action<Sound> callback = null)
        {
            Sound sound = new Sound(soundName);
            RegisterSound(sound);
            sound.loop = loop;
            sound.interrupts = interrupts;
            sound.callback = callback;
            return sound;
        }

        /// Registers a sound with the AudioManager and gives it an AudioSource if necessary
        /// You should probably avoid calling this function directly and just use 
        /// NewSound and PlayNewSound instead
        public void RegisterSound(Sound sound)
        {
            sounds.Add(sound);
            sound.audioMono = this;
            if (sound.source == null)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = sound.clip;
                sound.source = source;
            }
        }

        public void Update()
        {
            sounds.ToList().ForEach(sound => {
                sound.Update();
            });
        }
    }
    #endregion
}

