using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleContainer : MonoBehaviour
{
    [SerializeField] private float ttl, ttlCounter;

    // Start is called before the first frame update
    void Start()
    {
        ttl = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        AttractProjectiles();
        DecreaseTTL();
    }

    private void AttractProjectiles(){
        if(ttlCounter <= 0f) return;

        var projectiles = GameObject.FindGameObjectsWithTag("m_Projectile");
        foreach(var projectile in projectiles){
            projectile.GetComponent<m_Projectile>().setDir(transform.position);
        }
    }

    private void DecreaseTTL(){
        if(ttlCounter > 0f) ttlCounter -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.GetComponent<Projectile>() /* && mode == 3 */){ //Enter Projectilemode for blackhole here
            ttlCounter = ttl;
        }
    }
}
