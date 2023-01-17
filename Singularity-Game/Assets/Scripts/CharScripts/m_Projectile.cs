using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class m_Projectile : MonoBehaviour
{
    [SerializeField] private GameObject rockPiece;
    [SerializeField] private float speed;
    public bool freeze = false;
    private Vector3 dir;
    private int dmg;

    void Start(){
        this.speed = 15f;
        this.dmg = 100;
        this.dir = new Vector3(0f, -0.1f, 0f);
    }

    void Update(){
        Move();
    }

    public void setDir(Vector3 dir){
        this.dir = Vector3.Normalize(dir - transform.position);
        this.dir.z = 0;
        freeze = false;
    }
    
    public void OnDeath(){
        createPieces();
        Destroy(gameObject);
    }

    private void createPieces(){
        Vector3 pos = transform.position;
        for(int i = 0; i < 5; i++){
            Vector3 piecePos = new Vector3(pos.x+(Random.value%10)/10, pos.y+(Random.value%10)/10, pos.z+(Random.value%10)/10);
            GameObject pieceClone = Instantiate(rockPiece, piecePos, transform.rotation);
            Destroy(pieceClone, 5);
        }
    }

    private void Move(){
        if(freeze) return;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Rotate(1.5f, 1.5f, 1.5f, Space.Self);
    }

    private void OnTriggerEnter(Collider other){
        var col = other.gameObject;
        if(col.GetComponent<Damageable>()){
            col.GetComponent<Damageable>().ApplyDamage(dmg);
            OnDeath();
        } else if(freeze && !col.GetComponent<Projectile>()){
            OnDeath();
        }
    }
}
