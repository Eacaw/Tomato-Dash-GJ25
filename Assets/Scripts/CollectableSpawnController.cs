using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
    public GameObject collectablePrefab; // Reference to the prefab to instantiate

    // Parameters for placement
    public float platformY = 1.0f; // Set this to your platform's Y position
    public float abovePlatformOffset = 1f; // How much above the platform
    public float laneWidth = 2.5f;
    public PlatformController platformController;

    private float[] lanes = new float[] { -2.5f, 0f, 2.5f };
    private int collectableCount = 8;
    private float[] zPositions = new float[] { -12f, -8f, -4f, 0f, 4f, 8f, 12f, 16f };

    void Start()
    {
        if (!platformController.isInitialPlatform)
        {
            PlaceCollectables();
        }
    }

    private void PlaceCollectables()
    {
        int laneIndex = Random.Range(0, lanes.Length);
        for (int i = 0; i < collectableCount; i++)
        {
            // Determine next lane index (at most one away from previous)
            int minLane = Mathf.Max(0, laneIndex - 1);
            int maxLane = Mathf.Min(lanes.Length - 1, laneIndex + 1);
            laneIndex = Random.Range(minLane, maxLane + 1);

            Vector3 targetPos = new Vector3(
                lanes[laneIndex],
                platformY + abovePlatformOffset,
                transform.position.z + zPositions[i]
            );

            GameObject coin = Instantiate(
                collectablePrefab,
                targetPos,
                Quaternion.identity,
                this.transform
            );
        }
    }
}
