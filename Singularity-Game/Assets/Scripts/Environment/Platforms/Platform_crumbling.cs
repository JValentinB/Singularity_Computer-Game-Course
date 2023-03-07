using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_crumbling : Platform
{   
    [SerializeField] private GameObject rockPiece;
    [SerializeField] private AudioSource audio_rockBreak;
    [SerializeField] private AudioSource audio_crumbling;
    [SerializeField] private float ttl; //Time to live until platform crumbles together
    [SerializeField] private float ttlCounter = 0f;
    private ParticleSystem ps;

    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
        timer = 0f;

        if (waypoints.Count != 0)
            transform.position = waypoints[0];

        //Making sure it doesn't get pushed away by the player if there are no waypoints
        if(waypoints.Count == 0) rb.isKinematic = true;
        else rb.isKinematic = false;
    }

    private void OnDeath(){
        createPieces();
        audio_crumbling.Stop();
        audio_rockBreak.Play();
        Destroy(gameObject);
    }

    private void Crumbling(bool activate){
        if(ps.isPlaying == activate) return;

        if(activate){
            audio_crumbling.Play();
            ps.Play(false);
        } else {
            audio_crumbling.Stop();
            ps.Stop(true);
        }
    }

    private void createPieces(){
        Vector3 pos = transform.position;
        for(int i = 0; i < 10; i++){
            Vector3 piecePos = new Vector3(pos.x+(Random.value*2), pos.y+(Random.value*2), pos.z+(Random.value*2));
            GameObject pieceClone = Instantiate(rockPiece, piecePos, transform.rotation);
            Destroy(pieceClone, 5);
        }
    }

    void OnTriggerStay(Collider col){
        if(col.GetComponent<Damageable>()){
            Crumbling(true);
            if(ttlCounter >= ttl) OnDeath();

            ttlCounter += Time.deltaTime;
            // Debug.Log(ttlCounter);
        }
    }

    void OnTriggerExit(Collider col){
        if(col.GetComponent<Damageable>()){
            Crumbling(false);
            ttlCounter = 0f;
        }
    }
}
