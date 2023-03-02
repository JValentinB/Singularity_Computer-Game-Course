using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// This script is possible thanks to Sebastian Lague: https://www.youtube.com/watch?v=saAQNRSYU9k
public class LaserBeam : MonoBehaviour
{
    public float laserWidth = 0.1f;
    public int laserDamage = 1;
    public float colliderWidth = 1f;
    public float anchorDistance = 4f;
    public float maxLength = 100f;

    private PathCreator pathCreator;
    private BezierPath bezierPath;
    private VertexPath path;

    private MeshCollider meshCollider;
    private MeshCollider collisionDetector;
    private MeshFilter meshFilter;

    private bool inCollider = true;
    private float thickness = 2f;
    LayerMask benderLayer = 1 << 8; // Layer mask for the "LaserBender" layer

    List<Vector3> anchorPositions = new List<Vector3>();
    List<Vector3> anchorDirections = new List<Vector3>();
    List<LaserBender> benders = new List<LaserBender>();
    Vector3 startingPosition;
    Vector3 startingDirection;

    float timer;

    void Start()
    {
        pathCreator = GetComponent<PathCreator>();
        meshCollider = GetComponent<MeshCollider>();
        collisionDetector = gameObject.AddComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();

        increaseLength();
        addAnchors(0);
        startingPosition = pathCreator.bezierPath[0];
        startingDirection = pathCreator.bezierPath[3] - pathCreator.bezierPath[0];

        createColliderFromPath();
    }

    void Update()
    {
        if (benders.Count > 0)
        {
            bendPath();
            benders.Clear();

            OnCollisionCutLaser();
            for (int anchor = 0; anchor < pathCreator.bezierPath.NumPoints; anchor += 3)
            {
                pathCreator.bezierPath.MovePoint(anchor, anchorPositions[anchor / 3]);
            }
            createColliderFromPath();
        }
        else if (!inCollider)
        {
            resetPath(0);
            OnCollisionCutLaser();
            createColliderFromPath();
            inCollider = true;
        }
    }


    // Funcionally OnTriggerStay because the Collider Mesh changes every frame
    void OnTriggerEnter(Collider other)
    {
        LaserBender bender = other.GetComponent<LaserBender>();
        Damageable damageable = other.GetComponent<Damageable>();
        if (bender != null)
        {
            benders.Add(bender);
        }
        else if (damageable != null)
        {
            
            damageable.ApplyDamage(1);
        }
        else
        {

        }
    }

    void OnCollisionCutLaser()
    {
        // Find hit point
        Vector3 nextPoint = anchorPositions[0] + transform.position;
        RaycastHit hit;
        bool isHitting = false;
        Vector3 hitPoint = Vector3.zero;
        string hitName = "";
        for (int anchor = 0; anchor < pathCreator.bezierPath.NumPoints - 2; anchor += 3)
        {
            Vector3 point = nextPoint;
            nextPoint = anchorPositions[(anchor + 3) / 3] + transform.position;
            Vector3 direction = nextPoint - point;

            if (Physics.Raycast(point, direction, out hit, anchorDistance))
            {
                if (hit.transform.GetComponent<LaserBender>() != null) continue;

                isHitting = true;
                hitPoint = hit.point;
                hitName = hit.transform.name;
                break;
            }
        }
        if (!isHitting) return;

        // Find closest anchor
        int closestAnchor = 0;
        float minDistance = Vector3.Distance(hitPoint, anchorPositions[0] + transform.position);
        for (int anchor = 3; anchor < pathCreator.bezierPath.NumPoints; anchor += 3)
        {
            float distance = Vector3.Distance(hitPoint, anchorPositions[anchor / 3] + transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestAnchor = anchor;
            }
        }

        for (int anchor = closestAnchor; anchor < pathCreator.bezierPath.NumPoints; anchor += 3)
        {
            anchorPositions[anchor / 3] = hitPoint - transform.position;
        }
    }

    void increaseLength()
    {
        Vector3 origin = pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 4];
        Vector3 direction = Vector3.Normalize(pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1] - origin);

        pathCreator.bezierPath.MovePoint(pathCreator.bezierPath.NumPoints - 1, origin + maxLength * direction);

        // Ray ray = new Ray(origin, direction);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit, maxDistance))
        // {
        //     pathCreator.bezierPath.MovePoint(pathCreator.bezierPath.NumPoints - 1, hit.transform.position - transform.position);
        // }
        // else
        // {
        //     pathCreator.bezierPath.MovePoint(pathCreator.bezierPath.NumPoints - 1, origin + maxDistance * direction);
        // }
    }

    void addAnchors(int index)
    {
        int anchor = index;
        float distance = Vector3.Distance(pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1],
                                          pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 4]);
        Vector3 direction = Vector3.Normalize(pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1] - pathCreator.bezierPath[anchor]);

        while (distance > anchorDistance)
        {
            Vector3 anchorPosition = pathCreator.bezierPath[anchor] + anchorDistance * direction;

            pathCreator.bezierPath.SplitSegment(anchorPosition, anchor / 3, 0.5f);

            distance = Vector3.Distance(pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1],
                                          pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 4]);
            anchor += 3;
        }
        for (int i = 0; i < pathCreator.bezierPath.NumAnchorPoints; i++)
        {
            anchorPositions.Add(pathCreator.bezierPath[i * 3]);
            anchorDirections.Add(anchorDistance * direction);
        }
    }

    void bendPath()
    {
        inCollider = false;

        for (int anchor = 0; anchor < pathCreator.bezierPath.NumPoints - 2; anchor += 3)
        {
            foreach (LaserBender bender in benders)
            {
                Vector3 benderPosition = bender.transform.position - transform.position;
                float distance = Vector3.Distance(benderPosition, pathCreator.bezierPath[anchor]);

                if (distance < (colliderWidth * bender.bendingDistance))
                {
                    Vector3 directionToBender = benderPosition - pathCreator.bezierPath[anchor];
                    inCollider = true;

                    float t = distance / (colliderWidth * bender.bendingDistance);
                    Vector3 straightLineDirection = getStraightLineDirection(anchor + 3);
                    Vector3 interpolationPosition = Vector3.Lerp(directionToBender, straightLineDirection, bender.bendingAmount);
                    Vector3 newDirection = Vector3.Lerp(interpolationPosition, straightLineDirection, t);

                    for (int j = anchor; j < pathCreator.bezierPath.NumPoints; j += 3)
                    {
                        anchorDirections[j / 3] = newDirection;
                    }
                    updatePositions(anchor);
                }
            }

        }
    }

    void resetPath(int index)
    {
        for (int anchor = index; anchor < pathCreator.bezierPath.NumPoints; anchor += 3)
        {
            anchorDirections[anchor / 3] = startingDirection;
            if (anchor == 0)
            {
                anchorPositions[anchor] = startingPosition;
                continue;
            }

            pathCreator.bezierPath.MovePoint(anchor, pathCreator.bezierPath[anchor - 3] + anchorDirections[(anchor - 3) / 3]);
            anchorPositions[anchor / 3] = pathCreator.bezierPath[anchor];
        }
    }

    void updatePositions(int index)
    {
        for (int anchor = index + 3; anchor < pathCreator.bezierPath.NumPoints; anchor += 3)
        {
            anchorPositions[anchor / 3] = anchorPositions[(anchor - 3) / 3] + anchorDirections[(anchor - 3) / 3];
        }
    }

    Vector3 getStraightLineDirection(int index)
    {
        Vector3 direction = new Vector3(0, 0, 0);
        if (index >= 6)
        {
            direction = anchorDistance * Vector3.Normalize(pathCreator.bezierPath[index - 3] - pathCreator.bezierPath[index - 6]);
        }
        // cases for indices 0 and 3 missing
        return direction;
    }

    Vector3 getStraightLinePosition(int index)
    {
        Vector3 position = pathCreator.bezierPath[index];
        if (index >= 6)
        {
            Vector3 direction = Vector3.Normalize(pathCreator.bezierPath[index - 3] - pathCreator.bezierPath[index - 6]);

            position = pathCreator.bezierPath[index - 3] + anchorDistance * direction;
        }
        // cases for indices 0 and 3 missing
        return position;
    }

    // Delete all segments that are higher than anchor index
    void deleteSegments(int anchor)
    {
        for (int j = pathCreator.bezierPath.NumAnchorPoints - 1; j > anchor; j--)
        {
            pathCreator.bezierPath.DeleteSegment(j);
        }
    }

    // Credit to Sebastian Lague on YouTube
    void createColliderFromPath()
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
            Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(colliderWidth) - transform.position;
            Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(colliderWidth) - transform.position;

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


        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }


    void printArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log(i + ": " + array[i]);
        }
    }
}
