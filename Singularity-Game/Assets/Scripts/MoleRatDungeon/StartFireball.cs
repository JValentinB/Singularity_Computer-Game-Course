using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFireball : MonoBehaviour
{
    [SerializeField] private GameObject fireball;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Instantiate(fireball, transform.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y + 90, transform.rotation.x));
            
        }
    }
}
