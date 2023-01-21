using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        if (damagbleObjectToShift)
        {
            var rg = col.gameObject.GetComponent<Rigidbody>();
            damagbleObjectToShift.gravityStrength = 5f;
            damagbleObjectToShift.GetComponent<Character>().jumpForce = 3050f;
            rg.velocity = new Vector3(rg.velocity.x, -10f ,rg.velocity.z); 
        }
    }

    private void OnTriggerExit(Collider col)
    {
        var damagbleObjectToShift = col.gameObject.GetComponent<Damageable>();
        
        if (damagbleObjectToShift)
        {
            damagbleObjectToShift.gravityStrength = 18f;
            damagbleObjectToShift.GetComponent<Character>().jumpForce = 1050f;
        }

    }
}
