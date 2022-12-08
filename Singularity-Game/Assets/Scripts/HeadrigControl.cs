using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadrigControl : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        HeadControl();
    }

    void HeadControl() 
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(position.x, position.y, 0);
    }
}
