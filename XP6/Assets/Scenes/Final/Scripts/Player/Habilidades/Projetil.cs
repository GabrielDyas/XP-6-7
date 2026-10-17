using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projetil : MonoBehaviour
{
    [SerializeField] private float tempoDeVida = 5f;

    private GameObject efeito;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void Lancar(GameObject efeito, Vector3 velocidade, Collider[] ignorar, Quaternion rotacao)
    {
        this.efeito = efeito;
        rb.linearVelocity = velocidade * rotacao.eulerAngles.magnitude;

        foreach (Collider c in ignorar)
            if (c != null) Physics.IgnoreCollision(col, c);

        Destroy(gameObject, tempoDeVida);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Terreno")) return;

        if (efeito != null)
            Instantiate(efeito, collision.GetContact(0).point, transform.rotation);

        Destroy(gameObject);
    }
}
