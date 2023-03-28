using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationCrystal : MonoBehaviour
{
    [SerializeField] private int crystalModeId;
    AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        // gameObject.SetActive(!player.unlockedWeaponModes[crystalModeId]);

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            Player player = col.GetComponent<Player>();
            
            player.GiveItem(player.inventory.GetItem(crystalModeId), 10);
            player.unlockedWeaponModes[crystalModeId] = true;
            gameObject.SetActive(false);

            StartCoroutine(audioManager.PauseCategory(audioManager.music, 1f));
            audioManager.Play(audioManager.music, "CrystalSkill");
        }
    }
}
