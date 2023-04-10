using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iron_Dude_Projectile : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private float speed;
    public bool freeze = false;
    private Vector3 dir;
    private int dmg;
    private AudioSource fireSound;

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
        WaitBeforeMovingAmount = 0.2f;
        waitBeforeAttack = true;
        counter = 0f;

        speed = 20f;
        dmg = 20;
        dir = new Vector3(0f, 0.5f, 0f);
        fireSound = GetComponent<AudioSource>();
        fireSound.Play();
    }

    void Update(){
        /*MovedUp();*/
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
            Vector3 piecePos = new Vector3(pos.x, pos.y, pos.z);
            GameObject pieceClone = Instantiate(explosion, piecePos, transform.rotation);
            Destroy(pieceClone, 5);
        }
    }

    /*private void MovedUp(){
        if(init){
            if(transform.position.y - startPosition.y < MoveUpAmount) return;

            init = false;
            setDir(GameObject.FindWithTag("Staffstone").transform.position);
            waitBeforeAttack = true;
        }
    }*/

    private void WaitCounter(){

        
        if (!waitBeforeAttack) return;
        setDir(GameObject.FindWithTag("Staffstone").transform.position);
        if (counter >= WaitBeforeMovingAmount) waitBeforeAttack = false;
        counter += Time.deltaTime;
    }

    private void Move(){
        if(waitBeforeAttack) return;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Rotate(1.5f, 1.5f, 1.5f, Space.Self);
    }

    private void OnTriggerEnter(Collider col){
        var obj = col.gameObject;
        if(obj.GetComponent<Damageable>() && !obj.GetComponent<Iron_Dude>()){
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            OnDeath();
        } else if(!col.isTrigger)
        {
            OnDeath();
        }
    }

    private void OnTriggerStay(Collider col){
        var obj = col.gameObject;
        if(obj.GetComponent<Damageable>() && !obj.GetComponent<Iron_Dude>()){
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            OnDeath();
        } else if(!col.isTrigger)
        {
            OnDeath();
        }
    }
}
