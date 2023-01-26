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
    // public Shader toonShader;

    // GradientColorKey[] colorKey;
    // GradientAlphaKey[] alphaKey;

    // Start is called before the first frame update
    void Start()
    {
        // colorGradient = new Gradient();

        // GradientColorKey[] colorKeys = new GradientColorKey[3];
        // colorKeys[0].color = new Vector4(0.4f, 0.6f, 0.8f);
        // colorKeys[0].time = 0f; // at the start of the gradient
        // colorKeys[1].color = new Vector4(0f, 0.2f, 0.4f);
        // colorKeys[1].time = 0.1f; // at 10% of the gradient
        // colorKeys[2].color = new Vector4(0.2f, 0.5f, 0.77f); ;
        // colorKeys[2].time = 1f; // at the end of the gradient
        // GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        // alphaKeys[0].alpha = 1f;
        // alphaKeys[0].time = 0f;
        // alphaKeys[1].alpha = 1f;
        // alphaKeys[1].time = 0.1f;
        // alphaKeys[2].alpha = 1f;
        // alphaKeys[2].time = 1f;
        // colorGradient.SetKeys(colorKeys, alphaKeys);

        material = GetComponent<Renderer>().material;
        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_1st_ShadeColor", firstShadeColor);
        material.SetColor("_2nd_ShadeColor", secondShadeColor);


        // float distance = (Mathf.Abs(transform.position.z - Camera.main.transform.position.z) - 50) / 300;
        // // distance = Mathf.Clamp(distance, startDistance, endDistance);

        // material = GetComponent<Renderer>().material;
        // material.SetColor("_BaseColor", mixColors(material.GetColor("_BaseColor"), colorGradient.Evaluate(distance)));
        // material.SetColor("_1st_ShadeColor", mixColors(material.GetColor("_1st_ShadeColor"), colorGradient.Evaluate(distance)));
        // material.SetColor("_2nd_ShadeColor", mixColors(material.GetColor("_2nd_ShadeColor"  ), colorGradient.Evaluate(distance)));

        // Renderer[] renderers = Resources.FindObjectsOfTypeAll<Renderer>();

        // // Iterate through each renderer
        // foreach (Renderer renderer in renderers)
        // {
        //     // Check if the renderer's material uses the Toon shader
        //     Debug.Log(renderer.name);
        //     if(renderer.gameObject.scene.name == null) continue;
        //     if (renderer.material.shader == toonShader)
        //     {
        //         float distance = (Mathf.Abs(renderer.transform.position.z - Camera.main.transform.position.z) - 50) / 300;
        //         // distance = Mathf.Clamp(distance, startDistance, endDistance);

        //         material = renderer.material;
        //         material.SetColor("_BaseColor", mixColors(material.GetColor("_BaseColor"), colorGradient.Evaluate(distance)));
        //         material.SetColor("_1st_ShadeColor", mixColors(material.GetColor("_1st_ShadeColor"), colorGradient.Evaluate(distance)));
        //         material.SetColor("_2nd_ShadeColor", mixColors(material.GetColor("_2nd_ShadeColor"), colorGradient.Evaluate(distance)));
        //     }
        // }
    }

    Color mixColors(Color color1, Color color2)
    {
        float r = (color1.r + color2.r) / 2f;
        float g = (color1.g + color2.g) / 2f;
        float b = (color1.b + color2.b) / 2f;

        return new Color(r, g, b, 1.0f); ;
    }

}
