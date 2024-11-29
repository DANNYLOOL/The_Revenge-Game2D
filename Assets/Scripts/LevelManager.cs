using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para cargar escenas

public class LevelManager : MonoBehaviour
{
    public SceneSoundManager soundManager;  // Referencia al script de sonido

    // Método para cargar un nivel y cambiar el sonido
    public void LoadLevel(int level)
    {
        soundManager.ChangeLevelSound(level);  // Cambiar el sonido del nivel

        // Cargar la escena correspondiente
        string sceneName = "Nivel" + level;  // Ejemplo: "Nivel1", "Nivel2", etc.
        SceneManager.LoadScene(sceneName);
    }
}
