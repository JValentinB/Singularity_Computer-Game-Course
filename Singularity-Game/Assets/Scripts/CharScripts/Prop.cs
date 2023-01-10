using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Prop : Damageable
{
    //public AudioSource prop_break;
    public AudioSource prop_break;
    public abstract void createLoot();

    public void OnDeath(){
        if(currentHealth <= 0){
            createLoot();
            //...animation...
            prop_break.Play();
            Debug.Log("Prop destroyed!");
            gameObject.SetActive(false);
        }
    }
}
