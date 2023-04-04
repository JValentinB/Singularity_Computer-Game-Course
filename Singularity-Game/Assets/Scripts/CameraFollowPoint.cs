using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPoint : MonoBehaviour
{
    public GameObject objectToFollow;
    public float smoothTime = 1f;
    public float followPointRatio = 0.5f;

    Coroutine changeFollowPointCoroutine;

    private CameraControl cameraControl;
    void Start()
    {
        cameraControl = Camera.main.GetComponent<CameraControl>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cameraControl.objectToFollow = objectToFollow;
            cameraControl.followPlayer = false;
            cameraControl.followMouse = false;
            cameraControl.followPointRatio = followPointRatio;

            if(changeFollowPointCoroutine != null)
                StopCoroutine(changeFollowPointCoroutine);
            changeFollowPointCoroutine = StartCoroutine(cameraControl.changeFollowPoint(smoothTime, true, 5f));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cameraControl.followPoint = false;
            cameraControl.followPointRatio = followPointRatio;

            if(changeFollowPointCoroutine != null)
                StopCoroutine(changeFollowPointCoroutine);
            changeFollowPointCoroutine = StartCoroutine(cameraControl.changeFollowPoint(smoothTime, false, 5f));
        }
    }
}
