using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy y = -1.603938
//Player y = -0.01290798
//Diff = 1,59103002

public class Enemy_01 : MonoBehaviour
{
    [SerializeField] private float walking_speed = 1f;
    private float explForce = 700f;
    private float explRadius = 30f;
    private float explUplift = 5f;
    
    private Rigidbody enemybody;
    private CapsuleCollider triggerCollider;
    bool inRange = false;
    bool dead = false;
    bool time = false;


    // Start is called before the first frame update
    void Start()
    {
        enemybody = GetComponent<Rigidbody>();
        triggerCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        var velocity = Vector3.right * walking_speed;
        var player = GameObject.FindWithTag("Player").transform; //Make it global in this class!
        var distance = player.position.x - transform.position.x;

        if(distance < 10 & distance > 0 & !inRange){
            velocity = Vector3.right * walking_speed;
        }
        else if(distance > -10 & distance < 0 & !inRange){
            velocity = Vector3.left * walking_speed;
        }
        else{
            velocity = Vector3.zero;
        }
        
        transform.Translate(velocity * Time.deltaTime);
        
    }

    //Waits for "time" seconds 
    IEnumerator wait(int timer)
    {
        yield return new WaitForSeconds(timer); 
    }

    //Function will be called on entering Collider range
    //(col is the other collider)
    private void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<Collider>().tag == "Player" & !dead){
            inRange = true;
            Impact();
        }
    }

    //Function will be called on leaving Collider range
    private void OnTriggerExit(Collider col)
    {
        if(col.GetComponent<Collider>().tag == "Player" & !dead){
            inRange = true;
        }
    }

    //Creates knockback on hit
    //Create Impact function for Player, which is used by every enemy!
    //AddForce or ExplosionForce has to be fixed!
    private void Impact()
    {   
        //FIXME! Function shoudl wait 2 seconds, doesn't work rn
        wait(2);

        if(inRange){
            var playerRigid = GameObject.FindWithTag("Player").GetComponent<Rigidbody>(); //Make it global in this class
            var player = GameObject.FindWithTag("Player").transform; //Same as above
            var enemyPos = switchAxisToPlayer(transform.position);
            
            playerRigid.velocity = Vector3.zero;
            playerRigid.AddExplosionForce(explForce, enemyPos, explRadius, explUplift);
            dead = false;
        }

    }

    private Vector3 switchAxisToPlayer(Vector3 vec){
        var newVec = new Vector3(vec.x, vec.y+1.591f, vec.z);
        return newVec;
    }
}

