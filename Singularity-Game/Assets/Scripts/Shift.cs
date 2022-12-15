using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shift : MonoBehaviour
{
    [SerializeField] private Vector3 direction;    

    private PlayerControl playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Function will be called on leaving collider range
    private void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<Collider>().tag == "Player"){
            playerScript.shiftGravity(direction);
            //playerScript.gravityStrength = 5f;
        }
    }
}
