using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
    public GameObject collectablePrefab;
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
                1.0f,
                transform.position.z + zPositions[i]
            );

            // Todo use object pooling
            GameObject coin = Instantiate(
                collectablePrefab,
                targetPos,
                Quaternion.identity,
                this.transform
            );
        }
    }
}
