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
        currentMode = GameObject.FindWithTag("Player").GetComponent<Player>().weaponMode;
        if(currentMode < 0) return;
        rend.material = weaponMaterials[currentMode];
    }
}
