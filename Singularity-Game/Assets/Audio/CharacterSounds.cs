using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    public List<Sound> walkingSounds;
    public List<Sound> jumpingSounds;
    public List<Sound> landingSounds;
    public List<Sound> damageSounds;
    public List<Sound> otherSounds;
    
    private AudioSource audioSource;

    void Start()
    {
        // audioSource = GetComponent<AudioSource>();

        addSourceToList(walkingSounds);
        addSourceToList(jumpingSounds);
        addSourceToList(landingSounds);
        addSourceToList(damageSounds);
        addSourceToList(otherSounds);

    }

    void addSourceToList(List<Sound> soundList)
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

    public void footStep()
    {
        Sound sound = walkingSounds[Random.Range(0, walkingSounds.Count)];
        sound.source.Play();
    }

    public void jumping()
    {
        Sound sound = jumpingSounds[Random.Range(0, jumpingSounds.Count)];
        sound.source.Play();
    }

    public void landing()
    {
        Sound sound = landingSounds[Random.Range(0, landingSounds.Count)];
        sound.source.Play();
    }

    public void takingDamage()
    {
        Sound sound = damageSounds[Random.Range(0, damageSounds.Count)];
        sound.source.Play();
    }
}
