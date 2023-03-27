using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBridge : MonoBehaviour
{
    [SerializeField] public bool destroyBridge;
    private float ttl = 1f;

    // Update is called once per frame
    void Update()
    {
        if(!destroyBridge) return;
        transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("recede", true);
        transform.GetChild(1).gameObject.GetComponent<Animator>().SetBool("recede", true);
        if(ttl <= 0f) Destroy(gameObject);
        ttl -= Time.deltaTime;
    }
}
