using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    public float offset_x = 0;
    public float offset_y = 0;
    public bool overlayActive = true;

    private float zPosition;
    private Camera overlayCamera;

    // Start is called before the first frame update
    void Start()
    {
        zPosition = this.transform.position.z;
        overlayCamera = transform.GetChild(0).GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = new Vector3(Mathf.Round(player.position.x * 1000) * 0.001f + offset_x, Mathf.Round(player.position.y * 1000) * 0.001f + offset_y, zPosition);
        this.transform.position = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);

        // If there is something between player and Camera, activate the Overlay Camera
        if (checkSpace() && overlayActive)
        {
            overlayCamera.enabled = true;
        }
        else overlayCamera.enabled = false;
    }

    // Checks whether there is a mesh between the Camera and the player
    bool checkSpace()
    {
        Ray ray = new Ray(player.position + new Vector3(0,1,0), transform.position - player.position);
        RaycastHit hit;

        //Debug.DrawRay(player.position + new Vector3(0, 1, 0), transform.position - player.position, Color.red, 2);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.distance < 20;
        }
        return false;
    }
}
