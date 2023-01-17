using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Prop : Damageable
{
    [SerializeField] public GameObject cratePiece;
    [SerializeField] public GameObject lootPrefab;

    public AudioSource prop_break;
    public abstract void createLoot();
    public abstract void createPieces();

    public void OnDeath(){
        if(currentHealth <= 0){
            createLoot();
            createPieces();
            prop_break.Play();
            Debug.Log("Prop destroyed!");
            gameObject.SetActive(false);
        }
    }
}
