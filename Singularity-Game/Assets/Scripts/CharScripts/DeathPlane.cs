using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.GetComponent<Damageable>()){
            col.GetComponent<Damageable>().ApplyDamage(99999);
        }
    }
}
