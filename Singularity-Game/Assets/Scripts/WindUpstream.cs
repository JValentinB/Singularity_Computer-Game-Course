using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindUpstream : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Function will be called on leaving collider range
    private void OnTriggerEnter(Collider col)
    {
        var damagbleObjectToShift = col.gameObject.GetComponent<Damageable>();
        var player = col.gameObject.GetComponent<Damageable>().GetComponent<Character>();

        if (damagbleObjectToShift)
        {
            damagbleObjectToShift.gravityStrength = 0.1f;
            player.jumpForce = 3050f;

        }
    }

    private void OnTriggerExit(Collider col)
    {
        var damagbleObjectToShift = col.gameObject.GetComponent<Damageable>();
        var player = col.gameObject.GetComponent<Damageable>().GetComponent<Character>();
        if (damagbleObjectToShift)
        {
            damagbleObjectToShift.gravityStrength = 18f;
            player.jumpForce = 1050f;
        }
    }
}
