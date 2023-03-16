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
    [HideInInspector] public bool controllingPlatform = false;

    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;

        walkSpeed = 0.4f;
        runSpeed = 4.0f;
        sprintSpeed = 8.0f;
        mass = 75.0f;
        gravitationalDirection = Vector3.down;
        targetDirection = Vector3.down;
        direction = 1;
        jumpForce = 1200f;
        jumpNumber = 5;
        doubleJump = true;
        jumpsRemaining = jumpNumber;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        weaponMode = 0;
        weaponModes = 3;
        setDirectionShot = false;

        scenecontrol = GameObject.Find("Main Camera").GetComponent<SceneControl>();
        inventory = new InvManager();
        invUI = GetComponent<InvUI>();
        BlackOutSquare = GameObject.Find("/Canvas/black_screen");
        BlackOutSquare.GetComponent<Image>().color = new Color(0f, 0f, 0f, 255f);
        StartCoroutine(FadeBlackOutSquare(false));
        checkForStart();

    }

    void FixedUpdate()
    {
        SpeedToggle();
        ChangeLineOfSight();
        Turn();
        MovePlayer();
        GroundCheck();
        RotateGravity();
        ApplyGravity();
        //changeEquipment();

        killOnHighSpeed();
        if (currentHealth <= 0)
            OnDeath();
        
    }

    void Update()
    {
        Attack();
        if (!controllingPlatform)
            StartCoroutine(FireProjectile());
        Jump();
        ChangeBulletMode();
        SaveAndLoadGame();
    }



    private void MovePlayer()
    {
        float landing = (animator.GetCurrentAnimatorStateInfo(0).IsName("Landing")) ? 0.5f : 1;
        float shiftInversion = targetDirection == Vector3.up ? -1 : 1;
        var velocity = direction * Vector3.forward * Input.GetAxis("Horizontal") * landing * currentSpeed * shiftInversion;
        transform.Translate(velocity * Time.deltaTime);
        animator.SetFloat("Speed", velocity.magnitude);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void SpeedToggle()
    {
        if (Input.GetKey(KeyCode.LeftControl)) currentSpeed = walkSpeed;
        else if (Input.GetKey(KeyCode.LeftShift)) currentSpeed = sprintSpeed;
        else currentSpeed = runSpeed;
    }

    private void Turn()
    {
        int shiftInversion = targetDirection == Vector3.up ? -1 : 1;
        // turn around
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) direction = -1 * shiftInversion;
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) direction = 1 * shiftInversion;
    }

    public void Jump(){
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            createBurst();
            StartCoroutine(playAnimationForTime("Jumping", 0.5f));

            // factorwise multiply the velocity with another vector
            rb.velocity = Vector3.Scale(rb.velocity, new Vector3(1, 0.1f, 1));
            rb.AddForce((-1) * gravitationalDirection * jumpForce, ForceMode.Impulse);
            jumpsRemaining--;
        }
    }

    public void giveXp(int xp)
    {
        GetComponent<XpManager>().GainXp(xp);
    }

    public void GiveItem(InvItem item, int amount)
    {
        bool isSpace = invUI.AddItemToPlayerInventory(item, amount);
        //if(!isSpace) inventoryFull();
    }

    private void GroundCheck()
    {
        float falling_distance = 1.4f;
        Ray ray1 = new Ray(transform.position + new Vector3(0.5f, 1, 0), targetDirection);
        Ray ray2 = new Ray(transform.position + new Vector3(-0.5f, 1, 0), targetDirection);
        RaycastHit hit1;
        RaycastHit hit2;

        LayerMask hitLayer = LayerMask.NameToLayer("Ground");
        int layerMask = (1 << hitLayer);
        // Debug.DrawRay(transform.position + new Vector3( 0.5f, 1, 0), new Vector3(0, -5, 0), Color.green, 0.1f);
        // Debug.DrawRay(transform.position + new Vector3(-0.5f, 1, 0), new Vector3(0, -5, 0), Color.red);
        if (Physics.Raycast(ray1, out hit1, layerMask, 5) && Physics.Raycast(ray2, out hit2, layerMask, 5))
        {
            if (hit1.distance > falling_distance && hit2.distance > falling_distance)
            {
                if (checkFallingSpeed(0.2f) || checkFallingSpeed(-2f))
                {
                    animator.SetBool("Falling", true);
                }
                isGrounded = false;
            }
            else
            {
                if(animator.GetBool("Falling")) StartCoroutine(playAnimationForTime("Landing", 0.5f));
                animator.SetBool("Falling", false);
                // reset airjump number
                jumpsRemaining = jumpNumber;
                isGrounded = true;
            }
        }
        else
        {
            // if (checkFallingSpeed(0.2f))
            // {
            //     animator.SetBool("Falling", true);
            // }
            animator.SetBool("Landing", false);
            isGrounded = false;
        }

    }

    private void ChangeBulletMode()
    {
      
        if (Input.mouseScrollDelta.y > 0)
        {
            weaponMode = (weaponMode + 1) % weaponModes;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            weaponMode = (weaponMode - 1) % weaponModes;
            if (weaponMode < 0) weaponMode = weaponModes - 1;
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

    private IEnumerator FireProjectile()
    {   
        if (Input.GetMouseButtonDown(1))
        {
            if (!animator.GetBool("Casting"))
            {
                StartCoroutine(castingAnimation());
            }
            yield return new WaitForSeconds(0.75f);

            
            //Do not use nearClipPlane from main camera, it's somehow synced to the overlayy camera. 72.8 is the correct nearClipPlane
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 72.8f));
            Vector3 staffStonePos = GameObject.FindWithTag("Staffstone").transform.position;
            Vector3 projTarget = mousePos - staffStonePos;
            projTarget = new Vector3(projTarget.x, projTarget.y, 0f);

            GameObject projectileClone = (GameObject)Instantiate(weaponMode == 2 ? projectile_blackhole : projectile, staffStonePos, Quaternion.identity);
            var shape = projectileClone.GetComponent<ParticleSystem>().shape;
            shape.position = Vector3.zero;

            if (setDirectionShot)
            {
                projectileClone.GetComponent<Projectile>().setProjectileConfig(
                    projTarget, 20, 2);
                setDirectionShot = false;
            }
            else
            {
                projectileClone.GetComponent<Projectile>().setProjectileConfig(
                    projTarget, weaponMode == 2 ? 2 : 20, weaponMode);
            }
            if (weaponMode != 2)
            {
              Destroy(projectileClone, 5);
            }
            else
            {
              Destroy(projectileClone, 30);
            }
        }
    }

    IEnumerator castingAnimation()
    {
        animator.SetLayerWeight(3, 1);
        animator.SetBool("Casting", true);
        yield return new WaitForSeconds(2.5f);
        animator.SetLayerWeight(3, 0);
        animator.SetBool("Casting", false);
    }

    // IEnumerator to put the origin of the projectile always at the staffstone
    IEnumerator moveParticleOrigin(ParticleSystem particle)
    {
        var shape = particle.shape;
        shape.position = particle.transform.TransformPoint(GameObject.FindWithTag("Staffstone").transform.position);

        float t = 0f;
        float castingTime = 0.5f;
        while (t < castingTime)
        {
            t += Time.deltaTime;
            shape.position = particle.transform.TransformPoint(GameObject.FindWithTag("Staffstone").transform.position);
            yield return null;
        }
    }

    public void setCheckPoint(Vector3 pos)
    {
        latestCheckPointPos = pos;
        latestCheckPointPos.z = 0;
    }

    public Vector3 getCheckPoint()
    {
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
        }
        else
        {
            latestCheckPointPos = new Vector3(-200.71f, 77.35f, 0f);
        }
    }

    private void killOnHighSpeed()
    {
        bool tooFast = checkFallingSpeed(70f);
        if (tooFast)
            ApplyDamage(99999);
    }

    public bool checkFallingSpeed(float speedThreshold)
    {
        float fallingSpeed = rb.velocity.y;
        float sign = -1;
        if (targetDirection == Vector3.up)
        {
            fallingSpeed = rb.velocity.y;
            sign = 1;
        }
        else if (targetDirection == Vector3.right)
        {
            fallingSpeed = rb.velocity.x;
            sign = 1;
        }
        else if (targetDirection == Vector3.left)
        {
            fallingSpeed = rb.velocity.x;
            sign = -1;
        }
        return sign == 1 ? fallingSpeed > speedThreshold : fallingSpeed < -speedThreshold;
    }

    public void OnDeath()
    {
        StartCoroutine(Camera.main.GetComponent<CameraControl>().stopFollowing(2f));
        StartCoroutine(delayedDeath());
    }

    IEnumerator delayedDeath()
    {
        StartCoroutine(FadeBlackOutSquare());
        animator.SetBool("Dead", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("Dead", false);
        scenecontrol.reset_on_death();

    }


    private void createBurst()
    {
        GameObject burstClone = Instantiate(jumpBurst, transform.position, transform.rotation);
        Destroy(burstClone, 1);
    }

    private void SaveAndLoadGame()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
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
