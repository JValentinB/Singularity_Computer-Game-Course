using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.MainModule _ps;
    [SerializeField] private float projectileSpeed;
    private Vector3 dir;
    private int dmg;
    [SerializeField] public int mode;

    void Start(){
        ps = GetComponent<ParticleSystem>();
        _ps = ps.main;
        ChangeColor();
    }

    void Update(){
        Move();
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

    private void OnTriggerEnter(Collider col){
        if (mode == 2)
        {
            return;
        }
        var obj = col.gameObject;
        if(obj.GetComponent<m_Projectile>() && mode == 1){
            mProjCollision(obj);
            Destroy(gameObject);
        } else if(mode == 3 && obj.tag != "Player"){
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

    private void OnTriggerStay(Collider col)
    {
        var obj = col.gameObject;
        if (mode == 2 && obj.tag != "Player" && obj.tag != "Untagged")
        {
            var obj_rb = obj.GetComponent<Rigidbody>();
            Vector3 obj_pos = obj.GetComponent<Transform>().position;
            Vector3 projectile_pos = GetComponent<Transform>().position;

            obj_rb.AddForce((projectile_pos - obj_pos) * 81f, ForceMode.Acceleration);
        }

    }
}
