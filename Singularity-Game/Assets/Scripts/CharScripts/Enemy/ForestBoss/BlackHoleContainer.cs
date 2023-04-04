using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleContainer : MonoBehaviour
{
    void OnTriggerStay(Collider col){
        var obj = col.gameObject.GetComponent<Projectile>(); 
        if(obj && obj.mode == 2 && col.transform.position != transform.position){
            var dir = Vector3.Normalize(transform.position - col.gameObject.transform.position);
            col.transform.Translate(dir * Time.deltaTime * 20f);
        }
    }
}
