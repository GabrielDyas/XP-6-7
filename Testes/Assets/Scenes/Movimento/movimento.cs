using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Movimento3DUnificado : MonoBehaviour
{
    public float velocidade = 5f;
    public float forcaDoPulo = 5f;
    private bool podePular = true;
    public  Collider pe;
    public LayerMask terreno;
    public InputAction acaoMovimentar;
    private Rigidbody rb;
    private Vector3 inputVetor;

    void OnEnable()
    {
        acaoMovimentar.Enable();
    }

    void OnDisable()
    {
        acaoMovimentar.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        inputVetor = acaoMovimentar.ReadValue<Vector3>();


        if (inputVetor.y > 0.5f && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            Pular();
        }
    }
    private void OnCollisionEnter(Collision pe)
    {
        if (podePular == false)
        {
            Aguardar(1);

            if (pe.gameObject.layer == terreno)
            {
                Debug.Log("O personagem encostou no terreno!");
                podePular = true;
            }
        }
    }
    void FixedUpdate()
    {
        Movimentar();
    }

    public void Movimentar()
    {
        Vector3 direcao = (transform.right * inputVetor.x + transform.forward * inputVetor.z).normalized;
         
        Vector3 novaVelocidade = direcao * velocidade;
        rb.linearVelocity = new Vector3(novaVelocidade.x, rb.linearVelocity.y, novaVelocidade.z);
    }

    private void Pular()
    {
        rb.AddForce(Vector3.up * forcaDoPulo, ForceMode.Impulse);
        podePular = false;
    }

    IEnumerator Aguardar(float tempo)
    {
        yield return new WaitForSeconds(tempo);
    }
}