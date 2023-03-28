using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationCrystal : MonoBehaviour
{
    [SerializeField] private int crystalModeId;
    // Start is called before the first frame update
    void Start()
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        gameObject.SetActive(!player.unlockedWeaponModes[crystalModeId]);
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            Player player = col.GetComponent<Player>();
            
            player.GiveItem(player.inventory.GetItem(crystalModeId), 10);
            player.unlockedWeaponModes[crystalModeId] = true;
            gameObject.SetActive(false);
        }
    }
}
