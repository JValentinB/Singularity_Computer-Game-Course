using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleContainer : MonoBehaviour
{
    [SerializeField] private float ttl; 
    public float ttlCounter;

    // Start is called before the first frame update
    void Start()
    {
        ttl = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        AttractProjectiles();
        DecreaseTTL();
    }

    private void AttractProjectiles(){
        //if(ttlCounter <= 0f) return;

        var projectiles = GameObject.FindGameObjectsWithTag("m_Projectile");
        foreach(var projectile in projectiles){
            projectile.GetComponent<m_Projectile>().setDir(transform.position);
        }
        var bombFruits = GameObject.FindGameObjectsWithTag("BombFruit");
        foreach(var fruit in bombFruits){
            fruit.GetComponent<BombFruit>().setDir(transform.position);
        }
    }

    private void DecreaseTTL(){
        if(ttlCounter > 0f){
            ttlCounter -= Time.deltaTime;
            if(ttlCounter <= 0f){
                var projectilesAtPosition = Physics.OverlapSphere(transform.position, 1f);
                foreach(var projectile in projectilesAtPosition){
                    var obj = projectile.gameObject;
                    if(obj.GetComponent<m_Projectile>()) obj.GetComponent<m_Projectile>().OnDeath();
                }
                var projectiles = GameObject.FindGameObjectsWithTag("m_Projectile");
                foreach(var projectile in projectiles){
                    projectile.GetComponent<m_Projectile>().setDir(Vector3.down);
                }
            }
        }
    }

    void OnTriggerEnter(Collider col){
        var obj = col.gameObject.GetComponent<Projectile>(); 
        if(obj && obj.mode == 2){
            obj.setProjectileConfig(Vector3.zero, 0f, 2);
            ttlCounter = ttl;
            Destroy(col.gameObject, ttlCounter);
        }
    }

    void OnTriggerStay(Collider col){
        var obj = col.gameObject.GetComponent<Projectile>(); 
        if(obj && obj.mode == 2 && col.transform.position != transform.position){
            var dir = Vector3.Normalize(transform.position - col.gameObject.transform.position);
            col.transform.Translate(dir * Time.deltaTime * 20f);
        }
    }
}
