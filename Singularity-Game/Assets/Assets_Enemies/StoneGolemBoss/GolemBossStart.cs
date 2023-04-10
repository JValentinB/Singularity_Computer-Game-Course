using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBossStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Boss fight started!");
            if(transform.parent.GetComponentInChildren<StoneGolemBoss>() == null) return;
            
            transform.parent.GetComponentInChildren<StoneGolemBoss>().bossFightStarted = true;
        }
    }
}
