using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifter : MonoBehaviour
{
    [SerializeField] public Vector3 direction;
    [SerializeField] public float activeTime = 10f;
    [SerializeField] public bool noActiveTimer;
    private GameObject shifterField;
    private ShifterField shifterScript;
    private float shifterTTL;

    void Start(){
        shifterField = transform.GetChild(0).gameObject;
        shifterScript = shifterField.GetComponent<ShifterField>();
        shifterScript.active = noActiveTimer;
    }

    void Update(){
        TTLCounter();
    }

    public void ToggleShifter(){
        if(noActiveTimer) return;

        shifterScript.active = !shifterScript.active;
        if(shifterScript.active) shifterTTL = activeTime;
    }

    private void TTLCounter(){
        if(noActiveTimer) return;

        if(shifterScript.active && shifterTTL >= 0f){
            shifterTTL -= Time.deltaTime;
            return;
        }
        shifterScript.active = false;
    }
}
