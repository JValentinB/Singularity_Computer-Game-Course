using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombFruit : MonoBehaviour
{
    [SerializeField] private GameObject explosionObject;
    [SerializeField] private float speed;
    private int dmg;
    private Vector3 target = Vector3.down;

    void Start(){
        speed = 15f;
        dmg = 50;
    }

    void Update(){
        Move();
    }

    public void setDir(Vector3 target){
        this.target = Vector3.Normalize(target - transform.position);
        this.target.z = 0;
    }
    
    public void OnDeath(){
        var explosion = Instantiate(explosionObject, transform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject);
    }

    private void Move(){
        transform.Translate(target * speed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Rotate(1.5f, 1.5f, 1.5f, Space.Self);
    }

    private void OnTriggerEnter(Collider col){
        var obj = col.gameObject;
        if(obj.GetComponent<BlackHoleContainer>()){
            obj.GetComponent<TreeBoss>().ApplyDamage(dmg);
            OnDeath();
        } else if(obj.GetComponent<Damageable>()){
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            OnDeath();
        }
    }
}
