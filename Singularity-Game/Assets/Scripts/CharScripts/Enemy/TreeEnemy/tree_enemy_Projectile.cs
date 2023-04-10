using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree_enemy_Projectile : MonoBehaviour
{
    [SerializeField] private GameObject rockPiece;
    [SerializeField] private float speed;
    public bool freeze = false;
    private Vector3 dir;
    private int dmg;
    private AudioSource cast;

    //Move up, then move to player part
    private Vector3 startPosition;
    private bool init;
    private float MoveUpAmount;
    private float WaitBeforeMovingAmount;
    private bool waitBeforeAttack;
    private float counter;

    void Start(){
        startPosition = transform.position;
        init = true;
        MoveUpAmount = 8f;
        WaitBeforeMovingAmount = 0.5f;
        waitBeforeAttack = false;
        counter = 0f;

        this.speed = 15f;
        this.dmg = 15;
        this.dir = new Vector3(0f, 0.5f, 0f);
        cast = GetComponent<AudioSource>();
        cast.Play();
    }

    void Update(){
        MovedUp();
        WaitCounter();
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
            Vector3 piecePos = new Vector3(pos.x+Random.value, pos.y+Random.value, pos.z+Random.value);
            GameObject pieceClone = Instantiate(rockPiece, piecePos, transform.rotation);
            Destroy(pieceClone, 5);
        }
    }

    private void MovedUp(){
        if(init){
            if(transform.position.y - startPosition.y < MoveUpAmount) return;

            init = false;
            setDir(GameObject.FindWithTag("Staffstone").transform.position);
            waitBeforeAttack = true;
        }
    }

    private void WaitCounter(){
        if(!waitBeforeAttack) return;

        if(counter >= WaitBeforeMovingAmount) waitBeforeAttack = false;
        counter += Time.deltaTime;
    }

    private void Move(){
        if(freeze || waitBeforeAttack) return;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Rotate(1.5f, 1.5f, 1.5f, Space.Self);
    }

    private void OnTriggerEnter(Collider col){
        var obj = col.gameObject;
        if(obj.GetComponent<Damageable>()){
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            OnDeath();
        } else if(freeze && !obj.GetComponent<Projectile>()){
            OnDeath();
        } else if(obj.tag != "FOV" && obj.tag != "Shifter" && !obj.GetComponent<Projectile>())
            OnDeath();
    }
}
