using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.GetComponent<Player>()){
            GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().startFight = true;
            Destroy(this);
        }
    }
}
