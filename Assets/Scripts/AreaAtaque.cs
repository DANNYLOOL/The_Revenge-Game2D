using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAtaque : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    Debug.Log("Exito");
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.CompareTag("Enemigo"))
        {
            Debug.Log("Aplicar daño a enemigo");
        }
    }

}
