using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Collider is on the 5th bone because of retardation
//So trigger and dmg should be done by 5th bone
public class DamageByRoot : MonoBehaviour
{
    public int damage = 30;
    public float force = 50000f;

    private Vector3 direction;

    void Start(){
        direction = transform.parent.GetComponent<rootSpike>().growingDirection;
    }

    void OnTriggerEnter(Collider col){
        
        if(!col.GetComponent<TreeBoss>() && col.GetComponent<Damageable>()){

            col.GetComponent<Damageable>().ApplyDamage(damage); 
            col.GetComponent<Rigidbody>().AddForce(direction * force);
        }
    }
}
