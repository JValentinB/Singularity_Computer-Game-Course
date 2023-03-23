using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Collider is on the 5th bone because of retardation
//So trigger and dmg should be done by 5th bone
public class DamageByRoot : MonoBehaviour
{
    private int dmg = 30;

    void OnTriggerEnter(Collider col){
        if(!col.gameObject.GetComponent<Damageable>()){
            return; 
        }
        col.gameObject.GetComponent<Damageable>().ApplyDamage(dmg);
    }
}
