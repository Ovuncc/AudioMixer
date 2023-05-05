 using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("DronesMaster")]
    [Range(0.0001f, 1)]
    [SerializeField] private float MasterIntensity;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Layer1")]
    [Range(0.0001f, 1)]
    [SerializeField] private float Layer_1_Intensity;
    [SerializeField] private AudioSource AudioLayer1;

    [Header("Layer2")]
    [Range(0.0001f, 1)]
    [SerializeField] private float Layer_2_Intensity;
    [SerializeField] private AudioSource AudioLayer2;

    [Header("Layer3")]
    [Range(0.0001f, 1)]
    [SerializeField] private float Layer_3_Intensity;
    [SerializeField] private AudioSource AudioLayer3;

    private const float minSoundVolume = 0.0001f;
    private const float maxSoundVolume = 1f;

    private const float fadeDuration = 0.8f;

    private static bool isFading = false;
    private static bool isPlaying = true;

    private string referenceName;
    private string dronesMasterName;
    private string child1RefName;
    private string child2RefName;
    private string child3RefName;


    void Start()
    {
        //PlayerPrefs.DeleteAll();
        referenceName = this.gameObject.name;
        dronesMasterName = referenceName + StringNames.DronesMaster;
        child1RefName = referenceName + StringNames.child1Vol;
        child2RefName = referenceName + StringNames.child2Vol;
        child3RefName = referenceName + StringNames.child3Vol;

        //MasterIntensity = maxSoundVolume;

        AudioLayer1.volume = maxSoundVolume;
        AudioLayer2.volume = maxSoundVolume;
        AudioLayer3.volume = maxSoundVolume;

        HashKeyCheck();

        SetVolume(StringNames.DronesMaster, maxSoundVolume);
        SetVolume(StringNames.drone1, maxSoundVolume);
        SetVolume(StringNames.drone2, maxSoundVolume);
        SetVolume(StringNames.drone3, maxSoundVolume);
        DeactivateAudioSources();
        StopAudio();
      
        Debug.Log("To play audio: K");
        Debug.Log("To stop audio: Space");
        Debug.Log("To save the data: S");

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K) && !isFading) // Play
            PlayAudio();
        else if (Input.GetKeyDown(KeyCode.Space) && !isFading) // Stop
            StopAudio();
        else if (Input.GetKeyDown(KeyCode.S))
            SaveValues();

        if (isPlaying)
            ManageSoundLayers();
    }

    private void ManageSoundLayers()
    {
        SetVolume(StringNames.DronesMaster, MasterIntensity);
        AdjustVolumeAccordingtoItsMaster(MasterIntensity);
        SetAudioSourceVolume(AudioLayer1, Layer_1_Intensity, child1RefName);
        SetAudioSourceVolume(AudioLayer2, Layer_2_Intensity, child2RefName);
        SetAudioSourceVolume(AudioLayer3, Layer_3_Intensity, child3RefName);
    }

    private void SaveValues()
    {
        Debug.Log("Values successfully saved!");
        PlayerPrefs.SetFloat(dronesMasterName, MasterIntensity);
        PlayerPrefs.SetFloat(child1RefName, Layer_1_Intensity);
        PlayerPrefs.SetFloat(child2RefName, Layer_2_Intensity);
        PlayerPrefs.SetFloat(child3RefName, Layer_3_Intensity);
    }

    #region Fade

    private void PlayAudio()
    {
        ActivateAudioSources();
        float startVal = -80f;
        float targetVolume = 0f;
        audioMixer.GetFloat(StringNames.MasterVol, out startVal);
        bool fadeOkayCheck = (startVal != targetVolume);
        if (fadeOkayCheck)
        {
            Debug.Log("Playing...");
            StartCoroutine(FadeDroneMaster(fadeDuration, 0.0001f, PlayerPrefs.GetFloat(dronesMasterName)));
            StartCoroutine(FadeAudioSource(AudioLayer1, fadeDuration, 0f, Layer_1_Intensity));
            StartCoroutine(FadeAudioSource(AudioLayer2, fadeDuration, 0f, Layer_2_Intensity));
            StartCoroutine(FadeAudioSource(AudioLayer3, fadeDuration, 0f, Layer_3_Intensity));
            //StartCoroutine(FadeMaster(audioMixer, fadeDuration, startVal, targetVolume));
            Invoke("StartFadingMaster", 0.3f);

        }
        else
        {
            Debug.Log("Already playing!");
        }
    }
    private void StartFadingMaster()
    {
        float startVal = -80f;
        float targetVolume = 0f;
        StartCoroutine(FadeMaster(audioMixer, fadeDuration, startVal, targetVolume));
    }
    private void StopAudio()
    {
        float startVal = 0f;
        float targetVolume = -80f;
        audioMixer.GetFloat(StringNames.MasterVol, out startVal);
        bool fadeOkayCheck = (startVal != targetVolume);
        if (fadeOkayCheck)
        {
            Debug.Log("Stoping...");
            StartCoroutine(FadeMaster(audioMixer, fadeDuration, startVal, targetVolume));
            StartCoroutine(FadeDroneMaster(fadeDuration, PlayerPrefs.GetFloat(dronesMasterName), 0.0001f));
            StartCoroutine(FadeAudioSource(AudioLayer1, fadeDuration, Layer_1_Intensity, 0f));
            StartCoroutine(FadeAudioSource(AudioLayer2, fadeDuration, Layer_2_Intensity, 0f));
            StartCoroutine(FadeAudioSource(AudioLayer3, fadeDuration, Layer_3_Intensity, 0f));
            Invoke("DeactivateAudioSources", fadeDuration);
        }
        else
        {
            Debug.Log("Already stopped!");
        }
    }

    private static IEnumerator FadeMaster(AudioMixer mixer,float duration, float start, float targetVolume)
    {
        float currentTime = 0;
        isFading = true;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            mixer.SetFloat(StringNames.MasterVol, newVolume);
            yield return null;
        }
        isFading = false;
        yield break;

    }
    private IEnumerator FadeDroneMaster(float duration, float start, float targetVolume)
    {
        float currentTime2 = 0;
        while (currentTime2 < duration)
        {
            currentTime2 += Time.deltaTime;
            float newVolume2 = Mathf.Lerp(start, targetVolume, currentTime2 / duration);
            MasterIntensity = newVolume2;
            ManageSoundLayers();
            yield return null;
        }

        yield break;

    }
    private IEnumerator FadeAudioSource(AudioSource audioSource, float duration, float start, float targetVolume)
    {
        float currentTime3 = 0;
        while (currentTime3 < duration)
        {
            currentTime3 += Time.deltaTime;
            float newVolume2 = Mathf.Lerp(start, targetVolume, currentTime3 / duration);
            audioSource.volume = newVolume2;
            yield return null;
        }

        yield break;
    }

    private void ActivateAudioSources()
    {
        AudioLayer1.Play();
        AudioLayer2.Play();
        AudioLayer3.Play();
        isPlaying = true;
    }
    private void DeactivateAudioSources()
    {
        AudioLayer1.Stop();
        AudioLayer2.Stop();
        AudioLayer3.Stop();
        isPlaying = false;
    }

    private IEnumerator Delay(float duration)
    {
        float currentTime3 = 0;
        while (currentTime3 < duration)
        {
            currentTime3 += Time.deltaTime;
            yield return null;
        }

        yield break;
    }

    #endregion

    private void SetVolume(string name,float sliderValue)
    {
        audioMixer.SetFloat(name, Mathf.Log10(sliderValue) * 20);
    }

    private void AdjustVolumeAccordingtoItsMaster(float master_intensity)
    {
        if (master_intensity >= 0)
        {
            audioMixer.SetFloat(StringNames.drone1, Mathf.Log10(master_intensity) * 20);
            audioMixer.SetFloat(StringNames.drone2, Mathf.Log10(minSoundVolume) * 20);
            audioMixer.SetFloat(StringNames.drone3, Mathf.Log10(minSoundVolume) * 20);
            if (master_intensity >= 0.333f)
            {
                audioMixer.SetFloat(StringNames.drone2, Mathf.Log10((master_intensity - 0.332f)/0.666f) * 20);
                audioMixer.SetFloat(StringNames.drone3, Mathf.Log10(minSoundVolume) * 20);

                if (master_intensity >= 0.666f)
                {
                     audioMixer.SetFloat(StringNames.drone3, Mathf.Log10((master_intensity - 0.665f)/0.333f) * 20);
                }
            }
        }

    }

    private void SetAudioSourceVolume(AudioSource audioSource, float value, string name)
    {
        audioSource.volume = value;
        //PlayerPrefs.SetFloat(name, value);
    }
   
    private void HashKeyCheck()
    {
        if (PlayerPrefs.HasKey(dronesMasterName))
        {
            //Debug.Log("The key " + StringNames.DronesMaster + " exists");
            MasterIntensity = PlayerPrefs.GetFloat(dronesMasterName);

        }
        else
        {
            //Debug.Log("The key " + StringNames.DronesMaster + " does not exist");
            PlayerPrefs.SetFloat(dronesMasterName, maxSoundVolume);
            MasterIntensity = PlayerPrefs.GetFloat(dronesMasterName);
        }

        if (PlayerPrefs.HasKey(child1RefName))
        {
            //Debug.Log("The key " + StringNames.child1Vol + " exists");
            Layer_1_Intensity = PlayerPrefs.GetFloat(child1RefName);

        }
        else
        {
            //Debug.Log("The key " + StringNames.child1Vol + " does not exist");
            PlayerPrefs.SetFloat(child1RefName, maxSoundVolume);
            Layer_1_Intensity = PlayerPrefs.GetFloat(child1RefName);
        }

        if (PlayerPrefs.HasKey(child2RefName))
        {
            //Debug.Log("The key " + StringNames.child2Vol + " exists");
            Layer_2_Intensity = PlayerPrefs.GetFloat(child2RefName);

        }
        else
        {
            //Debug.Log("The key " + StringNames.child2Vol + " does not exist");
            PlayerPrefs.SetFloat(child2RefName, maxSoundVolume);
            Layer_2_Intensity = PlayerPrefs.GetFloat(child2RefName);
        }

        if (PlayerPrefs.HasKey(child3RefName))
        {
            //Debug.Log("The key " + StringNames.child3Vol + " exists");
            Layer_3_Intensity = PlayerPrefs.GetFloat(child3RefName);

        }
        else
        {
            //Debug.Log("The key " + StringNames.child3Vol + " does not exist");
            PlayerPrefs.SetFloat(child3RefName, maxSoundVolume);
            Layer_3_Intensity = PlayerPrefs.GetFloat(child3RefName);
        }
    }
}
