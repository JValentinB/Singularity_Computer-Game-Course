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
    public bool followPoint = false; // follows a point between player and a given object (e.g. a boss)
    public GameObject objectToFollow;
    [HideInInspector] public float followPointRatio = 0.5f;
    // public float offset_z = 0;
    public bool overlayActive = true;

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
        if (followPoint)
        {
            Vector3 targetPosition = Vector3.Lerp(Vector3.Scale(player.position, new Vector3(1, 1, 0)) + new Vector3(offset_x, offset_y, zPosition),
                                                  Vector3.Scale(objectToFollow.transform.position, new Vector3(1, 1, 0)) + new Vector3(offset_x, offset_y, zPosition),
                                                  followPointRatio);
            // Vector3 targetPosition = new Vector3((player.position.x + objectToFollow.transform.position.x) / followPointRatio + offset_x, (player.position.y + objectToFollow.transform.position.y) / followPointRatio + offset_y, zPosition);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
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

    public IEnumerator changeFollowPoint(float speed, bool endState, float time)
    {
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
        followPlayer = !endState;
    }
}
