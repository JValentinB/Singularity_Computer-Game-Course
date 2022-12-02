using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float walking_speed = 0.4f;
    [SerializeField] private float running_speed = 3.0f;
    [SerializeField] private float sprinting_speed = 4.0f;
    [SerializeField] private float jumpforce = 400;
    [SerializeField] private int airjumps = 2;
    private int jumpnumber;
    private float speed;

    private float direction = -1;
    private Animator animator;
    private Rigidbody rigidbody;
    private GameObject weapon;

    private float lastPosY;
    private float old_mass = 1f;
    public float[,] changedMassFields = { { 7.0f, 16.4f, -4.6f, -3.2f, 100.0f }, { 7.0f, 16.4f, 0f, 5f, 0.1f } };
    public int cMF_length = 2;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        lastPosY = this.transform.position.y;

        jumpnumber = airjumps;


    }
    
    // Update is called once per frame
    void Update()
    {
        walking_speed = 0.4f * 1 / rigidbody.mass;
        walking_speed = 0.4f * 1 / rigidbody.mass;
        walking_speed = 0.4f * 1 / rigidbody.mass;

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
        //rigidbody.AddForce(new Vector3(velocity.x,0,velocity.z) * rigidbody.mass * 100);
        animator.SetFloat("Speed", velocity.magnitude);


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

        updateMass(changedMassFields);
    }


    void updateMass(float[,] changedMassFields)
    {
        for (int i = 0; i < cMF_length; i++)
        {
            // changedMassFields[i] = {x_left, x_right, y_lower, y_upper, new_mass}
            float player_x = this.transform.position.x;
            float player_y = this.transform.position.y;
            if (changedMassFields[i, 0] < player_x && player_x < changedMassFields[i, 1]
             && changedMassFields[i, 2] < player_y && player_y < changedMassFields[i, 3])
            {
  
                rigidbody.mass = changedMassFields[i, 4];
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
