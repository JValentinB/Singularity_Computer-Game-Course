using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBranch : MonoBehaviour
{
    [SerializeField] private GameObject bombFruit;
    [SerializeField] private GameObject branchPiece;
    [Header("Position of the fruit relative to the branch")]
    [SerializeField] private Vector3 relativeFruitPos;
    [SerializeField] private float regrowTime;
    private float regrowCounter;
    private GameObject fruitObject;
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
            //Maybe grow dummy fruit slowly while regrowing
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
        if(col.gameObject.tag == "Staffstone" && regrown){
            ReleaseFruit();
        }
    }
}
