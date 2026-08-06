using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources of the sound")]
    [SerializeField] private AudioSource musicSource; // background music
    [SerializeField] private AudioSource sfxSource;   // sound of steps and small sounds

    [Header("Start sounds")]
    [SerializeField] private AudioClip backgroundMusic; 
    [SerializeField] private AudioClip stepSound;       

    [SerializeField] private float musicVolume = 0.1f; //background music volume

    [Header("Настройки шагов")]
    [SerializeField] private float stepInterval = 0.4f; //Interval of the steps
    private float stepTimer = 0f;

    private void Awake()
    {
        //I am not so sure that it should be like this!!!!!
        //but this make sure that the sound is the same for all scenes
        //kinda not that good, you know
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Start background music when the game start
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }

    //do a loop for music
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    //When the character move => sound of steps
    //Is called from the characterController2D script
    public void PlayFootstep()
    {
        stepTimer += Time.deltaTime;

        if (stepTimer >= stepInterval)
        {
            if (sfxSource != null && stepSound != null)
            {
                //randomization of pitch to make step sounds more random
                sfxSource.pitch = Random.Range(0.9f, 1.1f);
                sfxSource.PlayOneShot(stepSound, 1.0f);
            }

            stepTimer = 0f;
        }
    }

    //Turn of the timer if the character stoped, to start from the start of the sound next time
    public void ResetStepTimer()
    {
        stepTimer = stepInterval;
    }
}