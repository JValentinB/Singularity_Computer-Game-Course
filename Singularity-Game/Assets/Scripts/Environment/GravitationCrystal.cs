using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationCrystal : MonoBehaviour
{
    [SerializeField] private int crystalModeId;
    [SerializeField] private Player player;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(!player.unlockedWeaponModes[crystalModeId]);
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            player.GiveItem(player.inventory.GetItem(crystalModeId), 10);
            player.unlockedWeaponModes[crystalModeId] = true;
            gameObject.SetActive(false);
        }
    }
}
