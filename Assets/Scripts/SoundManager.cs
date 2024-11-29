using UnityEngine;

public class SceneSoundManager : MonoBehaviour
{
    public AudioClip nivel1Sound;  // Sonido del Nivel 1
    public AudioClip nivel2Sound;  // Sonido del Nivel 2
    public AudioClip nivel3Sound;  // Sonido del Nivel 3
    public AudioClip nivel4Sound;  // Sonido del Nivel 4

    private AudioSource audioSource;

    // M�todo que se llama cuando se inicia el juego
    void Start()
    {
        audioSource = GetComponent<AudioSource>();  // Obt�n el componente AudioSource
    }

    // M�todo para cambiar el sonido seg�n el nivel
    public void ChangeLevelSound(int level)
    {
        switch (level)
        {
            case 1:
                audioSource.clip = nivel1Sound;
                break;
            case 2:
                audioSource.clip = nivel2Sound;
                break;
            case 3:
                audioSource.clip = nivel3Sound;
                break;
            case 4:
                audioSource.clip = nivel4Sound;
                break;
            default:
                audioSource.clip = null;  // No sonido si no es ninguno de los niveles
                break;
        }
        audioSource.Play();  // Reproduce el sonido asignado
    }
}
