using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDeathSound : MonoBehaviour
{
    private AudioSource m_AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
