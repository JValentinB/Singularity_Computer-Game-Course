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
    [SerializeField] private CanvasGroup menuUI;
    [SerializeField] private GameObject menuEnvironment;
    [SerializeField] private ParticleSystem menuEnvironmentParticle;
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
        //activateAllModes();
    }

    void Update(){
        unlockedWeaponModes = player.unlockedWeaponModes;
        OpenCloseMenu();
        if(menuUI.alpha == 0){
            OpenCloseInventory();
            OpenCloseWeaponWheel();
            UpdateWeaponMode();
            UpdateBossHealthBar();
        }        
    }

    private void OpenCloseMenu(){
        if(Input.GetKeyDown(KeyCode.Escape) && !inventoryAnimator.GetBool("active") 
        && weaponWheelUI.alpha == 0){
            bool menuIsActive = menuUI.alpha == 1;
            if(menuIsActive) CloseMenu();
            else OpenMenu();
        }
    }

    public void OpenMenu(){
        menuEnvironment.active = true;
        menuEnvironmentParticle.Play();
        gameUI.alpha = 0;
        gameUI.interactable = false;
        gameUI.blocksRaycasts = false;
        
        menuUI.interactable = true;
        menuUI.blocksRaycasts = true;
        menuUI.GetComponent<IngameMenuButtonController>().UpdateValues();
        player.lockPlayerControl = true;
        menuUI.alpha = 1;
        Time.timeScale = 0;
    }

    public void CloseMenu(){
        if(!menuUI.GetComponent<IngameMenuButtonController>().menuUi.active){
            menuUI.GetComponent<IngameMenuButtonController>().BackToMenu();
            return;
        }
        Time.timeScale = 1;
        menuEnvironmentParticle.Stop();
        menuEnvironment.active = false;
        menuUI.GetComponent<IngameMenuButtonController>().BackToMenu();
        gameUI.alpha = 1;
        gameUI.interactable = true;
        gameUI.blocksRaycasts = true;
        
        menuUI.interactable = false;
        menuUI.blocksRaycasts = false;
        menuUI.GetComponent<IngameMenuButtonController>().UpdateValues();
        player.lockPlayerControl = false;
        menuUI.alpha = 0;
    }

    private void OpenCloseInventory()
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

    private void OpenCloseWeaponWheel()
    {
        //maybe animator
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWheelUI.blocksRaycasts = true;
            weaponWheelUI.interactable = true;
            gameUI.alpha = 0;
            weaponWheelUI.alpha = 1;
            player.lockPlayerControl = true;
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            weaponWheelUI.blocksRaycasts = false;
            weaponWheelUI.interactable = false;
            gameUI.alpha = 1;
            weaponWheelUI.alpha = 0;
            player.lockPlayerControl = false;
        }
    }
    
    private void UpdateWeaponMode(){
        if(modeImage && player.weaponMode != modeId){
            activeModeDisplay.GetComponent<Image>().color = Color.white;
            activeModeDisplay.GetComponent<Image>().sprite = modeImage;
            activeModeDisplay.GetComponent<Image>().preserveAspect = true;
            player.ChangeBulletMode(modeId);
        }
    }

    // Delete this before Release!!! (only for testing)
    private void activateAllModes(){
        for(int i = 0; i < unlockedWeaponModes.Count; i++){
            unlockedWeaponModes[i] = true;
        }
    }

    public void UpdateBossHealthBar(){
        stoneGolemHealthBar.alpha = stoneGolem.bossFightStarted ? 1 : 0;
        treeBossHealthBar.alpha = treeBoss.startFight ? 1 : 0;
    }
}