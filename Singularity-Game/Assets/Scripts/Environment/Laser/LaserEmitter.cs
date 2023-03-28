using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{   
    public bool isEmitting = false;
    private LaserBeam laserBeam;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        laserBeam = transform.parent.GetComponentInChildren<LaserBeam>();
        audioSource = GetComponent<AudioSource>();

        if(isEmitting)
            audioSource.Play();
        else    
            audioSource.Stop();
    }

    public void startEmitting()
    {
        isEmitting = true;
        laserBeam.isActive = true;
        laserBeam.becameActive = true;

        audioSource.Play();
    }

    public void stopEmitting()
    {
        isEmitting = false;
        laserBeam.isActive = false;
        laserBeam.becameActive = false;

        audioSource.Stop();
    }

    public bool IsEmitting()
    {
        return isEmitting;
    }
}
