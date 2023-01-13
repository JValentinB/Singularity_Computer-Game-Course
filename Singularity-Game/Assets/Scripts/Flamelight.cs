using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamelight : Flame
{
    [SerializeField] private GameObject Flame;
    Light lt;

    // Start is called before the first frame update
    void Start()
    {
        lt = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Flame.transform.position;
        //lt.range = getAmplitude() * 5;
    }
}
