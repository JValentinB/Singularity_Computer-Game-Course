using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int healthPoints = 100;
    private int xpPoints = 10;
    Color defaultColor;
    public bool tookDamage = false;
    float lastTimeHit;
    // Start is called before the first frame update
    void Start()
    {
        defaultColor = this.GetComponent<Renderer>().material.color;
        lastTimeHit = float.PositiveInfinity;
    }

    // Update is called once per frame
    void Update()
    {
        // color enemy red when hit
        if (Time.time - lastTimeHit > 0.2f && tookDamage)
        {
            tookDamage = false;
            lastTimeHit = float.PositiveInfinity;
            this.GetComponent<Renderer>().material.color = defaultColor;
        }
        if (tookDamage && lastTimeHit == float.PositiveInfinity)
        {
            lastTimeHit = Time.time;
            this.GetComponent<Renderer>().material.color = Color.red;
        }


        if (healthPoints <= 0  && Time.time - lastTimeHit > 0.1f)
        {
            PlayerControl player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
            player.giveXp(xpPoints);
            this.GetComponent<MeshRenderer>().enabled = false;
            DestroyNPC();
        }
    }

    private void DestroyNPC(){
        gameObject.SetActive(false);
    }

    public void takeDamage(int damage)
    {
        healthPoints -= damage;
    }
}
