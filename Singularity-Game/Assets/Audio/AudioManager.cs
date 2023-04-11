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

    public void PauseCategory(Sound[] soundCategory, float time)
    {   
        Sound playingSound = new Sound();
        foreach (Sound sound in soundCategory)
        {   
            if(sound.source.isPlaying){
                sound.source.Stop();
                playingSound = sound;
                break;
            }
        }
        StartCoroutine(PauseForTime(music, playingSound.soundName, time, 0.1f));
    }

    public IEnumerator PauseForTime(Sound[] soundCategory, string name, float time, float fadeTime)
    {
        Sound sound = Array.Find(soundCategory, Sound => Sound.soundName == name);
        
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            sound.source.volume = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }
        sound.source.Pause();
        yield return new WaitForSeconds(time);
        sound.source.UnPause();
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            sound.source.volume = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }
    }

    // Decrease volume over time and stop playing when volume is 0
    public IEnumerator fadeOut(Sound[] soundCategory, string name, float fadeTime){
        float startVolume = getSourceVolume(soundCategory, name);
        float time = 0f;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float volume = startVolume - startVolume * (time / fadeTime);
            setSourceVolume(soundCategory, name, volume);
            yield return null;
        }
        Stop(soundCategory, name);
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
        Sound sound = Array.Find(soundCategory, Sound => Sound.soundName == name);
        return sound.source.volume;
    }
}
