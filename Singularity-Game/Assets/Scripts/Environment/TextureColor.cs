using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureColor : MonoBehaviour
{
    [Header("This for changing the color of the Toon Material of a single object")]
    public Color baseColor = new Vector4(0.29f, 0.3984f, 0.5f);
    public Color firstShadeColor = new Vector4(0.1642f, 0.2236f, 0.283f);
    public Color secondShadeColor = new Vector4(0.0732f, 0.0943f, 0.1132f);
    public Gradient colorGradient;

    private Material material;
    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_1st_ShadeColor", firstShadeColor);
        material.SetColor("_2nd_ShadeColor", secondShadeColor);
    }

    Color mixColors(Color color1, Color color2)
    {
        float r = (color1.r + color2.r) / 2f;
        float g = (color1.g + color2.g) / 2f;
        float b = (color1.b + color2.b) / 2f;

        return new Color(r, g, b, 1.0f); ;
    }

}
