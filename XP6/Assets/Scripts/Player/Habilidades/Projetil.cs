using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projetil : MonoBehaviour
{
    [SerializeField] private float tempoDeVida = 5f;
    [SerializeField] private bool usarGravidade = false; // false = voa reto até a mira

    private GameObject efeito;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.useGravity = usarGravidade;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // evita atravessar paredes
    }

    public void Lancar(GameObject efeito, Vector3 velocidade, Collider[] ignorar)
    {
        this.efeito = efeito;
        rb.linearVelocity = velocidade;

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
