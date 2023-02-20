using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShader : MonoBehaviour
{
    private Renderer rend;
    [Header("Add a new Material here. Order matters!")]
    [SerializeField] private List<Material> weaponMaterials;
    private int currentMode;

    void Start()
    {
        rend = GetComponent<Renderer>();
        currentMode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeMaterial();
    }

    private void ChangeMaterial(){
        if(weaponMaterials.Count == 0 || currentMode < 0) return;
        currentMode = GameObject.FindWithTag("Player").GetComponent<Player>().weaponMode;
        rend.material = weaponMaterials[currentMode];
    }
}
