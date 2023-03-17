using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifter : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] public int mode;
    [SerializeField] public float activeTime = 10f;
    private ParticleSystem ps;
    private ParticleSystem.MainModule _ps;
    public bool active = false;
    private float shifterTTL;

    void Start(){
        ps = GetComponent<ParticleSystem>();
        _ps = ps.main;
        _ps.startColor = new Color(1, 0.3322569f, 0f, 1f);
    }

    void Update(){
        ChangeMode();
        TTLCounter();
    }

    public void ToggleShifter(){
        active = !active;
        if(active) shifterTTL = activeTime;
    }

    private void ChangeMode(){
        if(direction != Vector3.down){
            if(active){
                _ps.gravityModifier = -1.5f;
            } else if(!active){
                _ps.gravityModifier = 0.5f;
            }
        }
        else {
            if(active){
                _ps.gravityModifier = 1.5f;
            } else if(!active){
                _ps.gravityModifier = -0.5f;
            }
        }
    }

    private void TTLCounter(){
        if(active && shifterTTL >= 0f){
            shifterTTL -= Time.deltaTime;
            return;
        }
        active = false;
    }

    //Function will be called on entering collider range
    private void OnTriggerEnter(Collider col)
    {
        var ObjectToShift = col.gameObject;
        if(ObjectToShift.GetComponent<Damageable>() && active){
            ObjectToShift.GetComponent<Damageable>().ShiftGravity(direction);

            if(ObjectToShift.GetComponent<Player>()){
                StartCoroutine(Camera.main.GetComponent<CameraControl>().turnCamera(direction, 0.75f));
            }
        }
    }

    private void OnTriggerExit(Collider col){
        var ObjectToShift = col.gameObject;
        if(ObjectToShift.GetComponent<Damageable>()){
            ObjectToShift.GetComponent<Damageable>().ShiftGravity(Vector3.down);
        }

        if(ObjectToShift.GetComponent<Player>()){
            StartCoroutine(Camera.main.GetComponent<CameraControl>().turnCamera(Vector3.down, 0.75f));
        }
    }

    private void OnTriggerStay(Collider col){
        var ObjectToShift = col.gameObject;
        if(ObjectToShift.GetComponent<Damageable>() && !active){
            ObjectToShift.GetComponent<Damageable>().ShiftGravity(Vector3.down);
        }

        if(ObjectToShift.GetComponent<Player>() && !active){
            StartCoroutine(Camera.main.GetComponent<CameraControl>().turnCamera(Vector3.down, 0.75f));
        }
    }
}
