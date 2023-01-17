using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifter : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] public bulletMode mode;
    private ParticleSystem particle;
    ParticleSystem.MainModule _particle;
    public bool active = false;

    void Start(){
        particle = GetComponent<ParticleSystem>();
        _particle = particle.main;
    }

    void Update(){
        ChangeMode();
    }

    private void ChangeMode(){
        if(active){
            _particle.gravityModifier = -1.5f;
        } else if(!active){
            _particle.gravityModifier = 0.5f;
        }
    }

    //Function will be called on entering collider range
    private void OnTriggerEnter(Collider col)
    {
        var ObjectToShift = col.gameObject;
        if(ObjectToShift.GetComponent<Damageable>() && active){
            ObjectToShift.GetComponent<Damageable>().ShiftGravity(direction);
        }
    }
}
