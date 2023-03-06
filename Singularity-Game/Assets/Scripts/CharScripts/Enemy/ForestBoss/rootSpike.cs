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
        animator = GetComponent<Animator>();
        ps = GetComponent<ParticleSystem>();
        dmg = 30;
        startRumble = true;
        growBackCD = 3f;
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
        //animator.SetBool("growRoot", true);
    }

    private void growBackCount(){
        if(/*animator.GetBool("growRoot") &&*/growBackCounter < growBackCD){
            growBackCounter += Time.deltaTime;
            return;
        }
        //reverse growth of root

    }
}
