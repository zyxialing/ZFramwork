using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Sound
{
    public AudioManager.AudioMono audioMono;
    public string name;
    public AudioClip clip;
    public AudioSource source;
    public Action<Sound> callback;
    public bool loop;
    public bool interrupts;
    public bool isBgm;

    private HashSet<Sound> interruptedSounds =
        new HashSet<Sound> ();

    /// returns a float from 0.0 to 1.0 representing how much
    /// of the sound has been played so far
    public float progress {
        get {
            if (source == null || clip == null)
                return 0f;
            return (float)source.timeSamples / (float)clip.samples;
        }
    }

    /// returns true if the sound has finished playing
    /// will always be false for looping sounds
    public bool finished {
        get {
            return !loop && progress >= 1f;
        }
    }

    /// returns true if the sound is currently playing,
    /// false if it is paused or finished
    /// can be set to true or false to play/pause the sound
    /// will register the sound before playing
    public bool playing {
        get {
            return source != null && source.isPlaying;
        } 
        set {
            if (value) {
                audioMono.RegisterSound(this);
            }
            PlayOrPause(value, interrupts);
        }
    }

    /// Try to avoid calling this directly
    /// Use AudioManager.NewSound instead
    public Sound(string newName) {
        name = newName;
        clip = AudioUtils.GetAudio(name);
        ZLogUtil.Log(clip);
        if (clip == null)
            throw new Exception("Couldn't find AudioClip with name '"+name+"'. Are you sure the file is in a folder named 'Resources'?");
    }

    public void Update() {
        if (source != null)
            source.loop = loop;
        if (finished)
            Finish();
    }

    /// Try to avoid calling this directly
    /// Use the Sound.playing property instead
    public void PlayOrPause(bool play, bool pauseOthers) {
        if (pauseOthers) {
            if (play) {
                interruptedSounds = new HashSet<Sound>(audioMono.sounds.Where(snd => snd.playing &&
                                                                                        snd != this&&
                                                                                        !snd.isBgm));
            }
            interruptedSounds.ToList().ForEach(sound => sound.PlayOrPause(!play, false));
        }
        if (play && !source.isPlaying) {
            source.Play();
        } else {
            source.Pause();
        }
    }

    /// performs necessary actions when a sound finishes
    public void Finish() {
        PlayOrPause(false, true);
        callback?.Invoke(this);
        MonoBehaviour.Destroy(source);
        source = null;
        audioMono.sounds.Remove(this);
        AudioUtils.Release(name);
    }

    /// Reset the sound to its beginning
    public void Reset() {
        source.time = 0f;
    }
}
