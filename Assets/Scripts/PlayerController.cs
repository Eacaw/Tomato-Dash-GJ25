using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int currentLane = 1; // 0 = left, 1 = middle, 2 = right
    private float laneWidth = 2.5f; // Distance between lanes
    private Vector3 targetPosition;
    public GameObject Camera;

    public GameObject prefabToInstantiate;

    void Awake()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") < 0 && currentLane > 0)
        {
            currentLane--;
            UpdateTargetPosition();
            Input.ResetInputAxes();
        }
        else if (Input.GetAxis("Horizontal") > 0 && currentLane < 2)
        {
            currentLane++;
            UpdateTargetPosition();
            Input.ResetInputAxes();
        }

        // Smoothly move the player to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);

        // Subtly rotate the camera in the direction of the player
        Vector3 directionToPlayer =
            new Vector3(
            transform.position.x + (currentLane - 1) * 0.5f, // Adjust based on currentLane
            transform.position.y,
            transform.position.z
            ) - Camera.transform.position;

        // Offset the direction slightly to avoid pointing directly at the player
        directionToPlayer.x *= 0.1f; // Reduce the influence on the x-axis
        directionToPlayer.z *= 0.25f; // Reduce the influence on the z-axis

        Quaternion targetRotation = Quaternion.Slerp(
            Camera.transform.rotation,
            Quaternion.LookRotation(directionToPlayer),
            0.5f
        );

        Vector3 eulerRotation = targetRotation.eulerAngles;
        eulerRotation.x = Camera.transform.rotation.eulerAngles.x; // Maintain original x rotation
        eulerRotation.z = Camera.transform.rotation.eulerAngles.z; // Maintain original z rotation

        Camera.transform.rotation = Quaternion.Lerp(
            Camera.transform.rotation,
            Quaternion.Euler(eulerRotation),
            Time.deltaTime * 5f
        );
    }

    private void UpdateTargetPosition()
    {
        targetPosition = new Vector3(
            currentLane * laneWidth - laneWidth,
            transform.position.y,
            transform.position.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enters the spawn trigger to spawn a new platform
        if (other.gameObject.CompareTag("PlatformSpawnTrigger"))
        {
            Instantiate(
                prefabToInstantiate,
                new Vector3(0, 0, other.gameObject.transform.parent.position.z + 64),
                Quaternion.identity
            );
        }
    }
}
