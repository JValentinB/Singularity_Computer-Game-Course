using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsTreeBoss : MonoBehaviour
{
    public List<Sound> groaningSounds;
    public List<Sound> punchgroaningSounds;
    public List<Sound> attackSounds;
    public List<Sound> damageSounds;

    private AudioSource audioSource;

    void Start()
    {
        // audioSource = GetComponent<AudioSource>();

        addSourceToList(groaningSounds);
        addSourceToList(punchgroaningSounds);
        addSourceToList(attackSounds);
        addSourceToList(damageSounds);

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

    public void groaningSoundRandom()
    {
        Sound sound = groaningSounds[Random.Range(0, groaningSounds.Count)];
        sound.source.Play();
    }

    public void groaningSound(string name)
    {
        Sound sound = groaningSounds.Find(sound => sound.soundName == name);
        sound.source.Play();
    }

    public void punchgroaningSound(string name)
    {
        Sound sound = punchgroaningSounds.Find(sound => sound.soundName == name);
        sound.source.Play();
    }

    public void attackSound(string name)
    {
        Sound sound = attackSounds.Find(sound => sound.soundName == name);
        sound.source.Play();
    }

    public void takingDamage()
    {
        Sound sound = damageSounds[Random.Range(0, damageSounds.Count)];
        sound.source.Play();
    }
}
