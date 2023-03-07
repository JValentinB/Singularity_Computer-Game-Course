using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    [SerializeField] public int weaponMode, weaponModes;
    [SerializeField] public bool doubleJump;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject projectile_blackhole;

    [SerializeField] public GameObject jumpBurst;
    public bool setDirectionShot; //Will the next projectile control the direction of a Rockpiece?
    private SceneControl scenecontrol;
    [SerializeField] private static Vector3 latestCheckPointPos;
    private InvUI invUI;
    public GameObject BlackOutSquare;
    private static bool notFirstTime = false;

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
        weaponMode = 0;
        weaponModes = 2;
        doubleJump = true;
        setDirectionShot = false;
        scenecontrol = GameObject.Find("Main Camera").GetComponent<SceneControl>();
        inventory = new InvManager();
        invUI = GetComponent<InvUI>();
        BlackOutSquare = GameObject.Find("/Canvas/black_screen");
        BlackOutSquare.GetComponent<Image>().color = new Color(0f, 0f, 0f, 255f);
        StartCoroutine(FadeBlackOutSquare(false));
        checkForStart();

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
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    void Update(){
        Attack();
        FireProjectile();
        Jump();
        ChangeBulletMode();
        SaveAndLoadGame();
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

    public void GiveItem(InvItem item, int amount){
        bool isSpace = invUI.AddItemToPlayerInventory(item, amount);
        //if(!isSpace) inventoryFull();
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
        /* Debug.DrawRay(transform.position + new Vector3( 0.5f, 1, 0), new Vector3(0, -5, 0), Color.green);
        Debug.DrawRay(transform.position + new Vector3(-0.5f, 1, 0), new Vector3(0, -5, 0), Color.red); */
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

    private void ChangeBulletMode(){
        float scroll_delta = Input.mouseScrollDelta.y;
        if(scroll_delta > 0){
            weaponMode = (weaponMode + 1) % 3;
        } else if(scroll_delta < 0){
            weaponMode = (weaponMode - 1) % 3;
        }

        /*
        if (Input.GetKey(KeyCode.Keypad0))
        {
            weaponMode = 0;
        }
        else if (Input.GetKey(KeyCode.Keypad1))
        {
            weaponMode = 1;
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            weaponMode = 2;
        }
        */

    }

    private void FireProjectile(){
        if(!Input.GetMouseButtonDown(1)) return;

        //Do not use nearClipPlane from main camera, it's somehow synced to the overlayy camera. 72.8 is the correct nearClipPlane
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 72.8f));
        Vector3 staffStonePos = GameObject.FindWithTag("Staffstone").transform.position;
        Vector3 projTarget = mousePos - staffStonePos;
        projTarget = new Vector3(projTarget.x, projTarget.y, 0f);

        GameObject projectileClone = (GameObject) Instantiate(/*weaponMode == 2 ? projectile_blackhole : */projectile, staffStonePos, Quaternion.identity);
        if(setDirectionShot){
            projectileClone.GetComponent<Projectile>().setProjectileConfig(
                projTarget, 15, 3);
            setDirectionShot = false;
        } else{
            projectileClone.GetComponent<Projectile>().setProjectileConfig(
                projTarget, weaponMode == 2 ? 2 : 15, weaponMode); }
        if (weaponMode != 2)
        {
            Destroy(projectileClone, 5);
        }
        else
        {
            Destroy(projectileClone, 30);
        }
    }

    public void setCheckPoint(Vector3 pos)
    {
        latestCheckPointPos = pos;
        latestCheckPointPos.z = 0;
    }

    public Vector3 getCheckPoint(){
        return latestCheckPointPos;
    }

    public void setFirstTime()
    {
        notFirstTime = true;
    }

    public void checkForStart()
    {
        if (notFirstTime) {
            transform.position = latestCheckPointPos;
        } else {
            latestCheckPointPos = new Vector3(-200.71f, 77.35f, 0f);
        }
    }

    public void OnDeath()
    {
        StartCoroutine(delayedDeath());
    }

    IEnumerator delayedDeath()
    {
        StartCoroutine(FadeBlackOutSquare());
        yield return new WaitForSeconds(2);
        scenecontrol.reset_on_death();

    }

    private void createBurst(){
        GameObject burstClone = Instantiate(jumpBurst, transform.position, transform.rotation);
        Destroy(burstClone, 1);
    }

    private void SaveAndLoadGame(){
        if(Input.GetKeyDown(KeyCode.K)){
            SaveSystem.SaveGame(this);
        }
        if(Input.GetKeyDown(KeyCode.L)){
            SaveSystem.LoadGame();
        }
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


    private IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, float fadespeed = 1f)
    {

        Color objectColor = BlackOutSquare.GetComponent<Image>().color;
        float fadeAmount;
        if (fadeToBlack)
        {
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, 0f);
            while (BlackOutSquare.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadespeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                BlackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        else
        {
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, 1f);
            while (BlackOutSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadespeed / 2 * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                BlackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
    }
    //---------------------------------------
}
