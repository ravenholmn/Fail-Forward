using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    [SerializeField] private AudioSource audioObject;
    [SerializeField] private AudioMixerGroup sfxMixer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaySFX(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(audioObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = clip;

        audioSource.outputAudioMixerGroup = sfxMixer;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLenghth = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLenghth);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
