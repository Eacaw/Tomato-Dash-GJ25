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
            // Create a list of available lanes for the current row
            int[] availableLanes = new int[lanes.Length];
            int count = 0;

            for (int lane = 0; lane < lanes.Length; lane++)
            {
                if (!platformController.obstaclePositions[i, lane])
                {
                    availableLanes[count] = lane;
                    count++;
                }
            }

            // If there are available lanes, randomly pick one and place a collectable
            if (count > 0)
            {
                int randomLaneIndex = Random.Range(0, count);
                int chosenLane = availableLanes[randomLaneIndex];

                Vector3 targetPos = new Vector3(
                    lanes[chosenLane],
                    1.0f,
                    platformController.transform.position.z + zPositions[i]
                );

                // Instantiate the collectable
                GameObject coin = Instantiate(
                    collectablePrefab,
                    targetPos,
                    Quaternion.identity,
                    this.transform
                );
            }
        }
    }
}
