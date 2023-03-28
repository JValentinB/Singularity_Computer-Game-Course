using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.MainModule _ps;
    [SerializeField] private float projectileSpeed;
    private Vector3 dir;
    private int dmg;
    [SerializeField] public int mode;
    private bool foundhit = false;
    private Vector3 stop_pos;
    [SerializeField] private List<string> ignoreCollisionWithTag = new List<string>(){
        "Player",
        "FOV",
        "TreeBoss",
        "IgnoreCollision"
    };

    void Start(){
        ps = GetComponent<ParticleSystem>();
        _ps = ps.main;
        ChangeColor();
        if (mode == 2) { findcollision(); }
    }

    void Update(){
        if(mode != 2)
        {
            Move();
        }
        else
        {
            if (foundhit && (stop_pos - transform.position).magnitude < 1) { }
            else Move();
        }
    }

    public void setProjectileConfig(Vector3 dir, float speed, int mode){
        this.dir = Vector3.Normalize(dir);
        this.projectileSpeed = speed;
        this.mode = mode;
        if(mode == 2)
        {
            GetComponent<SphereCollider>().radius = 30f;
        }
        else
        {
            GetComponent<SphereCollider>().radius = 1.25f;
        }
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
                _ps.startColor = new Color(0f, 0f, 0f, 1f);
                break;
            case 3:
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
        transform.Translate(dir * projectileSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    //sets stop-position if raycast hit
    private void findcollision()
    {
        if (mode != 2) return;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;
        LayerMask hitLayer = LayerMask.NameToLayer("Default");
        int layerMask = (1 << hitLayer);
        if(Physics.Raycast(ray, out hit, 60, layerMask))
        {
            stop_pos = hit.point;
            Debug.Log(stop_pos);
            foundhit = true;
            
        }
    }

    private void OnTriggerEnter(Collider col){
        
        if (mode == 2) {
            return;
        }
        var obj = col.gameObject;
        if (obj.GetComponent<m_Projectile>() && mode == 1){
            mProjCollision(obj);
            Destroy(gameObject);
        } else if(mode == 3 && obj.tag != "Player"){
            controlShot();
            Destroy(gameObject);
        } else if(obj.GetComponent<Damageable>() && obj.tag != "Player" && obj.tag != "FOV"){
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            Destroy(gameObject);
        } else if(obj.tag == "Shifter"){
            obj.GetComponent<Shifter>().ToggleShifter();
            Destroy(gameObject);
        }  else if(!ignoreCollisionWithTag.Contains(obj.tag) && !obj.GetComponent<ShifterField>()){
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider col)
    {
        var obj = col.gameObject;
        Vector3 projectile_pos = GetComponent<Transform>().position;
        bool has_damageable = obj.GetComponent<Damageable>() != null;
        if (mode == 2 && obj.tag != "Player" && has_damageable)
        {
            var obj_rb = obj.GetComponent<Rigidbody>();
            Vector3 obj_pos = obj.GetComponent<Transform>().position;

            obj_rb.AddForce((projectile_pos - obj_pos) * 81f, ForceMode.Acceleration);
        }
    }
}
