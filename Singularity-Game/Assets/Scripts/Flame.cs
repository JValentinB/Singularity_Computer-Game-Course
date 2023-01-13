using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [SerializeField] private float amplitude = 1;
    private float x;
    private float y;
    private float z;

    // Start is called before the first frame update
    void Start()
    {
        x = transform.localScale.x;
        y = transform.localScale.y;
        z = transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
         transform.localScale = new Vector3(x * amplitude, y * amplitude, z * amplitude);
    }

    public float getAmplitude()
    {
        return amplitude;
    }
}
