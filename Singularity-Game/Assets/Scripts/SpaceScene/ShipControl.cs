using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    [SerializeField] GameObject LaserShot;
    [SerializeField] public float SpaceShipSpeed;
    [SerializeField] AudioClip LaserShot1;
    [SerializeField] AudioClip LaserShot2;
    private AudioSource LaserShotAudio;
    private float defaultXRotation;
    private float defaultYRotation;
    private float defaultZRotation;
    public bool lockPlayerControl;
    private bool doARoll;
    private float rollCounter;
    private float rollDirection;
    
    void Start()
    {
        LaserShotAudio = GetComponent<AudioSource>();
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
        LaserAttack();
        DodgeRoll();
    }

    private void MoveSpaceShip(){
        if(Input.GetAxis("Horizontal") != 0){
            var velocity = Vector3.right * Input.GetAxis("Horizontal") * SpaceShipSpeed;
            transform.Translate(velocity * Time.deltaTime, Space.World);
        }
        if(Input.GetAxis("Vertical") != 0){
            var velocity = Vector3.up * Input.GetAxis("Vertical") * SpaceShipSpeed;
            transform.Translate(velocity * Time.deltaTime, Space.World);
        }
    }

    private void AdjustShipRotation(){
        if(doARoll) return;
        //tilt forward and backward
        var rotationX = defaultXRotation - Input.GetAxis("Vertical") * 5f;
        //tilt right and left
        var rotationY = defaultYRotation - Input.GetAxis("Horizontal") * 15f;

        transform.localEulerAngles = new Vector3(rotationX, rotationY, defaultZRotation);
    }

    private void LaserAttack(){
        if(!Input.GetMouseButtonDown(0)) return;
        Vector3 leftLaserPos = new Vector3(transform.position.x - 0.6f, transform.position.y, transform.position.z);
        Vector3 rightLaserPos = new Vector3(transform.position.x + 0.6f, transform.position.y, transform.position.z);
        
        GameObject laserObject = Instantiate(LaserShot, leftLaserPos, Quaternion.identity);
        Destroy(laserObject, 5);
        laserObject = Instantiate(LaserShot, rightLaserPos, Quaternion.identity);
        Destroy(laserObject, 5);

        if(Random.Range(-1f, 1f) >= 0f) LaserShotAudio.clip = LaserShot1;
        else LaserShotAudio.clip = LaserShot2; 

        LaserShotAudio.Play();
    }

    private void DodgeRoll(){
        if(Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxis("Horizontal") > 0 && !doARoll){
            doARoll = true;
            rollDirection = 1f;
        }
        if(Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxis("Horizontal") < 0 && !doARoll){
            doARoll = true;
            rollDirection = -1f;
        }
        if(doARoll){
            if(rollCounter >= 360){
                doARoll = false;
                rollCounter = 0f;
                return;
            }
            transform.Rotate(0f, 1000f * rollDirection * Time.deltaTime, 0f);
            transform.Translate(Vector3.right * SpaceShipSpeed * rollDirection * 0.005f, Space.World);
            rollCounter += 1000f * Time.deltaTime;
        }
    }
}
