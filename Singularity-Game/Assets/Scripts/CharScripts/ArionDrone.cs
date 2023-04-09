using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArionDrone : MonoBehaviour
{   
    public bool droneActive = true;
    public float speed = 1f;
    public Vector3 target;

    private Transform player;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(droneActive){
            // smoothly follow a point behind the player 
            transform.position = Vector3.SmoothDamp(transform.position, player.position - target, ref velocity, speed);
        }
    }
}
