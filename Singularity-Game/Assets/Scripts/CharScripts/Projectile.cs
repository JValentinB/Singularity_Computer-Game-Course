using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 dir;
    private float speed;
    private int dmg;
    private bulletMode mode;

    void Update(){
        Move();
    }

    public void setProjectileConfig(Vector3 dir, float speed, int dmg, bulletMode mode){
        transform.LookAt(dir);
        this.dir = Vector3.Normalize(dir);
        this.speed = speed;
        this.dmg = dmg;
        this.mode = mode;

    }

    private void Move(){
        transform.Translate(dir * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    /* void OnTriggerEnter(Collider other){
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
    } */
}
