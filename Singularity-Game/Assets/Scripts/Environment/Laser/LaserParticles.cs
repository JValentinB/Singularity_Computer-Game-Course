using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class LaserParticles : MonoBehaviour
{
    public int emissionAtCollision = 100;
    public GameObject LaserSpark;
    private ParticleSystem particles;
    private GameObject collisionParticles;
    public ParticleSystem collisionParticleSystem;

    private PathCreator path;
    void Start()
    {
        particles = GetComponent<ParticleSystem>();

        path = GetComponent<PathCreator>();

        // Use the LaserSpark Prefab to create a child object that will be used to play the collision particle system
        collisionParticles = Instantiate(LaserSpark, transform.position, Quaternion.identity);
        collisionParticles.transform.parent = transform;

        collisionParticleSystem = collisionParticles.GetComponent<ParticleSystem>();
        
        var emission = collisionParticleSystem.emission;
        emission.rateOverTime = emissionAtCollision;
    }

    void Update()
    {
        Vector3 lastPoint = path.bezierPath[path.bezierPath.NumPoints - 1];
        Vector3 direction = path.path.GetDirection(1);
        // // Put the collision particle system at the end of the path
        // collisionParticles.transform.position = lastPoint;

        // // Rotate the collision particle system to face the direction of the path
        // collisionParticles.transform.rotation = Quaternion.LookRotation(direction);

        //Put the shape of the collision particle system at the end of the path
        var shape = collisionParticleSystem.shape;
        shape.position = lastPoint;
        shape.rotation = direction;
    }

}
