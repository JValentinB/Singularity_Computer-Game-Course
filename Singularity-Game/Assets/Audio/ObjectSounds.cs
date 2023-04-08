using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ObjectSounds : MonoBehaviour
{
    public Sound[] sounds;
    
    private AudioSource audioSource;

    void Start()
    {
        addSourceToList(sounds);
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
            sound.source.spatialBlend = sound.spatialBlend;
        }
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, Sound => Sound.soundName == name);
        if(sound.source != null)
            sound.source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = Array.Find(sounds, Sound => Sound.soundName == name);
        if(sound.source != null)
            sound.source.Stop();
    }

    public IEnumerator fadeInOut(string name, float targetVolume, float fadeTime){
        if(!isPlayed(name))
            Play(name);

        float startVolume = getSourceVolume(name);
        float time = 0f;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float volume = Mathf.Lerp(startVolume, targetVolume, time / fadeTime);
            setSourceVolume(name, volume);
            yield return null;
        }
        if(targetVolume == 0f)
            Stop(name);
    }

    public IEnumerator risePitch(string name, float targetPitch, float riseTime){
        float startPitch = getSourcePitch(name);
        float time = 0f;
        while (time < riseTime)
        {
            time += Time.deltaTime;
            float pitch = Mathf.Lerp(startPitch, targetPitch, time / riseTime);
            setSourcePitch(name, pitch);
            yield return null;
        }
    }

    public bool isPlayed(string name)
    {
        Sound sound = Array.Find(sounds, Sound => Sound.soundName == name);
        if(sound.source == null) return false;

        return sound.source.isPlaying;
    }

    public float getSourceVolume(string name)
    {
        Sound sound = Array.Find(sounds, Sound => Sound.soundName == name);
        if(sound.source == null) return 0f;
        return sound.source.volume;
    }

    public void setSourceVolume(string name, float volume)
    {
        Sound sound = Array.Find(sounds, Sound => Sound.soundName == name);
        if(sound.source != null)
            sound.source.volume = volume;
    }

    public float getSourcePitch(string name)
    {
        Sound sound = Array.Find(sounds, Sound => Sound.soundName == name);
        if(sound.source == null) return 0f;
        return sound.source.pitch;
    }

    public void setSourcePitch(string name, float pitch)
    {
        Sound sound = Array.Find(sounds, Sound => Sound.soundName == name);
        if(sound.source != null)
            sound.source.pitch = pitch;
    }

    public void playAtRandomTimePoint(string name, float min, float max)
    {
        Sound sound = Array.Find(sounds, Sound => Sound.soundName == name);
        if(sound.source != null){
            float timePoint = sound.source.clip.length * UnityEngine.Random.Range(min, max);
            sound.source.time = timePoint;

            sound.source.Play();
        }
    }
}
