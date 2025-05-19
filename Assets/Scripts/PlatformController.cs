using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour
{
    public float speed = 10f;
    public bool isInitialPlatform = false;

    [SerializeField]
    private bool[] RightLane = new bool[8];

    [SerializeField]
    private bool[] MiddleLane = new bool[8];

    [SerializeField]
    private bool[] LeftLane = new bool[8];

    public bool[,] obstaclePositions
    {
        get
        {
            bool[,] combined = new bool[8, 3];
            bool[][] rows = { RightLane, MiddleLane, LeftLane };
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    combined[i, j] = rows[j][i];
                }
            }
            return combined;
        }
    }

    void Update()
    {
        transform.parent.transform.Translate(Vector3.back * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DestroyTrigger"))
        {
            // TODO Use object pooling
            Destroy(transform.parent.gameObject);
        }
    }
}
