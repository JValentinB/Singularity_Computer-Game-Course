using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBranch : MonoBehaviour
{
    [SerializeField] private GameObject bombFruit;
    [Header("Position of the fruit relative to the branch")]
    [SerializeField] private float regrowTime;
    private Vector3 fruitSize, fruitLocalPos;
    private float regrowCounter;
    private GameObject fruitObject;
    private bool regrown;

    void Start()
    {
        fruitObject = gameObject.transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/AcornControl/Acorn").gameObject;
        fruitSize = fruitObject.transform.localScale;
        fruitLocalPos = fruitObject.transform.localPosition;
        regrowTime = 15f;
    }

    void Update()
    {
        Regrow();
    }

    private void Regrow(){
        if(regrown) return;
        if(regrowCounter > 0f){
            regrowCounter -= Time.deltaTime;
            fruitObject.transform.localPosition = Vector3.Lerp(fruitLocalPos, Vector3.zero, regrowCounter/regrowTime);
            fruitObject.transform.localScale = Vector3.Lerp(fruitSize, Vector3.zero, regrowCounter/regrowTime);
            return;
        }

        regrown = true;
    }

    private void ReleaseFruit(){
        regrowCounter = regrowTime;
        regrown = false;
        Instantiate(bombFruit, fruitObject.transform.position, fruitObject.transform.rotation);
    }

    void OnTriggerEnter(Collider col){
        var obj = col.gameObject;
        if(regrown && obj.tag == "Staffstone" && obj.GetComponent<StaffStoneControl>().CheckMeleeAttack()){
            ReleaseFruit();
        }
    }
}
