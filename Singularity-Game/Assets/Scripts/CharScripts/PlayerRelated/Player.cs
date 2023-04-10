using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    [Header("For the Player")]
    public int weaponMode = -1;
    public List<bool> unlockedWeaponModes = new List<bool>() { false, false, false, false };
    public bool killOnHighFallingSpeed = true;
    private static List<bool> savedWeaponModes = new List<bool>() { false, false, false, false };


    [SerializeField] public bool doubleJump, lockPlayerControl, freezeGame;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject projectile_blackhole;
    [SerializeField] public GameObject jumpBurst;
    [SerializeField] private InvUI invUi;
    [HideInInspector] public bool setDirectionShot;
    private SceneControl scenecontrol;
    private static Vector3 latestCheckPointPos;
    public GameObject BlackOutSquare;
    public InvManager inventory;
    public int meleeDamage;
    public bool infinite_ammo = false;

    private static bool notFirstTime = false;
    [HideInInspector] public bool controllingPlatform = false;

    private Coroutine castingCoroutine;
    private bool isCasting = false;
    private int attackCoroutineCount = 0;
    // [HideInInspector] public int shiftInversion = 1;
    private bool projectileCooldownActive;
    private float projectileCooldown = 0.5f;

    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        healthBar.maxHealth = maxHealth;
        healthBar.currentHealth = currentHealth;

        walkSpeed = 0.4f;
        runSpeed = 4.0f;
        sprintSpeed = 8.0f;
        mass = 75.0f;
        gravitationalDirection = Vector3.down;
        targetDirection = Vector3.down;
        direction = 1;
        jumpForce = 1250f;
        standardJumpForce = jumpForce;
        jumpNumber = 1;
        jumpsRemaining = jumpNumber;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        setDirectionShot = false;
        meleeDamage = 15;
        unlockedWeaponModes = savedWeaponModes;

        scenecontrol = GameObject.Find("Main Camera").GetComponent<SceneControl>();
        inventory = new InvManager();
        BlackOutSquare = GameObject.Find("/UI_Ingame/black_screen");
        BlackOutSquare.GetComponent<Image>().color = new Color(0f, 0f, 0f, 255f);
        StartCoroutine(FadeBlackOutSquare(false));
        checkForStart();


        // Delete Before Release!!!
        /* unlockedWeaponModes[0] = true;
        unlockedWeaponModes[1] = true;
        unlockedWeaponModes[2] = true;
        unlockedWeaponModes[3] = true; */
    }

    void FixedUpdate()
    {
        SpeedToggle();

        ChangeLineOfSight();
        Turn();
        if (!lockPlayerControl) MovePlayer();

        GroundCheck();
        FallAnimation();

        RotateGravity();
        ApplyGravity();
        //changeEquipment();

        killOnHighSpeed();
        if (currentHealth <= 0) OnDeath();
    }

    void Update()
    {
        // Debug.Log(shiftInversion);
        if (lockPlayerControl)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        StartCoroutine(Attack());
        StartCoroutine(FireProjectile());
        Jump();
        SaveAndLoadGame();
    }

    private void MovePlayer()
    {
        float landing = (animator.GetCurrentAnimatorStateInfo(0).IsName("Landing")) ? 0.5f : 1;

        var velocity = direction * Vector3.forward * Input.GetAxis("Horizontal") * landing * currentSpeed; // * shiftInversion;

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
        // int shiftInversion = targetDirection == Vector3.up ? -1 : 1;
        if (isCasting)
        {
            // check if the player casted the spell on the right or the left
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            if (Vector3.Distance(mousePos, transform.position) > 2.5f)
            {
                Vector3 playerToMouse = Vector3.Normalize(mousePos - transform.position);
                float dot = Vector3.Dot(Camera.main.transform.right, playerToMouse);
                if (dot > 0)
                    direction = 1; // * shiftInversion;
                else
                    direction = -1; // * shiftInversion;

                return;
            }
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) direction = -1; // * shiftInversion;
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) direction = 1; // * shiftInversion;
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            createBurst();
            StartCoroutine(playAnimationForTime("Jumping", 0.5f));

            // factorwise multiply the velocity with another vector
            if (targetDirection == Vector3.up || targetDirection == Vector3.down)
                rb.velocity = Vector3.Scale(rb.velocity, new Vector3(1, 0.1f, 1));
            else
                rb.velocity = Vector3.Scale(rb.velocity, new Vector3(0.1f, 1, 1));

            rb.AddForce((-1) * gravitationalDirection * jumpForce, ForceMode.Impulse);
            jumpsRemaining--;
        }
    }

    public void GiveItem(InvItem item, int amount)
    {
        bool isSpace = invUi.AddItemToPlayerInventory(item, amount);
    }

    private void GroundCheck()
    {
        float falling_distance = 1.7f;

        Vector4 offsets = offsetDict[targetDirection];
        Ray ray1 = new Ray(transform.position + new Vector3(offsets.z, offsets.x, 0), targetDirection);
        Ray ray2 = new Ray(transform.position + new Vector3(offsets.w, offsets.y, 0), targetDirection);
        RaycastHit hit1 = new RaycastHit();
        RaycastHit hit2 = new RaycastHit();

        // Debug.DrawRay(transform.position + new Vector3(offsets.z, offsets.x, 0), targetDirection * 2, Color.green);
        // Debug.DrawRay(transform.position + new Vector3(offsets.w, offsets.y, 0), targetDirection * 2, Color.green);

        bool raycast1 = Physics.Raycast(ray1, out hit1, 2);
        bool raycast2 = Physics.Raycast(ray2, out hit2, 2);
        if (!raycast1) hit1.distance = Mathf.Infinity;
        if (!raycast2) hit2.distance = Mathf.Infinity;

        // Debug.Log(hit1.distance + " " + hit2.distance);
        if (raycast1 || raycast2)
        {
            if (hit1.distance > falling_distance && hit2.distance > falling_distance)
                isGrounded = false;
            else
            {
                jumpsRemaining = jumpNumber;
                isGrounded = true;
            }
        }
        else
            isGrounded = false;
    }

    void FallAnimation()
    {
        if (isGrounded && !animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
        {
            animator.SetBool("Falling", false);
            StartCoroutine(playAnimationForTime("Landing", 0.5f));
        }
        else if (!isGrounded)
        {
            animator.SetBool("Falling", true);
            if(gravitationalDirection == Vector3.down && 
            (rb.velocity.x > 0.1f || rb.velocity.x < -0.1f)) AdjustVelocityOnFall();
             
        }
    }

    private void AdjustVelocityOnFall(){
        var adjustInDir = 0f;
        if(rb.velocity.x > 0f) adjustInDir = -10f;
        else if(rb.velocity.x < 0f) adjustInDir = 10f;

        rb.velocity = new Vector3(rb.velocity.x + adjustInDir * Time.deltaTime, rb.velocity.y, rb.velocity.z);
    }

    public void ChangeBulletMode(int modeId)
    {
        weaponMode = modeId;
    }

    private IEnumerator FireProjectile()
    {
        if (Input.GetMouseButtonDown(1) && (weaponMode != 0) && !projectileCooldownActive)
        {
            if ((weaponMode == 1 && !unlockedWeaponModes[1]) ||
               (weaponMode == 2 && !unlockedWeaponModes[2]) ||
               (weaponMode == 3 && !unlockedWeaponModes[3])) yield break;

            projectileCooldownActive = true;
            if (!infinite_ammo)
            {
                // use ammo on respective weaponmode
                int res = inventory.RemoveItem(inventory.GetItem(weaponMode), 1);
                //Debug.Log(res);
                if (res == -1) yield break;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            if (castingCoroutine != null)
                StopCoroutine(castingCoroutine);
            castingCoroutine = StartCoroutine(fadeInOutCastingAnimation(1f, 0.3f));
            yield return new WaitForSeconds(0.4f);
            castingCoroutine = StartCoroutine(fadeInOutCastingAnimation(0f, 0.5f));


            //Do not use nearClipPlane from main camera, it's somehow synced to the overlayy camera. 72.8 is the correct nearClipPlane

            Vector3 staffStonePos = getStaffStonePos();
            Vector3 projTarget = mousePos - staffStonePos;
            projTarget = projTarget.normalized;
            projTarget = new Vector3(projTarget.x, projTarget.y, 0f);

            GameObject projectileClone = (GameObject)Instantiate(weaponMode == 2 ? projectile_blackhole : projectile, staffStonePos, Quaternion.identity);
            Destroy(projectileClone, 20f);
            var shape = projectileClone.GetComponent<ParticleSystem>().shape;
            shape.position = Vector3.zero;

            if (setDirectionShot)
            {
                projectileClone.GetComponent<Projectile>().setProjectileConfig(
                    projTarget, 20, 4);
                setDirectionShot = false;
            }
            else
            {
                projectileClone.GetComponent<Projectile>().setProjectileConfig(
                    projTarget, weaponMode == 2 ? 10 : 20, weaponMode);
            }

            if (weaponMode != 2)
                Destroy(projectileClone, 5);
            else
                Destroy(projectileClone, 10);

            yield return new WaitForSeconds(projectileCooldown);
            projectileCooldownActive = false;
        }
    }

    IEnumerator fadeInOutCastingAnimation(float target, float duration)
    {
        float start = animator.GetLayerWeight(3);
        if (start == 0) isCasting = true;

        float counter = 0f;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            float weight = Mathf.Lerp(start, target, counter / duration);
            animator.SetLayerWeight(3, weight);
            yield return null;
        }
        animator.SetLayerWeight(3, target);

        if (target == 0) isCasting = false;
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

    public Vector3 getStaffStonePos()
    {
        return GameObject.FindWithTag("Staffstone").transform.position;
    }

    public void setFirstTime()
    {
        notFirstTime = true;
    }

    public void checkForStart()
    {
        if (notFirstTime)
        {
            transform.position = latestCheckPointPos;
        }
        else
        {
            latestCheckPointPos = new Vector3(-200.71f, 77.35f, 0f);
        }
    }

    private void killOnHighSpeed()
    {
        if (!killOnHighFallingSpeed) return;

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

    public void SetSavedWeaponModes(List<bool> weaponModes)
    {
        savedWeaponModes = weaponModes;
    }

    public List<bool> GetSavedWeaponModes()
    {
        return savedWeaponModes;
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
        if (SaveSystem.couldNotLoadGame && SaveSystem.loadingDelay <= 0f) SaveSystem.loadingDelay -= Time.deltaTime;
        else if (SaveSystem.couldNotLoadGame) SaveSystem.LoadGameNoReset();

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveSystem.SaveGame(this);
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            SaveSystem.LoadGame();
        }
    }

    void hitSoundAndParticles()
    {
        GameObject.FindWithTag("Staffstone").GetComponent<StaffStoneControl>().hitSoundAndParticles();
    }

    //FIXME Muss noch neu gemacht werden:
    //---------------------------------------
    private float last_Attack;
    private GameObject gun, sword;

    IEnumerator Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            last_Attack = Time.time;
            animator.SetLayerWeight(1, 1);
            animator.SetInteger("Attack", (animator.GetInteger("Attack") + 1) % 4);
        }
        else yield break;

        attackCoroutineCount++;
        yield return new WaitForSeconds(1);

        if (attackCoroutineCount == 1)
        {
            animator.SetLayerWeight(1, 0);
            animator.SetInteger("Attack", 0);
        }
        attackCoroutineCount--;
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


    Dictionary<Vector3, Vector4> offsetDict = new Dictionary<Vector3, Vector4>
    {
        { Vector3.up, new Vector4(-1, -1, 0.35f, -0.35f) },
        { Vector3.down, new Vector4(1, 1, -0.35f, 0.35f) },
        { Vector3.left, new Vector4(0.35f, -0.35f, 1, 1) },
        { Vector3.right, new Vector4(-0.35f, 0.35f, -1, -1) },
    };
    //---------------------------------------
}
