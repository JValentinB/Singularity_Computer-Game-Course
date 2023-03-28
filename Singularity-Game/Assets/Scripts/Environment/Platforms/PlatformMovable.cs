using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovable : MonoBehaviour
{
    enum Control
    { Mouse, Crystal }
    [SerializeField] Control control;
    public float force = 10f; // Adjustable variable for move speed
    public float friction = 5f;
    [Header("Bounding Box: Be Careful to set the corners correctly!")]
    public bool boundingBoxLimit = false;
    public Vector3 bottomLeft;
    public Vector3 topRight;
    [Header("Sphere")]
    public bool sphereLimit = false;
    private Vector3 center;
    public float radius = 10f;
    [Header("")]
    public ParticleSystem activeParticles;
    public ParticleSystem pullingParticles;
    public ParticleSystem borderParticles;
    public GameObject crystalPrefab; // The prefab of the crystal that will be placed
    [Range(0f, 1f)]
    public float maxVolume = 0.5f;
    private ObjectSounds objectSounds;

    private bool isHeld = false; // Whether the platform is currently being held
    private Rigidbody rb; // The Rigidbody component of the platform
    private Vector3 offset; // The offset between the mouse position and the platform's position when the platform is being held
    private Camera mainCamera;
    private Player player;
    private Vector3 lastPosition;
    private Vector3 mousePosAtStart;

    private bool onBorder;
    private Vector3 borderDirection;

    private bool platformActive;
    private GameObject activeCrystal;
    private Coroutine pullingCoroutine;
    private Coroutine castingCoroutine;
    private Coroutine borderCoroutine;
    private bool borderTimeOut = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        center = transform.localPosition;
        objectSounds = GetComponent<ObjectSounds>();

        // float pitchRatio = force / rb.mass;
        // Debug.Log(transform.name + " pitch ratio: " + pitchRatio);
        // foreach (Sound sound in objectSounds.sounds)
        // {
        //     sound.source.pitch = sound.source.pitch * pitchRatio; // Mathf.Clamp(sound.source.pitch * pitchRatio, 0.2f, 1.2f);
        // }
    }

    void Update()
    {
        // Limit the platform's movement
        if (boundingBoxLimit)
            boundingBox();
        if (sphereLimit)
            sphere();

        if (onBorder)
        {
            if (!borderTimeOut)
            {
                StartCoroutine(borderParticleTimeOut());
                particlesForBorder();
            }
            onBorder = false;
        }

        if (lastPosition == transform.position)
        {
            rb.velocity = Vector3.zero;
        }
        lastPosition = transform.position;

        if (player.unlockedWeaponModes[0] && player.weaponMode == 0)
        {
            if (control == Control.Mouse)
                mouseMovingPlatform();
            if (control == Control.Crystal)
                crystalMovingPlatform();
        }
    }

    void mouseMovingPlatform()
    {
        if (Input.GetMouseButtonDown(1) && !isHeld)
        {
            // Check if the mouse is over the platform
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask platformLayer = 1 << LayerMask.NameToLayer("Movable Platform");
            if (Physics.Raycast(ray, out hit, 200, platformLayer) && hit.collider.gameObject == gameObject)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = -mainCamera.transform.position.z;
                mousePosAtStart = mainCamera.ScreenToWorldPoint(mousePos) - transform.position;

                // Start holding the platform
                rb.isKinematic = false;
                isHeld = true;
                offset = transform.position - ray.origin;

                if (castingCoroutine != null)
                    StopCoroutine(castingCoroutine);
                castingCoroutine = StartCoroutine(fadeInOutCastingAnimation(1f, 0.25f));

                objectSounds.Play("Activation");
                if (pullingCoroutine != null)
                    StopCoroutine(pullingCoroutine);
                pullingCoroutine = StartCoroutine(objectSounds.fadeInOut("Pulling", maxVolume, 1f));

                StartCoroutine(particlesForActivePlatform());
                StartCoroutine(particlesForPullingPlatform());
            }
        }
        else if (Input.GetMouseButtonUp(1) && isHeld)
        {
            // Stop holding the platform
            // rb.isKinematic = true;
            isHeld = false;

            if (castingCoroutine != null)
                StopCoroutine(castingCoroutine);
            castingCoroutine = StartCoroutine(fadeInOutCastingAnimation(0f, 0.5f));

            if (pullingCoroutine != null)
                StopCoroutine(pullingCoroutine);
            pullingCoroutine = StartCoroutine(objectSounds.fadeInOut("Pulling", 0f, 1f));
            return;
        }

        if (isHeld)
        {
            // Update the platform's position based on the mouse position
            var mousePos = Input.mousePosition;
            mousePos.z = -mainCamera.transform.position.z;
            Vector3 moveDirection = mainCamera.ScreenToWorldPoint(mousePos) - (mousePosAtStart + transform.position);
            moveDirection.y *= 2f;
            moveDirection.z = 0;

          
            // float factor = Mathf.Pow(moveDirection.magnitude, 1.1f);
            rb.AddForce(moveDirection * force);

            rb.AddForce(-rb.velocity * friction);
        }
    }

    void crystalMovingPlatform()
    {
        if (Input.GetMouseButtonDown(1) && platformActive)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // LayerMask platformLayer = 1 << LayerMask.NameToLayer("Movable Platform");
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Destroy(activeCrystal);
                activeCrystal = Instantiate(crystalPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Check if the mouse is over the platform
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask platformLayer = 1 << LayerMask.NameToLayer("Movable Platform");
            if (Physics.Raycast(ray, out hit, 1000, platformLayer) && hit.collider.gameObject == gameObject)
            {
                if (!platformActive)
                {
                    platformActive = true;
                }
                else if (activeCrystal != null)
                {
                    platformActive = false;
                    Destroy(activeCrystal);
                }
            }
        }

        if (platformActive && activeCrystal)
        {
            // Update the platform's position based on the crystal position
            Vector3 moveDirection = activeCrystal.transform.position - transform.position;
            moveDirection.z = 0;

            rb.AddForce(moveDirection.normalized * force * 0.3f);

            float distanceFriction = Vector3.Distance(transform.position, activeCrystal.transform.position);
            rb.AddForce(-rb.velocity * friction * (1 / distanceFriction));
        }
        if (!platformActive)
            rb.velocity = Vector3.zero;
    }

    // Puts object back into bounding box
    void boundingBox()
    {
        float x = inXaxisBounds(transform.localPosition.x) ? 1f : 0f;
        float y = inYaxisBounds(transform.localPosition.y) ? 1f : 0f;

        if (x == 0f || y == 0f)
        {
            rb.velocity = Vector3.Scale(rb.velocity, new Vector3(x, y, 1));
            x = transform.localPosition.x < bottomLeft.x ? -1f : (transform.localPosition.x > topRight.x ? 1f : 0f);
            y = transform.localPosition.y < bottomLeft.y ? -1f : (transform.localPosition.y > topRight.y ? 1f : 0f);

            transform.localPosition = ClosestPointInBoundingBox(transform.localPosition);

            onBorder = true;
            borderDirection = (new Vector3(x, y, 0)).normalized;
        }
    }

    // Puts object back into sphere
    void sphere()
    {
        float distance = Vector3.Distance(transform.localPosition, center);
        if (distance > radius)
        {
            Vector3 direction = (transform.localPosition - center).normalized;
            transform.localPosition = center + direction * radius;
            rb.velocity = Vector3.zero;

            onBorder = true;
            borderDirection = direction.normalized;
        }
    }

    private bool inXaxisBounds(float x)
    {
        return x > bottomLeft.x && x < topRight.x;
    }
    private bool inYaxisBounds(float y)
    {
        return y > bottomLeft.y && y < topRight.y;
    }

    Vector3 ClosestPointInBoundingBox(Vector3 point)
    {
        return new Vector3(
            Mathf.Clamp(point.x, bottomLeft.x + 0.01f, topRight.x - 0.01f),
            Mathf.Clamp(point.y, bottomLeft.y + 0.01f, topRight.y - 0.01f),
            transform.localPosition.z
        );
    }

    // void OnCollisionStay(Collision other)
    // {
    //     Rigidbody other_rigidbody = other.rigidbody;
    //     // Check if the collider is on the "Characters" layer
    //     if (other.gameObject.tag == "Player")
    //     {
    //         other_rigidbody.velocity += GetComponent<Rigidbody>().velocity;
    //     }
    // }


    // Animations and Particle Effects
    IEnumerator fadeInOutCastingAnimation(float target, float duration)
    {
        Animator animator = player.GetComponent<Animator>();

        float start = animator.GetLayerWeight(3);
        float counter = 0f;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            float weight = Mathf.Lerp(start, target, counter / duration);
            animator.SetLayerWeight(3, weight);
            yield return null;
        }
        animator.SetLayerWeight(3, target);
    }

    IEnumerator particlesForActivePlatform()
    {
        ParticleSystem particleObject = Instantiate(activeParticles, transform.position, Quaternion.identity);

        var shape = particleObject.shape;
        shape.meshRenderer = GetComponent<MeshRenderer>();

        while (isHeld)
        {
            // shape.position = particleObject.transform.position - transform.position;
            yield return null;
        }
        particleObject.GetComponent<ParticleSystem>().Stop();
        Destroy(particleObject, 2f);
    }

    IEnumerator particlesForPullingPlatform()
    {
        Vector3 staffPosition = player.getStaffStonePos();
        ParticleSystem particleObject = Instantiate(pullingParticles, staffPosition, Quaternion.identity);

        while (isHeld)
        {
            particleObject.transform.position = player.getStaffStonePos();
            yield return null;
        }
        particleObject.GetComponent<ParticleSystem>().Stop();
        Destroy(particleObject, 2f);
    }

    void particlesForBorder()
    {
        // Find point on mesh that is in borderDirection from transform.position
        Vector3 borderPoint = transform.position;
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
                borderPoint = collider.ClosestPoint(transform.position + borderDirection * 10f);
        }

        if(borderDirection == Vector3.zero)
            return;

        Quaternion rotation = Quaternion.LookRotation(borderDirection, Vector3.forward);
        ParticleSystem particleObject = Instantiate(borderParticles, borderPoint, rotation);

        Destroy(particleObject, 2f);
    }

    IEnumerator borderParticleTimeOut()
    {
        borderTimeOut = true;
        yield return new WaitForSeconds(0.25f);
        borderTimeOut = false;
    }
}
