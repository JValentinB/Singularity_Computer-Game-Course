using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 dir;
    private int dmg;
    [SerializeField] private bulletMode mode;

    void Update(){
        Move();
    }

    public void setProjectileConfig(Vector3 dir, float speed, int dmg, bulletMode mode){
        this.dir = Vector3.Normalize(dir);
        this.speed = speed;
        this.dmg = dmg;
        this.mode = mode;
    }

    private void Move(){
        transform.Translate(dir * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void OnTriggerEnter(Collider other){
        var col = other.gameObject; 
        if(col.GetComponent<Damageable>() && col.tag != "Player"){
            col.GetComponent<Damageable>().ApplyDamage(dmg);
            Destroy(gameObject);
        } else if(col.GetComponent<Shifter>()){
            /* if(col.GetComponent<Shifter>().mode == mode) */ 
            col.GetComponent<Shifter>().active = true;
            Destroy(gameObject);
        } else if(col.tag != "Player"){
            Destroy(gameObject);
        }
        Debug.Log("test");
    }
}
