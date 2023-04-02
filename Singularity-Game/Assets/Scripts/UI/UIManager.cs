using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private CanvasGroup weaponWheelUI;
    [SerializeField] private CanvasGroup gameUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject inventoryInfoTextPanel;
    [SerializeField] private GameObject activeModeDisplay;
    [SerializeField] private HealthBar healthBar;
    private Animator inventoryAnimator, inventoryInfoTextPanelAnimator;
    public int modeId;
    public Sprite modeImage;
    public List<bool> unlockedWeaponModes;

    void Start(){
        modeId = 0;
        unlockedWeaponModes = player.unlockedWeaponModes;

        inventoryAnimator = inventoryUI.GetComponent<Animator>();
        inventoryInfoTextPanelAnimator = inventoryInfoTextPanel.GetComponent<Animator>();

        weaponWheelUI.blocksRaycasts = false;
        weaponWheelUI.alpha = 0;
    }

    void Update(){
        OpenCloseInventory();
        OpenCloseWeaponWheel();
        UpdateWeaponMode();
        UpdateHealthBar();
    }

    public void OpenCloseInventory()
    {
        if(Input.GetKeyDown(KeyCode.I) || (inventoryAnimator.GetBool("active") && Input.GetKeyDown(KeyCode.Escape)))
        {
            inventoryAnimator.SetBool("active", !inventoryAnimator.GetBool("active"));
            if(!inventoryAnimator.GetBool("active")){
                inventoryInfoTextPanelAnimator.SetBool("shutDown", true);
            } else {
                inventoryInfoTextPanelAnimator.SetBool("shutDown", false);
            }
        }
    }

    public void OpenCloseWeaponWheel()
    {
        //maybe animator
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWheelUI.blocksRaycasts = true;
            weaponWheelUI.interactable = true;
            gameUI.alpha = 0;
            weaponWheelUI.alpha = 1;
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            weaponWheelUI.blocksRaycasts = false;
            weaponWheelUI.interactable = false;
            gameUI.alpha = 1;
            weaponWheelUI.alpha = 0;
        }
    }
    

    public void UpdateWeaponMode(){
        if(modeImage && player.weaponMode != modeId){
            activeModeDisplay.GetComponent<Image>().color = Color.white;
            activeModeDisplay.GetComponent<Image>().sprite = modeImage;
            activeModeDisplay.GetComponent<Image>().preserveAspect = true;
            player.ChangeBulletMode(modeId);
        }
    }

    public void UpdateHealthBar(){
        healthBar.currentHealth = player.currentHealth;
        healthBar.maxHealth = player.maxHealth;
    }
}