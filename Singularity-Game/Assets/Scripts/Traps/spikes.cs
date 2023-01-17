using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikes : MonoBehaviour
{
    [SerializeField] private float speed = 4;
    [SerializeField] private float range = 2;
    [SerializeField] private float stopTimer  = 1.5f;

    private float count = 0;
    private float way = 0;
    private bool backward = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Fall 1/4
        if (!backward && count < stopTimer)
        {
            wait();
        }

        // Fall 2/4
        if (!backward && count >= stopTimer)
        {
            shootUp();
        }

        // Fall 3/4
        if (backward && count > 0)
        {
            wait();
        }

        // Fall 4/4
        if (backward && count <= 0)
        {
            getBack();
        }
    }

    void shootUp()
    {
        if(way < range)
        {
            float y = speed * Time.deltaTime;
            float distanceToGo = range - way;
            
            if(distanceToGo < y)
            {
                y = distanceToGo;
            }
            way += y;
            transform.Translate(0, y, 0, Space.World);
            if(way >= range)
            {
                backward = true;
            }
        }
    }

    void getBack()
    {
        if (way > 0)
        {
            float y = speed * Time.deltaTime;

            if (way < y)
            {
                y = way;
            }
            way -= y;
            transform.Translate(0, -y, 0, Space.World);
            if (way <= 0)
            {
                backward = false;
            }
        }
    }

    void wait()
    {
        if (!backward)
        {
            count += Time.deltaTime;
            if (count > stopTimer)
            {
                count = stopTimer;
            }
        }
        if (backward)
        {
            count -= Time.deltaTime;
            if (count < 0)
            {
                count = 0;
            }
        }
    }
  
}
