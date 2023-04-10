using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDeactivator : MonoBehaviour
{
    [SerializeField] private GameObject[] Traps;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Player>())
        {
            TrapsDeactivate();
        }

    }

    void TrapsDeactivate()
    {
        for (int i = 0; i < Traps.Length; i++)
        {
            Traps[i].SetActive(false);
        }
    }
}
