using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class negativeVelocity : MonoBehaviour
{
    [SerializeField] private float upstreamStrength; //-25f reicht für ca. 45 auf der Y-Achse
    private float prevGravityStrength;

    void OnTriggerEnter(Collider col){
        var obj = col.GetComponent<Damageable>();
        if(obj){
            prevGravityStrength = obj.gravityStrength;
            obj.gravityStrength = upstreamStrength;
        }
    }

    void OnTriggerExit(Collider col){
        var obj = col.GetComponent<Damageable>();
        if(obj){
            obj.gravityStrength = prevGravityStrength;
        }
    }
}
