using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCliff : MonoBehaviour
{
    enum TriggerEvent
    { StandingOnCliff, PullOut }
    [SerializeField] TriggerEvent triggerEvent;
    public float fallingDelay = 3f;
    public float disableDelay = 5f;
    public float destroyingDelay = 20f;
    public Vector3 fallingForce = new Vector3(0, -50, 0);
    public float speedMultiplier = 1.0f;

    private bool isFalling = false;
    private bool playerOnCliff = false;
    private Rigidbody[] childrenRigidbodies;

    private void Start()
    {
        childrenRigidbodies = GetComponentsInChildren<Rigidbody>(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEvent == TriggerEvent.StandingOnCliff && other.CompareTag("Player") && !isFalling)
        {
            playerOnCliff = true;
            Invoke("StartFalling", fallingDelay);
            Destroy(this.gameObject, destroyingDelay);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnCliff = false;
        }
    }

    public void pullOut(){
        if(triggerEvent == TriggerEvent.PullOut && !isFalling){
            Invoke("StartFalling", fallingDelay);
            Destroy(this.gameObject, destroyingDelay);
        }
    }

    private void StartFalling()
    {
        if (playerOnCliff || triggerEvent == TriggerEvent.PullOut)
        {
            isFalling = true;

            for (int i = 0; i < childrenRigidbodies.Length; i++)
            {
                Rigidbody rb = childrenRigidbodies[i];
                rb.isKinematic = false;
                rb.AddForce(fallingForce);
                Invoke("disableCollider", disableDelay);
            }
        }
    }
     void disableCollider()
    {

        for (int i = 0; i < childrenRigidbodies.Length; i++)
        {
            Rigidbody rb = childrenRigidbodies[i];
            rb.GetComponent<MeshCollider>().enabled = false;
        }
    }
}
