using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShifterField : MonoBehaviour
{
    public Vector3 direction;
    public bool active;
    
    [HideInInspector] public float time = 0f;
    [HideInInspector] public float warningTime = 0f;
    [SerializeField] private Vector3 activePos;
    [SerializeField] private Vector3 inactivePos;


    private ParticleSystem ps;
    private ParticleSystem.MainModule _psMain;
    private ParticleSystem.ShapeModule _psShape;

    private List<Damageable> damageables = new List<Damageable>();

    private Coroutine timerCoroutine;

    void Start()
    {
        ps = transform.Find("ShifterActiveParticles").GetComponent<ParticleSystem>();
        _psMain = ps.main;
        _psShape = ps.shape;
        _psMain.startColor = new Color(1, 0.3322569f, 0f, 1f);
    }

    void Update()
    {
        ChangeMode();
    }

    private void ChangeMode()
    {
        if (active && !ps.isPlaying)
        {   
            foreach (Damageable damageable in damageables)
                activateShiftField(damageable);

            ps.Play();
            timerCoroutine = StartCoroutine(shifterTimer());
        }
        else if (!active && ps.isPlaying)
        {
            foreach (Damageable damageable in damageables)
                deactivateShiftField(damageable);

            ps.Stop();
            StopCoroutine(timerCoroutine);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        var ObjectToShift = col.gameObject;
        if (ObjectToShift.GetComponent<Damageable>())
        {
            if (!damageables.Contains(ObjectToShift.GetComponent<Damageable>()))
                damageables.Add(ObjectToShift.GetComponent<Damageable>());

            if(active)
                activateShiftField(ObjectToShift.GetComponent<Damageable>());
        }
    }

    private void OnTriggerExit(Collider col)
    {
        var ObjectToShift = col.gameObject;
        if (ObjectToShift.GetComponent<Damageable>())
        {
            damageables.Remove(ObjectToShift.GetComponent<Damageable>());

            if(active)
                deactivateShiftField(ObjectToShift.GetComponent<Damageable>());
        }
    }

    void activateShiftField(Damageable obj)
    {
        if (obj.GetComponent<Player>() && obj.GetComponent<Damageable>().getGravityShiftCount() == 0 )
        {
            Vector3 oldDirection = obj.GetComponent<Damageable>().getGravityShift();
            Camera.main.GetComponent<CameraControl>().turnCameraWithShift(oldDirection, direction, 0.75f);
        }

        obj.GetComponent<Damageable>().AddGravityShift(direction);
        obj.GetComponent<Damageable>().ShiftGravity();
    }

    void deactivateShiftField(Damageable obj)
    {
        obj.GetComponent<Damageable>().RemoveGravityShift(direction);
        obj.GetComponent<Damageable>().ShiftGravity();

        if (obj.GetComponent<Player>())
        {
            Vector3 newDirection = obj.GetComponent<Damageable>().getGravityShift();
            Camera.main.GetComponent<CameraControl>().turnCameraWithShift(direction, newDirection, 0.75f);
        }
    }

    IEnumerator shifterTimer()
    {
        yield return new WaitForSeconds(time - warningTime);

        float t = 0f;
        while (t < 0.66f * warningTime)
        {
            t += Time.deltaTime;

            float maxValue = Mathf.Lerp(0, 15, t / (warningTime * 0.66f));
            _psMain.gravityModifier = new ParticleSystem.MinMaxCurve(0, maxValue);
            yield return null;
        }

        t = 0f;
        while (t < 0.33f * warningTime)
        {
            t += Time.deltaTime;

            float minValue = Mathf.Lerp(0, 30, t / (warningTime * 0.33f));
            float maxValue = Mathf.Lerp(15, 30, t / (warningTime * 0.33f));
            _psMain.gravityModifier = new ParticleSystem.MinMaxCurve(minValue, maxValue);
            yield return null;
        }
        _psMain.gravityModifier = new ParticleSystem.MinMaxCurve(0, 0);
    }
}
