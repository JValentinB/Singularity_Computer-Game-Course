using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public GameObject playerObject;
    public int xp;
    public float sightRange, attackRange;

    public bool InRange(float dist){
        return dist >= (transform.position - playerObject.transform.position).magnitude;
    }

    public void MoveEnemy(){
        var velocity = Vector3.zero;
        if(InRange(sightRange) && !InRange(attackRange-0.5f)){
            if(gravitationalDirection.x == 1){
                direction = playerObject.transform.position.y - transform.position.y > 0 ? 1 : -1;  
            }else if(gravitationalDirection.x == -1){
                direction = playerObject.transform.position.y - transform.position.y > 0 ? -1 : 1;
            }else {
                direction = playerObject.transform.position.x - transform.position.x > 0 ? 1 : -1;
            }
            velocity = Vector3.forward * currentSpeed;
        }
        transform.Translate(velocity * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }


    public void OnDeath(){
        if(currentHealth <= 0){
            currentHealth = 0;
            playerObject.GetComponent<Player>().giveXp(xp);
            //...drop Items...
            //...animation...
            gameObject.SetActive(false);
        } 
    }
}
