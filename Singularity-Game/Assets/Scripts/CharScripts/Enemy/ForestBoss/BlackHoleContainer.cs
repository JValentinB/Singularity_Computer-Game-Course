using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleContainer : MonoBehaviour
{   
    public bool active = false;
    public Projectile blackhole;

    void OnTriggerStay(Collider col){
        var projectile = col.GetComponent<Projectile>(); 
        if(projectile && projectile.mode == 2 && col.transform.position != transform.position){
            var dir = Vector3.Normalize(transform.position - col.gameObject.transform.position);
            col.transform.Translate(dir * Time.deltaTime * 20f);

            active = true;
            blackhole = projectile;
        }
    }
}
