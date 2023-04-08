using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class IngameMenuButtonController : MonoBehaviour
{
    [SerializeField] public Button OptionPixelizationUp, OptionPixelizationDown, 
    OptionResolutionUp, OptionResolutionDown,
    OptionFullScreenUp, OptionFullScreenDown,
    OptionUwuifyUp, OptionUwuifyDown;
    [SerializeField] public TMP_Text PixelizationValue, ResolutionValue, FullScreenValue, UwuifyValue;
    private AudioSource ButtonSound;
    [SerializeField] private bool fullScreen, uwuMode;
    [SerializeField] private List<Vector2> resolutions = new List<Vector2>(){
        new Vector2(640, 480),
        new Vector2(1280, 720),
        new Vector2(1600, 900),
        new Vector2(1920, 1080),
        new Vector2(2560, 1440),
        new Vector2(3840, 2160)
    };

    void Start(){
        ButtonSound = GetComponent<AudioSource>();
        OptionPixelizationUp.onClick.AddListener(IncPixelization);
        OptionPixelizationDown.onClick.AddListener(DecPixelization);
        OptionResolutionUp.onClick.AddListener(IncResolution);
        OptionResolutionDown.onClick.AddListener(DecResolution);
        OptionFullScreenUp.onClick.AddListener(IncFullScreen);
        OptionFullScreenDown.onClick.AddListener(DecFullScreen);
        OptionUwuifyUp.onClick.AddListener(IncUwuify);
        OptionUwuifyDown.onClick.AddListener(DecUwuify);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerEnter.transform.parent.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.pointerEnter.transform.parent.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void IncPixelization(){
        ButtonSound.Play();
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
        urpAsset.renderScale -= 0.1f;
        UpdatePixelizationValue();
    }

    private void DecPixelization(){
        ButtonSound.Play();
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
        if(urpAsset.renderScale >= 1f) return;
        urpAsset.renderScale += 0.1f;
        UpdatePixelizationValue();
    }

    private void UpdatePixelizationValue(){
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
        var pixelValue = Mathf.Round((1f - urpAsset.renderScale)*10);
        PixelizationValue.text = pixelValue.ToString("F0");
    }

    private void IncResolution(){
        ButtonSound.Play();
        var resolutionIndex = getResolutionIndex();

        if(resolutionIndex == -1){ 
            Screen.SetResolution((int) resolutions[0].x, (int) resolutions[0].y, fullScreen);
            UpdateResolutionValue();
            return;
        }
        if(resolutionIndex == resolutions.Count) return;

        Screen.SetResolution((int) resolutions[resolutionIndex+1].x, (int) resolutions[resolutionIndex+1].y, fullScreen);

        UpdateResolutionValue();
    }

    private int getResolutionIndex(){
        var currentWidth = Screen.width;
        for(int i = 0; i < resolutions.Count; i++){
            if(resolutions[i].x == currentWidth) return i;
        }
        return -1;
    }

    private void DecResolution(){
        ButtonSound.Play();
        var resolutionIndex = getResolutionIndex();

        if(resolutionIndex == -1){ 
            Screen.SetResolution((int) resolutions[0].x, (int) resolutions[0].y, fullScreen);
            UpdateResolutionValue();
            return;
        }
        if(resolutionIndex == 0) return;

        Screen.SetResolution((int) resolutions[resolutionIndex-1].x, (int) resolutions[resolutionIndex-1].y, fullScreen);

        UpdateResolutionValue();
    }

    private void UpdateResolutionValue(){
        ResolutionValue.text = Screen.width + " x " + Screen.height;
    }

    private void IncFullScreen(){
        ButtonSound.Play();
        if(fullScreen) return;

        fullScreen = true;
        Screen.SetResolution(Screen.width, Screen.height, fullScreen);

        UpdateFullScreenValue();
    }

    private void DecFullScreen(){
        ButtonSound.Play();
        if(!fullScreen) return;

        fullScreen = false;
        Screen.SetResolution(Screen.width, Screen.height, fullScreen);

        UpdateFullScreenValue();
    }

    private void UpdateFullScreenValue(){
        FullScreenValue.text = fullScreen ? "Yes" : "No";
    }
    private void IncUwuify(){
        ButtonSound.Play();
        if(StorytextControl.uwuMode) return;

        StorytextControl.uwuMode = true;

        UpdateUwuifyValue();
    }

    private void DecUwuify(){
        ButtonSound.Play();
        if(!StorytextControl.uwuMode) return;

        StorytextControl.uwuMode = false;

        UpdateUwuifyValue();
    }

    private void UpdateUwuifyValue(){
        UwuifyValue.text = StorytextControl.uwuMode ? "Yes" : "No";
    }

    public void UpdateValues(){
        UpdateResolutionValue();
        UpdateFullScreenValue();
        UpdateUwuifyValue();
        UpdatePixelizationValue();
    }
}
