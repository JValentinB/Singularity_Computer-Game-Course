using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleAcclerator : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 0.5f;
    public float launchForceMultiplier = 1.5f;

    private Rigidbody playerRigidbody;
    private bool isInsideSphere = false;
    private Vector3 initialPlayerPosition;
    private float currentSpeed = 0f;
    private float startDistance;
    private float startAngle = 0f;
    private float currentAngle = 0f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInsideSphere = true;
            initialPlayerPosition = other.transform.position;
            currentSpeed = 0f;
            currentAngle = 0f;
            startDistance = Vector3.Distance(transform.position, playerRigidbody.transform.position);
            startAngle = Mathf.Atan2(other.transform.position.y - transform.position.y, other.transform.position.x - transform.position.x);
            startAngle = Mathf.Repeat(startAngle, Mathf.PI * 2f);

            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.GetComponent<Player>().killOnHighFallingSpeed = false;
            playerRigidbody.GetComponent<Player>().gravityStrength = 0;
        }  
        // Debug.Log("0,1: " + Mathf.Repeat(Mathf.Atan2(0,1), Mathf.PI * 2f) + " 1,0: " + Mathf.Repeat(Mathf.Atan2(1, 0), Mathf.PI * 2f) + " 0,-1: " + Mathf.Repeat(Mathf.Atan2(0,-1), Mathf.PI * 2f) + " -1,0: " + Mathf.Repeat(Mathf.Atan2(-1, 0), Mathf.PI * 2f));
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInsideSphere = false;
            currentSpeed = 0f;
            currentAngle = 0f;

            playerRigidbody.GetComponent<Player>().killOnHighFallingSpeed = true;
            playerRigidbody.GetComponent<Player>().gravityStrength = 27;
        }
    }

    void FixedUpdate()
    {
        if (isInsideSphere)
        {
            float distance = Mathf.Lerp(startDistance, 0f, currentSpeed / maxSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

            currentAngle = Mathf.Repeat(currentAngle + (currentSpeed * Time.deltaTime), Mathf.PI * 2f);
            float angle = startAngle + currentAngle;

            Debug.Log("StartAngle: " + startAngle + " Angle: " + angle);
            float x = Mathf.Sin(angle) * distance;
            float y = Mathf.Cos(angle) * distance;
            Vector3 targetPosition = new Vector3(x, y, 0);

            // velocity = (transform.position + targetPosition) - playerRigidbody.transform.position;
            playerRigidbody.MovePosition(transform.position + targetPosition);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 launchDirection = (transform.position + targetPosition) - playerRigidbody.transform.position;
                float launchForce = currentSpeed * launchForceMultiplier;
                playerRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            }
        }
        // if (isInsideSphere)
        // {
        //     Vector3 directionToCenter = (transform.position - playerRigidbody.transform.position).normalized;

        //     // Calculate the direction of motion using the cross product of directionToCenter and upDirection
        //     Vector3 motionDirection = Vector3.Cross(directionToCenter, Vector3.forward).normalized;

        //     Debug.DrawRay(playerRigidbody.transform.position, motionDirection, Color.red);
        //     // Calculate the current distance from the center of the sphere
        //     float distance = Vector3.Distance(transform.position, playerRigidbody.transform.position);

        //     // Update the current speed
        //     currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

        //     // Update the position of the object in a circular path around the center of the sphere
        //     Vector3 velocity = (motionDirection) * currentSpeed;
        //     playerRigidbody.MovePosition(playerRigidbody.position + velocity);

        //     // Add a force to launch the object out of the sphere
        // }
    }

}

