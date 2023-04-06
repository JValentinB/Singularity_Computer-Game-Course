using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rootSpike : MonoBehaviour
{
    private Animator animator;
    private ParticleSystem ps;
    private int dmg;
    private bool startRumble;
    private float growBackCounter, growBackCD;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ps = GetComponentInChildren<ParticleSystem>();
        dmg = 30;
        startRumble = true;
        growBackCD = 0.5f;
    }

    void Update()
    {
        PlayEarthRumble();
        GrowRootSpike();
        growBackCount();
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
        
        animator.SetBool("grow", true);
        StartCoroutine(rootScale(0.1f, 1f));
    }

    private void growBackCount()
    {
        if (animator.GetBool("grow"))
        {
            growBackCD -= Time.deltaTime;
        }
        if (0f >= growBackCD)
        {
            animator.SetBool("grow", false);
            animator.SetBool("recede", true);
            Destroy(gameObject, 1f);
            StartCoroutine(rootScale(1f, 0.1f));
        }
    }

    IEnumerator rootScale(float startScale, float endScale)
    {
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1f, 1f, startScale));

        float time = 0f;
        while (time < growBackCD)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, endScale, time / growBackCD);
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1f, 1f, scale));
            yield return null;
        }
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1f, 1f, endScale));
    }
}
