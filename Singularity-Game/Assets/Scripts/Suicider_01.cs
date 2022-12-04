using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy y = -1.603938
//Player y = -0.01290798
//Diff = 1,59103002

public class Suicider_01 : MonoBehaviour
{
    [SerializeField] private float health = 100f; //Useless till it can be damaged
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float fov = 5f;

    [SerializeField] private float walking_speed = 1f;
    [SerializeField] private float explForce = 9000f;
    [SerializeField] private float explRadius = 30f;
    [SerializeField] private float explUplift = 250f;
    
    [SerializeField] private float done = 0;
    private bool dirRight = false;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private bool inRange = false;
    [SerializeField] private bool dead = false;
    [SerializeField] private bool onCooldown = true;

    //Enemy related
    private Rigidbody enemyBody;
    private CapsuleCollider triggerCollider;
    private BoxCollider collider;
    private Animator animator;

    //Player related
    private GameObject playerObject;
    private Transform player;
    private Rigidbody playerRigid;
    private PlayerControl playerScript;

    // Start is called before the first frame update
    void Start()
    {
        enemyBody = GetComponent<Rigidbody>();
        triggerCollider = GetComponent<CapsuleCollider>();
        collider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();

        playerObject = GameObject.FindWithTag("Player");
        playerRigid = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").transform;
        playerScript = playerObject.GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        var distance = player.position.x - transform.position.x;

        //movement and rotation
        if(distance < fov & distance > 0 & !inRange){
            if(!dirRight){ transform.Rotate(0, 180, 0); }
            velocity = Vector3.left * walking_speed;
            dirRight = true;
            animator.SetBool("walking", true);
        }
        else if(distance > -fov & distance < 0 & !inRange){
            if(dirRight){ transform.Rotate(0, 180, 0); }
            velocity = Vector3.left * walking_speed;
            dirRight = false;
            animator.SetBool("walking", true);
        }
        else{
            velocity = Vector3.zero;
            animator.SetBool("walking", false);
        }
        
        transform.Translate(velocity * Time.deltaTime);

        //wait function
        if(inRange & onCooldown){
            if(done >= cooldown) {
                Impact();
                done = 0;
            }
            done += Time.deltaTime;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f)*(Mathf.Max(done, 1f));
        }

        if(health <= 0){ DestroyNPC(); }
    }

    //Function will be called on entering collider range
    //(col is the other collider)
    private void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<Collider>().tag == "Player" & !dead){
            inRange = true;
        }
    }

    //Function will be called on leaving collider range
    private void OnTriggerExit(Collider col)
    {
        if(col.GetComponent<Collider>().tag == "Player" & !dead){
            inRange = false;
            onCooldown = true;
            done = 0;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    //Creates knockback on hit (later on with force as parameter)
    //Should later be placed in enemy_lib
    private void Impact()
    {   
        var enemyPos = SwitchAxisToPlayer(transform.position);
        
        playerRigid.velocity = Vector3.zero;
        playerRigid.AddExplosionForce(explForce, enemyPos, explRadius, explUplift);
        DamagePlayer(50);
        onCooldown = true;

        health = 0; //only suicider destroy themselves on hit
    }

    //Adjusting height diff between player.pos and enemy.pos
    private Vector3 SwitchAxisToPlayer(Vector3 vec){
        return new Vector3(vec.x, vec.y+1.591f, vec.z);
    }

    //Give player damage 
    //Should later be placed in enemy_lib
    private void DamagePlayer(int dmg){
        playerScript.HEALTH -= 50;
    }
    
    //Destroy selected NPC / Enemy (later on with specific gameObject as parameter)
    //Should later be placed in enemy_lib
    private void DestroyNPC(){
        gameObject.SetActive(false);
    }
}
