using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] GameObject[] redActive;
    [SerializeField] GameObject[] greenActive;
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
        DoSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DoSwitch()
    {
        if (redAct) {
            redLight.SetActive(true);
            for (int i = 0; i < redActive.Length; i++)
            {
                redActive[i].SetActive(true);
                
            }
        }
        if(!redAct)
        {
            redLight.SetActive(false);
            for (int i = 0; i < redActive.Length; i++)
            {
                redActive[i].SetActive(false);
                
            }
        }
        if (greenAct)
        {
            greenLight.SetActive(true);
            for (int i = 0; i < greenActive.Length; i++)
            {
                greenActive[i].SetActive(true);
                
            }
        }
        if (!greenAct)
        {
            greenLight.SetActive(false);
            for (int i = 0; i < greenActive.Length; i++)
            {
                greenActive[i].SetActive(false);
                
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
