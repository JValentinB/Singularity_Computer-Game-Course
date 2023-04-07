using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopper : MonoBehaviour
{
    [SerializeField] private GameObject counterpart;

    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        counterpart.GetComponent<Stopper>();
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (active)
        {
            var obj = col.gameObject;
            if (obj.GetComponent<Elevator>())
            {
                obj.GetComponent<Elevator>().setNotActive();

                if (obj.GetComponent<Elevator>().getUp())
                {
                    obj.GetComponent<Elevator>().setDown();
                }else 
                if (obj.GetComponent<Elevator>().getDown())
                {
                    obj.GetComponent<Elevator>().setUp();
                }

                counterpart.GetComponent<Stopper>().setActive();
                setNotActive();
            }
        }
    }

    public void setActive()
    {
        active = true;
    }

    public void setNotActive()
    {
        active = false;
    }
}
