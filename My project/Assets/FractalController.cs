using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalController : MonoBehaviour
{
    public Material FractalMaterial;
    public float ZoomSpeed = 1.0f;
    public float MoveSpeed = 0.1f;

    private Vector2 _offset;
    private float _zoom = 1.0f;

    private void Start(){
        FractalMaterial = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _offset.x += horizontal * MoveSpeed / _zoom;
        _offset.y += vertical * MoveSpeed / _zoom;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _zoom *= Mathf.Pow(2, scroll * ZoomSpeed);

        FractalMaterial.SetFloat("_Zoom", _zoom);
        FractalMaterial.SetVector("_Offset", _offset);
    }
}
