using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSwitch : MonoBehaviour
{
    [SerializeField] GameObject elevator;
    [SerializeField] GameObject redLight;
    [SerializeField] GameObject greenLight;

    private bool redAct;
    private bool greenAct;
    // Start is called before the first frame update
    void Start()
    {
        redAct = true;
        redLight.SetActive(true);
        greenAct = false;
        greenLight.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DoSwitch()
    {
        if (elevator.GetComponent<Elevator>()) 
        {
            elevator.GetComponent<Elevator>().setActive();
            if (redAct)
            {
                redLight.SetActive(true);
            }
            if (!redAct)
            {
                redLight.SetActive(false);
            }
            if (greenAct)
            {
                greenLight.SetActive(true);
            }
            if (!greenAct)
            {
                greenLight.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Projectile>())
        {
            if (redAct)
            {
                redAct = false;
            }else if (!redAct)
            {
                redAct = true;
            }
            if (greenAct)
            {
                greenAct = false;
            }
            else if (!greenAct)
            {
                greenAct = true;
            }

            DoSwitch();
        }

    }
}
