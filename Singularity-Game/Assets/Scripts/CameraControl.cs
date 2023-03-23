using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float offset_x = 0;
    public float offset_y = 0;
    public bool rotateOnShift = true;
    public bool followPlayer = true;
    public float smoothTime = 0.3f;
    // public float offset_z = 0;
    public bool overlayActive = true;

    [HideInInspector] public float bottom;
    [HideInInspector] public float top;
    [HideInInspector] public float left;
    [HideInInspector] public float right;

    private Transform player;
    private Vector3 velocity = Vector3.zero;
    private float zPosition;
    private Vector3 downDirection;
    private Camera overlayCamera;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        zPosition = this.transform.position.z;
        overlayCamera = transform.GetChild(0).GetComponent<Camera>();
        this.transform.position = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);  
        
        downDirection = Vector3.down;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (followPlayer)
        {
            Vector3 targetPosition = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        if (overlayActive)
        {
            overlayCamera.fieldOfView = this.GetComponent<Camera>().fieldOfView;
            // // If there is something between player and Camera, activate the Overlay Camera
            // if (checkSpace() && overlayActive)
            // {
            //     overlayCamera.enabled = true;
            // }
            // else overlayCamera.enabled = false;
        }
    }

    // Checks whether there is a mesh between the Camera and the player
    bool checkSpace()
    {
        Ray ray = new Ray(player.position + new Vector3(0, 1, 0), transform.position - player.position);
        RaycastHit hit;

        //Debug.DrawRay(player.position + new Vector3(0, 1, 0), transform.position - player.position, Color.red, 2);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.distance < 20;
        }
        return false;
    }

    public IEnumerator stopFollowing(float time)
    {
        followPlayer = false;
        yield return new WaitForSeconds(time);
        followPlayer = true;
    }

    // turn Camera in direction of gravity by lerp
    public IEnumerator turnCamera(Vector3 direction, float time)
    {
        if (!rotateOnShift) yield break;

        float elapsedTime = 0;
        Vector3 startRotation = transform.rotation.eulerAngles;
        Vector3 endRotation = new Vector3(startRotation.x, 0, 0);
        if (direction == Vector3.left) endRotation = new Vector3(0, 0, -90);
        else if (direction == Vector3.up) endRotation = new Vector3(0, 0, 180);
        else if (direction == Vector3.right) endRotation = new Vector3(0, 0, 90);

        while (elapsedTime < time)
        {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(startRotation, endRotation, (elapsedTime / time)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(endRotation);
    }

    // calculate the boundaries of the Camera with perspective projection at distance from camera
    public void calculateBoundaries()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, distance));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distance));

        bottom = bottomLeft.y;
        top = topRight.y;
        left = bottomLeft.x;
        right = topRight.x;
    }
    
}
