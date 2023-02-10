using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    [Header("Master")]
    [Range(0.0001f, 1)]
    [SerializeField] private float MasterIntensity;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Layer1")]
    [Range(0, 1)]
    [SerializeField] private float Layer_1_Intensity;
    [SerializeField] private AudioSource AudioLayer1;

    [Header("Layer2")]
    [Range(0, 1)]
    [SerializeField] private float Layer_2_Intensity;
    [SerializeField] private AudioSource AudioLayer2;


    [Header("Layer3")]
    [Range(0, 1)]
    [SerializeField] private float Layer_3_Intensity;
    [SerializeField] private AudioSource AudioLayer3;


    // Start is called before the first frame update
    void Start()
    {
        MasterIntensity = 0.0001f;
        Layer_1_Intensity = 1f;
        Layer_2_Intensity = 1f;
        Layer_3_Intensity = 1f;

        SetVolume(StringNames.MasterVol, 0.00001f);
        SetVolume(StringNames.drone1, 0.00001f);
        SetVolume(StringNames.drone2, 0.00001f);
        SetVolume(StringNames.drone3, 0.00001f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            PlayAudio();
        else if (Input.GetKeyDown(KeyCode.Space))
            StopAudio();

        SetVolume(StringNames.MasterVol, MasterIntensity);
        AdjustVolumeAccordingtoItsMaster(MasterIntensity);
        SetAudioSourceVolume(AudioLayer1, Layer_1_Intensity);
        SetAudioSourceVolume(AudioLayer2, Layer_2_Intensity);
        SetAudioSourceVolume(AudioLayer3, Layer_3_Intensity);


    }

    private void PlayAudio()
    {
        Debug.Log("Playing...");
        AudioLayer1.Play();
        AudioLayer2.Play();
        AudioLayer3.Play();

    }

    private void StopAudio()
    {
        Debug.Log("Stoping...");
        AudioLayer1.Stop();
        AudioLayer2.Stop();
        AudioLayer3.Stop();

    }

    private void SetVolume(string name,float sliderValue)
    {
        audioMixer.SetFloat(name, Mathf.Log10(sliderValue) * 20);
    }

    private void AdjustVolumeAccordingtoItsMaster(float master_intensity)
    {
        if (master_intensity >= 0)
        {
            audioMixer.SetFloat(StringNames.drone1, Mathf.Log10(master_intensity) * 20);
            audioMixer.SetFloat(StringNames.drone2, Mathf.Log10(0.0001f) * 20);
            audioMixer.SetFloat(StringNames.drone3, Mathf.Log10(0.0001f) * 20);
            if (master_intensity >= 0.333f)
            {
                audioMixer.SetFloat(StringNames.drone2, Mathf.Log10((master_intensity - 0.332f)/0.666f) * 20);
                audioMixer.SetFloat(StringNames.drone3, Mathf.Log10(0.0001f) * 20);

                if (master_intensity >= 0.666f)
                {
                     audioMixer.SetFloat(StringNames.drone3, Mathf.Log10((master_intensity - 0.665f)/0.333f) * 20);
                }
            }
        }

    }

    private void SetAudioSourceVolume(AudioSource audioSource, float value)
    {
        audioSource.volume = value;
    }
}
