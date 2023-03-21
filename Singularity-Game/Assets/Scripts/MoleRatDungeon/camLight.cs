using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camLight : MonoBehaviour
{
    [SerializeField] private float offset = 30;
    [SerializeField] GameObject cam;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + offset);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + offset);
    }
}
