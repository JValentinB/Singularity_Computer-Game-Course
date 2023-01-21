using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For changing the light color in designated area
// can be used to change the camera fov
public class LightAreas : MonoBehaviour
{
    public float transitionTime = 2f; 
    public bool color = true;
    public Color areaColor;
    public bool fov = false;
    public float areaFov;

    private Light mainLight;
    private Camera mainCamera;
    private Color mainColor;
    private float mainFov;
    private Coroutine colorTransition;
    private Coroutine fovTransition;
    private int boxesEntered; // number of box colliders of the same object the player is in

    // Start is called before the first frame update
    void Start()
    {
        mainLight = GameObject.Find("Main Light").GetComponent<Light>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        mainColor = mainLight.color;
        mainFov = mainCamera.fieldOfView;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boxesEntered++;
            if(colorTransition != null)
                StopCoroutine(colorTransition);
            if(fovTransition != null)
                StopCoroutine(fovTransition);    
            if(color)
                colorTransition = StartCoroutine(TransitionLight(mainLight.color, areaColor));
            if(fov)
                fovTransition = StartCoroutine(TransitionFov(mainCamera.fieldOfView, areaFov));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boxesEntered--;
            if(boxesEntered <= 0){
                if(colorTransition != null)
                    StopCoroutine(colorTransition);
                if(fovTransition != null)
                    StopCoroutine(fovTransition); 
                if(color)
                    colorTransition = StartCoroutine(TransitionLight(mainLight.color, mainColor));
                if(fov)
                    fovTransition = StartCoroutine(TransitionFov(mainCamera.fieldOfView, mainFov));
            }
        }
    }

    IEnumerator TransitionLight(Color currentColor, Color targetColor)
    {
        float t = 0f;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            mainLight.color = Color.Lerp(currentColor, targetColor, t / transitionTime);
            yield return null;
        }
    }
    IEnumerator TransitionFov(float currentFov, float targetFov)
    {
        float t = 0f;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            mainCamera.fieldOfView = Mathf.Lerp(currentFov, targetFov, t / transitionTime);
            yield return null;
        }
    }
}
