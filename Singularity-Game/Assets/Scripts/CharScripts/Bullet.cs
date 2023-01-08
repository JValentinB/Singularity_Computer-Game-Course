using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 dir;
    private float speed;
    private int dmg;
    private bulletMode mode;

    public Projectile(Vector3 dir, float speed, int dmg, bulletMode mode){
        this.dir = dir;
        this.speed = speed;
        this.dmg = dmg;
        this.mode = mode;
    }

    void Update(){
        Move();
    }

    private void Move(){
        transform.Translate(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other){
        GameObject col = other.gameObject; 
        if(col.GetComponent<Damageable>()){
            col.GetComponent<Damageable>().ApplyDamage(dmg);
            Destroy(this);
        } else if(col.GetComponent<Shifter>()){
            if(col.GetComponent<Shifter>().mode == mode) col.GetComponent<Shifter>().active = true;
            Destroy(this);
        } else {
            Destroy(this);
        }
    }
}
