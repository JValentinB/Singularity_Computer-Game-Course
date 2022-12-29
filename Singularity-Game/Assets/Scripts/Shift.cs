using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shift : MonoBehaviour
{
    [SerializeField] private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Function will be called on leaving collider range
    private void OnTriggerEnter(Collider col)
    {
        var damagbleObjectToShift = col.gameObject.GetComponent<Damageable>();
        if(damagbleObjectToShift){
            damagbleObjectToShift.ShiftGravity(direction);
        }
    }
}
