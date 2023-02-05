using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class LaserBeam : MonoBehaviour
{
    private PathCreator pathCreator;
    public float colliderWidth = 1f;



    private BezierPath bezierPath;
    private VertexPath path;

    private bool pathChanged;
    private bool meshChanged;
    private MeshCollider meshCollider;
    private MeshFilter meshFilter;
    // private Mesh mesh;
    private float thickness = 2f;
    LayerMask benderLayer = 1 << 8; // Layer mask for the "LaserBender" layer

    int[] closestAnchors;
    float[] anchorDistances;
    Vector3 closestPoint;
    Vector3 endPoint;
    Vector3 startDirection;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        pathCreator = GetComponent<PathCreator>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();

        endPoint = pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1];
        startDirection = Vector3.Normalize(pathCreator.bezierPath[3] - pathCreator.bezierPath[0]); 
        // bezierPath = createPath();
        // bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic; 

        // pathCreator.bezierPath = bezierPath;
        createMeshFromPath();
    }

    // Update is called once per frame
    void Update()
    {
        if (pathChanged)
        {
            createMeshFromPath();
            meshChanged = true;
            pathChanged = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (benderLayer == (benderLayer | (1 << other.gameObject.layer)))
        {
            closestAnchors = closestAnchorPoint(other.transform.position - transform.position);
            int closestSegment = closestSegmentToPoint(other.transform.position - transform.position);

            if (closestAnchorDistance(closestPoint) > 2f)
            {
                Debug.Log("Added");
                addAnchor(closestPoint, closestSegment);
                pathChanged = true;
                bendPath(other.transform.position - transform.position, closestAnchors[0] + 3);
                if (closestAnchorDistance(other.transform.position - transform.position) <= colliderWidth){
                    bendPath(other.transform.position - transform.position, closestAnchors[0] + 3);
                }
            }
            else if (closestAnchorDistance(other.transform.position - transform.position) <= colliderWidth){
                pathChanged = true;
                bendPath(other.transform.position - transform.position, closestAnchors[0]);
            }
            pathChanged = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (benderLayer == (benderLayer | (1 << other.gameObject.layer)))
        {
            meshChanged = false;
        }
    }

    // BezierPath createPath(){
    //     Vector3 start = transform.position;
    //     Vector3 end = Vector3.zero;

    //     Ray ray = new Ray(transform.position, transform.right);
    //     RaycastHit hit;
    //     if( Physics.Raycast(ray, out hit, 100)){
    //         end = hit.transform.position;
    //     }
    //     Vector3[] points = {start, end};
    //     return new BezierPath(points, false, PathSpace.xy);
    // }


    void addAnchor(Vector3 anchorPosition, int closestSegment)
    {
        // int segment = Mathf.Min(closestPoints[0], closestPoints[1]);
        // Debug.Log(segment);
        if (closestSegment != pathCreator.bezierPath.NumAnchorPoints - 2)
        {
            deleteSegments(closestSegment);
        }

        if(closestSegment == pathCreator.bezierPath.NumSegments) closestSegment--;
        pathCreator.bezierPath.SplitSegment(anchorPosition, closestSegment, 0.5f);
    }

    void bendPath(Vector3 benderPosition, int anchorIndex)
    {
        
        float t = Vector3.Distance(benderPosition, pathCreator.bezierPath[anchorIndex]) / colliderWidth;
        float direction = Vector3.Dot(benderPosition - pathCreator.bezierPath[anchorIndex], pathCreator.path.GetNormal(pathCreator.path.GetClosestDistanceAlongPath(benderPosition + transform.position)));

        for (int i = anchorIndex; i < pathCreator.bezierPath.NumPoints - 2; i += 3)
        {
            Vector3 anchorPosition = pathCreator.bezierPath[i];
            Vector3 nextAnchorPosition = getFollowingPosition(i);

            float x = Mathf.Lerp(benderPosition.x, nextAnchorPosition.x, t);
            float y = Mathf.Lerp(benderPosition.y - 100 * Mathf.Sign(direction), nextAnchorPosition.y, t);
            Vector3 newPosition = new Vector3(x, y, 0);
            
            pathCreator.bezierPath.MovePoint(i + 3, newPosition);
        }

    }

    Vector3 getFollowingPosition(int anchorIndex){
        float distanceToNext = Vector3.Distance(pathCreator.bezierPath[anchorIndex], pathCreator.bezierPath[anchorIndex + 3]);
        if(anchorIndex == 0){
            return pathCreator.bezierPath[anchorIndex] + startDirection * distanceToNext;
        }
        return pathCreator.bezierPath[anchorIndex] + Vector3.Normalize(pathCreator.bezierPath[anchorIndex] - pathCreator.bezierPath[anchorIndex - 3]) * distanceToNext;
    }

    // return an array with the sorted anchor point indices, from closest to most distant
    int[] closestAnchorPoint(Vector3 point)
    {
        int numberPoints = pathCreator.bezierPath.NumAnchorPoints;
        float[] distances = new float[numberPoints];
        int[] closestPoints = new int[numberPoints];

        for (int i = 0; i < numberPoints; i++)
            closestPoints[i] = i * 3;
        
        for (int i = 0; i < pathCreator.bezierPath.NumPoints; i += 3)
        {
            float distance = Vector3.Distance(point, pathCreator.bezierPath[i]);
            distances[i / 3] = distance;
        }

        anchorDistances = distances;

        System.Array.Sort(closestPoints, (x, y) => distances[x / 3].CompareTo(distances[y / 3]));
        return closestPoints;
    }

    int closestSegmentToPoint(Vector3 point)
    {
        float minDist = float.MaxValue;
        int segment = 0;

        for (int i = 0; i < pathCreator.bezierPath.NumPoints - 1; i += 3)
        {
            Vector3 closestPosition = closestPointOnPath(pathCreator.bezierPath[i], pathCreator.bezierPath[i + 3], point);
            float dist = Vector3.Distance(point, closestPosition);

            if (dist < minDist)
            {
                minDist = dist;
                closestPoint = closestPosition;
                segment = i;
            }
        }
        return segment / 3;
    }

    // Gives back the distance to the closest anchor
    float closestAnchorDistance(Vector3 point)
    {
        float minDist = float.MaxValue;

        for (int i = 0; i < pathCreator.bezierPath.NumPoints - 1; i += 3)
        {
            float dist = Vector3.Distance(point, pathCreator.bezierPath[i]);

            if (dist < minDist)
            {
                minDist = dist;
            }
        }
        return minDist;
    }

    Vector3 closestPointOnPath(Vector3 a, Vector3 b, Vector3 point)
    {
        float t = Vector3.Dot(Vector3.Normalize(point - a), Vector3.Normalize(b - a)) * Vector3.Distance(point, a);
        t /= Vector3.Distance(a, b);
        if (t < 0) return a;
        if (t > 1) return b;

        return a + t * (b - a);
    }

    // Delete all segments that are higher when index i
    void deleteSegments(int i)
    {
        for (int j = pathCreator.bezierPath.NumAnchorPoints - 1; j > i; j--)
        {
            pathCreator.bezierPath.DeleteSegment(j);
        }
        // closestSegment = 
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
}
