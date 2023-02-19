using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// This script is possible thanks to Sebastian Lague: https://www.youtube.com/watch?v=saAQNRSYU9k
public class LaserBeam : MonoBehaviour
{
    public float colliderWidth = 1f;
    public float anchorDistance = 4f;
    [Range(0f, 1f)]
    public float bendingAmount = 0.5f;
    public float maxDistance = 100f;

    private PathCreator pathCreator;
    private BezierPath bezierPath;
    private VertexPath path;

    private MeshCollider meshCollider;
    private MeshFilter meshFilter;

    private bool inCollider = true;
    private float thickness = 2f;
    LayerMask benderLayer = 1 << 8; // Layer mask for the "LaserBender" layer

    List<Vector3> anchorPositions = new List<Vector3>();
    List<Vector3> anchorDirections = new List<Vector3>();
    List<Collider> colliders = new List<Collider>();
    Vector3 startingPosition;
    Vector3 startingDirection;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        pathCreator = GetComponent<PathCreator>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();

        increaseLength();
        addAnchors(0);
        startingPosition = pathCreator.bezierPath[0];
        startingDirection = pathCreator.bezierPath[3] - pathCreator.bezierPath[0];

        createMeshFromPath();
    }

    // Update is called once per frame
    void Update()
    {
        if (colliders.Count > 0)
        {
            bendPath();
            colliders.Clear();

            for (int anchor = 0; anchor < pathCreator.bezierPath.NumPoints; anchor += 3)
            {
                pathCreator.bezierPath.MovePoint(anchor, anchorPositions[anchor / 3]);
            }

            createMeshFromPath();
        }
        else if (!inCollider)
        {
            resetPath(0);
            createMeshFromPath();
            inCollider = true;
        }
    }


    // Funcionally OnTriggerStay because the Collider Mesh changes every frame
    void OnTriggerEnter(Collider other)
    {
        if (benderLayer == (benderLayer | (1 << other.gameObject.layer)))
        {
            colliders.Add(other);
        }
        else
        {
            Debug.Log(other.name);
        }
    }


    void increaseLength()
    {
        Vector3 origin = pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 4];
        Vector3 direction = Vector3.Normalize(pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1] - origin);

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            pathCreator.bezierPath.MovePoint(pathCreator.bezierPath.NumPoints - 1, hit.transform.position - transform.position);
        }
        else
        {
            pathCreator.bezierPath.MovePoint(pathCreator.bezierPath.NumPoints - 1, origin + maxDistance * direction);
        }
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
            foreach (Collider other in colliders)
            {
                Vector3 benderPosition = other.transform.position - transform.position;
                float distance = Vector3.Distance(benderPosition, pathCreator.bezierPath[anchor]);

                if (distance < colliderWidth)
                {
                    Vector3 directionToBender = benderPosition - pathCreator.bezierPath[anchor];
                    inCollider = true;

                    float t = distance / colliderWidth;
                    Vector3 straightLineDirection = getStraightLineDirection(anchor + 3);
                    Vector3 interpolationPosition = Vector3.Lerp(directionToBender, straightLineDirection, bendingAmount);
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

    void stopPathAtCollision(Vector3 colliderPosition)
    {   // Find the closest Anchor
        int closestAnchor = 0;
        float minDistance = Vector3.Distance(colliderPosition, pathCreator.bezierPath[0]);
        for (int anchor = 0; anchor < pathCreator.bezierPath.NumPoints; anchor += 3)
        {
            float distance = Vector3.Distance(colliderPosition, pathCreator.bezierPath[anchor]);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestAnchor = anchor;
            }
        }
        
    }

    // Credit to Sebastian Lague on YouTube
    void createMeshFromPath()
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

    // OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD
    // OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD
    // OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD
    // OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD
    // OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD _ _ _ OLD

    // void OnTriggerEnter(Collider other)
    // {

    //     if (benderLayer == (benderLayer | (1 << other.gameObject.layer)))
    //     {
    //         closestAnchors = closestAnchorPoint(other.transform.position - transform.position);
    //         int closestSegment = closestSegmentToPoint(other.transform.position - transform.position);

    //         if (closestAnchorDistance(closestPoint) > 2f)
    //         {
    //             Debug.Log("Added");
    //             addAnchor(closestPoint, closestSegment);
    //             pathChanged = true;
    //             bendPath(other.transform.position - transform.position, closestAnchors[0] + 3);
    //             if (closestAnchorDistance(other.transform.position - transform.position) <= colliderWidth){
    //                 bendPath(other.transform.position - transform.position, closestAnchors[0] + 3);
    //             }
    //         }
    //         else if (closestAnchorDistance(other.transform.position - transform.position) <= colliderWidth){
    //             pathChanged = true;
    //             bendPath(other.transform.position - transform.position, closestAnchors[0]);
    //         }
    //         pathChanged = true;
    //     }
    // }
    // void addAnchor(Vector3 anchorPosition, int closestSegment)
    // {
    //     // int segment = Mathf.Min(closestPoints[0], closestPoints[1]);
    //     // Debug.Log(segment);
    //     if (closestSegment != pathCreator.bezierPath.NumAnchorPoints - 2)
    //     {
    //         deleteSegments(closestSegment);
    //     }

    //     if (closestSegment == pathCreator.bezierPath.NumSegments) closestSegment--;
    //     pathCreator.bezierPath.SplitSegment(anchorPosition, closestSegment, 0.5f);
    // }

    // void bendPath(Vector3 benderPosition, int anchorIndex)
    // {

    //     float t = Vector3.Distance(benderPosition, pathCreator.bezierPath[anchorIndex]) / colliderWidth;
    //     float direction = Vector3.Dot(benderPosition - pathCreator.bezierPath[anchorIndex], pathCreator.path.GetNormal(pathCreator.path.GetClosestDistanceAlongPath(benderPosition + transform.position)));

    //     for (int i = anchorIndex; i < pathCreator.bezierPath.NumPoints - 2; i += 3)
    //     {
    //         Vector3 anchorPosition = pathCreator.bezierPath[i];
    //         Vector3 nextAnchorPosition = getFollowingPosition(i);

    //         float x = Mathf.Lerp(benderPosition.x, nextAnchorPosition.x, t);
    //         float y = Mathf.Lerp(benderPosition.y - 100 * Mathf.Sign(direction), nextAnchorPosition.y, t);
    //         Vector3 newPosition = new Vector3(x, y, 0);

    //         pathCreator.bezierPath.MovePoint(i + 3, newPosition);
    //     }

    // }

    // Vector3 getFollowingPosition(int anchorIndex)
    // {
    //     float distanceToNext = Vector3.Distance(pathCreator.bezierPath[anchorIndex], pathCreator.bezierPath[anchorIndex + 3]);
    //     if (anchorIndex == 0)
    //     {
    //         return pathCreator.bezierPath[anchorIndex] + startDirection * distanceToNext;
    //     }
    //     return pathCreator.bezierPath[anchorIndex] + Vector3.Normalize(pathCreator.bezierPath[anchorIndex] - pathCreator.bezierPath[anchorIndex - 3]) * distanceToNext;
    // }

    // // return an array with the sorted anchor point indices, from closest to most distant
    // int[] closestAnchorPoint(Vector3 point)
    // {
    //     int numberPoints = pathCreator.bezierPath.NumAnchorPoints;
    //     float[] distances = new float[numberPoints];
    //     int[] closestPoints = new int[numberPoints];

    //     for (int i = 0; i < numberPoints; i++)
    //         closestPoints[i] = i * 3;

    //     for (int i = 0; i < pathCreator.bezierPath.NumPoints; i += 3)
    //     {
    //         float distance = Vector3.Distance(point, pathCreator.bezierPath[i]);
    //         distances[i / 3] = distance;
    //     }

    //     anchorDistances = distances;

    //     System.Array.Sort(closestPoints, (x, y) => distances[x / 3].CompareTo(distances[y / 3]));
    //     return closestPoints;
    // }

    // int closestSegmentToPoint(Vector3 point)
    // {
    //     float minDist = float.MaxValue;
    //     int segment = 0;

    //     for (int i = 0; i < pathCreator.bezierPath.NumPoints - 1; i += 3)
    //     {
    //         Vector3 closestPosition = closestPointOnPath(pathCreator.bezierPath[i], pathCreator.bezierPath[i + 3], point);
    //         float dist = Vector3.Distance(point, closestPosition);

    //         if (dist < minDist)
    //         {
    //             minDist = dist;
    //             closestPoint = closestPosition;
    //             segment = i;
    //         }
    //     }
    //     return segment / 3;
    // }

    // // Gives back the distance to the closest anchor
    // float closestAnchorDistance(Vector3 point)
    // {
    //     float minDist = float.MaxValue;

    //     for (int i = 0; i < pathCreator.bezierPath.NumPoints - 1; i += 3)
    //     {
    //         float dist = Vector3.Distance(point, pathCreator.bezierPath[i]);

    //         if (dist < minDist)
    //         {
    //             minDist = dist;
    //         }
    //     }
    //     return minDist;
    // }

    // Vector3 closestPointOnPath(Vector3 a, Vector3 b, Vector3 point)
    // {
    //     float t = Vector3.Dot(Vector3.Normalize(point - a), Vector3.Normalize(b - a)) * Vector3.Distance(point, a);
    //     t /= Vector3.Distance(a, b);
    //     if (t < 0) return a;
    //     if (t > 1) return b;

    //     return a + t * (b - a);
    // }
}
