using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    enum Movement
    { Line, Circle }
    [SerializeField] Movement movement;
    public float speed; // Speed at which the platform should move

    [Header("Line")]
    public List<Vector3> waypoints; // List of waypoints for the platform to move between
    private int waypointIndex;

    [Header("Circle")]
    public Vector3 center;
    public float radius;
    private float angle;

    private LayerMask playerLayer = 1 << 3; // Layer mask for the "Player" layer
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = transform.GetComponent<Rigidbody>();

        if (waypoints.Count != 0)
            transform.position = waypoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (movement == Movement.Line)
            moveInLine();
        else if (movement == Movement.Circle)
            moveInCircle();
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody other_rigidbody = other.GetComponent<Rigidbody>();
        // Check if the collider is on the "Characters" layer
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            other_rigidbody.velocity += rigidbody.velocity;
        }
    }

    private void moveInLine()
    {
        // If the platform has reached the current waypoint, move to the next one
        if (Vector3.Distance(transform.position, waypoints[waypointIndex]) < 0.1f)
            waypointIndex = (waypointIndex + 1) % waypoints.Count;
        // Move the platform towards the current waypoint
        //Vector3 targetdirection = Vector3.MoveTowards(transform.position, waypoints[waypointIndex],1).normalized;

        rigidbody.velocity = (waypoints[waypointIndex] - transform.position).normalized * speed * Time.deltaTime * 50;
    }

    private void moveInCircle()
    {
        // Calculate the new angle based on the elapsed time and speed
        angle += speed * Time.deltaTime * 0.5f;

        // Calculate the new position of the platform based on the angle and radius
        Vector3 newPosition = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

        // Update the platform's position and rotation to match the new values
        //transform.position = newPosition;
        rigidbody.velocity = (newPosition - transform.position) * 5;
    }
}