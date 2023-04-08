using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player player;
    [SerializeField] private CanvasGroup gameUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject inventoryInfoTextPanel;
    [SerializeField] private GameObject activeModeDisplay;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private CanvasGroup weaponWheelUI;
    private Animator inventoryAnimator, inventoryInfoTextPanelAnimator;
    public int modeId;
    public Sprite modeImage;
    public List<bool> unlockedWeaponModes;

    [Header("TreeBoss")]
    [SerializeField] private CanvasGroup treeBossHealthBar;
    [SerializeField] private TreeBoss treeBoss;

    [Header("StoneGolemBoss")]
    [SerializeField] private CanvasGroup stoneGolemHealthBar;
    [SerializeField] private StoneGolemBoss stoneGolem;
    
    void Start(){
        modeId = 0;
        unlockedWeaponModes = player.unlockedWeaponModes;

        inventoryAnimator = inventoryUI.GetComponent<Animator>();
        inventoryInfoTextPanelAnimator = inventoryInfoTextPanel.GetComponent<Animator>();

        weaponWheelUI.blocksRaycasts = false;
        weaponWheelUI.alpha = 0;
        
        // Delete before release!!!
        activateAllModes();
    }

    void Update(){
        OpenCloseInventory();
        OpenCloseWeaponWheel();
        UpdateWeaponMode();
        UpdateHealthBar();
        UpdateBossHealthBar();
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


    // Delete this before Release!!! (only for testing)
    void activateAllModes(){
        for(int i = 0; i < unlockedWeaponModes.Count; i++){
            unlockedWeaponModes[i] = true;
        }
    }

    private void UpdateBossHealthBar(){
        stoneGolemHealthBar.alpha = stoneGolem.bossFightStarted ? 1 : 0;
        treeBossHealthBar.alpha = treeBoss.startFight ? 1 : 0;
    }
}