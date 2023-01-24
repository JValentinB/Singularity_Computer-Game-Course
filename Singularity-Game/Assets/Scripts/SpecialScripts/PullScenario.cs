using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullScenario : MonoBehaviour
{   
    private bool pulledOut;

    void Update(){
        if(pulledOut) PullOut();
    }

    void OnTriggerEnter(Collider col){
        var proj = col.GetComponent<Projectile>();
        if(proj){
            if(proj.mode == 0) pulledOut = true;
        }
    }

    private void PullOut(){
        transform.Translate((transform.right).normalized*Time.deltaTime*5f);
    }
}
