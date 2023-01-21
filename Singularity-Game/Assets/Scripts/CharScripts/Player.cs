using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private bulletMode weaponMode;
    [SerializeField] private GameObject projectile;
    [SerializeField] public GameObject jumpBurst;
    public bool setDirectionShot; //Will the next projectile control the direction of a Rockpiece?

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
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        weaponMode = bulletMode.Blue;
        setDirectionShot = false;
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
        
    }

    void Update(){
        Attack(); 
        Jump();
        if(Input.GetKeyDown(KeyCode.Space)) createBurst();
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
            if (hit1.distance > falling_distance && hit2.distance > falling_distance && rb.velocity.y < -0.2f)
                animator.SetBool("Falling", true);
            else {
                animator.SetBool("Falling", false);
                // reset airjump number
                jumpNumber = 2;
            }
        }
    }

    private void FireProjectile(){
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Vector3 fixedPos = new Vector3(transform.position.x, transform.position.y + 1.3f, 0);
        Vector3 projTarget = mousePos - fixedPos;
        projTarget = new Vector3(projTarget.x, projTarget.y, 0f);

        GameObject projectileClone = (GameObject) Instantiate(projectile, fixedPos, Quaternion.identity);
        if(setDirectionShot){
            bulletMode cMode = bulletMode.Control;
            projectileClone.GetComponent<Projectile>().setProjectileConfig(
                projTarget, 15, 20, cMode);
            setDirectionShot = false;
        } else{
            projectileClone.GetComponent<Projectile>().setProjectileConfig(
                projTarget, 15, 20, weaponMode); }
        Destroy(projectileClone, 5);
    }

    public void OnDeath(){
        //...
    }

    private void createBurst(){
        GameObject burstClone = Instantiate(jumpBurst, transform.position, transform.rotation);
        Destroy(burstClone, 1);
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
            FireProjectile();
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
