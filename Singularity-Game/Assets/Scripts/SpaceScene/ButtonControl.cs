using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public Button NewGameButton, LoadGameButton, OptionsButton, ExitButton, 
    OptionBack, OptionPixelizationUp, OptionPixelizationDown, 
    OptionResolutionUp, OptionResolutionDown,
    OptionFullScreenUp, OptionFullScreenDown,
    OptionUwuifyUp, OptionUwuifyDown;
    [SerializeField] public TMP_Text SpecialMessageField, PixelizationValue, ResolutionValue, FullScreenValue, UwuifyValue;
    private CanvasGroup ParentUI, MenuUI, OptionsUI;
    private AudioSource ButtonSound;
    [SerializeField] public bool fadeOutUI, fadeInUI;
    private float alphaOffset = 0.01f;
    private string specialMessage = "";
    private float specialMessagePosY, specialMessageCD;
    private bool showSpecial;
    [SerializeField] private bool fullScreen, uwuMode;
    [SerializeField] private List<Vector2> resolutions = new List<Vector2>(){
        new Vector2(640, 480),
        new Vector2(1280, 720),
        new Vector2(1600, 900),
        new Vector2(1920, 1080),
        new Vector2(2560, 1440),
        new Vector2(3840, 2160)
    };

    void Start()
    {
        ParentUI = GetComponent<CanvasGroup>();
        MenuUI = GameObject.FindWithTag("MenuUI").GetComponent<CanvasGroup>();
        OptionsUI = GameObject.FindWithTag("OptionsUI").GetComponent<CanvasGroup>();
        ButtonSound = GetComponent<AudioSource>();

        ParentUI.alpha = 0f;
        ActivateOptions(false);
        ActivateUI(false);
        NewGameButton.onClick.AddListener(NewGameClicked);
        LoadGameButton.onClick.AddListener(LoadGameClicked);
        OptionsButton.onClick.AddListener(ShowOptions);
        ExitButton.onClick.AddListener(ExitGame);
        OptionBack.onClick.AddListener(ShowMenu);
        OptionPixelizationUp.onClick.AddListener(IncPixelization);
        OptionPixelizationDown.onClick.AddListener(DecPixelization);
        OptionResolutionUp.onClick.AddListener(IncResolution);
        OptionResolutionDown.onClick.AddListener(DecResolution);
        OptionFullScreenUp.onClick.AddListener(IncFullScreen);
        OptionFullScreenDown.onClick.AddListener(DecFullScreen);
        OptionUwuifyUp.onClick.AddListener(IncUwuify);
        OptionUwuifyDown.onClick.AddListener(DecUwuify);
        ResetRenderScale();
        UpdatePixelizationValue();
        UpdateFullScreenValue();
        UpdateUwuifyValue();
    }

    void Update(){
        FadeUI();
        ShowSpecialMessage();
        UpdateResolutionValue();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerEnter.transform.parent.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.pointerEnter.transform.parent.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void ResetRenderScale(){
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
        urpAsset.renderScale = 1f;
    }

    private void ActivateUI(bool activate){
        ParentUI.interactable = activate;
    }

    private void ActivateOptions(bool activate){
        MenuUI.gameObject.SetActive(!activate);
        OptionsUI.gameObject.SetActive(activate);
    }

    private void FadeUI(){
        if(!fadeOutUI && !fadeInUI) return;

        if(fadeOutUI && ParentUI.alpha >= 0 + alphaOffset){
            ParentUI.alpha -= Time.deltaTime;
        } else if(ParentUI.alpha <= 0 + alphaOffset){
            fadeOutUI = false;
        }

        if(fadeInUI && ParentUI.alpha <= 1 - alphaOffset){
            ParentUI.alpha += Time.deltaTime;
        } else if(ParentUI.alpha >= 1 - alphaOffset){
            ActivateUI(true);
            fadeInUI = false;
        }    
    }

    public void SpecialMessage(string msg, float ttl){
        specialMessageCD = ttl;
        specialMessage = msg;
        showSpecial = true;
    }

    private void ShowSpecialMessage(){
        var specialMessagePos = SpecialMessageField.rectTransform.pivot;
        SpecialMessageField.text = specialMessage;

        if(showSpecial && specialMessagePos.y <= 1f){
            //Show special message and special message is in top position
            SpecialMessageField.rectTransform.pivot = new Vector2(specialMessagePos.x, specialMessagePos.y + Time.deltaTime*3f);
            return;
        } else if(!showSpecial && specialMessagePos.y >= 0f){
            //Do not show special message and special message is in bottom position
            SpecialMessageField.rectTransform.pivot = new Vector2(specialMessagePos.x, specialMessagePos.y - Time.deltaTime*3f);
            return;
        }
        if(specialMessageCD >= 0f){ 
            specialMessageCD -= Time.deltaTime;
            return;
        } 
        showSpecial = false;
    }

    private void NewGameClicked(){
        ButtonSound.Play();
        fadeOutUI = true;
        var gameTitle = GameObject.FindWithTag("GameTitle").GetComponent<Menu>();
        gameTitle.fadeOut = true;
        if(StorytextControl.uwuMode) GameObject.FindWithTag("StoryField").GetComponent<StorytextControlSpace>().UwuifiyStory();
        ActivateUI(false);
    }

    private void LoadGameClicked(){
        ButtonSound.Play();
        SaveSystem.LoadGame();
        if(SceneManager.GetActiveScene().name != "Forest1.0_Valentin"){
            SpecialMessage("No savegame found!", 3f);
        }
    }

    private void ShowOptions(){
        ButtonSound.Play();
        ActivateOptions(true);
    }

    private void ExitGame(){
        ButtonSound.Play();
        Application.Quit(); 
    }

    private void ShowMenu(){
        ButtonSound.Play();
        ActivateOptions(false);
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

    //Returns index of resolution list >= current window width
    private int getResolutionIndex(){
        var currentWidth = Screen.width;
        for(int i = 0; i < resolutions.Count; i++){
            if(resolutions[i].x == currentWidth) return i;
        }
        return -1;
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
}
