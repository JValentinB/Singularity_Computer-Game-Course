using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class LaserParticles : MonoBehaviour
{
    public Sound laserSound;
    public int emissionAtCollision = 100;

    private GameObject laserSpark;
    private GameObject collisionParticles;
    private ParticleSystem collisionParticleSystem;

    private PathCreator path;
    private LaserBeam laserBeam;
    private GameObject soundObject;

    // private bool becameActive = false;

    void Start()
    {
        laserBeam = GetComponent<LaserBeam>();
        laserSpark = transform.Find("LaserSpark").gameObject;
        path = GetComponent<PathCreator>();

        // Use the LaserSpark Prefab to create a child object that will be used to play the collision particle system
        collisionParticles = Instantiate(laserSpark, transform.position, Quaternion.identity);
        collisionParticles.transform.parent = transform;
        collisionParticleSystem = collisionParticles.GetComponent<ParticleSystem>();

        var emission = collisionParticleSystem.emission;
        emission.rateOverTime = emissionAtCollision;

        soundObject = new GameObject("LaserSound");
        soundObject.transform.parent = transform;
        addSound(soundObject, laserSound);
    }

    void Update()
    {
        if (laserBeam.isActive)
        {
            Vector3 lastPoint = path.bezierPath[path.bezierPath.NumPoints - 1];
            Vector3 direction = path.path.GetDirection(1);

            //Put the shape of the collision particle system at the end of the path
            var shape = collisionParticleSystem.shape;
            shape.position = lastPoint;
            shape.rotation = direction;

            soundObject.transform.position = lastPoint + transform.position;

            if (!soundObject.GetComponent<AudioSource>().isPlaying)
                soundObject.GetComponent<AudioSource>().Play();
            if (!laserSpark.GetComponent<ParticleSystem>().isPlaying)
                laserSpark.GetComponent<ParticleSystem>().Play();
            if (!collisionParticleSystem.isPlaying)
                collisionParticleSystem.Play();
        }
        else
        {
            if (soundObject.GetComponent<AudioSource>().isPlaying)
                soundObject.GetComponent<AudioSource>().Stop();
            if (laserSpark.GetComponent<ParticleSystem>().isPlaying)
                laserSpark.GetComponent<ParticleSystem>().Stop();
            if (collisionParticleSystem.isPlaying)
                collisionParticleSystem.Stop();
        }
    }

    void addSound(GameObject gameObject, Sound sound)
    {
        sound.source = gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.loop;
        sound.source.spatialBlend = sound.spatialBlend;

        sound.source.Play();
    }

}
