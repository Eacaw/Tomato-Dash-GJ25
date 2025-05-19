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
        Debug.Log("Obastacle positions:");
        for (int i = 0; i < platformController.obstaclePositions.GetLength(0); i++)
        {
            for (int j = 0; j < platformController.obstaclePositions.GetLength(1); j++)
            {
                if (platformController.obstaclePositions[i, j])
                {
                    Debug.Log($"[{i},{j}]: {platformController.obstaclePositions[i, j]}");
                }
            }
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

            // Find a valid lane that doesn't have an obstacle
            bool validLaneFound = false;
            for (int attempt = 0; attempt < lanes.Length; attempt++)
            {
                laneIndex = Random.Range(minLane, maxLane + 1);
                if (!platformController.obstaclePositions[i, laneIndex])
                {
                    validLaneFound = true;
                    break;
                }
            }

            // If no valid lane is found, skip this collectable
            if (!validLaneFound)
            {
                Debug.Log($"No valid lane found for collectable at index {i}");
                continue;
            }

            Vector3 targetPos = new Vector3(
                lanes[laneIndex],
                1.0f,
                platformController.transform.position.z + zPositions[i]
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
