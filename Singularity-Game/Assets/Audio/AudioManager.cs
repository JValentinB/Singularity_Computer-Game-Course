using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] environmentSounds;
    [Header("")]
    [Range(0f, 1f)]
    public float MusicVolume = 1f;
    public Sound[] music;  

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        addSourceToList(environmentSounds);
        addSourceToList(music);
    }

    void addSourceToList(Sound[] soundList)
    {
        foreach (Sound sound in soundList)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void Play(Sound[] soundCategory, string name)
    {
        Sound sound = Array.Find(soundCategory, Sound => Sound.soundName == name);
        sound.source.Play();
    }

    public void Stop(Sound[] soundCategory, string name)
    {
        Sound sound = Array.Find(soundCategory, Sound => Sound.soundName == name);
        sound.source.Stop();
    }

    public IEnumerator LoopSoundWithGap(Sound[] soundCategory, string nameSound1, float loopGap, string nameSound2 = "")
    {
        bool switchSound = false;
        while (true)
        {
            if (switchSound || nameSound2 == "")
            {
                Play(soundCategory, nameSound1);
                switchSound = false;
            }
            else if (nameSound2 != "")
            {
                Play(soundCategory, nameSound2);
                switchSound = true;
            }
            yield return new WaitForSeconds(loopGap);
        }
    }

    public bool isPlayed(Sound[] soundCategory, string name)
    {
        Sound sound = Array.Find(soundCategory, Sound => Sound.soundName == name);
        return sound.source.isPlaying;
    }

    public void setSourceVolume(Sound[] soundCategory, string name, float volume)
    {
        Sound sound = Array.Find(soundCategory, Sound => Sound.soundName == name);
        float volumeMultiplier = soundCategory == music ? MusicVolume : 1f;
        sound.source.volume = volume * volumeMultiplier;
    }

    public float getSourceVolume(Sound[] soundCategory, string name)
    {
        Debug.Log(soundCategory);
        Sound sound = Array.Find(soundCategory, Sound => Sound.soundName == name);
        return sound.source.volume;
    }
}
