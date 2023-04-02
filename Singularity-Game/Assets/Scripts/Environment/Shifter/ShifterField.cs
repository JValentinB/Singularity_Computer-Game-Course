using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShifterField : MonoBehaviour
{
    public bool active;
    private Vector3 direction;
    private ParticleSystem ps;
    private ParticleSystem.MainModule _psMain;
    private ParticleSystem.ShapeModule _psShape;
    [SerializeField] private Vector3 activePos;
    [SerializeField] private Vector3 inactivePos;

    void Start(){
        direction = transform.parent.gameObject.GetComponent<Shifter>().direction;
        ps = GetComponent<ParticleSystem>();
        _psMain = ps.main;
        _psShape = ps.shape;
        _psMain.startColor = new Color(1, 0.3322569f, 0f, 1f);
    }

    void Update(){
        ChangeMode();
    }

    private void ChangeMode(){
        if(direction != Vector3.up){
            if(active){
                _psMain.gravityModifier = -1.5f;
            } else if(!active){
                _psMain.gravityModifier = 0.5f;
            }
        }
        else {
            if(active){
                _psShape.position = activePos;
                _psMain.gravityModifier = -0.2f;
            } else if(!active){
                _psShape.position = inactivePos;
                _psMain.gravityModifier = 1f;
            }
        }
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
        if(ObjectToShift.GetComponent<Damageable>()){
            if(ObjectToShift.GetComponent<Damageable>().gravitationalDirection == direction && !active){
                ObjectToShift.GetComponent<Damageable>().ShiftGravity(Vector3.down);
                StartCoroutine(Camera.main.GetComponent<CameraControl>().turnCamera(Vector3.down, 0.75f));
            } else if(ObjectToShift.GetComponent<Damageable>().gravitationalDirection != direction && active){
                ObjectToShift.GetComponent<Damageable>().ShiftGravity(direction);
                StartCoroutine(Camera.main.GetComponent<CameraControl>().turnCamera(direction, 0.75f));
            }
        }
    }
}
