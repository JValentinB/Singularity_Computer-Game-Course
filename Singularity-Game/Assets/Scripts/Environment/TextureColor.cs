using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureColor : MonoBehaviour
{
    [Header("This for changing the color of the Toon Material of a single object")]
    public Color baseColor = new Vector4(0.29f,0.3984f, 0.5f);
    public Color firstShadeColor = new Vector4(0.1642f,0.2236f, 0.283f);
    public Color secondShadeColor = new Vector4(0.0732f,0.0943f, 0.1132f);

    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_1st_ShadeColor", firstShadeColor);
        material.SetColor("_2nd_ShadeColor", secondShadeColor);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
