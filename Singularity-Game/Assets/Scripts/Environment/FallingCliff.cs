using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCliff : MonoBehaviour
{
    public float fallingDelay = 3f;
    public float destroyingDelay = 20f;
    public Vector3 fallingForce = new Vector3(0, -50, 0);
    public float speedMultiplier = 1.0f;
    private Rigidbody rb;
    private bool isFalling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFalling)
        {
            isFalling = true;
            Invoke("StartFalling", fallingDelay);
            Destroy(this.gameObject, destroyingDelay);
        }
        if (!rb.isKinematic)
        {
            rb.velocity *= speedMultiplier;
        }
    }

    private void StartFalling()
    {
        rb.isKinematic = false;
        rb.AddForce(fallingForce);
    }
}
