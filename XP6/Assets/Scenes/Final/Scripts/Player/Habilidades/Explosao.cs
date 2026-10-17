using System.Collections.Generic;
using UnityEngine;

public class Explosao : MonoBehaviour
{
    [SerializeField] private float forcaExplosao = 10f;
    [SerializeField] private float raio = 3f;
    [SerializeField] private float duracao = 1f; // tempo até o efeito sumir (partículas etc.)
    [SerializeField] private LayerMask camadas = ~0;

    private void Start()
    {
        // Aplica a força uma única vez, em todos os inimigos dentro do raio
        Collider[] alvos = Physics.OverlapSphere(transform.position, raio, camadas, QueryTriggerInteraction.Ignore);
        HashSet<Rigidbody> jaAtingidos = new HashSet<Rigidbody>();

        foreach (Collider alvo in alvos)
        {
            if (!alvo.CompareTag("Inimigo")) continue;

            Rigidbody rb = alvo.attachedRigidbody;
            if (rb == null || !jaAtingidos.Add(rb)) continue;

            Vector3 direcao = (rb.position - transform.position).normalized;
            rb.AddForce(direcao * forcaExplosao, ForceMode.Impulse);
        }

        Destroy(gameObject, duracao);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raio);
    }
}
