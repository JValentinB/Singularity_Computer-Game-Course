using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 dir;
    private int dmg;
    [SerializeField] public bulletMode mode;
    public bool test;

    void Update(){
        Move();
    }

    public void setProjectileConfig(Vector3 dir, float speed, int dmg, bulletMode mode){
        this.dir = Vector3.Normalize(dir);
        this.speed = speed;
        this.dmg = dmg;
        this.mode = mode;
    }

    private void mProjCollision(GameObject obj){
        var m_Proj = obj.GetComponent<m_Projectile>();
        if(!m_Proj.freeze){
            m_Proj.freeze = true;
            GameObject.FindWithTag("Player").GetComponent<Player>().setDirectionShot = true;
        } else if(m_Proj.freeze){ 
            m_Proj.OnDeath();
        }
    }

    private void controlShot(){
        var m_Proj = GameObject.FindGameObjectsWithTag("m_Projectile");
        foreach(var mProjObject in m_Proj){
            if(mProjObject.GetComponent<m_Projectile>().freeze){
                mProjObject.GetComponent<m_Projectile>().setDir(transform.position);
            }
        }
    }

    private void Move(){
        transform.Translate(dir * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void OnTriggerEnter(Collider col){
        var obj = col.gameObject;
        if(obj.GetComponent<m_Projectile>()){
            mProjCollision(obj);
            Destroy(gameObject);
        } else if(mode == bulletMode.Control && obj.tag != "Player"){
            controlShot();
            Destroy(gameObject);
        } else if(obj.GetComponent<Damageable>() && obj.tag != "Player"){
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            Destroy(gameObject);
        } else if(obj.tag == "Shifter"){
            obj.GetComponent<Shifter>().active = true;
            Destroy(gameObject);
        } else if(obj.tag != "Player"){
            Destroy(gameObject);
        }
    }
}
