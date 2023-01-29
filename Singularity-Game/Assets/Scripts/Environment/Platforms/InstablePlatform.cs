using System.Collections;
using UnityEngine;

public class InstablePlatform : Platform
{
    private Animator animator;
    private bool isShaking = false;
    private bool isBroken = false;

    [Header("Instable")]
    public float breakingThreshold = 100;
    public float breakDuration = 0.5f;
    public float fixDuration = 5f;
    public float flashDuration = 0.1f;

    private Light crystalLight;
    private float intensity;
    private float threshold = 0f;
    int shakingHash = Animator.StringToHash("Base Layer.Shaking");

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        crystalLight = transform.Find("Crystal").GetComponent<Light>();
        intensity = crystalLight.intensity;
    }

    void FixedUpdate()
    {
        if (isShaking && !isBroken)
        {
            threshold += 1;
            crystalLight.intensity += 1;
            Debug.Log(threshold);
            animator.SetFloat("ShakingMultiplier", (1 / breakingThreshold) * threshold);

            if (threshold > breakingThreshold)
            {
                isBroken = true;
                animator.SetBool("isBroken", true);
                isShaking = false;
                animator.SetBool("isShaking", false);
                StartCoroutine(Break());
            }
        }
        else if(!isBroken && crystalLight.intensity > 1){
            threshold -= 0.1f;
            crystalLight.intensity -= 0.1f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody other_rigidbody = other.GetComponent<Rigidbody>();
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            other_rigidbody.velocity += rb.velocity;

            isShaking = true;
            animator.SetBool("isShaking", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody other_rigidbody = other.GetComponent<Rigidbody>();
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            other_rigidbody.velocity += rb.velocity;

            isShaking = false;
            animator.SetBool("isShaking", false);
        }
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

        yield return new WaitForSeconds(breakDuration);
        StartCoroutine(Fix());
    }

    IEnumerator Fix()
    {
        isBroken = false;
        animator.SetBool("isBroken", false);
        threshold = 0f;
        yield return new WaitForSeconds(fixDuration);
        StartCoroutine(TransitionLight(crystalLight.intensity, intensity, 1f));
        foreach (Collider collider in GetComponents<Collider>())
        {
            if (!collider.isTrigger)
            {
                collider.enabled = true;
                break;
            }
        }
        // isShaking = false;
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
}
