using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderReset : MonoBehaviour
{
    [Header("Sets back the boulder to its starting position")]
    private GameObject boulder;
    // Start is called before the first frame update
    void Start()
    {
        boulder = GameObject.Find("Boulder");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == boulder)
            boulder.GetComponent<Boulder>().ResetBoulder();
    }
}
