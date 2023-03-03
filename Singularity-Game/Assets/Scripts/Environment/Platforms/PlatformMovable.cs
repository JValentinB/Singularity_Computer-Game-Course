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
    public bool isLimited = true;
    public Vector3 bottomLeft;
    public Vector3 topRight;

    public GameObject crystalPrefab; // The prefab of the crystal that will be placed

    private bool isHeld = false; // Whether the platform is currently being held
    private Rigidbody rb; // The Rigidbody component of the platform
    private Vector3 offset; // The offset between the mouse position and the platform's position when the platform is being held
    private Camera mainCamera;
    private LayerMask playerLayer = 1 << 3; // Layer mask for the "Player" layer
    private Vector3 lastPosition;

    // crystal Movement
    private bool platformActive;
    private GameObject activeCrystal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        gameObject.layer = LayerMask.NameToLayer("Movable Platform");
    }

    void Update()
    {
        if (control == Control.Mouse)
            mouseMovingPlatform();
        if (control == Control.Crystal)
            crystalMovingPlatform();
        if (isLimited)
            boundingBox();
    }

    void mouseMovingPlatform()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Check if the mouse is over the platform
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask platformLayer = 1 << LayerMask.NameToLayer("Movable Platform");
            if (Physics.Raycast(ray, out hit, 1000, platformLayer) && hit.collider.gameObject == gameObject)
            {
                // Start holding the platform
                isHeld = true;
                offset = transform.position - ray.origin;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // Stop holding the platform
            isHeld = false;
        }

        if (isHeld)
        {
            // Update the platform's position based on the mouse position
            var mousePos = Input.mousePosition;
            mousePos.z = -mainCamera.transform.position.z;
            Vector3 moveDirection = mainCamera.ScreenToWorldPoint(mousePos) - transform.position;
            moveDirection.z = 0;

            rb.AddForce(moveDirection * force);

            rb.AddForce(-rb.velocity * friction);
        }

        if (lastPosition == transform.position)
        {
            rb.velocity = Vector3.zero;
        }
        lastPosition = transform.position;
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
        if(!platformActive)
            rb.velocity = Vector3.zero;
    }

    // Puts object back into bounding box
    void boundingBox()
    {
        float x = inXaxisBounds(transform.localPosition.x) ? 1f : 0f;
        float y = inYaxisBounds(transform.localPosition.y) ? 1f : 0f;
        rb.velocity = Vector3.Scale(rb.velocity, new Vector3(x,y,1));
        
        transform.localPosition = ClosestPointInBoundingBox(transform.localPosition);
    }

    private bool inXaxisBounds(float x)
    {
        return x > bottomLeft.x && x < topRight.x;
    }
    private bool inYaxisBounds(float y){
        return y > bottomLeft.y && y < topRight.y;
    }

    Vector3 ClosestPointInBoundingBox(Vector3 point)
    {
        return new Vector3(
            Mathf.Clamp(point.x, bottomLeft.x + 0.01f, topRight.x - 0.01f),
            Mathf.Clamp(point.y, bottomLeft.y + 0.01f, topRight.y - 0.01f),
            0f //transform.localPosition.z
        );
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody other_rigidbody = other.GetComponent<Rigidbody>();
        // Check if the collider is on the "Characters" layer
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            other_rigidbody.velocity += GetComponent<Rigidbody>().velocity;
        }
    }
}
