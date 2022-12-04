using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //declare and initialize constants
    [SerializeField] private static int     FIELDS          = 1;
    [SerializeField] private static float   FIELDFACTOR     = 2.0f;
    [SerializeField] private static float   START_MASS      = 75.0f;
    [SerializeField] private float          WALK            = 0.4f;
    [SerializeField] private float          RUN             = 3.0f;
    [SerializeField] private float          SPRINT          = 4.0f;
    [SerializeField] private int            JUMPFACTOR      = 200;
    


    //declare variables
    private float           walking_speed;
    private float           running_speed;
    private float           sprinting_speed;
    private float           jumpforce;
    private float           speed;
    private float           direction;
    private float           lastPosY;
    private int             jumpnumber;
    private bool            jumpboots;
    private Animator        animator;
    private Rigidbody       rigidbody;
    private GameObject gun;
    private GameObject sword;
    private float last_Attack; // Time since last Attack

    [SerializeField] private int             airjumps;
    [SerializeField] private GameObject[]    gravityFields = new GameObject[FIELDS];




    //=========================================================================================================================================

    //=========================================================================================================================================

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        lastPosY = this.transform.position.y;

        airjumps = 0;
        rigidbody.mass = START_MASS;
        jumpforce = START_MASS * JUMPFACTOR;
        jumpnumber = jumpnum();
        direction = -1;
        jumpboots = false;

    }

    //=========================================================================================================================================

    //=========================================================================================================================================

    // Update is called once per frame
    void Update()
    {
        speedUpdate();

        walk_run_sprint();

        directionChange();

        foo();

        jump();

        changeEquipment();

        GroundCheck();
        
        updateMass(gravityFields);
    }

    //=========================================================================================================================================
                                                                                                                                         
    //=========================================================================================================================================

    void speedUpdate()
    {
        float ratio_mass_speed = START_MASS / rigidbody.mass;
        walking_speed = WALK * ratio_mass_speed;
        running_speed = RUN * ratio_mass_speed;
        sprinting_speed = SPRINT * ratio_mass_speed;
    }

    void walk_run_sprint()
    {
        // press shift to run fast, or strg/cmd to walk
        if (Input.GetKey(KeyCode.LeftControl)) speed = walking_speed;
        else if (Input.GetKey(KeyCode.LeftShift)) speed = sprinting_speed;
        else speed = running_speed;
    }

    void directionChange()
    {
        // turn around
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) direction = -1;
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) direction = 1;
        this.transform.rotation = Quaternion.Euler(0, direction * 90, 0);
    }

    void foo()
    {
        // Running
        float landing = (animator.GetCurrentAnimatorStateInfo(0).IsName("Landing")) ? 0.5f : 1;
        var velocity = direction * Vector3.forward * Input.GetAxis("Horizontal") * landing * speed;
        transform.Translate(velocity * Time.deltaTime);
        animator.SetFloat("Speed", velocity.magnitude);
    }

    void jump()
    {
        // Jumping and how many airjumps you can do
        if (animator.GetBool("Jumping") && animator.GetBool("Falling")) 
            animator.SetBool("Jumping", false);
        if (Input.GetKeyDown(KeyCode.Space) && jumpnumber > 0)
        {
            animator.SetTrigger("Jumping");
            animator.SetBool("Falling", true);
            rigidbody.AddForce(Vector3.up * jumpforce);
            jumpnumber--;
        }
    }

    void changeEquipment()
    {
        // Scroll Mouse Wheel to change Equipment
        animator.SetInteger("Current Equip", animator.GetInteger("Equipment"));

        if (Input.mouseScrollDelta.y > 0)
        {
            animator.SetInteger("Equipment", (animator.GetInteger("Equipment") + 1) % 3);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            animator.SetInteger("Equipment", (3 + (animator.GetInteger("Equipment") - 1)) % 3);
        }
        // Show equipped Weapon
        if (animator.GetInteger("Equipment") == 0)
        {
            weapon = GameObject.Find("Gun");
            weapon.GetComponent<MeshRenderer>().enabled = false;
        }
        if (animator.GetInteger("Equipment") == 1)
        {
            weapon = GameObject.Find("Gun");
            weapon.GetComponent<MeshRenderer>().enabled = true;
        }
        EquipWeapon(animator.GetInteger("Equipment"));
    }

    // checks if player is in a GravityField and changes mass, if necessary
    void updateMass(GameObject[] gravityFields)
    {
        float player_x = this.transform.position.x;
        float player_y = this.transform.position.y;

        for (int i = 0; i < FIELDS; i++)
        {
            
            if (gravityFields[i].transform.position.x - gravityFields[i].transform.localScale.x * 0.5f < player_x &&
                gravityFields[i].transform.position.x + gravityFields[i].transform.localScale.x * 0.5f > player_x &&
                gravityFields[i].transform.position.y - gravityFields[i].transform.localScale.y * 0.5f < player_y &&
                gravityFields[i].transform.position.y + gravityFields[i].transform.localScale.y * 0.5f > player_y)
            {
  
                rigidbody.mass = 75.0f * FIELDFACTOR;
                Console.WriteLine(rigidbody.mass);
                return;
            }
        }
        rigidbody.mass = START_MASS;
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
        if (Physics.Raycast(ray1, out hit1) && Physics.Raycast(ray2, out hit2))
        {
            //print(hit1.collider.name + " " + hit1.distance);
            if (hit1.distance > falling_distance && hit2.distance > falling_distance)
                animator.SetBool("Falling", true);
            else {
                animator.SetBool("Falling", false);
                // reset airjump number
                jumpnumber = jumpnum();
            }



        }
    }

    int jumpnum()
    {
        return airjumpnum() + 1;
    }

    int airjumpnum()
    {
        if (jumpboots)
        {
            return 1;
        }
        else return 0;

    void Attack()
    {

        if (Input.GetMouseButtonDown(0))
        {
            last_Attack = Time.time;
            animator.SetLayerWeight(1, 1);
            animator.SetInteger("Attack", (animator.GetInteger("Attack") + 1) % 3);
        }
        else if (Time.time - last_Attack > 1)
        {
            animator.SetLayerWeight(1, 0);
            animator.SetInteger("Attack", 0);
        }
    }

    
    void EquipWeapon(int weapon)
    {
        gun = GameObject.Find("Gun");
        sword = GameObject.Find("Sword");
        gun.GetComponent<MeshRenderer>().enabled = (weapon == 1);
        sword.GetComponent<MeshRenderer>().enabled = (weapon == 2);
        sword.GetComponent<BoxCollider>().enabled = (weapon == 2);
    }
}
