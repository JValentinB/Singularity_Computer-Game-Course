using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraControlSpace : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public bool followPlayer;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.1f;
    public float offset_x = 0;
    public float offset_y = 0;
    private float zPosition;
    private AudioSource bgm;
    private Bloom bloomControl;
    private bool incBloom;

    // Start is called before the first frame update
    void Start()
    {
        zPosition = this.transform.position.z;
        GetComponent<Volume>().profile.TryGet(out bloomControl);
        bgm = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeBloomThreshold();
        if(!followPlayer) return;
        Vector3 targetPosition = new Vector3(player.position.x + offset_x, player.position.y + offset_y, zPosition);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        DecreaseAudioVolume();
    }

    private void ChangeBloomThreshold(){
        if(incBloom && bloomControl.threshold.value >= 1.3f) incBloom = false;
        else if(!incBloom && bloomControl.threshold.value <= 0.8f) incBloom = true;

        if(incBloom) bloomControl.threshold.value += Time.deltaTime*0.1f;
        else bloomControl.threshold.value -= Time.deltaTime*0.1f;
    }   

    private void DecreaseAudioVolume(){
        if(bgm.volume <= 0.1) return; 
        bgm.volume -= Time.deltaTime*0.1f;
    }
}
