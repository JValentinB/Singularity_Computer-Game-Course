using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
   private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Player")
        {

            collision.GetComponent<Player>().setCheckPoint(transform.position); 
        }
    }
}
