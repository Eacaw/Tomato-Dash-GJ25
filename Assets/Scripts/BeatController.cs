using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeatDetector : MonoBehaviour
{
  public float sensitivity = 2.0f;
  public float beatCooldown = 0.5f;
  public int spectrumIndex = 2; // Low Freq.
  public AudioSource audioSource;
  public Transform cameraTransform;
  public float jumpIntensity = 1.0f;
  public float jumpDuration = 0.4f;
  public float smoothSpeed = 15f;


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

    AudioClip clip = Resources.Load<AudioClip>("Audio/DRIVE");
    audioSource.clip = clip;
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

    cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCameraPos, Time.deltaTime * smoothSpeed);

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
    // Implement your beat detection logic here
    Debug.Log("Beat detected!");
    targetCameraPos = originalCameraPos + Vector3.up * jumpIntensity;
    jumpTimer = 0f;
    isJumping = true;
  }
}