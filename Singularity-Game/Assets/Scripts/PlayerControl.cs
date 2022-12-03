using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private static int              FIELDS          = 1;
    private static float            FIELDFACTOR     = 2.0f;
    public  static float            START_MASS      = 75.0f;
    private float                   WALK            = 0.4f;
    private float                   RUN             = 3.0f;
    private float                   SPRINT          = 4.0f;

    [SerializeField] private float  walking_speed   = 0.4f;
    [SerializeField] private float  running_speed   = 3.0f;
    [SerializeField] private float  sprinting_speed = 4.0f;
    [SerializeField] private float  jumpforce       = 14000;
    [SerializeField] private int    airjumps        = 2;

    private int                     jumpnumber;
    private float                   speed;

    private float                   direction       = -1;
    private Animator                animator;
    private Rigidbody               rigidbody;
    private GameObject              weapon;
    public  GameObject[]            gravityFields   = new GameObject[FIELDS];

    private float                   lastPosY;
    private float                   old_mass        = START_MASS;
    

    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        lastPosY = this.transform.position.y;

        rigidbody.mass = START_MASS;

        jumpnumber = airjumps;


    }
    
    // Update is called once per frame
    void Update()
    {
        walking_speed   = WALK   * START_MASS * 1 / rigidbody.mass;
        running_speed   = RUN    * START_MASS * 1 / rigidbody.mass;
        sprinting_speed = SPRINT * START_MASS * 1 / rigidbody.mass;

        // press shift to run fast
        if (Input.GetKey(KeyCode.LeftControl)) speed = walking_speed;
        else if (Input.GetKey(KeyCode.LeftShift)) speed = sprinting_speed;
        else speed = running_speed;

        // turn around
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) direction = 1;
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) direction = -1;
        this.transform.rotation = Quaternion.Euler(0, direction * 90, 0);

        // Running
        float landing = (animator.GetCurrentAnimatorStateInfo(0).IsName("Landing")) ? 0.5f : 1;
        var velocity = direction * Vector3.forward * Input.GetAxis("Horizontal") * landing * speed;
        transform.Translate(velocity * Time.deltaTime);
        animator.SetFloat("Speed", velocity.magnitude);

        jump();

        // Scroll Mouse Wheel to change Equipment
        if (Input.mouseScrollDelta.y != 0)
        {
            animator.SetInteger("Equipment", (animator.GetInteger("Equipment") + 1) % 2);
        }
        // Show equipped Weapon
        if (animator.GetInteger("Equipment") == 0) {
            weapon = GameObject.Find("Gun");
            weapon.GetComponent<MeshRenderer>().enabled = false;
        }
        if (animator.GetInteger("Equipment") == 1)
        {
            weapon = GameObject.Find("Gun");
            weapon.GetComponent<MeshRenderer>().enabled = true;
        }
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);

        GroundCheck();

        updateMass(gravityFields);
    }

    void jump()
    {
        // Jumping and how many airjumps you can do
        if (animator.GetBool("Jumping"))
            animator.SetBool("Jumping", false);
        if (Input.GetKeyDown(KeyCode.Space) && jumpnumber > 0)
        {
            animator.SetTrigger("Jumping");
            animator.SetBool("Falling", true);
            rigidbody.AddForce(Vector3.up * jumpforce);
            jumpnumber--;
        }
    }

    // checks if player is in a GravityField and changes mass, if necessary
    void updateMass(GameObject[] gravityFields)
    {
        float player_x = this.transform.position.x;
        float player_y = this.transform.position.y;

        for (int i = 0; i < FIELDS; i++)
        {
            
            if (gravityFields[i].transform.position.x - gravityFields[i].transform.localScale.x < player_x &&
                gravityFields[i].transform.position.x + gravityFields[i].transform.localScale.x > player_x &&
                gravityFields[i].transform.position.y - gravityFields[i].transform.localScale.y < player_x &&
                gravityFields[i].transform.position.y + gravityFields[i].transform.localScale.y > player_x)
            {
  
                rigidbody.mass = 75.0f * FIELDFACTOR;
                Console.WriteLine(rigidbody.mass);
                return;
            }
        }
        rigidbody.mass = old_mass;
        return;
    }

    // Checks how far away the ground is and sets the Falling bool
    void GroundCheck()
    {
        float falling_distance = 1.4f;
        Ray ray1 = new Ray(transform.position + new Vector3( 0.5f, 1, 0), new Vector3(0, -5, 0));
        Ray ray2 = new Ray(transform.position + new Vector3(-0.5f, 1, 0), new Vector3(0, -5, 0));
        RaycastHit hit1;
        RaycastHit hit2;
        if (Physics.Raycast(ray1, out hit1) && Physics.Raycast(ray1, out hit2))
        {
            //print(hit1.collider.name + " " + hit1.distance);
            if (hit1.distance > falling_distance && hit2.distance > falling_distance)
                animator.SetBool("Falling", true);
            else {
                animator.SetBool("Falling", false);
                // reset airjump number
                jumpnumber = airjumps;
            }



        }
    }
}
