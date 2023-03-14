using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlSpace : MonoBehaviour
{
    [SerializeField] private Transform player;
    public float offset_x = 0;
    public float offset_y = 0;
    // public float offset_z = 0;

    private float zPosition;

    // Start is called before the first frame update
    void Start()
    {
        zPosition = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        
        this.transform.position = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);
    }
}
