using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelButtonController : MonoBehaviour
{
    [SerializeField] private GameObject weaponWheelUi;
    [SerializeField] private WeaponWheelController upperPartBackgroundController;
    [SerializeField] private WeaponWheelController upperPartForegroundController;
    [SerializeField] private UIManager uiManager;
    private CanvasGroup weaponWheelUiCanvasGroup;
    public int id;
    private Animator anim;
    public float alphaThreshhold = 0.1f;
    private bool selected = false;
    private bool deselected = false;
    // Start is called before the first frame update
    void Start()
    {
        weaponWheelUiCanvasGroup = weaponWheelUi.GetComponent<CanvasGroup>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Activate();
        ActivateUpper();
    }

    public void Selected(){
        anim.SetBool("selected", true);
        selected = true;
        uiManager.modeId = id;
    }

    public void Deselected(){
        anim.SetBool("selected", false);
        deselected = true;
        uiManager.modeId = 0;
    }

    public void HoverEnter(){
        anim.SetBool("hover", true);
    }

    public void HoverExit(){
        anim.SetBool("hover", false);
    }

    public void Activate(){
        anim.SetBool("active", weaponWheelUiCanvasGroup.alpha == 1);
    }

    public void ActivateUpper(){
        if(selected){
            upperPartBackgroundController.Selected();
            upperPartForegroundController.Selected();
            selected = false;
        } else if(deselected){
            upperPartBackgroundController.Deselected();
            upperPartForegroundController.Deselected();
            deselected = false;
        }
    }
}
