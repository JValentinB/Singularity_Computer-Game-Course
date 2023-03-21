using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShot : MonoBehaviour
{
    [SerializeField] private float laserSpeed;
    void Start()
    {
        laserSpeed = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move(){
        transform.Translate(Vector3.up * laserSpeed * Time.deltaTime);
    }
}
