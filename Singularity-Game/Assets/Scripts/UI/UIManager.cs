using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup invUI;
    [SerializeField] private CanvasGroup weaponWheelUI;
    public int modeId;

    void Start(){
        invUI.blocksRaycasts = false;
        invUI.alpha = 0;
        weaponWheelUI.blocksRaycasts = false;
        weaponWheelUI.alpha = 0;
    }

    void Update(){
        OpenCloseInventory();
        UpdateWeaponWheel();
    }

    public void OpenCloseInventory()
    {
        if(Input.GetKeyDown(KeyCode.I) || (invUI.alpha == 1 && Input.GetKeyDown(KeyCode.Escape)))
        {
            invUI.alpha = invUI.alpha == 1 ? 0 : 1;
            invUI.blocksRaycasts = invUI.blocksRaycasts == true ? false : true;
        }
    }
    
    public void UpdateWeaponWheel(){
        OpenCloseWeaponWheel();
        ChangeMode();
    }

    public void OpenCloseWeaponWheel()
    {
        //maybe animator
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWheelUI.blocksRaycasts = true;
            weaponWheelUI.interactable = true;
            weaponWheelUI.alpha = 1;
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            weaponWheelUI.blocksRaycasts = false;
            weaponWheelUI.interactable = false;
            weaponWheelUI.alpha = 0;
        }
    }
    

    public void ChangeMode(){
        if(weaponWheelUI.alpha == 1){
            switch (modeId)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }
    }

}