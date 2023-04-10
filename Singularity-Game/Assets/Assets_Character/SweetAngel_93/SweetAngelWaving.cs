using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetAngelWaving : MonoBehaviour
{
    Animator animator;

    void Start(){
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            animator.SetTrigger("Waving");
        }
    }
}
