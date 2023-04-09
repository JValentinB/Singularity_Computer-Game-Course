using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float offset_x = 0;
    public float offset_y = 0;
    public bool rotateOnShift = true;

    [Header("Camera Follows Player")]
    public bool followPlayer = true;
    [Range(0, 2)]
    public float smoothTime = 0.3f;
    [Header("Camera Follows Mouse")]
    public bool followMouse = false;
    [Range(0, 1)]
    public float mouseFollowRatio = 0.5f;
    [Range(0, 2)]
    public float mouseFollowSpeed = 0.5f;
    [Header("Camera Follows Object")]
    public bool followPoint = false; // follows a point between player and a given object (e.g. a boss)
    public GameObject objectToFollow;
    [HideInInspector] public float followPointRatio = 0.5f;
    [HideInInspector] public Vector3 downDirection = Vector3.down;
    // public float offset_z = 0;
    // public bool overlayActive = true;

    private Transform player;
    private Vector3 velocity = Vector3.zero;
    private float zPosition;

    private Camera overlayCamera;

    private Coroutine turnCameraCoroutine;
    private bool isTurningCamera = false;
    private float directionTurningTo = 0;

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
        if (followPoint && objectToFollow != null)
        {   
            Vector3 targetPosition = Vector3.Lerp(Vector3.Scale(player.position, new Vector3(1, 1, 0)) + new Vector3(offset_x, offset_y, zPosition),
                                                  Vector3.Scale(objectToFollow.transform.position, new Vector3(1, 1, 0)) + new Vector3(offset_x, offset_y, zPosition),
                                                  followPointRatio);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else if (followMouse || (followPoint && objectToFollow == null))
        {   
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            mousePosition.z = zPosition;

            Vector3 playerPos = Vector3.Lerp(transform.position, Vector3.Scale(player.position, new Vector3(1, 1, 0)) + new Vector3(offset_x, offset_y, zPosition), smoothTime);
            Vector3 mousePos = Vector3.Lerp(transform.position, Vector3.Scale(mousePosition, new Vector3(1, 1, 0)) + new Vector3(offset_x, offset_y, zPosition), mouseFollowRatio);

            transform.position = Vector3.SmoothDamp(playerPos, mousePos, ref velocity, mouseFollowSpeed);
        }
        else if (followPlayer)
        {   
            Vector3 targetPosition = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    public IEnumerator stopFollowing(float time)
    {
        followPlayer = false;
        yield return new WaitForSeconds(time);
        followPlayer = true;
    }

    public void turnCameraWithShift(Vector3 startDirection, Vector3 direction, float time)
    {
        if (!rotateOnShift) return;

        if (directionTurningTo != getRotation(direction))
        {   
            downDirection = direction;
            if (turnCameraCoroutine != null)
                StopCoroutine(turnCameraCoroutine);
            turnCameraCoroutine = StartCoroutine(turnCamera(startDirection, direction, time));
        }
    }

    public IEnumerator turnCamera(Vector3 startDirection, Vector3 direction, float time)
    {
        isTurningCamera = true;
        float startRotation = transform.eulerAngles.z;
        // startRotation = startRotation > 180 ? startRotation - 360 : startRotation;
        float endRotation = getRotation(direction);
        directionTurningTo = endRotation;

        if (startDirection == Vector3.left && direction == Vector3.down)
            endRotation += 360;
        else if (startDirection == Vector3.up && direction == Vector3.left)
            startRotation -= 360;

        // if(startRotation > 200) startRotation -= 360;
        // else if(startRotation < -200) startRotation += 360;

        float angleDiff = endRotation - startRotation;
        // Debug.Log(startRotation + " " + endRotation + " " + angleDiff);

        if (angleDiff > 180) angleDiff -= 360;
        else if (angleDiff < -180) angleDiff += 360;

        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            float step = angleDiff * (Time.deltaTime / time);
            transform.Rotate(new Vector3(0, 0, step));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, endRotation));
        isTurningCamera = false;
    }

    float getRotation(Vector3 direction)
    {
        float rotation = 0;
        if (direction == Vector3.left) rotation = -90;
        else if (direction == Vector3.up) rotation = 180;
        else if (direction == Vector3.right) rotation = 90;
        return rotation;
    }

    public IEnumerator changeFollowPoint(float speed, bool endState, float time)
    {   
        if(objectToFollow == null){
            followMouse = !endState;
            yield break;
        }

        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;

            if (endState)
            {
                // lerp between 1f and smoothTime value
                float smooth = Mathf.Lerp(speed, smoothTime, t / time);
                Vector3 targetPosition = Vector3.Lerp(Vector3.Scale(player.position, new Vector3(1, 1, 0)) + new Vector3(offset_x, offset_y, zPosition),
                                                      Vector3.Scale(objectToFollow.transform.position, new Vector3(1, 1, 0)) + new Vector3(offset_x, offset_y, zPosition),
                                                      followPointRatio);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smooth);
            }
            else
            {
                float smooth = Mathf.Lerp(speed, smoothTime, t / time);
                Vector3 targetPosition = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smooth);
            }

            yield return null;
        }
        followPoint = endState;
        // followPlayer = !endState;
        followMouse = !endState;
    }
}
