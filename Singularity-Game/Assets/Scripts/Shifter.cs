using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum bulletMode{
    Red,
    Green,
    Blue
}

public class Shifter : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    public bool active = false;
    [SerializeField] public bulletMode mode;

    //Function will be called on entering collider range
    private void OnTriggerEnter(Collider col)
    {
        var damagableObjectToShift = col.gameObject.GetComponent<Damageable>();
        if(damagableObjectToShift && !active){
            damagableObjectToShift.ShiftGravity(direction);
        }
    }
}
