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

    private float minSoundVolume = 0.0001f;
    private float fadeDuration = 0.8f;

    private bool soundPlaying = false;
    private bool fadeInLerp = false;
    private bool fadeOutLerp = false;

    private bool setVolumeLayer = false;

    private string referenceName;
    private string child1RefName;
    private string child2RefName;
    private string child3RefName;


    void Start()
    {
        //PlayerPrefs.DeleteAll();
        referenceName = this.gameObject.name;
        child1RefName = referenceName + StringNames.child1Vol;
        child2RefName = referenceName + StringNames.child2Vol;
        child3RefName = referenceName + StringNames.child3Vol;

        MasterIntensity = minSoundVolume;

        AudioLayer1.volume = minSoundVolume;
        AudioLayer2.volume = minSoundVolume;
        AudioLayer3.volume = minSoundVolume;

        HashKeyCheck();

        SetVolume(StringNames.MasterVol, minSoundVolume);
        SetVolume(StringNames.drone1, minSoundVolume);
        SetVolume(StringNames.drone2, minSoundVolume);
        SetVolume(StringNames.drone3, minSoundVolume);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            FadeInAudio();
        else if (Input.GetKeyDown(KeyCode.Space))
            FadeOutAudio();

        SetVolume(StringNames.MasterVol, MasterIntensity);
        AdjustVolumeAccordingtoItsMaster(MasterIntensity);

        if (setVolumeLayer)
        {
            SetAudioSourceVolume(AudioLayer1, Layer_1_Intensity, child1RefName);
            SetAudioSourceVolume(AudioLayer2, Layer_2_Intensity, child2RefName);
            SetAudioSourceVolume(AudioLayer3, Layer_3_Intensity, child3RefName);
        }

    }

    #region Fade
    private void FadeInAudio()
    {
        Debug.Log("Playing...");
        PlayAudio();
        float layer1Soundlevel = PlayerPrefs.GetFloat(child1RefName);
        float layer2Soundlevel = PlayerPrefs.GetFloat(child2RefName);
        float layer3Soundlevel = PlayerPrefs.GetFloat(child3RefName);

        StartCoroutine(StartFade(AudioLayer1, fadeDuration, layer1Soundlevel));
        StartCoroutine(StartFade(AudioLayer2, fadeDuration, layer2Soundlevel));
        StartCoroutine(StartFade(AudioLayer3, fadeDuration, layer3Soundlevel));
        Invoke("MatchAudioVolume_with_Layer", fadeDuration);

    }

    private void FadeOutAudio()
    {
        Debug.Log("Stoping...");
        DontMatchAudioVolume_with_Layer();
        StartCoroutine(StartFade(AudioLayer1, fadeDuration, minSoundVolume));
        StartCoroutine(StartFade(AudioLayer2, fadeDuration, minSoundVolume));
        StartCoroutine(StartFade(AudioLayer3, fadeDuration, minSoundVolume));
        Invoke("StopAudio", fadeDuration);
    }
    private void MatchAudioVolume_with_Layer()
    {
        setVolumeLayer = true;
    }
    private void DontMatchAudioVolume_with_Layer()
    {
        setVolumeLayer = false;
    }
    private void PlayAudio()
    {
        AudioLayer1.volume = minSoundVolume;
        AudioLayer1.Play();
        AudioLayer2.volume = minSoundVolume;
        AudioLayer2.Play();
        AudioLayer3.volume = minSoundVolume;
        AudioLayer3.Play();
    }
    private void StopAudio()
    {
        AudioLayer1.Stop();
        AudioLayer2.Stop();
        AudioLayer3.Stop();
    }
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            //print(layer);
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
        PlayerPrefs.SetFloat(name, value);
    }

    private void HashKeyCheck()
    {
        if (PlayerPrefs.HasKey(child1RefName))
        {
            //Debug.Log("The key " + StringNames.child1Vol + " exists");
            Layer_1_Intensity = PlayerPrefs.GetFloat(child1RefName);

        }
        else
        {
            //Debug.Log("The key " + StringNames.child1Vol + " does not exist");
            PlayerPrefs.SetFloat(child1RefName, minSoundVolume);
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
            PlayerPrefs.SetFloat(child2RefName, minSoundVolume);
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
            PlayerPrefs.SetFloat(child3RefName, minSoundVolume);
            Layer_3_Intensity = PlayerPrefs.GetFloat(child3RefName);
        }
    }
}
