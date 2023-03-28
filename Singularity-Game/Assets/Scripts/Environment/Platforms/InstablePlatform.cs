using System.Collections;
using UnityEngine;

public class InstablePlatform : Platform
{
    private Animator animator;
    private bool isShaking = false;
    private bool isBroken = false;
    private bool takingDamage = false;

    [Header("Instable")]
    public bool fixedAfterBreaking = true;
    public float maxHealth = 1000;
    public float damageOnCollision = 10;
    private float currentHealth;
    public float breakDuration = 0.5f;
    public float fixDuration = 5f;
    public float maxIntensity = 50f;
    [Range(0f, 1f)]
    public float maxVolume = 1f;
    public float flashDuration = 0.1f;

    Coroutine breakCoroutine;
    Coroutine damageCoroutine;

    private Light crystalLight;
    private float intensity;
    private float threshold = 0f;
    int shakingHash = Animator.StringToHash("Base Layer.Shaking");

    Rigidbody[] rigidbodies;
    Collider[] colliders;

    private ObjectSounds objectSounds;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        crystalLight = transform.Find("Crystal").GetComponent<Light>();
        intensity = crystalLight.intensity;

        currentHealth = maxHealth;

        objectSounds = GetComponent<ObjectSounds>();

    }

    void Update()
    {
        if (!isBroken && crystalLight.intensity > 1)
        {
            crystalLight.intensity -= 0.1f;
        }

        if ((!isShaking && objectSounds.isPlayed("Shaking")) || isBroken)
        {
            isShaking = false;
            StartCoroutine(objectSounds.fadeInOut("Shaking", 0f, 0.3f));
        }
    }

    void OnCollisionStay(Collision collision)
    {
        Collider other = collision.collider;
        Rigidbody other_rigidbody = other.GetComponent<Rigidbody>();
        if (other.tag == "Player")
        {   
            if(damageOnCollision < 0) return; 
            ApplyDamage(damageOnCollision);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Collider other = collision.collider;
        Rigidbody other_rigidbody = other.GetComponent<Rigidbody>();
        if (other.tag == "Player")
        {
            isShaking = false;
            animator.SetBool("isShaking", false);
        }
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        if (!takingDamage)
        {
            if (damageCoroutine != null)
                StopCoroutine(damageCoroutine);
            damageCoroutine = StartCoroutine(checkForDamage());
        }

        if (currentHealth <= 0 && !isBroken)
        {
            isShaking = false;
            animator.SetBool("isShaking", false);
            objectSounds.Stop("Shaking");

            isBroken = true;

            if (fixedAfterBreaking)
            {
                animator.SetBool("isBroken", true);
                StartCoroutine(Break());
            }
            else
                BreakWithoutFix();
            return;
        }
        else if (isBroken) return;

        float healtRatio = 1 - currentHealth / maxHealth;

        isShaking = true;
        animator.SetBool("isShaking", true);
        animator.SetFloat("ShakingMultiplier", healtRatio);
        crystalLight.intensity = healtRatio * maxIntensity;

        if (!objectSounds.isPlayed("Shaking"))
            objectSounds.Play("Shaking");
        objectSounds.setSourceVolume("Shaking", healtRatio * maxVolume);
    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (Collider collider in GetComponents<Collider>())
        {
            if (!collider.isTrigger)
            {
                collider.enabled = false;
                break;
            }
        }
        StartCoroutine(TransitionLight(crystalLight.intensity, crystalLight.intensity * 10, 0.1f));

        objectSounds.Play("Breaking");
        yield return new WaitForSeconds(breakDuration);
        StartCoroutine(Fix());
    }

    void BreakWithoutFix()
    {
        animator.enabled = false;
        GetComponent<Collider>().enabled = false;

        Transform armature = transform.Find("Armature");
        rigidbodies = armature.GetComponentsInChildren<Rigidbody>();
        colliders = armature.GetComponentsInChildren<Collider>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
        }
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        objectSounds.Play("Breaking");
        Destroy(gameObject, 10f);
    }

    IEnumerator Fix()
    {
        isBroken = false;
        animator.SetBool("isBroken", false);
        currentHealth = maxHealth;

        yield return new WaitForSeconds(fixDuration / 2f);
        objectSounds.Play("Fixing");
        yield return new WaitForSeconds(fixDuration / 2f);

        StartCoroutine(TransitionLight(crystalLight.intensity, intensity, 1f));
        foreach (Collider collider in GetComponents<Collider>())
        {
            if (!collider.isTrigger)
            {
                collider.enabled = true;
                break;
            }
        }
    }

    IEnumerator TransitionLight(float current, float target, float transitionTime)
    {
        float t = 0f;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            crystalLight.intensity = Mathf.Lerp(current, target, t / transitionTime);
            yield return null;
        }
    }

    IEnumerator playAnimationForTime(string animationName, float time)
    {
        animator.SetBool(animationName, true);
        yield return new WaitForSeconds(time);
        animator.SetBool(animationName, false);
    }

    IEnumerator checkForDamage()
    {
        takingDamage = true;
        float healthLastFrame = maxHealth;

        while (healthLastFrame != currentHealth)
        {
            healthLastFrame = currentHealth;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        isShaking = false;
        takingDamage = false;
    }
}
