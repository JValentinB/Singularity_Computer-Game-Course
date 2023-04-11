using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// This script is possible thanks to Sebastian Lague: https://www.youtube.com/watch?v=saAQNRSYU9k
public class LaserBeam : MonoBehaviour
{
    [Header("Laser")]
    public float laserWidth = 0.1f;
    private float laserWidthAtStart;
    public int laserDamage = 1;
    public int laserDamageOnGolem = 1;
    public float laserForce = 5f;
    public float laserChargeAmount = 1f;
    [Header("Path")]
    public float anchorDistance = 4f;
    public float maxLength = 100f;
    public float timer = 1f;

    private PathCreator pathCreator;
    [HideInInspector] public BezierPath bezierPath;
    private VertexPath path;

    private MeshCollider meshCollider;
    private MeshFilter meshFilter;

    private int lastAnchor;
    private float thickness = 2f;
    private Vector3 startDirection;

    List<Vector3> anchorPositions = new List<Vector3>();
    List<Vector3> anchorPositionsAtStart = new List<Vector3>();
    List<Vector3> anchorPositionsWithoutHit = new List<Vector3>();
    List<int> anchorsOfInterest = new List<int>();

    [HideInInspector] public List<LaserBender> benders = new List<LaserBender>();

    [HideInInspector] public bool beamInteraction = false;
    [HideInInspector] public bool benderReset = false;
    private bool beamReseted = false;
    bool isBlocked = false;
    bool laserHit = false;
    [HideInInspector] public bool laserRotated = false;
    [HideInInspector] public float rotationAngle = 0f;

    public bool isActive = false;
    [HideInInspector] public bool becameActive = false;
    [HideInInspector] public bool becameInactive = false;
    private LaserEmitter laserEmitter;

    void Start()
    {
        pathCreator = GetComponent<PathCreator>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        laserWidthAtStart = laserWidth;

        bezierPath = pathCreator.bezierPath;
        startDirection = (bezierPath[bezierPath.NumPoints - 1] - bezierPath[bezierPath.NumPoints - 4]).normalized;

        increaseLength(0);
        addAnchors(0);

        anchorPositionsAtStart = getAnchorPoints();
        anchorPositionsWithoutHit = getAnchorPoints();
        lastAnchor = bezierPath.NumPoints - 1;

        anchorsOfInterest.Add(3);
        anchorsOfInterest.Add(lastAnchor);

        // Find the LaserEmitter in other children of same parent
        laserEmitter = transform.parent.GetComponentInChildren<LaserEmitter>();
        isActive = laserEmitter.IsEmitting();
        if (!isActive)
            return;

        Mesh mesh = createMeshFromPath(laserWidth);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void Update()
    {   
        if(laserRotated){
            laserRotated = false;
            rotateLaser(rotationAngle);
        }
        if (becameInactive)
        {
            becameInactive = false;
            anchorPositions = new List<Vector3>(anchorPositionsAtStart);
            meshFilter.sharedMesh = null;
            meshCollider.sharedMesh = null;
        }

        if (!isActive)
            return;
        if (becameActive)
        {
            becameActive = false;
            OnLaserCollision();
            Mesh mesh = createMeshFromPath(laserWidth);
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        if (beamInteraction)
        {
            bendPath();
            OnLaserCollision();
        }
        else if (benderReset)
        {
            anchorPositions = new List<Vector3>(anchorPositionsAtStart);
            anchorPositionsWithoutHit = new List<Vector3>(anchorPositionsAtStart);
            beamReseted = true;
        }
        else
        {
            anchorPositions = new List<Vector3>(anchorPositionsWithoutHit);
            OnLaserCollision();
        }

        if (beamInteraction || isBlocked || benderReset || laserHit)
        {
            // if(benderReset)Debug.Log(compareVectorLists(anchorPositions, anchorPositionsAtStart));
            for (int anchor = 3; anchor < bezierPath.NumPoints; anchor += 3)
            {
                bezierPath.MovePoint(anchor, anchorPositions[anchor / 3]);
            }
            pathCreator.bezierPath = bezierPath;

            Mesh mesh = createMeshFromPath(laserWidth);
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;

            beamInteraction = false;
            isBlocked = false;
            laserHit = false;
            if (beamReseted)
                benderReset = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;

        if (!other.isTrigger && !other.transform.GetComponent<LaserBender>() && !other.transform.GetComponent<LaserEmitter>())
        {
            isBlocked = true;
        }
    }

    void OnLaserCollision()
    {
        // Find hit point
        RaycastHit hit;
        Vector3 hitPoint = Vector3.zero;
        bool isHitting = false;


        Vector3 nextPoint = anchorPositions[1] + transform.position;
        for (int i = 0; i < anchorsOfInterest.Count - 1; i++)
        {
            int nextAnchor = anchorsOfInterest[i + 1];
            Vector3 point = nextPoint;
            nextPoint = anchorPositions[nextAnchor / 3] + transform.position;
            Vector3 direction = nextPoint - point;

            if (Physics.Raycast(point, direction, out hit, direction.magnitude))
            {
                if (hit.transform.GetComponent<LaserBender>())
                    break;
                // Apply Damage
                if (hit.transform.GetComponent<Damageable>())
                {
                    hit.transform.GetComponent<Damageable>().ApplyDamage(laserDamage);
                    invertVelocity(hit.rigidbody);
                }
                if (hit.transform.tag == "GolemHeart" && laserDamageOnGolem > 0)
                {
                    hit.transform.parent.GetComponent<StoneGolemBoss>().ApplyDamage(laserDamageOnGolem, this);
                }
                if (hit.transform.GetComponent<InstablePlatform>())
                {
                    hit.transform.GetComponent<InstablePlatform>().ApplyDamage(laserDamage);
                }
                if (hit.transform.GetComponent<LaserEmitter>())
                {
                    chargingEmitter(hit.transform.GetComponent<LaserEmitter>());
                }
                laserHit = true;
                isHitting = true;
                hitPoint = hit.point;
                break;
            }
        }
        if (!isHitting)
        {
            return;
        }

        // Find closest anchor
        int closestAnchor = 3;
        float minDistance = Vector3.Distance(hitPoint, anchorPositions[3] + transform.position);
        for (int anchor = 6; anchor < bezierPath.NumPoints; anchor += 3)
        {
            float distance = Vector3.Distance(hitPoint, anchorPositions[anchor / 3] + transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestAnchor = anchor;
            }
        }

        for (int anchor = closestAnchor; anchor < bezierPath.NumPoints; anchor += 3)
        {
            anchorPositions[anchor / 3] = hitPoint - transform.position;
        }
    }

    void chargingEmitter(LaserEmitter otherEmitter)
    {   
        if(!otherEmitter.chargeable) return;
        
        otherEmitter.charge += laserChargeAmount;
        laserEmitter.charge -= laserChargeAmount;
        laserWidth = laserWidthAtStart * (laserEmitter.charge / laserEmitter.maxCharge);
        if (laserEmitter.charge <= 0)
        {
            otherEmitter.startEmitting();
            laserEmitter.stopEmitting();
            becameInactive = true;
            isActive = false;
        }
    }

    void increaseLength(float angleInDegrees)
    {
        Vector3 origin = bezierPath[bezierPath.NumPoints - 4];
        Vector3 direction = Quaternion.AngleAxis(angleInDegrees, Vector3.forward) * startDirection;

        bezierPath.MovePoint(bezierPath.NumPoints - 1, origin + maxLength * direction);
    }

    void addAnchors(int index)
    {
        int anchor = index;
        float distance = Vector3.Distance(bezierPath[bezierPath.NumPoints - 1],
                                          bezierPath[bezierPath.NumPoints - 4]);
        Vector3 direction = Vector3.Normalize(bezierPath[bezierPath.NumPoints - 1] - bezierPath[anchor]);

        while (distance > anchorDistance)
        {
            Vector3 anchorPosition = bezierPath[anchor] + anchorDistance * direction;

            bezierPath.SplitSegment(anchorPosition, anchor / 3, 0.5f);

            distance = Vector3.Distance(bezierPath[bezierPath.NumPoints - 1],
                                          bezierPath[bezierPath.NumPoints - 4]);
            anchor += 3;
        }
        for (int i = 0; i < bezierPath.NumAnchorPoints; i++)
        {
            anchorPositions.Add(bezierPath[i * 3]);
        }
    }

    void bendPath()
    {
        anchorsOfInterest.Clear();
        anchorsOfInterest.Add(3);
        anchorsOfInterest.Add(lastAnchor);

        anchorPositions = new List<Vector3>(anchorPositionsAtStart);
        List<LaserBender> bendersCopy = new List<LaserBender>();
        for (int anchor = 3; anchor < bezierPath.NumPoints - 2; anchor += 3)
        {
            Vector3 averageDirection = Vector3.zero;
            foreach (LaserBender bender in benders)
            {
                Vector3 benderPosition = bender.transform.position - transform.position;
                float distance = Vector3.Distance(benderPosition, anchorPositions[anchor / 3]);

                if (distance < (bender.radius * bender.bendingDistance))
                {
                    float t = distance / (bender.radius * bender.bendingDistance);
                    Vector3 straightLinePosition = getStraightLinePosition(anchor + 3);
                    Vector3 interpolationPosition = Vector3.Lerp(straightLinePosition, benderPosition, bender.bendingAmount);
                    Vector3 newPosition = Vector3.Lerp(interpolationPosition, straightLinePosition, t);
                    averageDirection += newPosition - anchorPositions[(anchor + 3) / 3];

                    sortedAdd(anchorsOfInterest, anchor);
                    if (!bendersCopy.Contains(bender))
                        bendersCopy.Add(bender);
                }
            }
            anchorPositions[(anchor + 3) / 3] += averageDirection;
            // all following anchors move with the new anchor in a straight line
            Vector3 direction = Vector3.Normalize(anchorPositions[(anchor + 3) / 3] - anchorPositions[anchor / 3]) * anchorDistance;
            for (int j = anchor + 6; j < bezierPath.NumPoints; j += 3)
            {
                anchorPositions[j / 3] = anchorPositions[(j - 3) / 3] + direction;
            }
        }
        anchorPositionsWithoutHit = new List<Vector3>(anchorPositions);
        benders = new List<LaserBender>(bendersCopy);
    }

    List<Vector3> getAnchorPoints()
    {
        List<Vector3> points = new List<Vector3>();
        for (int anchor = 0; anchor < bezierPath.NumPoints; anchor += 3)
        {
            points.Add(bezierPath[anchor]);
        }
        return points;
    }

    public void rotateLaser(float angleInDegrees)
    {
        List<Vector3> points = rotateAnchorPoints(angleInDegrees);
        anchorPositionsAtStart = new List<Vector3>(points);
        anchorPositionsWithoutHit = new List<Vector3>(points);
    }

    List<Vector3> rotateAnchorPoints(float angleInDegrees){
        Vector3 origin = bezierPath[0];
        Vector3 direction = (Quaternion.AngleAxis(angleInDegrees, Vector3.forward) * startDirection).normalized;

        List<Vector3> points = new List<Vector3>();
        points.Add(origin);
        Vector3 lastPoint = origin;
        for (int anchor = 3; anchor < bezierPath.NumPoints; anchor += 3)
        {
            Vector3 anchorPosition = lastPoint + anchorDistance * direction;
            points.Add(anchorPosition);

            lastPoint = anchorPosition;
        }
        return points;
    }

    // void copyList(List<Vector3> list1, List<Vector3> list2)
    // {
    //     list1.Clear();
    //     foreach (Vector3 point in list2)
    //     {
    //         list1.Add(point);
    //     }
    // }

    Vector3 getStraightLinePosition(int index)
    {
        Vector3 position = bezierPath[index];
        if (index >= 6)
        {
            Vector3 direction = Vector3.Normalize(bezierPath[index - 3] - bezierPath[index - 6]);

            position = bezierPath[index - 3] + anchorDistance * direction;
        }
        return position;
    }

    void sortedAdd(List<int> list, int value)
    {
        int index = list.BinarySearch(value);
        if (index < 0)
        {
            index = ~index;
        }
        list.Insert(index, value);
    }

    void invertVelocity(Rigidbody rb)
    {
        Vector3 velocity = -Vector3.Normalize(rb.velocity) * 5000 * laserForce;
        rb.transform.position -= Vector3.Normalize(rb.velocity) * 0.1f;
        rb.AddForce(velocity);
        // rb.velocity = Vector3.ClampMagnitude(rb.velocity, 2f);
    }

    // Credit to Sebastian Lague on YouTube
    Mesh createMeshFromPath(float width, float lengthOffset = 0f)
    {
        path = pathCreator.path;
        Vector3[] verts = new Vector3[path.NumPoints * 8];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];


        int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
        int[] roadTriangles = new int[numTris * 3];
        int[] underRoadTriangles = new int[numTris * 3];
        int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

        int vertIndex = 0;
        int triIndex = 0;

        // Vertices for the top of the road are layed out:
        // 0  1
        // 8  9
        // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
        int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
        int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

        bool usePathNormals = !(path.space == PathSpace.xyz);

        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
            Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

            // Find position to left and right of current path vertex
            Vector3 vertSideA;
            Vector3 vertSideB;
            if (lengthOffset > 0f && i == path.NumPoints - 1)
            {
                Vector3 direction = path.GetPoint(i) - path.GetPoint(i - 1);
                vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(width) - transform.position + direction * lengthOffset;
                vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(width) - transform.position + direction * lengthOffset;
            }
            else
            {
                vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(width) - transform.position;
                vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(width) - transform.position;
            }
            // Add top of road vertices
            verts[vertIndex + 0] = vertSideA;
            verts[vertIndex + 1] = vertSideB;
            // Add bottom of road vertices
            verts[vertIndex + 2] = vertSideA - localUp * thickness;
            verts[vertIndex + 3] = vertSideB - localUp * thickness;

            // Duplicate vertices to get flat shading for sides of road
            verts[vertIndex + 4] = verts[vertIndex + 0];
            verts[vertIndex + 5] = verts[vertIndex + 1];
            verts[vertIndex + 6] = verts[vertIndex + 2];
            verts[vertIndex + 7] = verts[vertIndex + 3];

            // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
            uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
            uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

            // Top of road normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;
            // Bottom of road normals
            normals[vertIndex + 2] = -localUp;
            normals[vertIndex + 3] = -localUp;
            // Sides of road normals
            normals[vertIndex + 4] = -localRight;
            normals[vertIndex + 5] = localRight;
            normals[vertIndex + 6] = -localRight;
            normals[vertIndex + 7] = localRight;

            // Set triangle indices
            if (i < path.NumPoints - 1 || path.isClosedLoop)
            {
                for (int j = 0; j < triangleMap.Length; j++)
                {
                    roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                    // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                    underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                }
                for (int j = 0; j < sidesTriangleMap.Length; j++)
                {
                    sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                }

            }

            vertIndex += 8;
            triIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Collider Mesh";
        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.subMeshCount = 3;
        mesh.SetTriangles(roadTriangles, 0);
        mesh.SetTriangles(underRoadTriangles, 1);
        mesh.SetTriangles(sideOfRoadTriangles, 2);
        mesh.RecalculateBounds();


        return mesh;
    }


    void printVectorArray(List<Vector3> array)
    {
        for (int i = 0; i < array.Count; i++)
        {
            Debug.Log(i + ": " + array[i]);
        }
    }
    void printIntArray(List<int> array)
    {
        for (int i = 0; i < array.Count; i++)
        {
            Debug.Log(i + ": " + array[i]);
        }
    }

    bool compareVectorLists(List<Vector3> list1, List<Vector3> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }
        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
            {
                return false;
            }
        }
        return true;
    }

}
