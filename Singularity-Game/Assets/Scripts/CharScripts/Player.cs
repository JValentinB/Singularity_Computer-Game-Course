using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private bulletMode weaponMode;
    [SerializeField] GameObject projectile;

    void Start(){
        maxHealth = 100;
        currentHealth = maxHealth;
        jumpNumber = 2;
        walkSpeed = 0.4f;
        runSpeed = 4.0f;
        sprintSpeed = 8.0f;
        mass = 75.0f;
        gravitationalDirection = Vector3.down;
        direction = 1;
        jumpForce = 1250f;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = mass;
        weaponMode = bulletMode.Blue;
    }

    void FixedUpdate(){
        SpeedToggle();
        ChangeLineOfSight();
        Turn();
        MovePlayer();
        GroundCheck();
        RotateGravity();
        ApplyGravity();

        //changeEquipment();
        Attack();
    }

    void Update(){
        Jump();
    }

    private void MovePlayer(){
        float landing = (animator.GetCurrentAnimatorStateInfo(0).IsName("Landing")) ? 0.5f : 1;
        var velocity = direction * Vector3.forward * Input.GetAxis("Horizontal") * landing * currentSpeed;
        transform.Translate(velocity * Time.deltaTime);
        animator.SetFloat("Speed", velocity.magnitude);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void SpeedToggle(){
        if (Input.GetKey(KeyCode.LeftControl)) currentSpeed = walkSpeed;
        else if (Input.GetKey(KeyCode.LeftShift)) currentSpeed = sprintSpeed;
        else currentSpeed = runSpeed;
    }

    private void Turn()
    {
        // turn around
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) direction = -1;
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) direction = 1;
    }

    public void giveXp(int xp){
        GetComponent<XpManager>().GainXp(xp);
    }

    private void GroundCheck()
    {
        float falling_distance = 1.4f;
        Ray ray1 = new Ray(transform.position + new Vector3( 0.5f, 1, 0), new Vector3(0, -5, 0));
        Ray ray2 = new Ray(transform.position + new Vector3(-0.5f, 1, 0), new Vector3(0, -5, 0));
        RaycastHit hit1;
        RaycastHit hit2;

        LayerMask hitLayer = LayerMask.NameToLayer("Ground");
        int layerMask = (1 << hitLayer);
        if (Physics.Raycast(ray1, out hit1, layerMask) && Physics.Raycast(ray2, out hit2, layerMask))
        {
            //print(hit1.collider.name + " " + hit1.distance);
            if (hit1.distance > falling_distance && hit2.distance > falling_distance && rigidbody.velocity.y < -0.2f)
                animator.SetBool("Falling", true);
            else {
                animator.SetBool("Falling", false);
                // reset airjump number
                jumpNumber = 2;
            }
        }
    }

    public void OnDeath(){
        //...
    }

    //FIXME Muss noch neu gemacht werden:
    //---------------------------------------
    private float last_Attack;
    private GameObject gun, sword;

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            last_Attack = Time.time;
            animator.SetLayerWeight(1, 1);
            animator.SetInteger("Attack", (animator.GetInteger("Attack") + 1) % 4);
            
            //Fire Projectile
            Vector3 projSpawn = new Vector3(transform.position.x, transform.position.y, 0);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Quaternion projRot = Quaternion.FromToRotation(projSpawn, mousePos);

            Debug.Log("Projectile spawns at: X = " + projSpawn.x + "| Y = " + projSpawn.y);
            Debug.Log("Mouse psoition is at: X = " + mousePos.x + "| Y = " + mousePos.y);

            projectile = Instantiate(projectile, projSpawn, projRot);
            projectile.GetComponent<Projectile>().setProjectileConfig(
                mousePos - projSpawn, 10f, 20, weaponMode);

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
        EquipWeapon(animator.GetInteger("Equipment"));
    }
    //---------------------------------------
}
