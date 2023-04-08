using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rootSpike : MonoBehaviour
{   
    public int damage = 30;
    public float growSize = 1f;
    public float growTime = 0.2f;
    public float waitTime = 0.5f;
    public float retractionTime = 0.5f;

    private Animator animator;
    private ParticleSystem ps;
    private bool startRumble;

    Vector3 localScale;
    Vector3 startPosition;
    private bool isGrowing = false;
    private bool isShrinking = false;

    [HideInInspector] public Vector3 growingDirection;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ps = GetComponentInChildren<ParticleSystem>();
        startRumble = true;

        float randomScale = Random.Range(0.5f, 1f);
        localScale = transform.localScale * randomScale;
        transform.localScale = localScale * 0.1f;

        startPosition = transform.position;

        growSize *= randomScale;
    }

    void Update()
    {
        PlayEarthRumble();
        GrowRootSpike();
        // growBackCount();
    }

    private void PlayEarthRumble()
    {
        if (startRumble)
        {
            ps.Play();
            startRumble = false;
        }
    }

    private void GrowRootSpike()
    {
        if (ps.isEmitting) return;

        // animator.SetBool("grow", true);
        if (!isGrowing)
        {
            isGrowing = true;
            StartCoroutine(rootScale(0.1f, 1f));
        }
    }

    // private void growBackCount()
    // {
    //     if (animator.GetBool("grow"))
    //     {
    //         growBackCD -= Time.deltaTime;
    //     }
    //     if (0f >= growBackCD)
    //     {
    //         // animator.SetBool("grow", false);
    //         // animator.SetBool("recede", true);
    //         Destroy(gameObject, 1f);
    //     }
    // }

    IEnumerator rootScale(float startScale, float endScale)
    {
        isGrowing = true;

        transform.localScale = localScale * startScale;

        float time = 0f;
        while (time < growTime)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, endScale, time / growTime);
            transform.localScale = localScale * scale;

            if (growingDirection == Vector3.up)
            {
                float position = Mathf.Lerp(startPosition.y, startPosition.y + growSize, time / growTime);
                transform.position = new Vector3(transform.position.x, position, transform.position.z);
            }
            else if(growingDirection == Vector3.down){
                float position = Mathf.Lerp(startPosition.y, startPosition.y - growSize, time / growTime);
                transform.position = new Vector3(transform.position.x, position, transform.position.z);
            }
            else if (growingDirection == Vector3.right)
            {
                float position = Mathf.Lerp(startPosition.x, startPosition.x + growSize, time / growTime);
                transform.position = new Vector3(position, transform.position.y, transform.position.z);
            }
            else
            {
                float position = Mathf.Lerp(startPosition.x, startPosition.x - growSize, time / growTime);
                transform.position = new Vector3(position, transform.position.y, transform.position.z);
            }

            yield return null;
        }
        transform.localScale = localScale * endScale;

        yield return new WaitForSeconds(waitTime);

        time = 0f;
        while (time < retractionTime)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(endScale, startScale, time / retractionTime);
            transform.localScale = localScale * scale;

            if (growingDirection == Vector3.up)
            {
                float position = Mathf.Lerp(startPosition.y + growSize, startPosition.y, time / retractionTime);
                transform.position = new Vector3(transform.position.x, position, transform.position.z);
            }
            else if(growingDirection == Vector3.down){
                float position = Mathf.Lerp(startPosition.y - growSize, startPosition.y, time / retractionTime);
                transform.position = new Vector3(transform.position.x, position, transform.position.z);
            }
            else if (growingDirection == Vector3.right)
            {
                float position = Mathf.Lerp(startPosition.x + growSize, startPosition.x, time / retractionTime);
                transform.position = new Vector3(position, transform.position.y, transform.position.z);
            }
            else
            {
                float position = Mathf.Lerp(startPosition.x - growSize, startPosition.x, time / retractionTime);
                transform.position = new Vector3(position, transform.position.y, transform.position.z);
            }

            yield return null;
        }
        transform.localScale = localScale * startScale;

        Destroy(gameObject, 0.5f);
    }
}
