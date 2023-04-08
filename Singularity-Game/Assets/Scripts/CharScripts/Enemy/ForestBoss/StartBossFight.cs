using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    private bool startedFight;

    void OnTriggerEnter(Collider col){
        if(col.GetComponent<Projectile>() && col.GetComponent<Projectile>().mode == 2){
            col.GetComponent<Projectile>().closeToTreeBoss = true;
        }
        else if(col.GetComponent<Player>() && !startedFight){
            GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().startFight = true;
            startedFight = true;
        }
    }

    void OnTriggerExit(Collider col){
        if(col.GetComponent<Projectile>() && col.GetComponent<Projectile>().mode == 2){
            col.GetComponent<Projectile>().closeToTreeBoss = false;
        }
    }
}
