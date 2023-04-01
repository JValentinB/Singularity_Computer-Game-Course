using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombFruit : MonoBehaviour
{
    [SerializeField] private GameObject fruitPiece;
    [SerializeField] private GameObject explosionObject;
    [SerializeField] private float speed;
    private Vector3 target;
    private int dmg;
    public bool released;

    void Start(){
        speed = 15f;
        dmg = 50;
        target = Vector3.down;
    }

    void Update(){
        Move();
    }

    public void setDir(Vector3 target){
        this.target = Vector3.Normalize(target - transform.position);
        this.target.z = 0;
    }
    
    public void OnDeath(){
        //createPieces();
        Instantiate(explosionObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void createPieces(){
        Vector3 pos = transform.position;
        for(int i = 0; i < 5; i++){
            Vector3 piecePos = new Vector3(pos.x+Random.value, pos.y+Random.value, pos.z+Random.value);
            GameObject pieceClone = Instantiate(fruitPiece, piecePos, transform.rotation);
            Destroy(pieceClone, 5);
        }
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
        } else if(obj.GetComponent<Player>()){
            obj.GetComponent<TreeBoss>().ApplyDamage(dmg);
            OnDeath();
        }
    }
}
