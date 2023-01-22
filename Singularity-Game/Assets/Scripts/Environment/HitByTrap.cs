using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitByTrap : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.GetComponent<Player>()){
            col.GetComponent<Player>().ApplyDamage(50);
        }
    }
}
