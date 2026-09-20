using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Movimento : MonoBehaviour
{
    public InputActionReference acaoMovimentar;
    public float velocidade = 5f;
    public float forcaDoPulo = 5f;
    public InputActionReference acaoVirar;
    public InputActionReference acaoDashear;
    public float dashForce = 10f;
    public float dashCooldown = 1f;
    private bool podeDashear = true;
    private bool podePular = true;
    private bool puloDuplo = false;
    public LayerMask terreno;


    private Rigidbody rb;
    private Vector3 inputVetor;

    void OnEnable()
    {
        if (acaoMovimentar != null)
            acaoMovimentar.action.Enable();
        if (acaoVirar != null)
            acaoVirar.action.Enable();
        if (acaoDashear != null)
            acaoDashear.action.Enable();
    }

    void OnDisable()
    {
        if (acaoMovimentar != null)
            acaoMovimentar.action.Disable();
        if (acaoVirar != null)
            acaoVirar.action.Disable();
        if (acaoDashear != null)
            acaoDashear.action.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (acaoMovimentar != null)
        {
            inputVetor = acaoMovimentar.action.ReadValue<Vector3>();
        }
        Girar();
    }

    void FixedUpdate()
    {
        if (podePular)
        {
            Movimentar();
        }

        if (inputVetor.y > 0.5f && podePular && Mathf.Abs(rb.linearVelocity.y) < 0.05f)
        {
            Pular();
        }

        if (acaoDashear != null && acaoDashear.action.triggered && podeDashear)
        {
            Dash();
            StartCoroutine(DashCooldownCoroutine());
        }
    }

    public void Movimentar()
    {
        Vector3 direcaoCamera = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        direcaoCamera.y = 0;
        camRight.y = 0;
        direcaoCamera.Normalize();
        camRight.Normalize();

        Vector3 direcao = (camRight * inputVetor.x + direcaoCamera * inputVetor.z).normalized;
        Vector3 novaVelocidade = direcao * velocidade;

        rb.linearVelocity = new Vector3(novaVelocidade.x, rb.linearVelocity.y, novaVelocidade.z);

    }
    private void Girar()
    {
        if (acaoVirar != null)
        {
            //girar o personagem com base na direcão x da câmera
            Vector3 direcaoCamera = Camera.main.transform.forward;
            direcaoCamera.y = 0;

            if (direcaoCamera != Vector3.zero)
            {
                Quaternion rotacao = Quaternion.LookRotation(direcaoCamera);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacao, Time.deltaTime * 10f);
            }
        }
    }

    private void Pular()
    {
        Vector3 direcao =Vector3.up + rb.linearVelocity.normalized;
        rb.AddForce(direcao * forcaDoPulo, ForceMode.Impulse);
        podePular = false;
    }
    private void OnCollisionEnter(Collision col)
    {
        if (!podePular)
        {
            if (((1 << col.gameObject.layer) & terreno) != 0)
            {
                Debug.Log("O personagem encostou no terreno!");
                podePular = true;
                puloDuplo = true;
            }
        }
    }

    private void Dash()
    {
        podeDashear = false;
        Vector3 direcao = new Vector3(inputVetor.x, 0, inputVetor.z).normalized;
        Vector3 direcaoCamera = Camera.main.transform.forward;
        Vector3 direcaoDash = (direcaoCamera * inputVetor.z + Camera.main.transform.right * inputVetor.x).normalized;
        direcaoDash.y = 0;
        rb.AddForce(direcaoDash * dashForce, ForceMode.Impulse);

    }

    private IEnumerator DashCooldownCoroutine()
    {
        yield return new WaitForSeconds(dashCooldown);
        podeDashear = true;
    }
}