using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // Referencia al objeto interactuable actual
    public Interactuable interactuableActual;

    // Método para interactuar con el objeto actual
    public void Interactuar()
    {
        if (interactuableActual != null)
        {
            interactuableActual.OnInteractuarButtonPressed();
        }
    }

    // Detecta cuando el jugador entra en el rango de un objeto interactuable
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactuable"))
        {
            interactuableActual = collision.GetComponent<Interactuable>();

            // Opcional: Muestra un indicador para el jugador
            if (interactuableActual != null && interactuableActual.indicadorInteractuable != null)
            {
                interactuableActual.indicadorInteractuable.SetActive(true);
            }
        }
    }

    // Detecta cuando el jugador sale del rango de un objeto interactuable
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactuable"))
        {
            // Verifica que el objeto que sale del rango sea el mismo que el interactuable actual
            if (interactuableActual == collision.GetComponent<Interactuable>())
            {
                // Oculta el indicador y limpia la referencia
                if (interactuableActual.indicadorInteractuable != null)
                {
                    interactuableActual.indicadorInteractuable.SetActive(false);
                }

                interactuableActual = null;
            }
        }
    }
}
