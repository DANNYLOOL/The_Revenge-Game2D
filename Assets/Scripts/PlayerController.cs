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

    [Header("Estadisticas")]
    public float velocidadDeMovimiento = 10;
    public float fuerzaDeSalto = 5;
    public float velocidadDash = 20;

    [Header("Colisiones")]
    public LayerMask layerPiso;
    public float radioDeColision;
    public Vector2 abajo;

    [Header("Booleanos")]
    public bool puedeMover = true;
    public bool enSuelo = true;
    public bool puedeDash;
    public bool haciendoDash;
    public bool tocadoPiso;
    public bool haciendoShake;

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

        Caminar();

        MejorarSalto();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (enSuelo)
            {
                anim.SetBool("saltar", true);
                Saltar();
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
    }

    private void Saltar()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * fuerzaDeSalto;
    }

    private void Caminar()
    {
        if (puedeMover && !haciendoDash)
        {
            rb.velocity = new Vector2(direccion.x * velocidadDeMovimiento, rb.velocity.y);

            if (direccion != Vector2.zero)
            {
                if (!enSuelo)
                {
                    anim.SetBool("saltar", true);
                }
                else
                {
                    anim.SetBool("caminar", true);
                }

                if (direccion.x < 0 && transform.localScale.x > 0)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else if (direccion.x > 0 && transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                anim.SetBool("caminar", false);
            }
        }
    }
}
