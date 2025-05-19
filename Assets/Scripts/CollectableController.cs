using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    private Vector3 startPosition;
    private ScoreController scoreController;

    private void Start()
    {
        startPosition = transform.position;
        scoreController = FindFirstObjectByType<ScoreController>();
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            scoreController.AddScore(1);

            // Todo play a sound

            Destroy(gameObject);
        }
    }
}
