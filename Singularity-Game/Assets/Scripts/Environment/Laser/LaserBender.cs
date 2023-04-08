using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBender : MonoBehaviour
{
    [Header("Time until the bender is moving back to initial position")]
    public bool resetPosition = true;
    public float timeUntilReset = 10f;
    [Range(0f, 1f)]
    public float bendingAmount = 1f;
    [Range(0f, 1f)]
    public float bendingDistance = 1f; // is bending if the actual distance is smaller than bendingDistance * colliderWidth
    [Header("Destruction")]
    public GameObject destructionParticles;
    public GameObject shatteredCrystal;

    [HideInInspector] public float radius = 1f;
    [HideInInspector] public bool isMoving = false;

    private SphereCollider triggerCollider;
    private Rigidbody rb;
    private List<LaserBeam> laserBeams = new List<LaserBeam>();

    private Vector3 initialPosition;
    Coroutine timerCoroutine;
    bool timerTicking = false;

    void Start()
    {
        SphereCollider[] colliders = GetComponents<SphereCollider>();
        triggerCollider = colliders[0].isTrigger ? colliders[0] : colliders[1];
        radius = triggerCollider.radius;

        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (resetPosition && !timerTicking && IsMoving())
        {
            timerCoroutine = StartCoroutine(Timer());
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            DestroyBender();
        }
    }


    void OnTriggerEnter(Collider other)
    {
        LaserBeam beam = other.GetComponent<LaserBeam>();

        if (beam != null)
        {
            if (!laserBeams.Contains(beam))
                laserBeams.Add(beam);
            if (!beam.benders.Contains(this))
                beam.benders.Add(this);

            beam.beamInteraction = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        LaserBeam beam = other.GetComponent<LaserBeam>();

        if (beam != null)
        {
            if (beam.benders.Contains(this))
                beam.benders.Remove(this);

            beam.beamInteraction = true;
        }
    }

    void clearFromLists()
    {
        
    }

    // check if the bender is moving
    bool IsMoving()
    {
        if (rb.velocity.magnitude > 0.01f)
            return true;
        else
            return false;
    }

    IEnumerator Timer()
    {
        timerTicking = true;
        yield return new WaitForSeconds(timeUntilReset);
        // move back to initial position with high velocity
        transform.position = initialPosition;
        timerTicking = false;

        if (laserBeams.Count > 0)
        {
            foreach (LaserBeam laserBeam in laserBeams)
            {
                laserBeam.benderReset = true;
            }
        }
    }

    public void DestroyBender()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        if (destructionParticles != null){
            GameObject particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            Destroy(particles, 5f);
        }

        if (shatteredCrystal != null)
        {
            GameObject crystals = Instantiate(shatteredCrystal, transform.position, transform.rotation);
            // add a explosive force to the shattered crystals
            Rigidbody[] rigidbodies = crystals.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.AddExplosionForce(60000f, transform.position, 10f);
            }
            Destroy(crystals, 12f);
        }

        ObjectSounds objectSounds = GetComponent<ObjectSounds>();
        if (objectSounds != null)
            objectSounds.Play("Explosion");

        if (laserBeams.Count > 0)
        {
            foreach (LaserBeam laserBeam in laserBeams)
            {
                laserBeam.benderReset = true;
                laserBeam.benders.Remove(this);
            }
        }
        // Remove all components of the bender but the ObjectSounds
        Component[] components = GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component.GetType() != typeof(AudioSource) && component.GetType() != typeof(Transform))
                Destroy(component);
        }

        Destroy(gameObject, 5f);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
