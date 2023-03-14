using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    [SerializeField] public float SpaceShipSpeed;
    private float defaultXRotation;
    private float defaultYRotation;
    private float defaultZRotation;
    public bool lockPlayerControl;
    
    void Start()
    {
        lockPlayerControl = true;
        SpaceShipSpeed = 5f;
        defaultXRotation = 0f;
        defaultYRotation = 180f;
        defaultZRotation = 180f;
    }

    // Update is called once per frame
    void Update()
    {
        if(lockPlayerControl) return;
        MoveSpaceShip();
        AdjustShipRotation();
    }

    private void MoveSpaceShip(){
        if(Input.GetAxis("Horizontal") != 0){
            var velocity = Vector3.right * Input.GetAxis("Horizontal") * SpaceShipSpeed;
            transform.Translate(velocity * Time.deltaTime);
        }
        if(Input.GetAxis("Vertical") != 0){
            var velocity = Vector3.down * Input.GetAxis("Vertical") * SpaceShipSpeed;
            transform.Translate(velocity * Time.deltaTime);
        }
        
    }

    private void AdjustShipRotation(){
        //tilt forward and backward
        var rotationX = defaultXRotation - Input.GetAxis("Vertical") * 5f;
        //tilt right and left
        var rotationY = defaultYRotation - Input.GetAxis("Horizontal") * 15f;

        transform.localEulerAngles = new Vector3(rotationX, rotationY, defaultZRotation);
    }
}
