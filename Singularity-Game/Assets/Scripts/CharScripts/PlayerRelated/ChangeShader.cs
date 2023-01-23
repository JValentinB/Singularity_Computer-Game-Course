using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShader : MonoBehaviour
{
    private Renderer rend;
    [SerializeField] private Material weaponMode0;
    [SerializeField] private Material weaponMode1;
    private int mode;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeMaterial();
    }

    private void ChangeMaterial(){
        mode = GameObject.FindWithTag("Player").GetComponent<Player>().weaponMode;
        switch (mode){
            case 0:
                rend.material = weaponMode0;
                break;
            case 1:
                rend.material = weaponMode0;
                break;
        }
    }
}
