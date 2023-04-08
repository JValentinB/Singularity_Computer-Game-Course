using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffStoneControl : MonoBehaviour
{
    private Renderer rend;
    [Header("Add a new Material here. Order matters!")]
    [SerializeField] private List<Material> weaponMaterials;
    private int currentMode;
    //public bool meleeAttack;
    private GameObject player;

    void Start()
    {
        rend = GetComponent<Renderer>();
        player = GameObject.FindWithTag("Player");
        currentMode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(weaponMaterials.Count == 0) return;
        ChangeMaterial();
    }

    private void ChangeMaterial(){
        currentMode = player.GetComponent<Player>().weaponMode;
        
        if(currentMode < 0) return;
        rend.material = weaponMaterials[currentMode];
    }

    public bool CheckMeleeAttack(){
        return !player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Empty");
    }

    void OnTriggerEnter(Collider col){
        if(CheckMeleeAttack() && !col.GetComponent<Player>()){
            if(col.GetComponent<Damageable>())
                col.GetComponent<Damageable>().ApplyDamage(player.GetComponent<Player>().meleeDamage);
            if(col.GetComponent<TreeBossHitzone>())
                col.GetComponent<TreeBossHitzone>().gettingHit(player.GetComponent<Player>().meleeDamage);
        }
    }
}
