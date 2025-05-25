using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class BeatController : MonoBehaviour
{
    public event System.Action OnBeat;
    public event System.Action TrackChanged;

    [Header("Audio Settings")]
    public AudioClip[] trackList;
    public AudioSource audioSource;
    
    [Header("Beat Analysis")]
    [Range(1.0f, 20.0f)]
    public float sensitivity = 7.0f;
    
    [Range(0.1f, 1.0f)]
    public float beatCooldown = 0.5f;
    
    public int windowSize = 1024;
    public int analysisHopSize = 512;
    
    [Range(0.01f, 0.5f)]
    public float filterFactor = 0.1f;

    [Range(10, 100)]
    public int historySize = 43;
    
    [Range(1.0f, 3.0f)]
    public float energyThreshold = 1.3f;
    
    [Header("Timing Fine-Tuning")]
    [Range(-0.5f, 0.5f)]
    public float beatTimeOffset = 0.0f;
    
    [Range(0.0f, 0.5f)]
    public float lookAheadTime = 0.05f;
    
    [Range(0.0f, 0.25f)]
    public float beatQuantization = 0.0f;

    private List<float[]> beatTimestamps;
    private int currentTrackIndex = 0;
    private int nextBeatIndex = 0;
    private bool audioEndHandled = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        AnalyzeAllTracks();
        StartFirstTrack();
    }

    private void StartFirstTrack()
    {
        currentTrackIndex = 0;
        ChangeTrack(currentTrackIndex);
    }

    private void AnalyzeAllTracks()
    {
        beatTimestamps = new List<float[]>();
        
        foreach (AudioClip clip in trackList)
        {
            beatTimestamps.Add(AnalyzeTrack(clip));
        }
    }

    private float[] AnalyzeTrack(AudioClip clip)
    {
        List<float> beats = new List<float>();
        if (clip == null)
            return beats.ToArray();

        // Get audio samples
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        
        // Apply low-pass filtering for bass focus
        float[] filteredSamples = ApplyLowPassFilter(samples);
        
        // Detect beats using energy analysis
        float sampleRate = clip.frequency;
        float lastBeatTime = -beatCooldown;
        List<float> energyHistory = new List<float>();
        
        // Process audio in overlapping windows
        for (int startSample = 0; startSample < samples.Length - windowSize; startSample += analysisHopSize)
        {
            float timeInSeconds = startSample / sampleRate;
            float energy = CalculateWindowEnergy(filteredSamples, startSample, clip.channels);
            
            energyHistory.Add(energy);
            if (energyHistory.Count > historySize)
                energyHistory.RemoveAt(0);
            
            // Need enough history to make accurate decisions
            if (energyHistory.Count >= historySize / 2)
            {
                float localAverage = CalculateLocalAverage(energyHistory);
                bool isBeat = IsSignificantBeat(energy, localAverage, timeInSeconds, lastBeatTime);
                
                if (isBeat)
                {
                    float adjustedBeatTime = AdjustBeatTiming(timeInSeconds);
                    beats.Add(adjustedBeatTime);
                    lastBeatTime = timeInSeconds;
                }
            }
        }
        
        Debug.Log($"Found {beats.Count} beats in track");
        return beats.ToArray();
    }

    private float[] ApplyLowPassFilter(float[] samples)
    {
        float[] filtered = new float[samples.Length];
        float prevSample = 0;
        
        for (int i = 0; i < samples.Length; i++)
        {
            filtered[i] = filterFactor * samples[i] + (1 - filterFactor) * prevSample;
            prevSample = filtered[i];
        }
        
        return filtered;
    }

    private float CalculateWindowEnergy(float[] samples, int startIndex, int channels)
    {
        float energy = 0;
        
        for (int i = 0; i < windowSize; i++)
        {
            int sampleIndex = startIndex + i;
            if (channels == 2)
                sampleIndex *= 2;
                
            if (sampleIndex < samples.Length)
                energy += Mathf.Abs(samples[sampleIndex]);
        }
        
        return energy / windowSize;
    }

    private float CalculateLocalAverage(List<float> energyHistory)
    {
        float sum = 0;
        for (int i = 0; i < energyHistory.Count - 1; i++)
        {
            sum += energyHistory[i];
        }
        return sum / (energyHistory.Count - 1);
    }

    private bool IsSignificantBeat(float energy, float localAverage, float timeInSeconds, float lastBeatTime)
    {
        return energy > localAverage * energyThreshold && 
               energy > sensitivity / 100f && 
               (timeInSeconds - lastBeatTime) >= beatCooldown;
    }

    private float AdjustBeatTiming(float timeInSeconds)
    {
        float adjustedBeatTime = timeInSeconds + beatTimeOffset;
        
        if (beatQuantization > 0.001f)
        {
            adjustedBeatTime = Mathf.Round(adjustedBeatTime / beatQuantization) * beatQuantization;
        }
        
        return adjustedBeatTime;
    }

    private void Update()
    {
        if (!audioSource.isPlaying && !audioEndHandled)
        {
            HandleTrackEnd();
            return;
        }

        CheckForBeats();
    }

    private void CheckForBeats()
    {
        if (currentTrackIndex < 0 || currentTrackIndex >= beatTimestamps.Count)
            return;
            
        float[] trackBeats = beatTimestamps[currentTrackIndex];
        
        if (nextBeatIndex < trackBeats.Length && 
            (audioSource.time + lookAheadTime) >= trackBeats[nextBeatIndex])
        {
            TriggerOnBeat();
            nextBeatIndex++;
        }
    }

    private void HandleTrackEnd()
    {
        audioEndHandled = true;
        
        if (audioSource.time >= audioSource.clip.length - 0.5f || !audioSource.isPlaying)
        {
            int nextTrackIndex = (currentTrackIndex + 1) % trackList.Length;
            ChangeTrack(nextTrackIndex);
        }
    }

    private void TriggerOnBeat()
    {
        OnBeat?.Invoke();
    }

    public void ChangeTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= trackList.Length)
            return;

        currentTrackIndex = trackIndex;
        audioSource.clip = trackList[currentTrackIndex];
        audioSource.Play();
        
        nextBeatIndex = 0;
        audioEndHandled = false;

        TrackChanged?.Invoke();
    }

    // Event registration methods
    public void AddBeatListener(System.Action listener) => OnBeat += listener;
    public void RemoveBeatListener(System.Action listener) => OnBeat -= listener;
    public void AddTrackChangeListener(System.Action listener) => TrackChanged += listener;
    public void RemoveTrackChangeListener(System.Action listener) => TrackChanged -= listener;
}