using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int healthPoints = 100;
    Color defaultColor;
    public bool tookDamage = false;
    float lastTimeHit;
    // Start is called before the first frame update
    void Start()
    {
        defaultColor = this.GetComponent<Renderer>().material.color;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastTimeHit > 0.5f)
        {
            tookDamage = false;
            this.GetComponent<Renderer>().material.color = defaultColor;
        }
        if (healthPoints <= 0)
        {
            this.GetComponent<MeshRenderer>().enabled = false;
        }
        if (tookDamage)
        {
            lastTimeHit = Time.time;
            this.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void takeDamage(int damage)
    {
        healthPoints -= damage;
    }
}
