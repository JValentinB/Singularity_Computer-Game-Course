using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// For changing the light color in designated area
// can be used to change the camera fov
public class LightAreas : MonoBehaviour
{
    public float transitionTime = 2f;

    [Header("")]
    public bool color = true;
    public bool backToMainColor = true;
    public Color areaColor;
    public bool intensity = false;
    public float areaIntensity = 1.58f;
    [Header("")]
    public bool fov = false;
    public bool backToMainFov = true;
    public float areaFov;
    [Header("")]
    public bool vignette = false;
    public bool backToMainVignette = true;
    public float vignetteIntensity = 0.4f;
    [Header("")]
    public bool music = false;
    public string musicName;
    [Range(0f, 1f)]
    public float maxMusicVolume = 0.25f;


    private Light mainLight;
    private Camera mainCamera;
    private Color mainColor;
    private float mainIntensity;
    private float mainFov;
    private VolumeProfile volumeProfile;
    private Vignette vignetteLayer;
    private AudioManager audioManager;

    private Coroutine colorTransition;
    private Coroutine intensityTransition;
    private Coroutine fovTransition;
    private Coroutine vignetteTransition;
    private Coroutine musicTransition;
    private int boxesEntered; // number of box colliders of the same object the player is in

    // Start is called before the first frame update
    void Start()
    {
        mainLight = GameObject.Find("Main Light").GetComponent<Light>();
        mainCamera = Camera.main;
        mainColor = mainLight.color;
        mainIntensity = mainLight.intensity;
        mainFov = mainCamera.fieldOfView;
        volumeProfile = mainCamera.GetComponent<Volume>().profile;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        // vignetteLayer = volumeProfile.TryGet<Vignette>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boxesEntered++;
            if (colorTransition != null)
                StopCoroutine(colorTransition);
            if(intensityTransition != null)
                StopCoroutine(intensityTransition);
            if (fovTransition != null)
                StopCoroutine(fovTransition);
            if (vignetteTransition != null)
                StopCoroutine(vignetteTransition);
            if(musicTransition != null)
                StopCoroutine(musicTransition);
            
            // Debug.Log(audioManager.getSourceVolume(audioManager.music, musicName));
            if (color)
                colorTransition = StartCoroutine(TransitionLight(mainLight.color, areaColor));
            if(intensity)
                intensityTransition = StartCoroutine(TransitionIntensity(mainLight.intensity, areaIntensity));
            if (fov)
                fovTransition = StartCoroutine(TransitionFov(mainCamera.fieldOfView, areaFov));
            Vignette mainVignette;
            if (vignette && volumeProfile.TryGet<Vignette>(out mainVignette))
                vignetteTransition = StartCoroutine(TransitionVignette(mainVignette, mainVignette.intensity.value, vignetteIntensity));
            if (music)
                musicTransition = StartCoroutine(TransitionMusic(musicName, audioManager.getSourceVolume(audioManager.music, musicName), maxMusicVolume));            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boxesEntered--;
            if (boxesEntered <= 0)
            {
                if (colorTransition != null && backToMainColor)
                {
                    StopCoroutine(colorTransition);
                    if (color)
                        colorTransition = StartCoroutine(TransitionLight(mainLight.color, mainColor));
                }
                if(intensityTransition != null && backToMainColor)
                {
                    StopCoroutine(intensityTransition);
                    if (intensity)
                        intensityTransition = StartCoroutine(TransitionIntensity(mainLight.intensity, mainIntensity));
                }
                if (fovTransition != null && backToMainFov)
                {
                    StopCoroutine(fovTransition);
                    if (fov)
                        fovTransition = StartCoroutine(TransitionFov(mainCamera.fieldOfView, mainFov));
                }
                if (vignetteTransition != null && backToMainVignette)
                {
                    StopCoroutine(vignetteTransition);
                    Vignette mainVignette;
                    if (vignette && volumeProfile.TryGet<Vignette>(out mainVignette))
                        vignetteTransition = StartCoroutine(TransitionVignette(mainVignette, mainVignette.intensity.value, 0f));
                }
                if (musicTransition != null)
                {
                    StopCoroutine(musicTransition);
                    if (music)
                        musicTransition = StartCoroutine(TransitionMusic(musicName, audioManager.getSourceVolume(audioManager.music, musicName), 0));
                }
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
    IEnumerator TransitionIntensity(float currentIntensity, float targetIntensity)
    {
        float t = 0f;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            mainLight.intensity = Mathf.Lerp(currentIntensity, targetIntensity, t / transitionTime);
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

    IEnumerator TransitionVignette(Vignette mainVignette, float currentIntensity, float targetIntensity)
    {
        float t = 0f;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            mainVignette.intensity.value = Mathf.Lerp(currentIntensity, targetIntensity, t / transitionTime);
            yield return null;
        }
    }

    IEnumerator TransitionMusic(string musicName, float currentVolume, float targetVolume)
    {   
        bool isPlaying = audioManager.isPlayed(audioManager.music, musicName);
        audioManager.setSourceVolume(audioManager.music, musicName, 0);
        if(!isPlaying)
            audioManager.Play(audioManager.music, musicName);

        float t = 0f;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            float volume = Mathf.Lerp(currentVolume, targetVolume, t / transitionTime);
            audioManager.setSourceVolume(audioManager.music, musicName, volume);
            yield return null;
        }
        if(isPlaying)
            audioManager.Stop(audioManager.music, musicName);
    }
}
