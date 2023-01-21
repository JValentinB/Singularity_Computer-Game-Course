using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    enum Movement
    { Line, Circle }
    [SerializeField] Movement movement;
    public float speed = 1; // Speed at which the platform should move

    [Header("Line")]
    public List<Vector3> waypoints; // List of waypoints for the platform to move between
    public List<float> waypointTime;
    private int waypointIndex;

    [Header("Circle")]
    public Vector3 center;
    public float radius;
    public bool direction;
    public float directionChange = float.PositiveInfinity;
    private float angle;

    private LayerMask playerLayer = 1 << 3; // Layer mask for the "Player" layer
    private new Rigidbody rigidbody;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = transform.GetComponent<Rigidbody>();
        timer = 0f;

        if (waypoints.Count != 0)
            transform.localPosition = waypoints[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movement == Movement.Line)
            moveInLine();
        else if (movement == Movement.Circle)
            moveInCircle();

        timer += Time.deltaTime;
        if(direction && timer > directionChange)
        {
            direction = !direction;
            timer = 0f;
        }
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
        if (Vector3.Distance(transform.localPosition, waypoints[waypointIndex]) < 0.1f){
            if(waypointTime.Count >= waypointIndex){
                StartCoroutine(Wait(waypointTime[waypointIndex], waypointIndex));
            }
            waypointIndex = (waypointIndex + 1) % waypoints.Count;
        }
        // Move the platform towards the current waypoint
        //Vector3 targetdirection = Vector3.MoveTowards(transform.position, waypoints[waypointIndex],1).normalized;

        rigidbody.velocity = (waypoints[waypointIndex] - transform.localPosition).normalized * speed * Time.fixedDeltaTime * 50;
    }

    private void moveInCircle()
    {
        // Calculate the new angle based on the elapsed time and speed
        int clockwise = direction ? 1 : -1;
        angle += clockwise * speed * Time.fixedDeltaTime * 0.5f;

        // Calculate the new position of the platform based on the angle and radius
        Vector3 newPosition = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

        // Update the platform's position and rotation to match the new values
        //transform.position = newPosition;
        rigidbody.velocity = (newPosition - transform.localPosition) * 5;
    }

    IEnumerator Wait(float waitTime, int index)
    {
        timer = 0;
        while(timer < waitTime){
            timer += Time.fixedDeltaTime;
            transform.localPosition = waypoints[index];
            yield return null;
        }
        // yield return new WaitForSeconds(waitTime);
    }
}