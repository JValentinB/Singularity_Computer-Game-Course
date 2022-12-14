using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifter : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    public bool active = false;
    [SerializeField] public bulletMode mode;

    //Function will be called on entering collider range
    private void OnTriggerEnter(Collider col)
    {
        var ObjectToShift = col.gameObject;
        if(ObjectToShift.GetComponent<Damageable>() && active){
            ObjectToShift.GetComponent<Damageable>().ShiftGravity(direction);
        }
    }
}
