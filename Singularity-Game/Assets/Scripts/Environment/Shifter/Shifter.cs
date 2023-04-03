using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifter : MonoBehaviour
{
    [SerializeField] public Vector3 direction;
    [SerializeField] public float activeTime = 10f;
    [Range(0f, 1f)]
    public float activeVolume = 0.5f;
    [SerializeField] public bool noActiveTimer;
    public float warningTime = 3f;
    [Range(0f, 1f)]
    public float warningVolume = 0.5f;
    [Range(0.1f, 3f)]
    public float warningPitch = 1f;
    public float rechargingTime = 5f;

    [ColorUsageAttribute(true, true)]
    public Color crystalEmptyColor;
    private Color crystalChargedColor;

    private bool active = false;
    [SerializeField] public bool alwaysActive;

    private GameObject shifterField;
    private ShifterField shifterScript;
    private ShifterField[] shifterFields;
    private float shifterTTL;
    private bool colorShifting;
    private bool recharging;


    private ObjectSounds objectSounds;
    private float startPitch;

    private Coroutine consumeCoroutine;
    private Coroutine timerCoroutine;
    private Coroutine soundCoroutine;
    private Coroutine warnCoroutine;
    private Coroutine pitchCoroutine;
    private Coroutine tickingCoroutine;


    void Start()
    {

        if (transform.Find("ShifterField"))
        {
            shifterField = transform.Find("ShifterField").gameObject;
            shifterScript = shifterField.GetComponent<ShifterField>();
            shifterScript.active = noActiveTimer;
            shifterScript.time = activeTime;
            shifterScript.warningTime = warningTime;        }

        active = noActiveTimer;

        Transform shifterFieldsTransform = transform.Find("ShifterFields");
        if (shifterFieldsTransform)
        {
            shifterFields = shifterFieldsTransform.GetComponentsInChildren<ShifterField>();
            foreach (ShifterField shifterField in shifterFields)
            {
                shifterField.active = noActiveTimer;
                shifterField.time = activeTime;
                shifterField.warningTime = warningTime;
            }
        }

        if(transform.Find("Crystals"))
            crystalChargedColor = transform.Find("Crystals").GetChild(0).GetComponent<Renderer>().material.GetColor("_Glow_Color");

        objectSounds = GetComponent<ObjectSounds>();
    }

    public void ToggleShifter()
    {
        if (noActiveTimer || recharging) return;

        if (!colorShifting)
            consumeCoroutine = StartCoroutine(consumeCrystalEnergy());

        active = !active;
        if (active)
        {
            if (timerCoroutine != null)
                StopCoroutine(timerCoroutine);
            timerCoroutine = StartCoroutine(shifterTimer());
        }
        else{
            StopCoroutine(timerCoroutine);
            StopCoroutines();
        }

        if (shifterScript != null)
            shifterScript.active = !shifterScript.active;
        if (shifterFields != null)
            toggleShifterFields(active);


        if (colorShifting && !active)
        {
            StopCoroutine(consumeCoroutine);
            StartCoroutine(rechargeCrystalEnergy());
        }
    }

    void toggleShifterFields(bool active)
    {
        foreach (ShifterField shifterField in shifterFields)
        {
            shifterField.active = active;
        }
    }

    IEnumerator shifterTimer()
    {
        soundCoroutine = StartCoroutine(shifterSound());

        yield return new WaitForSeconds(activeTime - warningTime);
        warnCoroutine = StartCoroutine(warningSound());
        tickingCoroutine = StartCoroutine(tickingSound());

        yield return new WaitForSeconds(warningTime);
        StopCoroutines();

        active = false;
        if (shifterScript != null)
            shifterScript.active = false;
        if (shifterFields != null)
            toggleShifterFields(active);
    }

    IEnumerator shifterSound()
    {
        StartCoroutine(objectSounds.fadeInOut("Active", activeVolume, 0.5f));
        yield return new WaitForSeconds(activeTime - 0.5f);
        StartCoroutine(objectSounds.fadeInOut("Active", 0, 0.5f));
    }
    IEnumerator warningSound()
    {
        startPitch = objectSounds.getSourcePitch("Warning");

        StartCoroutine(objectSounds.fadeInOut("Warning", warningVolume, warningTime - 0.5f));
        pitchCoroutine = StartCoroutine(objectSounds.risePitch("Warning", warningPitch, warningTime - 0.5f));
        yield return new WaitForSeconds(warningTime - 0.5f);
        StartCoroutine(objectSounds.fadeInOut("Warning", 0, 0.5f));
        pitchCoroutine = StartCoroutine(objectSounds.risePitch("Warning", warningPitch + 0.5f, 0.5f));

        yield return new WaitForSeconds(0.5f);
        objectSounds.setSourcePitch("Warning", startPitch);
    }

    IEnumerator tickingSound()
    {
        objectSounds.Play("Ticking");

        float second = 0f;
        while (second < warningTime)
        {
            second++;
            yield return new WaitForSeconds(1f);
            objectSounds.Play("Ticking");
        }
        yield return new WaitForSeconds(0.8f);
        objectSounds.Play("Ticking");
    }

    IEnumerator consumeCrystalEnergy()
    {
        colorShifting = true;

        Renderer[] crystals = transform.Find("Crystals").GetComponentsInChildren<Renderer>();
        Color crystalColor = crystals[0].material.GetColor("_Glow_Color");

        float time = 0f;
        while (time < activeTime)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(crystalColor, crystalEmptyColor, time / activeTime);
            foreach (Renderer crystal in crystals)
            {
                crystal.material.SetColor("_Glow_Color", newColor);
            }
            yield return null;
        }
        StartCoroutine(rechargeCrystalEnergy());
        colorShifting = false;
    }

    IEnumerator rechargeCrystalEnergy()
    {
        recharging = true;
        yield return new WaitForSeconds(rechargingTime * 0.5f);
        Renderer[] crystals = transform.Find("Crystals").GetComponentsInChildren<Renderer>();
        Color crystalColor = crystals[0].material.GetColor("_Glow_Color");

        float time = 0f;
        while (time < rechargingTime * 0.5f)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(crystalColor, crystalChargedColor, time / (rechargingTime * 0.5f));
            foreach (Renderer crystal in crystals)
            {
                crystal.material.SetColor("_Glow_Color", newColor);
            }
            yield return null;
        }
        recharging = false;
    }

    void StopCoroutines()
    {
        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
            StartCoroutine(objectSounds.fadeInOut("Active", 0, 0.5f));
        }
        if (warnCoroutine != null)
        {
            StopCoroutine(warnCoroutine);
            StopCoroutine(pitchCoroutine);
            objectSounds.setSourcePitch("Warning", startPitch);

            StartCoroutine(objectSounds.fadeInOut("Warning", 0, 0.5f));
            StartCoroutine(objectSounds.risePitch("Warning", warningPitch + 0.5f, 0.5f));
        }
        if (tickingCoroutine != null)
            StopCoroutine(tickingCoroutine);
    }
}
