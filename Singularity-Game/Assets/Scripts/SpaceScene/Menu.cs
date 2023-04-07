using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private float targetY;
    private float targetInterval;
    private bool newGame, activatedButtons;
    private TextMeshPro TMP;
    private Color currentColor;
    public bool fadeOut;
    private GameObject Spaceship, parentUI;
    private ShipControl ShipScript;
    private CameraControlSpace SpaceCameraScript;
    private AudioSource ThrusterAudio;
    private float showControlTime, timeBetweenControls;
    private List<string> controls = new List<string>(){
        "Controls for the Ship and Player:",
        "Use W,A,S,D to control your Player",
        "Press 'LMB' to Shoot and 'RMB' to Aim",
        "Press 'Space' to Jump",
        "Press 'Shift' to Dodge / Run",
        "Use 'I' to open your Inventory",
        "Press 'Space' to Continue!"
    };

    void Start()
    {
        showControlTime = 6f;
        timeBetweenControls = 2f;
        targetY = 3f;
        targetInterval = 0.01f;
        TMP = GetComponent<TextMeshPro>();
        currentColor = TMP.color;
        Spaceship = GameObject.FindWithTag("Spaceship");
        ShipScript = Spaceship.GetComponent<ShipControl>();
        SpaceCameraScript = GameObject.FindWithTag("SpaceCamera").GetComponent<CameraControlSpace>();
        ThrusterAudio = GameObject.FindWithTag("SpaceShipThruster").GetComponent<AudioSource>();
        parentUI = GameObject.FindWithTag("ParentUI");
    }

    // Update is called once per frame
    void Update()
    {
        MoveTitleToCenter();
        ActivateMenuButtons();
        MenuFadeOut();
        StartNewGame();
    }

    private void MoveTitleToCenter(){
        if(transform.position.y < targetY + targetInterval && transform.position.y > targetY - targetInterval) return;
        transform.Translate(0f, -0.005f, 0f);
    }

    private void ActivateMenuButtons(){
        if(activatedButtons) return;

        if(transform.position.y < targetY + targetInterval && transform.position.y > targetY - targetInterval){
            parentUI.GetComponent<ButtonControl>().fadeInUI = true;
            activatedButtons = true;
        }
    }

    private void MenuFadeOut(){
        if(!fadeOut) return;

        if(TMP.color.a >= 0){
            TMP.color = new Color(currentColor.r, currentColor.g, currentColor.b, TMP.color.a - Time.deltaTime);
            return;
        }
        fadeOut = false;
        newGame = true;
    }

    private void StartNewGame(){
        if(!newGame) return;
        if(!ThrusterAudio.isPlaying) ThrusterAudio.Play();
        
        if(Spaceship.transform.position.y <= 0 && ShipScript.lockPlayerControl){
            Spaceship.transform.Translate(Vector3.down * ShipScript.SpaceShipSpeed * Time.deltaTime);
            return;
        }
        SpaceCameraScript.followPlayer = true;
        ShipScript.lockPlayerControl = false;

        GameObject.FindWithTag("MenuUI").GetComponent<CanvasGroup>().alpha = 0f;
        parentUI.GetComponent<CanvasGroup>().alpha = 1;
        newGame = false;       
        StartCoroutine(showControls());
    }

    private IEnumerator showControls(){
        yield return new WaitForSeconds(timeBetweenControls);
        
        foreach(var control in controls){
            parentUI.GetComponent<ButtonControl>().SpecialMessage(control, showControlTime);
            
            if(control == controls[controls.Count-1]) break;
            yield return new WaitForSeconds(showControlTime + timeBetweenControls);
        }

        GameObject.FindWithTag("StoryField").GetComponent<StorytextControlSpace>().startStory = true;
    }
}
