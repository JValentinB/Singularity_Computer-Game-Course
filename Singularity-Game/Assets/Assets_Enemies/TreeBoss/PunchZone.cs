using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchZone : MonoBehaviour
{   
    [HideInInspector] public bool playerInZone = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {   
            playerInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInZone = false;
        }
    }
}
