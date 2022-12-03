using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField ]GameObject player;
    [SerializeField] int damagePerHit = 40;
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (other.gameObject.layer == 6 && player.GetComponent<Animator>().GetInteger("Attack") > 0)
        {
            enemy.takeDamage(damagePerHit);
            enemy.tookDamage = true;
            other.GetComponent<Renderer>().material.color = Color.red;
            Debug.Log(other.GetComponent<Renderer>().material.color);
        }
    }
}
