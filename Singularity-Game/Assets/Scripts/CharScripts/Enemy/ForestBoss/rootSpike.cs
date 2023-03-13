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
        growBackCD = 1f;
    }

    void Update()
    {
        PlayEarthRumble();
        GrowRootSpike();
        growBackCount();
    }

    private void PlayEarthRumble(){
        if(startRumble){
            ps.Play();
            startRumble = false;
        }
    }

    private void GrowRootSpike(){
        if(ps.isEmitting){ return; }
        animator.SetBool("grow", true);
    }

    private void growBackCount(){
        if(animator.GetBool("grow")){
            growBackCD -= Time.deltaTime;
        }
        if(0f >= growBackCD){
            animator.SetBool("grow", false);
            animator.SetBool("recede", true);
            Destroy(gameObject, 1f);
        }
        Debug.Log(growBackCD);
    }
}
