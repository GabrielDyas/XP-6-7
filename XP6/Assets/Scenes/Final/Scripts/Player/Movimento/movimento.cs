using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Movimento : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float forcaDoPulo = 5f;
    [SerializeField] private LayerMask terreno;

    [Header("Dash")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashCooldown = 1f;

    // Classe gerada a partir do asset InputSystem_Actions
    private InputSystem_Actions input;

    private Rigidbody rb;
    private Transform cam;

    private Vector2 inputMovimento;
    private bool pedidoPulo;
    private bool pedidoDash;

    private bool podePular = true;
    private bool podeDashear = true;

    #region Ciclo de vida

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Player.Move.performed += AoMovimentar;
        input.Player.Move.canceled += AoMovimentar;
        input.Player.Jump.performed += AoPular;
        input.Player.Sprint.performed += AoDashear; // troque por input.Player.Dash se criar a ação

        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Move.performed -= AoMovimentar;
        input.Player.Move.canceled -= AoMovimentar;
        input.Player.Jump.performed -= AoPular;
        input.Player.Sprint.performed -= AoDashear;

        input.Player.Disable();

        inputMovimento = Vector2.zero;
        pedidoPulo = false;
        pedidoDash = false;
    }

    private void OnDestroy()
    {
        input.Dispose();
    }

    #endregion

    #region Callbacks do Input System

    private void AoMovimentar(InputAction.CallbackContext ctx) => inputMovimento = ctx.ReadValue<Vector2>();
    private void AoPular(InputAction.CallbackContext ctx) => pedidoPulo = true;
    private void AoDashear(InputAction.CallbackContext ctx) => pedidoDash = true;

    #endregion

    private void Update()
    {
        Girar();
    }

    private void FixedUpdate()
    {
        if (podePular)
            Movimentar();

        if (pedidoPulo)
        {
            pedidoPulo = false;
            if (podePular && Mathf.Abs(rb.linearVelocity.y) < 0.05f)
                Pular();
        }

        if (pedidoDash)
        {
            pedidoDash = false;
            if (podeDashear)
            {
                Dash();
                StartCoroutine(DashCooldownCoroutine());
            }
        }
    }

    #region Ações

    public void Movimentar()
    {
        Vector3 direcao = DirecaoRelativaACamera(inputMovimento);
        Vector3 novaVelocidade = direcao * velocidade;

        rb.linearVelocity = new Vector3(novaVelocidade.x, rb.linearVelocity.y, novaVelocidade.z);
    }

    private void Girar()
    {
        Vector3 direcaoCamera = cam.forward;
        direcaoCamera.y = 0;

        if (direcaoCamera != Vector3.zero)
        {
            Quaternion rotacao = Quaternion.LookRotation(direcaoCamera);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacao, Time.deltaTime * 10f);
        }
    }

    private void Pular()
    {
        Vector3 direcao = Vector3.up + rb.linearVelocity.normalized;
        rb.AddForce(direcao * forcaDoPulo, ForceMode.Impulse);
        podePular = false;
    }

    private void Dash()
    {
        podeDashear = false;

        Vector3 direcaoDash = DirecaoRelativaACamera(inputMovimento);

        if (direcaoDash == Vector3.zero)
            direcaoDash = transform.forward;

        rb.AddForce(direcaoDash * dashForce, ForceMode.Impulse);
    }

    private IEnumerator DashCooldownCoroutine()
    {
        yield return new WaitForSeconds(dashCooldown);
        podeDashear = true;
    }

    #endregion

    private void OnCollisionEnter(Collision col)
    {
        if (podePular) return;

        if (((1 << col.gameObject.layer) & terreno) != 0)
            podePular = true;
    }

    private Vector3 DirecaoRelativaACamera(Vector2 input)
    {
        Vector3 frente = cam.forward;
        Vector3 direita = cam.right;
        frente.y = 0;
        direita.y = 0;
        frente.Normalize();
        direita.Normalize();

        return (direita * input.x + frente * input.y).normalized;
    }
}