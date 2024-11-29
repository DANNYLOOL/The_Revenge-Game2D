using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataforma : MonoBehaviour
{
    private bool aplicarFuerza;
    private bool detectaJugador;
    private PlayerController player;

    public bool daSalto;
    public BoxCollider2D plataformaCollider;
    public BoxCollider2D plataformaTrigger;

    private void Awake()
    {
        BuscarJugador(); // Busca al jugador al iniciar
    }

    private void Start()
    {
        if (!daSalto)
        {
            Physics2D.IgnoreCollision(plataformaCollider, plataformaTrigger, true);
        }
    }

    private void Update()
    {
        // Asegúrate de que el jugador siga siendo válido
        if (player == null)
        {
            BuscarJugador();
            return;
        }

        if (daSalto)
        {
            if (player.transform.position.y - 0.8f > transform.position.y)
            {
                plataformaCollider.isTrigger = false;
            }
            else
            {
                plataformaCollider.isTrigger = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // Valida que el jugador exista antes de aplicar la fuerza
        if (aplicarFuerza && player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.up * 25, ForceMode2D.Impulse);
            }
            aplicarFuerza = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!daSalto && player != null && collision.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(plataformaCollider, player.GetComponent<CapsuleCollider2D>(), true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!daSalto && player != null && collision.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(plataformaCollider, player.GetComponent<CapsuleCollider2D>(), false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectaJugador = true;
            if (daSalto)
            {
                aplicarFuerza = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectaJugador = false;
        }
    }

    // Método para buscar al jugador si no está asignado
    private void BuscarJugador()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<PlayerController>();
        }
    }
}
