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
    [SerializeField] public Button NewGameButton, LoadGameButton, OptionsButton;
    [SerializeField] public Button OptionBack, OptionPixelizationUp, OptionPixelizationDown;
    private CanvasGroup ParentUI, MenuUI, OptionsUI;
    [SerializeField] public TMP_Text SpecialMessageField;
    private TMP_Text PixelizationValue;
    private AudioSource ButtonSound;
    [SerializeField] public bool fadeOutUI, fadeInUI;
    private float alphaOffset = 0.01f;
    private string specialMessage = "";
    private float specialMessagePosY, specialMessageCD;
    private bool showSpecial;

    void Start()
    {
        ParentUI = GetComponent<CanvasGroup>();
        MenuUI = GameObject.FindWithTag("MenuUI").GetComponent<CanvasGroup>();
        OptionsUI = GameObject.FindWithTag("OptionsUI").GetComponent<CanvasGroup>();
        PixelizationValue = GameObject.FindWithTag("PixelizationValue").GetComponent<TMP_Text>();
        ButtonSound = GetComponent<AudioSource>();

        ParentUI.alpha = 0f;
        ActivateOptions(false);
        ActivateUI(false);
        NewGameButton.onClick.AddListener(NewGameClicked);
        LoadGameButton.onClick.AddListener(LoadGameClicked);
        OptionsButton.onClick.AddListener(ShowOptions);
        OptionBack.onClick.AddListener(ShowMenu);
        OptionPixelizationUp.onClick.AddListener(IncPixelization);
        OptionPixelizationDown.onClick.AddListener(DecPixelization);
        ResetRenderScale();
        UpdatePixelizationValue();
        specialMessagePosY = SpecialMessageField.rectTransform.position.y;
    }

    void Update(){
        FadeUI();
        SpecialMessage();
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

    private void SpecialMessage(){
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
        ActivateUI(false);
    }

    private void LoadGameClicked(){
        ButtonSound.Play();
        SaveSystem.LoadGame();
        if(SceneManager.GetActiveScene().name != "Forest1.0_Valentin"){
            specialMessageCD = 3f;
            specialMessage = "No savegame found!";
            showSpecial = true;
        }
    }

    private void ShowOptions(){
        ButtonSound.Play();
        ActivateOptions(true);
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
}
