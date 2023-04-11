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
        "Press 'LMB' to Shoot / Melee and 'RMB' to Cast",
        "Press 'Space' to Jump",
        "Press 'Shift' to Dodge / Run",
        "Press 'Tabulator' to choose your weapon mode",
        "Use 'I' to open your Inventory",
        "Use 'F5' to save and 'F9' to load the game",
        "Press 'Space' to Continue!"
    };

    void Start()
    {
        showControlTime = 6f;
        timeBetweenControls = 2f;
        targetY = 3f;
        targetInterval = 0.02f;
        TMP = GetComponent<TextMeshPro>();
        currentColor = TMP.color;
        Spaceship = GameObject.FindWithTag("Spaceship");
        ShipScript = Spaceship.GetComponent<ShipControl>();
        SpaceCameraScript = GameObject.FindWithTag("SpaceCamera").GetComponent<CameraControlSpace>();
        ThrusterAudio = GameObject.FindWithTag("SpaceShipThruster").GetComponent<AudioSource>();
        parentUI = GameObject.FindWithTag("ParentUI");
        ShipScript.lockPlayerControl = true;
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
        transform.Translate(0f, -0.02f, 0f);
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
        GameObject.FindWithTag("MenuUI").GetComponent<CanvasGroup>().alpha = 0f;
        
        if(Spaceship.transform.position.y <= 0 && ShipScript.lockPlayerControl){
            Spaceship.transform.Translate(Vector3.down * ShipScript.SpaceShipSpeed * Time.deltaTime);
            return;
        }
        SpaceCameraScript.followPlayer = true;
        ShipScript.lockPlayerControl = false;

        parentUI.GetComponent<CanvasGroup>().alpha = 1;
        newGame = false;       
        StartCoroutine(showControls());
    }

    private IEnumerator showControls(){
        yield return new WaitForSeconds(timeBetweenControls);

        GameObject.FindWithTag("StoryField").GetComponent<StorytextControlSpace>().startStory = true;

        foreach(var control in controls){
            parentUI.GetComponent<ButtonControl>().SpecialMessage(control, showControlTime);
            
            if(control == controls[controls.Count-1]) break;
            yield return new WaitForSeconds(showControlTime + timeBetweenControls);
        }
    }

    private void ResetGameData(){
        Player.savedWeaponModes = new List<bool>() { false, false, false, true };
        Player.notFirstTime = true;

        Checkpoint.storyPartIndexCheckpoint = new List<int>();
        Checkpoint.storyShownCheckpoint = new List<bool>();
        Checkpoint.invItemID = new List<int>();
        Checkpoint.invItemAmount = new List<int>();
        Checkpoint.treeBossEntryOpened = false;
        Checkpoint.treeBossDead = false;
        Checkpoint.golemDead = false;
        Checkpoint.playerDoubleJump = false;
    }
}
