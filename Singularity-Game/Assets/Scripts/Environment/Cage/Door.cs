using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool interaction = false;
    bool closed = true;
    bool open = false;

    private float openingSpeed = 45;
    private float range = 90.0f;
    private float counter;
    private float move;
    // Start is called before the first frame update
    void Start()
    {
       /* if (closed)
        {
            open = false;
            transform.rotation = Quaternion.Euler(0, 0, 0,);
            counter = 0;
        }
        if (open)
        {
            closed = false;
            transform.rotation = Quaternion.Euler(0, range, 0,);
            counter = range;
        } */
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interaction = true;
        }

        if (closed && interaction)
        {
            opening();
        }
        if (open && interaction)
        {
            close();
        }
        
    }

    void opening()
    {
        move = openingSpeed * Time.deltaTime;
        transform.Rotate(0, move, 0);
        counter += move;
        if(counter >= range)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            closed = false;
            open = true;
            interaction = false;
            counter = range;
        }
    }
    void close()
    {
        move = -openingSpeed * Time.deltaTime;
        transform.Rotate(0, move, 0);
        counter += move;
        if (counter <= 0)
        {
            
            transform.rotation = Quaternion.Euler(0, range, 0);
            closed = true;
            open = false;
            interaction = false;
            counter = 0;
        }
    }

    public bool getOpen()
    {
        return open;
    }
    public bool getClosed()
    {
        return closed; 
    }
    public bool getInteraction()
    {
        return interaction;
    }
}
