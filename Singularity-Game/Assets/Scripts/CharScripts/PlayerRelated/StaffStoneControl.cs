using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffStoneControl : MonoBehaviour
{
    private Renderer rend;
    [Header("Add a new Material here. Order matters!")]
    [SerializeField] private List<Material> weaponMaterials;
    private int currentMode;
    public bool meleeAttack;
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
        ChangeMaterial();
    }

    private void ChangeMaterial(){
        if(weaponMaterials.Count == 0 || currentMode < 0) return;
        currentMode = player.GetComponent<Player>().weaponMode;
        rend.material = weaponMaterials[currentMode];
    }

    private bool CheckMeleeAttack(){
        return !player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Empty");
    }

    void OnTriggerEnter(Collider col){
        if(CheckMeleeAttack() && col.gameObject.GetComponent<Damageable>() 
        && !col.gameObject.GetComponent<Player>()){
            col.gameObject.GetComponent<Damageable>().ApplyDamage(player.GetComponent<Player>().meleeDamage);
        }
    }
}
