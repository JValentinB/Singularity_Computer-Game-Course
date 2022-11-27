using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// 
public class FeetRigControl : MonoBehaviour
{

    [SerializeField] private Transform footTransformRight;
    [SerializeField] private Transform footTransformLeft;
    private Transform[] bothFootTransforms;
    [SerializeField] private Transform footTargetRight;
    [SerializeField] private Transform footTargetLeft;
    private Transform[] bothFootTargets;

    [SerializeField] private GameObject footRigRight;
    [SerializeField] private GameObject footRigLeft;
    private TwoBoneIKConstraint[] bothFootIKConstrains;

    private LayerMask groundLayerMask;
    private LayerMask hitLayer;

    private float maxHitDistance = 1.2f;
    private float addedHeight = 1f;

    private bool[] allGroundSpherecastHits;

    private Vector3[] allHitNormals;
    private float angleAboutX;
    private float angleAboutZ;
    private float yOffset = 0.05f;

    [SerializeField] Animator animator;
    private float[] bothFootWeights;
        
    // Start is called before the first frame update
    void Start()
    {
        bothFootTransforms = new Transform[2];
        bothFootTargets = new Transform[2];
        bothFootIKConstrains = new TwoBoneIKConstraint[2];

        bothFootTransforms[0] = footTransformRight;
        bothFootTransforms[1] = footTransformLeft;

        bothFootTargets[0] = footTargetRight;
        bothFootTargets[1] = footTargetLeft;

        bothFootIKConstrains[0] = footRigRight.GetComponent<TwoBoneIKConstraint>();
        bothFootIKConstrains[1] = footRigLeft.GetComponent<TwoBoneIKConstraint>();

        groundLayerMask = LayerMask.NameToLayer("Ground");

        allGroundSpherecastHits = new bool[3];
        allHitNormals = new Vector3[2];

        bothFootWeights = new float[2];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateCharacterFeet();
    }
    private void CheckGroundBelow(out Vector3 hitPoint, out bool gotGroundSpherecastHit, out Vector3 hitNormal, out LayerMask hitLayer,
        out float currentHitDistance, Transform objectTransform, int checkForLayerMask, float maxHitdistance, float addedHeight)
    {
        RaycastHit hit;
        Vector3 startSpherecast = objectTransform.position + new Vector3(0f, addedHeight, 0f);

        if (checkForLayerMask == -1 )
        {
            Debug.LogError("Layer does not exist!");
            currentHitDistance = 0;
            hitLayer = LayerMask.NameToLayer("Player");
            hitNormal = Vector3.up;
            hitPoint = objectTransform.position;
            gotGroundSpherecastHit = false;
        }
        else
        {
            int layerMask = (1 << checkForLayerMask);
            if (Physics.SphereCast(startSpherecast, .2f, Vector3.down, out hit, maxHitdistance, layerMask))
            {
                hitLayer = hit.transform.gameObject.layer;
                currentHitDistance = hit.distance - addedHeight;
                hitNormal = hit.normal;
                gotGroundSpherecastHit = true;
                hitPoint = hit.point;   
            }
            else
            {
                gotGroundSpherecastHit = false;
                currentHitDistance = 0;
                hitLayer = LayerMask.NameToLayer("Player");
                hitNormal = Vector3.up;
                hitPoint = objectTransform.position;
            }
        } 
    }

    Vector3 ProjectOnContactPlane(Vector3 vector, Vector3  hitNormal)
    {
        return vector - hitNormal * Vector3.Dot(vector, hitNormal);  
    }

    private void ProjectedAxisAngles(out float angleAboutX, out float angleAboutZ, Transform footTarget, Vector3 hitNormal)
    {
        Vector3 xAxisProjected = ProjectOnContactPlane(footTarget.forward, hitNormal).normalized;
        Vector3 zAxisProjected = ProjectOnContactPlane(footTarget.right, hitNormal).normalized;
        angleAboutX = Vector3.SignedAngle(footTarget.forward, xAxisProjected, footTarget.right);
        angleAboutZ = Vector3.SignedAngle(footTarget.right, zAxisProjected, footTarget.forward);
    }

    private void RotateCharacterFeet()
    {
        bothFootWeights[0] = animator.GetFloat("RightFoot Weight");
        bothFootWeights[1] = animator.GetFloat("LeftFoot Weight");
        for (int i = 0; i < 2; i++)
        {
            bothFootIKConstrains[i].weight = bothFootWeights[i];

            CheckGroundBelow(out Vector3 hitPoint, out allGroundSpherecastHits[i], out Vector3 hitNormal, out hitLayer, out _,
                             bothFootTransforms[i], groundLayerMask, maxHitDistance, addedHeight);
            allHitNormals[i] = hitNormal;

            if (allGroundSpherecastHits[i])
            {
                ProjectedAxisAngles(out angleAboutX, out angleAboutZ, bothFootTransforms[i], allHitNormals[i]);

                bothFootTargets[i].position = new Vector3(bothFootTransforms[i].position.x, hitPoint.y + yOffset, bothFootTransforms[i].position.z);

                bothFootTargets[i].rotation = bothFootTransforms[i].rotation;
                bothFootTargets[i].localEulerAngles = new Vector3(bothFootTargets[i].localEulerAngles.x,
                                                                  bothFootTargets[i].localEulerAngles.y,
                                                                  bothFootTargets[i].localEulerAngles.z);

            }
            else
            {
                bothFootTargets[i].position = bothFootTransforms[i].position;
                bothFootTargets[i].rotation = bothFootTransforms[i].rotation;
            }
        }
    }
}
