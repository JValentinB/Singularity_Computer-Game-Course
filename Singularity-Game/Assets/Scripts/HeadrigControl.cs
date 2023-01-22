using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadrigControl : MonoBehaviour
{
    private Camera mainCamera;
    GameObject player;
    Player playerValue;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerValue = player.GetComponent<Player>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HeadControl();
    }

    void HeadControl() 
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        Vector3 position = mainCamera.ScreenToWorldPoint(mousePos);
        //Debug.Log(playerValue.direction);
        transform.position = new Vector3(player.transform.position.x + (playerValue.direction * 5), position.y, 0);
    }
}
