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


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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

    private void Update()
    {
        if (daSalto) 
        {
            if(player.transform.position.y - 0.8f > transform.position.y)
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
        if (aplicarFuerza)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 25, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }



}
