using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSpawnZone : MonoBehaviour
{
    public GameObject rootSpike;
    public int spikeCount = 15;
    public Vector3 spikeOrientation;
    public Vector3 gravityDirection;

    public enum Orientation { vertical, horizontal };
    public Orientation planeOrientation = Orientation.vertical;
    public float spawnRange = 5f;



    TreeBoss treeBoss;
    Player player;
    private Transform spikeSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        treeBoss = GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        spikeSpawnPoint = transform.GetChild(0);
        spikeSpawnPoint.position = new Vector3(spikeSpawnPoint.position.x, spikeSpawnPoint.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            treeBoss.spikeSpawnZones.Add(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            treeBoss.spikeSpawnZones.Remove(this);
        }
    }

    public IEnumerator spawnRootSpikes()
    {
        for (int spike = 0; spike < spikeCount; spike++)
        {
            if (player.gravitationalDirection == gravityDirection)
            {
                // var spawnPos = new Vector3(rightSideMidPos.x, rightSideMidPos.y + Random.Range(-sideRadiusLR, sideRadiusLR), rightSideMidPos.z);
                Vector3 spawnPosition = spikeSpawnPoint.position + spawnPositionOffset(planeOrientation);
                GameObject rootSpikeObject = Instantiate(rootSpike, spawnPosition, quatOrientation[spikeOrientation]);
                rootSpikeObject.GetComponent<rootSpike>().growingDirection = spikeOrientation;
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }


    Dictionary<Vector3, Quaternion> quatOrientation = new Dictionary<Vector3, Quaternion>
    {
        { Vector3.up,    Quaternion.identity},
        { Vector3.down,  Quaternion.Euler(0f, 0f, -180f) },
        { Vector3.left,  Quaternion.Euler(0f, 0f, 90f) },
        { Vector3.right, Quaternion.Euler(0f, 0f, -90f) },
    };

    Vector3 spawnPositionOffset(Orientation orientation)
    {
        if (orientation == Orientation.horizontal)
            return new Vector3(Random.Range(-spawnRange, spawnRange), 0, 0f);
        else
            return new Vector3(0, Random.Range(-spawnRange, spawnRange), 0f);
    }
}
