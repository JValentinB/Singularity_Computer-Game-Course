using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{   
    public bool isEmitting = false;
    public Material deadMaterial;
    [HideInInspector] public bool wasAlreadyActive = false;

    private LaserBeam laserBeam;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        wasAlreadyActive = isEmitting;
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
        wasAlreadyActive = true;
        laserBeam.becameActive = true;

        audioSource.Play();
    }

    public void stopEmitting()
    {
        isEmitting = false;
        laserBeam.isActive = false;
        laserBeam.becameActive = false;

        audioSource.Stop();
        GetComponent<MeshRenderer>().material = deadMaterial;
    }

    public bool IsEmitting()
    {
        return isEmitting;
    }
}
