using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifter : MonoBehaviour
{
    enum bulletMode{
        Blue,
        Green,
        Red
    }

    [SerializeField] private Vector3 direction;
    private Vector3 previousDir;
    private bool active = false;
    [SerializeField] private bulletMode mode;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Function will be called on entering collider range
    private void OnTriggerEnter(Collider col)
    {
        var damagableObjectToShift = col.gameObject.GetComponent<Damageable>();
        if(damagableObjectToShift && !active){
            previousDir = damagableObjectToShift.gravitationalDirection;
            damagableObjectToShift.ShiftGravity(direction);
        }/* else if(col.gameObject.GetComponent<Bullet>().mode == mode){
            active = true;
            Destroy(col.gameObject);
        } */
    }

    //Function will be called on leaving collider range
    /* private void OnTriggerExit(Collider col)
    {
        var damagableObjectToShift = col.gameObject.GetComponent<Damageable>();
        if(damagableObjectToShift && !active){
            damagableObjectToShift.ShiftGravity(previousDir);
        }
    } */
}
