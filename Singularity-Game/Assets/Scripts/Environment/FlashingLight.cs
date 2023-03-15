using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    public float flashingSpeed = 1f;

    private Light flashLight;
    private float maxIntensity;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        flashLight = GetComponent<Light>();
        maxIntensity = flashLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        flashLight.intensity = Mathf.Cos(time * flashingSpeed) * maxIntensity;
    }
}
