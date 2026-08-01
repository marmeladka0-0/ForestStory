using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Источники звука")]
    [SerializeField] private AudioSource musicSource; // Для фоновой музыки
    [SerializeField] private AudioSource sfxSource;   // Для шагов и коротких звуков

    [Header("Стартовые звуки")]
    [SerializeField] private AudioClip backgroundMusic; // Фоновое сопровождение
    [SerializeField] private AudioClip stepSound;       // Звук шага

    [SerializeField] private float musicVolume = 0.1f;

    [Header("Настройки шагов")]
    [SerializeField] private float stepInterval = 0.4f; // Частота шагов в секундах
    private float stepTimer = 0f;

    private void Awake()
    {
        // Делаем менеджер единым для всех сцен
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
        // Автоматически запускаем фоновую музыку при старте игры
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }

    // Воспроизведение фоновой музыки
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Вызывается из скрипта движения, когда персонаж идет
    public void PlayFootstep()
    {
        stepTimer += Time.deltaTime;

        if (stepTimer >= stepInterval)
        {
            if (sfxSource != null && stepSound != null)
            {
                // Небольшая рандомизация тона (Pitch) делает шаги живыми, а не роботоподобными
                sfxSource.pitch = Random.Range(0.9f, 1.1f);
                sfxSource.PlayOneShot(stepSound, 1.0f); // 0.6f — громкость шага
            }

            stepTimer = 0f; // Сбрасываем таймер
        }
    }

    // Сброс таймера шагов при остановке
    public void ResetStepTimer()
    {
        stepTimer = stepInterval; // Чтобы следующий шаг сработал сразу при начале движения
    }
}