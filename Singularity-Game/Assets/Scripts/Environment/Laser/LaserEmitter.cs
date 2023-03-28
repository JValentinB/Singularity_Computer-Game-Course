using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{   
    public bool isEmitting = false;
    public GameObject chargedParticles;
    public Material deadMaterial;
    [HideInInspector] public bool wasAlreadyActive = false;
    [HideInInspector] public float charge = 0f;
    private float maxCharge = 250f;

    private LaserBeam laserBeam;
    private AudioSource audioSource;
    private ObjectSounds objectSounds;

    // Start is called before the first frame update
    void Start()
    {
        wasAlreadyActive = isEmitting;
        laserBeam = transform.parent.GetComponentInChildren<LaserBeam>();
        audioSource = GetComponent<AudioSource>();
        objectSounds = GetComponent<ObjectSounds>();
        

        if(isEmitting){
            audioSource.Play();
            charge = maxCharge;
        }
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
        objectSounds.Play("LaserCharged");

        GameObject particles = Instantiate(chargedParticles, transform.position, Quaternion.identity);
        particles.transform.parent = transform;
        particles.GetComponent<ParticleSystem>().Play();
        Destroy(particles, 2f);
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
