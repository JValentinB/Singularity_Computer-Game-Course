using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifter : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] public int mode;
    private ParticleSystem ps;
    private ParticleSystem.MainModule _ps;
    public bool active = false;

    void Start(){
        ps = GetComponent<ParticleSystem>();
        _ps = ps.main;
        _ps.startColor = new Color(1, 0.3322569f, 0f, 1f);
    }

    void Update(){
        ChangeMode();
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
}
