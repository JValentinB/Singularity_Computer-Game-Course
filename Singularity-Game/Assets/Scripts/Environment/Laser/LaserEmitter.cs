using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    public bool isEmitting = false;
    public bool chargeable = true;
    public GameObject chargedParticles;
    public Material deadMaterial;
    [HideInInspector] public bool wasAlreadyActive = false;
    [HideInInspector] public float charge = 0f;
    [HideInInspector] public float maxCharge = 250f;

    private Quaternion initialRotation;

    private LaserBeam laserBeam;
    private AudioSource audioSource;
    private ObjectSounds objectSounds;

    float time = 0f;

    // Start is called before the first frame update
    void Start()
    {   
        initialRotation = transform.rotation;
        wasAlreadyActive = isEmitting;
        laserBeam = transform.parent.GetComponentInChildren<LaserBeam>();
        audioSource = GetComponent<AudioSource>();
        objectSounds = GetComponent<ObjectSounds>();


        if (isEmitting)
        {
            audioSource.Play();
            charge = maxCharge;
        }
        else
            audioSource.Stop();
    }

    public void startEmitting(bool playSound = true)
    {
        isEmitting = true;
        laserBeam.isActive = true;
        wasAlreadyActive = true;
        laserBeam.becameActive = true;
        charge = maxCharge;

        audioSource.Play();

        if (playSound)
        {
            objectSounds.Play("LaserCharged");
            GameObject particles = Instantiate(chargedParticles, transform.position, Quaternion.identity);
            particles.transform.parent = transform;
            particles.GetComponent<ParticleSystem>().Play();
            Destroy(particles, 2f);
        }
    }

    public void stopEmitting(bool changeMaterial = true)
    {
        isEmitting = false;
        laserBeam.isActive = false;
        laserBeam.becameInactive = true;

        audioSource.Stop();
        if (changeMaterial)
            GetComponent<MeshRenderer>().material = deadMaterial;
    }

    public bool IsEmitting()
    {
        return isEmitting;
    }

    public void rotateLaserEmitter(float rotationAngle)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle) * initialRotation;
        laserBeam.laserRotated = true;
        laserBeam.rotationAngle = rotationAngle;
    }
}
