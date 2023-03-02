using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBender : MonoBehaviour
{
    [Range(0f, 1f)]
    public float bendingAmount = 0.05f;
    [Range(0f, 1f)]
    public float bendingDistance = 1f; // is bending if the actual distance is smaller than bendingDistance * colliderWidth
    
}
