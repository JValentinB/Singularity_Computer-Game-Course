using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class IngameMenuButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    [Header("General")]
    [SerializeField] public GameObject menuUi;
    [SerializeField] public GameObject optionsUi;
    [SerializeField] public GameObject controlsUi;
    [SerializeField] public GameObject mainMenuWarningUi;
    [SerializeField] public GameObject closeWarningUi;
    [SerializeField] public GameObject backButton;
    [SerializeField] public UIManager uiManager;
    private AudioSource ButtonSound;
    [Header("Menu")]
    [SerializeField] public Button Options;
    [SerializeField] public Button Controls; 
    [SerializeField] public Button Continue;
    [SerializeField] public Button MainMenu;
    [SerializeField] public Button QuitGame;
    [Header("Warning")]
    [SerializeField] public Button closeYes;
    [SerializeField] public Button closeNo;
    [SerializeField] public Button mainMenuYes;
    [SerializeField] public Button mainMenuNo;
    [Header("Options")]
    [SerializeField] public Button OptionPixelizationUp;
    [SerializeField] public Button OptionPixelizationDown; 
    [SerializeField] public Button OptionResolutionUp;
    [SerializeField] public Button OptionResolutionDown;
    [SerializeField] public Button OptionFullScreenUp;
    [SerializeField] public Button OptionFullScreenDown;
    [SerializeField] public Button OptionUwuifyUp;
    [SerializeField] public Button OptionUwuifyDown;
    [SerializeField] public TMP_Text PixelizationValue, ResolutionValue, FullScreenValue, UwuifyValue;
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
        backButton.GetComponent<Button>().onClick.AddListener(BackToMenu);
        //Menu buttons
        Options.onClick.AddListener(OpenOptions);
        Controls.onClick.AddListener(OpenControlls);
        Continue.onClick.AddListener(CloseMenu);
        MainMenu.onClick.AddListener(BackToMainMenuWarning);
        QuitGame.onClick.AddListener(CloseGameWarning);
        //Options buttons
        OptionPixelizationUp.onClick.AddListener(IncPixelization);
        OptionPixelizationDown.onClick.AddListener(DecPixelization);
        OptionResolutionUp.onClick.AddListener(IncResolution);
        OptionResolutionDown.onClick.AddListener(DecResolution);
        OptionFullScreenUp.onClick.AddListener(IncFullScreen);
        OptionFullScreenDown.onClick.AddListener(DecFullScreen);
        OptionUwuifyUp.onClick.AddListener(IncUwuify);
        OptionUwuifyDown.onClick.AddListener(DecUwuify);
        //Warning buttons
        closeNo.onClick.AddListener(BackToMenu);
        mainMenuNo.onClick.AddListener(BackToMenu);
        closeYes.onClick.AddListener(CloseGame);
        mainMenuYes.onClick.AddListener(BackToMainMenu);        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!controlsUi.active || eventData.pointerEnter.CompareTag("BackButton")){
            eventData.pointerEnter.transform.parent.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!controlsUi.active || eventData.pointerEnter.CompareTag("BackButton")){
            eventData.pointerEnter.transform.parent.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void BackToMenu(){
        ButtonSound.Play();
        optionsUi.SetActive(false);
        controlsUi.SetActive(false);
        backButton.SetActive(false);
        mainMenuWarningUi.SetActive(false);
        closeWarningUi.SetActive(false);
        menuUi.SetActive(true);
    }

    public void OpenOptions(){
        ButtonSound.Play();
        menuUi.SetActive(false);
        optionsUi.SetActive(true);
        backButton.SetActive(true);
    }

    public void OpenControlls(){
        ButtonSound.Play();
        menuUi.SetActive(false);
        controlsUi.SetActive(true);
        backButton.SetActive(true);
    }

    public void CloseMenu(){
        ButtonSound.Play();
        uiManager.CloseMenu();
    }

    public void BackToMainMenuWarning(){
        ButtonSound.Play();
        menuUi.SetActive(false);
        mainMenuWarningUi.SetActive(true);
    }

    public void CloseGameWarning(){
        ButtonSound.Play();
        menuUi.SetActive(false);
        closeWarningUi.SetActive(true);
    }

    public void BackToMainMenu(){
        ButtonSound.Play();
        Time.timeScale = 1;
        SceneManager.LoadScene("Space");
    }

    public void CloseGame(){
        ButtonSound.Play();
        Application.Quit();
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
