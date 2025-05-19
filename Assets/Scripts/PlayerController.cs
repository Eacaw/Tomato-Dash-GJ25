using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public GameObject Camera;
    public GameObject[] prefabsToInstantiate;

    private int currentLane = 1; // 0 = left, 1 = middle, 2 = right
    private float laneWidth = 2.5f; // Distance between lanes
    private Vector3 targetPosition;

    private bool spawnCooldown = false;
    private float spawnCooldownTime = 3.0f; // seconds
    private float spawnCooldownTimer = 0f;
    private Animator animator;
    private UIDocument uiDocument;
    private Label gameOverLabel;

    void Awake()
    {
        uiDocument = FindFirstObjectByType<UIDocument>();

        gameOverLabel = uiDocument.rootVisualElement.Q<Label>("GameOver");
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    private float laneTurnAngle = 20f; // Maximum angle to tilt when changing lanes
    private float turnResetSpeed = 10f; // How quickly to return to forward
    private float currentTurn = 0f; // Current turn angle
    private bool isJumping = false;

    void Update()
    {
        bool laneChanged = false;
        float turnDirection = 0f;

        if (Input.GetAxis("Horizontal") < 0 && currentLane > 0)
        {
            currentLane--;
            UpdateTargetPosition();
            Input.ResetInputAxes();
            laneChanged = true;
            turnDirection = -1f;
            animator.SetTrigger("StrafeLeft");
        }
        else if (Input.GetAxis("Horizontal") > 0 && currentLane < 2)
        {
            currentLane++;
            UpdateTargetPosition();
            Input.ResetInputAxes();
            laneChanged = true;
            turnDirection = 1f;
            animator.SetTrigger("StrafeRight");
        }

        // Handle jump input
        if (!isJumping && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(JumpRoutine());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Restart the game
            Time.timeScale = 1f;
            gameOverLabel.style.display = DisplayStyle.None;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        IEnumerator JumpRoutine()
        {
            isJumping = true;
            animator.SetTrigger("Jump");

            // Smooth jump parameters
            float jumpHeight = 1.5f;
            float jumpDuration = 0.5f;
            float elapsedTime = 0f;

            Vector3 startPosition = transform.position;
            Vector3 peakPosition = startPosition + Vector3.up * jumpHeight;

            // Move up to the peak
            while (elapsedTime < jumpDuration)
            {
                float t = Mathf.Clamp01(elapsedTime / jumpDuration);
                transform.position = Vector3.Lerp(startPosition, peakPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset elapsed time for descent
            elapsedTime = 0f;

            // Move down to the original position
            while (elapsedTime < jumpDuration)
            {
                float t = Mathf.Clamp01(elapsedTime / jumpDuration);
                transform.position = Vector3.Lerp(peakPosition, startPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = startPosition; // Ensure exact position
            animator.SetTrigger("JumpLand");
            isJumping = false;
        }

        // Smoothly move the player to the target position, preserving the y-position during a jump
        Vector3 newPosition = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * 10f
        );
        if (!isJumping)
        {
            newPosition.y = transform.position.y; // Preserve current y-position if not jumping
        }
        transform.position = newPosition;

        // // Handle player model rotation for lane change
        if (laneChanged)
        {
            currentTurn = laneTurnAngle * turnDirection;
        }
        else
        {
            // Smoothly return to forward
            currentTurn = Mathf.Lerp(currentTurn, 0f, Time.deltaTime * turnResetSpeed);
        }
        // Apply rotation around Y axis (for yaw) or Z axis (for roll)
        // Smoothly interpolate the rotation for a smooth turn effect
        Quaternion targetPlayerRotation = Quaternion.Euler(0f, 0f, currentTurn);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetPlayerRotation,
            Time.deltaTime * 8f
        );

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

        if (spawnCooldown)
        {
            spawnCooldownTimer -= Time.deltaTime;
            if (spawnCooldownTimer <= 0f)
            {
                spawnCooldown = false;
            }
        }
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
        if (!spawnCooldown && other.gameObject.CompareTag("PlatformSpawnTrigger"))
        {
            // TODO Use object pooling
            GameObject prefabToInstantiate = prefabsToInstantiate[
                Random.Range(0, prefabsToInstantiate.Length)
            ];
            Instantiate(
                prefabToInstantiate,
                new Vector3(0, 0, other.gameObject.transform.parent.position.z + 64),
                Quaternion.identity
            );
            spawnCooldown = true;
            spawnCooldownTimer = spawnCooldownTime;
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            // Stop the game by pausing the time scale
            animator.SetTrigger("Death");
            // Final all instances of PlatformController using FindObjectsByType in the scene and set their speed to 0
            PlatformController[] platformControllers = FindObjectsByType<PlatformController>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
            foreach (var controller in platformControllers)
            {
                controller.speed = 0f;
            }
            gameOverLabel.style.display = DisplayStyle.Flex;
        }
    }
}
