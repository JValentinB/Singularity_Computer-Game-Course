using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBranch : MonoBehaviour
{
    [SerializeField] private GameObject bombFruit, branchPiece, fruitDummy;
    [Header("Position of the fruit relative to the branch")]
    [SerializeField] private Vector3 relativeFruitPos, dummyStartPos;
    [SerializeField] private float regrowTime;
    private float regrowCounter;
    private GameObject fruitObject, dummyObject;
    private bool regrown;

    void Start()
    {
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
            /* float newScale = regrowCounter;
            dummyObject.transform.localScale = new Vector3(newScale, newScale, newScale); */
            //dummy fruit is only mesh. No script, collider, etc.
            return;
        }

        //Push it of a bit, dont let it straight up fall down
        regrown = true;
        var fruitPosition = transform.position + relativeFruitPos;
        fruitObject = Instantiate(bombFruit, fruitPosition, Quaternion.identity);
    }

    private void ReleaseFruit(){
        fruitObject.GetComponent<BombFruit>().released = true;
        regrowCounter = regrowTime;
        regrown = false;
        //dummyObject = Instantiate(fruitDummy, dummyStartPos, Quaternion.identity);
    }

    private void createPieces(){
        Vector3 pos = transform.position;
        for(int i = 0; i < 5; i++){
            Vector3 piecePos = new Vector3(pos.x+Random.value, pos.y+Random.value, pos.z+Random.value);
            GameObject pieceClone = Instantiate(branchPiece, piecePos, transform.rotation);
            Destroy(pieceClone, 5);
        }
    }

    void OnTriggerEnter(Collider col){
        var obj = col.gameObject;
        if(regrown && obj.tag == "Staffstone" && obj.GetComponent<StaffStoneControl>().CheckMeleeAttack()){
            ReleaseFruit();
        }
    }
}
