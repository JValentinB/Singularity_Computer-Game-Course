using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.MainModule _ps;
    [SerializeField] private float speed;
    private Vector3 dir;
    private int dmg;
    [SerializeField] public int mode;

    void Start(){
        ps = GetComponent<ParticleSystem>();
        _ps = ps.main;
        ChangeColor();
        // Debug.Log(ps.startColor);
    }

    void Update(){
        Move();
    }

    public void setProjectileConfig(Vector3 dir, float speed, int mode){
        this.dir = Vector3.Normalize(dir);
        this.speed = speed;
        this.mode = mode;
    }

    private void ChangeColor(){
        switch (mode){
            case 0:
                _ps.startColor = new Color(0.5447297f, 0f, 1f, 1f);
                dmg = 20;
                break;
            case 1:
                _ps.startColor = new Color(1, 0.3322569f, 0f, 1f);
                dmg = 20;
                break;
            case 2:
                _ps.startColor = new Color(1f, 1f, 1f, 1f);
                break;
        }
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
        if(obj.GetComponent<m_Projectile>() && mode == 1){
            mProjCollision(obj);
            Destroy(gameObject);
        } else if(mode == 2 && obj.tag != "Player"){
            controlShot();
            Destroy(gameObject);
        } else if(obj.GetComponent<Damageable>() && obj.tag != "Player" && obj.tag != "FOV"){
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            Destroy(gameObject);
        } else if(obj.tag == "Shifter"){
            if(obj.GetComponent<Shifter>().mode == mode) obj.GetComponent<Shifter>().active = true;
            Destroy(gameObject);
        } else if(obj.tag != "Player" && obj.tag != "FOV"){
            Destroy(gameObject);
        }
    }
}
