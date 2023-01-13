using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamelight : MonoBehaviour
{
    [SerializeField] private GameObject Flame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Flame.transform.position;
    }
}
