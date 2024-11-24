using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool ejecutando;

    public static GameManager instance;
    public GameObject vidasUI;
    public PlayerController player;
    public Text textoMonedas;
    public int monedas;
    public Text guardarPartidaTexto;

    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelCarga;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (PlayerPrefs.GetInt("vidas") != 0)
        {
            CargarPartida();
        }
    }

    public void GuardarPartida()
    {
        float x, y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int vidas = player.vidas;

        PlayerPrefs.SetInt("monedas", monedas);
        PlayerPrefs.SetFloat("x", x);
        PlayerPrefs.SetFloat("y", y);
        PlayerPrefs.SetInt("vidas", vidas);

        if (!ejecutando)
        {
            StartCoroutine(MostrarTextoGuardado());
        }
    }

    private IEnumerator MostrarTextoGuardado()
    {
        ejecutando = true;
        guardarPartidaTexto.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        guardarPartidaTexto.gameObject.SetActive(false);
        ejecutando = false;
    }

    public void CargarPartida()
    {
        monedas = PlayerPrefs.GetInt("monedas");
        player.transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        player.vidas = PlayerPrefs.GetInt("vidas");
        textoMonedas.text = monedas.ToString();

        int vidasADescontar = 3 - player.vidas;

        player.ActualizarVidasUI(vidasADescontar);
    }

    public void ActualizarContadorMonedas()
    {
        monedas++;
        textoMonedas.text =  monedas.ToString();
    }

    public void PausarJuego()
    {
        Time.timeScale = 0;
        panelPausa.SetActive(true);
    }

    public void DespausarJuego()
    {
        Time.timeScale = 1;
        panelPausa.SetActive(false);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void CargarSelector()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    }

    public void CargarEscena(string escenaACargar)
    {
        SceneManager.LoadScene(escenaACargar);
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    public void CargarEscenaSelector()
    {
        StartCoroutine(CargarEscena());
    }

    private IEnumerator CargarEscena()
    {
        panelCarga.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelSelect");

        //Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            //yield return new WaitForSeconds(1);
            yield return null;
        }
    }
}
