using System.Collections;
using System.Collections.Generic;
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
    private new Rigidbody rigidbody;
    private GameObject gun;
    private GameObject sword;
    private float last_Attack; // Time since last Attack
    private float lastPosY;

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
        // press shift to run fast
        if (Input.GetKey(KeyCode.LeftControl)) speed = walking_speed;
        else if (Input.GetKey(KeyCode.LeftShift)) speed = sprinting_speed;
        else speed = running_speed;

        // turn around
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) direction = -1;
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) direction = 1;
        this.transform.rotation = Quaternion.Euler(0, direction * 90, 0);

        // Running
        float landing = (animator.GetCurrentAnimatorStateInfo(0).IsName("Landing")) ? 0.5f : 1;
        var velocity = direction * Vector3.forward * Input.GetAxis("Horizontal") * landing * speed;
        transform.Translate(velocity * Time.deltaTime);
        //rigidbody.AddForce(new Vector3(velocity.x,0,velocity.z) * rigidbody.mass * 100);
        animator.SetFloat("Speed", velocity.magnitude);


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
        EquipWeapon(animator.GetInteger("Equipment"));


        // Cant move on the z-Plane
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        
        if (Input.GetMouseButtonDown(1))
        {
            EnemyPull();
        }
        Attack();
        GroundCheck();
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
                jumpnumber = airjumps;
            }



        }
    }
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

    void EnemyPull()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up   , transform.TransformDirection(Vector3.forward));
        LayerMask hitLayer = LayerMask.NameToLayer("Enemy");

        Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward), Color.red, 5);

        float sphereRadius = 2;
        float maxDistance = 10;
        int layerMask = (1 << hitLayer);
        if (Physics.SphereCast(ray, sphereRadius, out hit, maxDistance, layerMask))
        {
            Debug.Log("There is an enemy!");
            Rigidbody enemy = hit.rigidbody;
            enemy.AddForce(transform.TransformDirection(Vector3.back) * 100);
        }
        else
        {
            Debug.Log("There is no enemy!");
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
