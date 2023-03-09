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
        uiManager.modeId = id;        
    }

    public void Deselected(){
        anim.SetBool("selected", false);
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
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("SelectedIdle")){
            Debug.Log("move upper part");
            upperPartBackgroundController.Selected();
            upperPartForegroundController.Selected();
        } else {
            Debug.Log("dont move upper part");
            upperPartBackgroundController.Deselected();
            upperPartForegroundController.Deselected();
        }
    }
}
