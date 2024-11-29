using UnityEngine;

public class SceneAudioManager : MonoBehaviour
{
    // Referencias a los objetos que contienen los AudioSources
    public GameObject sonido1;
    public GameObject sonido2;
    public GameObject sonido3;
    public GameObject sonido4;

    private GameObject[] sonidos; // Array para almacenar los sonidos

    void Awake()
    {
        // Aseg�rate de que el AudioManager no se destruya entre cambios de nivel
        DontDestroyOnLoad(gameObject);

        // Almacenar los objetos de sonido en el array
        sonidos = new GameObject[] { sonido1, sonido2, sonido3, sonido4 };

        // Inicialmente, desactivar todos los sonidos
        DesactivarTodosLosSonidos();
    }

    void OnEnable()
    {
        GameManager.OnNivelCambio += CambiarMusica; // Escuchar el evento de cambio de nivel
    }

    void OnDisable()
    {
        GameManager.OnNivelCambio -= CambiarMusica; // Dejar de escuchar cuando el objeto se desactive
    }

    // M�todo para cambiar la m�sica seg�n el nivel
    public void CambiarMusica(int indiceNivel)
    {
        // Desactivar todos los sonidos antes de activar el correspondiente
        DesactivarTodosLosSonidos();

        // Verificar si el �ndice est� dentro del rango de los objetos de sonido
        if (indiceNivel >= 0 && indiceNivel < sonidos.Length)
        {
            // Verificar si el objeto de sonido est� disponible (no es null)
            if (sonidos[indiceNivel] != null)
            {
                sonidos[indiceNivel].SetActive(true);  // Activar el sonido correspondiente
                AudioSource audioSource = sonidos[indiceNivel].GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play(); // Reproducir el audio
                }
            }
            else
            {
                Debug.LogWarning($"El objeto de sonido en el �ndice {indiceNivel} ha sido destruido o no est� disponible.");
            }
        }
        else
        {
            Debug.LogWarning($"�ndice de nivel inv�lido: {indiceNivel}");
        }
    }


    // M�todo para desactivar todos los sonidos
    private void DesactivarTodosLosSonidos()
    {
        foreach (GameObject sonido in sonidos)
        {
            if (sonido != null) // Asegurarse de que el objeto de sonido no sea null
            {
                sonido.SetActive(false); // Desactivar el sonido
            }
        }
    }

}
