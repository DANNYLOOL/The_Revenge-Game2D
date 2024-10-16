using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 direccion;
    private CinemachineVirtualCamera cm;
    private Vector2 direccionMovimiento;

    [Header("Estadisticas")]
    public float velocidadDeMovimiento = 10;
    public float fuerzaDeSalto = 5;
    public float velocidadDash = 20;
    public float velocidadDeslizar;

    [Header("Colisiones")]
    public LayerMask layerPiso;
    public float radioDeColision;
    public Vector2 abajo, derecha, izquierda;
    

    [Header("Booleanos")]
    public bool puedeMover = true;
    public bool enSuelo = true;
    public bool puedeDash;
    public bool haciendoDash;
    public bool tocadoPiso;
    public bool haciendoShake;
    public bool estaAtacando;
    public bool enMuro;
    public bool muroDerecho;
    public bool muroIzquierdo;
    public bool agarrarse;
    public bool saltarDeMuro;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        Agarres();
    }

    private void Atacar(Vector2 direccion)
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!estaAtacando && !haciendoDash)
            {
                estaAtacando = true;

                anim.SetFloat("ataqueX", direccion.x);
                anim.SetFloat("ataqueY", direccion.y);

                anim.SetBool("atacar", true);

            }    
        }
    }

    public void FinalizarAtaque()
    {
        anim.SetBool("atacar", false);
        estaAtacando = false;
    }

    private Vector2 DireccionAtaque(Vector2 direccionMovimiento, Vector2 direccion)
    {
        if (rb.velocity.x == 0 && direccion.y != 0)
        {
            return new Vector2(0, direccion.y);
        }

        return new Vector2(direccionMovimiento.x, direccion.y);
    }

    private IEnumerator AgitarCamara()
    {
        haciendoShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(0.3f);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        haciendoShake = false;
    }

    private IEnumerator AgitarCamara(float tiempo)
    {
        haciendoShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(tiempo);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        haciendoShake = false;
    }

    private void Dash(float x, float y)
    {
        anim.SetBool("dash", true);
        Vector3 posicionJugador = Camera.main.WorldToViewportPoint(transform.position);
        Camera.main.GetComponent<RippleEffect>().Emit(posicionJugador);
        StartCoroutine(AgitarCamara());

        puedeDash = true;
        rb.velocity = Vector2.zero;
        rb.velocity += new Vector2(x, y).normalized * velocidadDash;
        StartCoroutine(PrepararDash());
    }

    private IEnumerator PrepararDash()
    {
        StartCoroutine(DashSuelo());
        rb.gravityScale = 0;
        haciendoDash = true;

        yield return new WaitForSeconds(0.3f);

        rb.gravityScale = 3;
        haciendoDash = false;
        FinalizarDash();
    }

    private IEnumerator DashSuelo()
    {
        yield return new WaitForSeconds(0.15f);
        if (enSuelo)
        {
            puedeDash = false;
        }
    }

    public void FinalizarDash()
    {
        anim.SetBool("dash", false);
    }

    private void TocarPiso()
    {
        puedeDash = false;
        haciendoDash = false;
        anim.SetBool("saltar", false);
    }

    private void Movimiento()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        direccion = new Vector2(x, y);
        Vector2 direccionRaw = new Vector2(xRaw, yRaw);

        Caminar();
        Atacar(DireccionAtaque(direccionMovimiento, direccionRaw));
        
        if(enSuelo && !haciendoDash)
        {
            saltarDeMuro = false;
        }

        agarrarse = enMuro && Input.GetKey(KeyCode.LeftShift);

        if (enMuro)
        {
            anim.SetBool("escalar", true);
            if (rb.velocity == Vector2.zero)
            {
                anim.SetFloat("velocidad", 0);
            }
            else
            {
                anim.SetFloat("velocidad", 1);
            }
        }
        else 
        {
            anim.SetBool("escalar", false);
            anim.SetFloat("velocidad", 0);

        }

        if (agarrarse && !haciendoDash) 
        {
            rb.gravityScale = 0;
            if (x > 0.2f || x < -0.2f) 
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            float modificadorVelocidad = y > 0 ? 0.5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (velocidadDeMovimiento * modificadorVelocidad));
            
            if(muroIzquierdo && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }else if(muroDerecho && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

        }
        else
        {
            rb.gravityScale = 3;
        }

        if(enMuro && !enSuelo)
        {
            if(x != 0 && !agarrarse)
            {
                DeslizarPared();
            }
        }


        MejorarSalto();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (enSuelo)
            {
                anim.SetBool("saltar", true);
                Saltar();
            }

            if(enMuro && !enSuelo)
            {
                anim.SetBool("escalar", false);
                anim.SetBool("saltar", true);
                SaltarDesdeMuro();
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && !haciendoDash)
        {
            if (xRaw != 0 || yRaw != 0)
            {
                Dash(xRaw, yRaw);
            }
        }

        if (enSuelo && !tocadoPiso)
        {
            anim.SetBool("escalar", false);

            TocarPiso();
            tocadoPiso = true;
        }

        if (!enSuelo && tocadoPiso)
        {
            tocadoPiso = false;
        }

        float velocidad;
        if (rb.velocity.y > 0)
        {
            velocidad = 1;
        }
        else
        {
            velocidad = -1;
        }

        if (!enSuelo)
        {
            anim.SetFloat("velocidadVertical", velocidad);
        }
        else
        {
            if (velocidad == -1)
            {
                FinalizarSalto();
            }
        }
    }

    private void DeslizarPared()
    {
        if(puedeMover)
        {
            rb.velocity = new Vector2(rb.velocity.x, -velocidadDeslizar);
        }
    }

    private void SaltarDesdeMuro()
    {
        StopCoroutine(DeshabilitarMovimiento(0));
        StartCoroutine(DeshabilitarMovimiento(0.1f));

        Vector2 direccionMuro = muroDerecho ? Vector2.left : Vector2.right;

        if(direccion.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }else if (direccion.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        anim.SetBool("saltar", true);
        anim.SetBool("escalar", false);
        Saltar((Vector2.up + direccionMuro), true);

        saltarDeMuro = true;
    }

    private IEnumerator DeshabilitarMovimiento(float tiempo)
    {
        puedeMover = false;
        yield return new WaitForSeconds(tiempo);
        puedeMover = true;
    }


    public void FinalizarSalto()
    {
        anim.SetBool("saltar", false);
    }

    private void MejorarSalto()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f - 1) * Time.deltaTime;
        }
    }

    private void Agarres()
    {
        enSuelo = Physics2D.OverlapCircle((Vector2)transform.position + abajo, radioDeColision, layerPiso);

        muroDerecho = Physics2D.OverlapCircle((Vector2)transform.position + derecha, radioDeColision, layerPiso);
        muroIzquierdo = Physics2D.OverlapCircle((Vector2)transform.position + izquierda, radioDeColision, layerPiso);

        enMuro = muroDerecho || muroIzquierdo;
    }

    private void Saltar()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * fuerzaDeSalto;
    }

    private void Saltar(Vector2 direccionSalto, bool muro)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += direccionSalto * fuerzaDeSalto;
    }

    private void Caminar()
    {
        if (puedeMover && !haciendoDash)
        {
            if(saltarDeMuro)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(direccion.x * velocidadDeMovimiento, rb.velocity.y)), Time.deltaTime / 2);
            }
            else
            {
                if (direccion != Vector2.zero && !agarrarse)
                {
                    if (!enSuelo)
                    {
                        anim.SetBool("saltar", true);
                    }
                    else
                    {
                        anim.SetBool("caminar", true);
                    }

                    rb.velocity = (new Vector2(direccion.x * velocidadDeMovimiento, rb.velocity.y));
                    if (direccion.x < 0 && transform.localScale.x > 0)
                    {
                        direccionMovimiento = DireccionAtaque(Vector2.left, direccion);
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                    else if (direccion.x > 0 && transform.localScale.x < 0)
                    {
                        direccionMovimiento = DireccionAtaque(Vector2.right, direccion);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                }
                else
                {
                    if (direccion.y > 0 && direccion.x == 0)
                    {
                        direccionMovimiento = DireccionAtaque(direccion, Vector2.up);
                    }
                    anim.SetBool("caminar", false);
                }
            }

            
        }
    }
}
