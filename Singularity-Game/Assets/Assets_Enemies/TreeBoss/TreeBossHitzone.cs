using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBossHitzone : MonoBehaviour
{   
    public float damageAmplifier = 1f;
    TreeBoss treeBoss;
    // Start is called before the first frame update
    void Start()
    {
        treeBoss = GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>();
    }

    public void gettingHit(int damage){
        treeBoss.ApplyDamage((int)(damage * damageAmplifier));
    }   
}
