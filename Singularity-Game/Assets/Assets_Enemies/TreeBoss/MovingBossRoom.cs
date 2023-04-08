using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBossRoom : MonoBehaviour
{
    private Vector3 startPosition;
    public Vector3 targetPosition;
    public float timeToMove = 5f;
    public Vector3 shakeIntensity = new Vector3(0.1f, 0.1f, 0.1f);
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
    }

    public IEnumerator moveRoom()
    {
        float time = 0;
        Vector3 shakeOffset = Vector3.zero;

        while (time < timeToMove)
        {
            shakeOffset = Vector3.Scale(Random.insideUnitSphere, shakeIntensity);
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (time / timeToMove)) + shakeOffset;
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
    }
}
    