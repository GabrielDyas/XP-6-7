using UnityEngine;
using UnityEngine.AI;

public class IA_inimigo : MonoBehaviour
{
    private enum Estado
    {
        Inativo,
        Procurando,
        Atacando
    }

    [Header("Configurações")]
    [SerializeField] private float raioBusca = 20f;
    [SerializeField] private int bufferSearchtoIdle = 10; 

    private float raioAtual;
    [Header("Debug")]
    [SerializeField]private Estado estado = Estado.Inativo;
    private NavMeshAgent inimigo;
    [SerializeField] private Transform objetivo;

    private void Start()
    {
        inimigo = GetComponent<NavMeshAgent>();
        raioAtual = raioBusca;
    }

    private void Update()
    {
        switch (estado)
        {
            case Estado.Inativo:

                // Aumenta o raio até o tamanho normal em 10 segundos.
                raioAtual = Mathf.MoveTowards(
                    raioAtual,
                    raioBusca,
                    (raioBusca / 2f / bufferSearchtoIdle) * Time.deltaTime
                );

                // Procura o jogador dentro do raio atual.
                if (EncontrarPlayer(raioAtual, out Transform player))
                {
                    objetivo = player;

                    // Diminui o raio pela metade.
                    raioAtual = raioBusca / 2f;

                    // Vai até a posição onde o jogador foi encontrado.
                    inimigo.destination = objetivo.position;
                    estado = Estado.Procurando;
                }
                break;

            case Estado.Procurando:

                // Se encontrar o jogador no raio menor, começa a perseguição.
                if (EncontrarPlayer(raioBusca / 2f, out player))
                {
                    objetivo = player;
                    estado = Estado.Atacando;
                }
                // Verifica se chegou ao destino.
                else if (!inimigo.pathPending && inimigo.hasPath && inimigo.remainingDistance <= inimigo.stoppingDistance)
                {
                    inimigo.ResetPath();
                    objetivo = null;
                    estado = Estado.Inativo;
                }
                break;

            case Estado.Atacando:

                // Continua seguindo a posição atual do jogador.
                if (objetivo == null)
                {
                    inimigo.ResetPath();
                    estado = Estado.Inativo;
                    break;
                }

                inimigo.destination = objetivo.position;
                break;
        }
    }

    private bool EncontrarPlayer(float raio, out Transform player)
    {
        Collider[] colisoes = Physics.OverlapSphere(transform.position, raio);

        foreach (Collider col in colisoes)
        {
            if (col.CompareTag("Player"))
            {
                player = col.transform;
                return true;
            }
        }

        player = null;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        float raioVisual = raioBusca;

        if (Application.isPlaying)
        {
            if (estado == Estado.Inativo)
                raioVisual = raioAtual;
            else
                raioVisual = raioBusca / 2f;
        }

        if (estado == Estado.Inativo)
            Gizmos.color = Color.green;
        else if (estado == Estado.Procurando)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            Mathf.Max(0f, raioVisual)
        );
    }
}