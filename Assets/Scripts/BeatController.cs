using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeatController : MonoBehaviour
{
    public event System.Action OnBeat;

    public float sensitivity = 7.0f;
    public float beatCooldown = 0.5f;
    public int spectrumIndex = 4; // Low Freq.
    public AudioSource audioSource;
    public Transform cameraTransform;
    public float jumpIntensity = 0.3f;
    public float jumpDuration = 0.2f;
    public float smoothSpeed = 10f;

    private float[] spectrum = new float[1024];
    private float timer = 0f;
    private bool canDetectBeat = true;
    private Vector3 originalCameraPos;
    private Vector3 targetCameraPos;
    private bool isJumping = false;
    private float jumpTimer = 0f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        originalCameraPos = cameraTransform.localPosition;
        targetCameraPos = originalCameraPos;

        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        timer += Time.deltaTime;
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float energy = spectrum[spectrumIndex] * 100;

        // Trigger on beat if energy exceeds sensitivity
        if (energy > sensitivity && canDetectBeat)
        {
            TriggerOnBeat();
            canDetectBeat = false;
            timer = 0f;
        }

        // Reset beat detection after cooldown
        if (!canDetectBeat && timer > beatCooldown)
        {
            canDetectBeat = true;
        }

        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            targetCameraPos,
            Time.deltaTime * smoothSpeed
        );

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= jumpDuration)
            {
                targetCameraPos = originalCameraPos;
                isJumping = false;
            }
        }
    }

    void TriggerOnBeat()
    {
        OnBeat?.Invoke();
        targetCameraPos = originalCameraPos + Vector3.up * jumpIntensity;
        jumpTimer = 0f;
        isJumping = true;
    }

    public void AddBeatListener(System.Action listener)
    {
        OnBeat += listener;
    }

    public void RemoveBeatListener(System.Action listener)
    {
        OnBeat -= listener;
    }
}
