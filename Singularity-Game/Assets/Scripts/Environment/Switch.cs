using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] GameObject[] redActive;
    [SerializeField] GameObject[] greenActive;
    [SerializeField] GameObject redLight;
    [SerializeField] GameObject greenLight;

    private bool redAct;
    private bool greenAct;
    private bool switchOnCooldown;

    private AudioSource switcher;
    // Start is called before the first frame update
    void Start()
    {
        redAct = true;
        redLight.SetActive(true);
        greenAct = false;
        greenLight.SetActive(false);
        DoSwitch();
        switcher = GetComponent<AudioSource>();
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
        if (col.GetComponent<Projectile>() && !switchOnCooldown)
        {
            switchOnCooldown = true;
            switcher.Play();
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
        StartCoroutine(SwitchCooldown());
    }

    private IEnumerator SwitchCooldown(){
        yield return new WaitForSeconds(0.3f);
        switchOnCooldown = false;
    }
}
