using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// This script is possible thanks to Sebastian Lague: https://www.youtube.com/watch?v=saAQNRSYU9k
public class LaserBeam : MonoBehaviour
{
    [Header("Laser")]
    public float laserWidth = 0.1f;
    public int laserDamage = 1;
    public float laserForce = 5f;
    [Header("Path")]
    public float anchorDistance = 4f;
    public float maxLength = 100f;
    public float timer = 1f;

    private PathCreator pathCreator;
    [HideInInspector] public BezierPath bezierPath;
    private VertexPath path;

    private MeshCollider meshCollider;
    private MeshCollider collisionDetector;
    private MeshFilter meshFilter;

    private int lastAnchor;
    private bool inCollider = true;
    private float thickness = 2f;
    LayerMask benderLayer = 1 << 8; // Layer mask for the "LaserBender" layer

    List<Vector3> anchorPositions = new List<Vector3>();
    List<Vector3> anchorPositionsAtStart = new List<Vector3>();
    List<Vector3> anchorPositionsBeforeLastHit = new List<Vector3>();
    List<Vector3> anchorDirections = new List<Vector3>();

    [HideInInspector] public List<LaserBender> benders = new List<LaserBender>();
    [HideInInspector] public List<LaserBender> prevBender = new List<LaserBender>();
    List<Vector3> otherObjects = new List<Vector3>();
    List<Vector3> prevObjects = new List<Vector3>();

    [HideInInspector] public bool beamInteraction = false;
    [HideInInspector] public bool benderReset = false;
    bool trigger = false;
    bool hittedDamageable = false;
    bool noChange = true;
    bool willClear = false;
    bool isBenderMoving = false;

    public bool isActive = false;
    [HideInInspector] public bool becameActive = false;
    private bool becameInactive = false;
    private LaserEmitter laserEmitter;

    Vector3 startingPosition;
    Vector3 startingDirection;

    void Start()
    {
        pathCreator = GetComponent<PathCreator>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();

        bezierPath = pathCreator.bezierPath;

        increaseLength();
        addAnchors(0);

        anchorPositionsAtStart = getAnchorPoints();
        startingPosition = pathCreator.bezierPath[0];
        startingDirection = pathCreator.bezierPath[3] - pathCreator.bezierPath[0];
        lastAnchor = bezierPath.NumAnchorPoints - 1;

        // Find the LaserEmitter in other children of same parent
        laserEmitter = transform.parent.GetComponentInChildren<LaserEmitter>();
        isActive = laserEmitter.IsEmitting();
        if(!isActive)
            return;

        Mesh mesh = createMeshFromPath(laserWidth);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void Update()
    {
        if(becameInactive){
            becameInactive = false;
            resetPath();
            meshFilter.sharedMesh = null;
            meshCollider.sharedMesh = null;
        }

        if(!isActive)
            return;
        if(becameActive)
        {
            becameActive = false;
            Mesh mesh = createMeshFromPath(laserWidth);
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
        
        if (beamInteraction && benders.Count > 0)
        {
            bendPath();
            OnCollisionCutLaser();

            if (!willClear)
                StartCoroutine(clearBendersAfterTime(1f));
        }
        else if (trigger)
        {
            OnCollisionCutLaser();
        }
        else if (!noChange)
        {
            copyList(anchorPositions, anchorPositionsBeforeLastHit);
        }
        else if(benderReset)
        {
            resetPath();
        }

        if (beamInteraction || !noChange || trigger || benderReset)
        {
            for (int anchor = 3; anchor < bezierPath.NumPoints; anchor += 3)
            {
                bezierPath.MovePoint(anchor, anchorPositions[anchor / 3]);
            }
            pathCreator.bezierPath = bezierPath;

            Mesh mesh = createMeshFromPath(laserWidth);
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;

            noChange = hittedDamageable ? false : true;
            beamInteraction = false;
            benderReset = false;
            trigger = false;
            hittedDamageable = false;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if(!isActive)
            return;

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
        {
            trigger = true;
        }
        LaserEmitter otherEmitter = other.GetComponent<LaserEmitter>();
        if(otherEmitter != null && otherEmitter != laserEmitter)
        {
            otherEmitter.startEmitting();
            laserEmitter.stopEmitting();
            becameInactive = true;
        }
    }

    void OnCollisionCutLaser()
    {
        // Find hit point
        RaycastHit hit;
        Vector3 hitPoint = Vector3.zero;
        bool isHitting = false;

        Vector3 nextPoint = anchorPositions[1] + transform.position;
        for (int anchor = 3; anchor < bezierPath.NumPoints - 2; anchor += 3)
        {
            Vector3 point = nextPoint;
            nextPoint = anchorPositions[(anchor + 3) / 3] + transform.position;
            Vector3 direction = nextPoint - point;

            if (Physics.Raycast(point, direction, out hit, anchorDistance))
            {
                if (hit.transform.GetComponent<LaserBender>() != null)
                    continue;
                // Apply Damage
                if (hit.transform.GetComponent<Damageable>() != null)
                {
                    hittedDamageable = true;
                    copyList(anchorPositionsBeforeLastHit, anchorPositions);
                    hit.transform.GetComponent<Damageable>().ApplyDamage(laserDamage);
                    invertVelocity(hit.rigidbody);
                }

                isHitting = true;
                hitPoint = hit.point;
                break;
            }
        }
        if (!isHitting)
        {
            lastAnchor = bezierPath.NumAnchorPoints - 1;
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
        lastAnchor = closestAnchor;

        for (int anchor = closestAnchor; anchor < bezierPath.NumPoints; anchor += 3)
        {
            anchorPositions[anchor / 3] = hitPoint - transform.position;
        }
    }

    void increaseLength()
    {
        Vector3 origin = bezierPath[bezierPath.NumPoints - 4];
        Vector3 direction = Vector3.Normalize(bezierPath[bezierPath.NumPoints - 1] - origin);

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
            anchorDirections.Add(anchorDistance * direction);
        }
    }

    void bendPath()
    {
        inCollider = false;

        copyList(anchorPositions, anchorPositionsAtStart);
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

    void copyList(List<Vector3> list1, List<Vector3> list2)
    {
        list1.Clear();
        foreach (Vector3 point in list2)
        {
            list1.Add(point);
        }
    }

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

    IEnumerator clearBendersAfterTime(float time)
    {
        willClear = true;
        yield return new WaitForSeconds(time);
        willClear = false;
        benders.Clear();
    }

    // Check if at least one of the benders is moving
    void areBenderMoving()
    {
        foreach (LaserBender bender in benders)
        {
            // Debug.Log(bender.isMoving);
            if (bender.isMoving)
            {
                StartCoroutine(movingForTime(2f));
            }
        }
        isBenderMoving = false;
    }

    IEnumerator movingForTime(float time)
    {
        isBenderMoving = true;
        Debug.Log(isBenderMoving);
        yield return new WaitForSeconds(time);
        Debug.Log(isBenderMoving);
        isBenderMoving = false;
    }

    void invertVelocity(Rigidbody rb)
    {
        Vector3 velocity = -Vector3.Normalize(rb.velocity) * 10000 * laserForce;
        rb.transform.position -= Vector3.Normalize(rb.velocity) * 0.1f;
        rb.AddForce(velocity);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 2f);
    }

    void resetPath(){
        for(int i = 3; i < bezierPath.NumPoints; i += 3){
            bezierPath.MovePoint(i, anchorPositionsAtStart[i / 3]);
        }
        copyList(anchorPositions, anchorPositionsAtStart);
    }
    // Credit to Sebastian Lague on YouTube
    Mesh createMeshFromPath(float width, bool lengthOffset = false)
    {
        // path = new VertexPath (bezierPath, transform);

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
            if (lengthOffset && i == path.NumPoints - 1)
            {
                Vector3 direction = path.GetPoint(i) - path.GetPoint(i - 1);
                vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(width) - transform.position + direction * 5;
                vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(width) - transform.position + direction * 5;
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


    void printArray(List<Vector3> array)
    {
        for (int i = 0; i < array.Count; i++)
        {
            Debug.Log(i + ": " + array[i]);
        }
    }
}
