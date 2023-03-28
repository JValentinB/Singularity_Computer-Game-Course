using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlSpace : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public bool followPlayer;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.1f;
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
        if(!followPlayer) return;
        Vector3 targetPosition = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
