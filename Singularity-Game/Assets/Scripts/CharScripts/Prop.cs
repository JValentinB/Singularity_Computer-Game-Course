using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Damageable
{
    public AudioSource prop_break;

    public void OnDeath(){
        if(currentHealth <= 0){
            //...drop Items...
            //...animation...
            prop_break.Play();
            Debug.Log("Prop destroyed!");
            gameObject.SetActive(false);
        }
    }
}
