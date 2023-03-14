using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private float targetY;
    private float targetInterval;
    private bool newGame;
    private TextMeshPro TMP;
    private Color currentColor;
    private bool fadeOut;
    private GameObject Spaceship;
    private ShipControl ShipScript;
    private CameraControlSpace SpaceCameraScript;

    void Start()
    {
        targetY = 3f;
        targetInterval = 0.01f;
        TMP = GetComponent<TextMeshPro>();
        currentColor = TMP.color;
        Spaceship = GameObject.FindWithTag("Spaceship");
        ShipScript = Spaceship.GetComponent<ShipControl>();
        SpaceCameraScript = GameObject.FindWithTag("SpaceCamera").GetComponent<CameraControlSpace>();
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
        transform.Translate(0f, -0.01f, 0f);
    }

    private void ActivateMenuButtons(){
        //Leave button objects deactivated at start
        //Activate here when Title is in Targetposition
        if(transform.position.y < targetY + targetInterval && transform.position.y > targetY - targetInterval){
            //Buttons slowly fading in
            if(Input.GetKey(KeyCode.Space)){
                fadeOut = true;
            }
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
        if(Spaceship.transform.position.y <= 0 && ShipScript.lockPlayerControl){
            Spaceship.transform.Translate(Vector3.down * ShipScript.SpaceShipSpeed * Time.deltaTime);
            return;
        }
        SpaceCameraScript.followPlayer = true;
        ShipScript.lockPlayerControl = false;

        //Start story

        //Lock player control and let Spaceship fly out of screen
    }
}
