using System.Collections.Generic;
using UnityEngine;

public class Explosao : MonoBehaviour
{
    [Tooltip("Velocidade inicial do empurrão, em m/s (diminui até zero durante a duração).")]
    [SerializeField] private float forcaExplosao = 10f;
    [SerializeField] private float raio = 3f;
    [Tooltip("Tempo, em segundos, que o inimigo é empurrado.")]
    [SerializeField] private float duracaoEmpurrao = 0.3f;
    [SerializeField] private float duracao = 1f; // tempo até o efeito sumir (partículas etc.)
    [SerializeField] private LayerMask camadas = ~0;

    private void Start()
    {
        // Aplica o empurrão uma única vez, em todos os inimigos dentro do raio
        Collider[] alvos = Physics.OverlapSphere(transform.position, raio, camadas, QueryTriggerInteraction.Ignore);
        HashSet<GameObject> jaAtingidos = new HashSet<GameObject>();

        foreach (Collider alvo in alvos)
        {
            if (!alvo.CompareTag("Inimigo")) continue;

            // Inimigos usam NavMeshAgent + Rigidbody kinematic, então AddForce não funciona neles:
            // o empurrão é feito pela própria IA do inimigo.
            IA_inimigo ia = alvo.GetComponentInParent<IA_inimigo>();
            GameObject raiz = ia != null ? ia.gameObject : alvo.gameObject;
            if (!jaAtingidos.Add(raiz)) continue;

            // Direção horizontal, afastando o inimigo do centro da explosão
            Vector3 direcao = raiz.transform.position - transform.position;
            direcao.y = 0f;
            if (direcao.sqrMagnitude < 0.0001f)
                direcao = Random.insideUnitSphere;
            direcao.y = 0f;
            direcao.Normalize();

            if (ia != null)
            {
                ia.Empurrar(direcao * forcaExplosao, duracaoEmpurrao);
            }
            else
            {
                // Alvo com física normal (sem IA_inimigo)
                Rigidbody rb = alvo.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                    rb.AddForce(direcao * forcaExplosao, ForceMode.Impulse);
            }
        }

        Destroy(gameObject, duracao);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raio);
    }
}
