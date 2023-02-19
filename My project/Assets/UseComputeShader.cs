using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseComputeShader : MonoBehaviour
{
    public ComputeShader shader;
    public RenderTexture renderTexture;

    public float Zoom = 1.0f;
    public Vector2 Offset = Vector2.zero;
    public int MAX_ITER = 100;

    // Start is called before the first frame update
    void Start()
    {
        // Set the parameters for the Compute Shader
        shader.SetFloat("Zoom", Zoom);
        shader.SetVector("Offset", Offset);
        shader.SetInt("MAX_ITER", MAX_ITER);

        renderTexture = new RenderTexture(512, 512, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Bind the output Render Texture
        int kernelIndex = shader.FindKernel("CSMain");
        Debug.Log(kernelIndex);
        shader.SetTexture(kernelIndex, "Out", renderTexture);

        // Dispatch the Compute Shader
        int width = renderTexture.width;
        int height = renderTexture.height;
        shader.Dispatch(kernelIndex, width / 8, height / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
