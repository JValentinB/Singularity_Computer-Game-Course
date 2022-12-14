using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    public float offset_x = 0;
    public float offset_y = 0;

    private float zPosition;
    // Start is called before the first frame update
    void Start()
    {
        zPosition = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = new Vector3(Mathf.Round(player.position.x * 1000) * 0.001f + offset_x, Mathf.Round(player.position.y * 1000) * 0.001f + offset_y, zPosition);
        this.transform.position = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);
    }
}
