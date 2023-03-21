using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup invUI;
    [SerializeField] private Player player;
    [SerializeField] private CanvasGroup weaponWheelUI;
    [SerializeField] private GameObject activeModeDisplay;
    public int modeId;
    public Sprite modeImage;

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
        UpdateWeaponMode();
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
    

    public void UpdateWeaponMode(){
        if(modeImage){
            activeModeDisplay.GetComponent<Image>().sprite = modeImage;
            activeModeDisplay.GetComponent<Image>().preserveAspect = true;
        }
        player.ChangeBulletMode(modeId);
    }

    public void UpdateAvailableModes(){
        
    }
}