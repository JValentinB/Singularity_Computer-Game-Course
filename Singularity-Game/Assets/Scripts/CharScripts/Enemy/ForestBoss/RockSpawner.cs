using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{   
    Transform player;

    Collider collisionCollider;
    Collider triggerCollider;

    [HideInInspector] public bool playerOnIsland = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        collisionCollider = GetComponent<Collider>();
        triggerCollider = GetComponent<Collider>();

        if(collisionCollider.isTrigger){
            Collider tempCollider = collisionCollider;
            collisionCollider = triggerCollider;
            triggerCollider = tempCollider;
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            playerOnIsland = false;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag == "Player"){
            playerOnIsland = true;
        }
    }

    public void spawnRock(GameObject manipulatableProjectile){
        if(!playerOnIsland) return;

        var spawnPos = GetComponent<Collider>().ClosestPointOnBounds(player.transform.position);
        GameObject projectileObject = Instantiate(manipulatableProjectile, spawnPos, Quaternion.identity);
        projectileObject.GetComponentInChildren<m_Projectile>().direction = (player.transform.position - spawnPos).normalized;
        Destroy(projectileObject, 10f);
    }
}
