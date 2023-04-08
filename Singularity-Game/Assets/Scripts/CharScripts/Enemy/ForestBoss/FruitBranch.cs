using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBranch : MonoBehaviour
{
    [SerializeField] private GameObject bombFruit;
    [SerializeField] private float regrowTime;
    private Rigidbody rb;
    private Vector3 fruitSize, fruitLocalPos;
    private float regrowCounter;
    private GameObject fruitObject;
    private bool regrown;

    private ObjectSounds objectSounds;

    void Start()
    {
        rb = transform.Find("Rig 1/BranchTarget").gameObject.GetComponent<Rigidbody>();
        fruitObject = gameObject.transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/AcornControl/Acorn").gameObject;
        fruitSize = fruitObject.transform.localScale;
        fruitLocalPos = fruitObject.transform.localPosition;
        regrowTime = 15f;

        objectSounds = GetComponent<ObjectSounds>();
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
        GameObject acornBomb = Instantiate(bombFruit, fruitObject.transform.position, fruitObject.transform.rotation);
        acornBomb.GetComponent<BombFruit>().isMoving = true;

        Destroy(acornBomb, 15f);
    }

    void OnTriggerEnter(Collider col){
        var obj = col.gameObject;

        if((obj.GetComponent<StaffStoneControl>() && obj.GetComponent<StaffStoneControl>().CheckMeleeAttack()) || obj.GetComponent<m_Projectile>()){
            if(regrown) ReleaseFruit();
            StartCoroutine(shakeBranch());
            rb.AddForce(Vector3.up * 150f, ForceMode.Impulse);
        }
    }

    IEnumerator shakeBranch(){
        objectSounds.playAtRandomTimePoint("LeaveRustling", 0, 0.7f);
        yield return new WaitForSeconds(1f);
        StartCoroutine(objectSounds.fadeInOut("LeaveRustling", 0f, 0.25f));
    }
}
