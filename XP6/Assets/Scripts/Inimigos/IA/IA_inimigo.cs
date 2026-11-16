using System.Collections;
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

    [Header("Audição")]
    [Tooltip("Área de som do jogador. Se vazia, é procurada no objeto com a tag Player.")]
    [SerializeField] private AreaDeSom areaDeSomJogador;

    [Tooltip("Fração da área de som em que o inimigo percebe o jogador durante a busca (0 a 1). " +
             "É também a sensibilidade logo após ouvir algo.")]
    [SerializeField, Range(0.1f, 1f)] private float sensibilidadeEmBusca = 0.5f;

    [Tooltip("Segundos para a sensibilidade voltar ao máximo enquanto o inimigo está Inativo.")]
    [SerializeField] private float tempoRecuperacao = 10f;

    [Header("Ataque")]
    [SerializeField] private float distanciaAtaque = 2f;

    [Header("Debug")]
    [SerializeField] private Estado estado = Estado.Inativo;
    [SerializeField] public Transform objetivo;

    private NavMeshAgent inimigo;
    private ataqueInimigo ataque;
    private float sensibilidade = 1f;
    private Coroutine empurraoAtual;
    private bool emEmpurrao;

    private void Awake()
    {
        inimigo = GetComponent<NavMeshAgent>();
        ataque = GetComponent<ataqueInimigo>();
        sensibilidade = 1f;

        if (ataque == null)
            Debug.LogError("[IA_inimigo] Adicione o componente ataqueInimigo.", this);

        // Descarta referência a um asset (prefab fora da cena): ela apontaria para a origem do mundo.
        if (areaDeSomJogador != null && !areaDeSomJogador.gameObject.scene.IsValid())
            areaDeSomJogador = null;

        if (areaDeSomJogador == null)
        {
            GameObject jogador = GameObject.FindGameObjectWithTag("Player");

            if (jogador != null)
                areaDeSomJogador = jogador.GetComponentInChildren<AreaDeSom>();
        }

        if (areaDeSomJogador == null)
            Debug.LogError("[IA_inimigo] Não encontrei um AreaDeSom no jogador (tag Player).", this);
    }

    private void Update()
    {
        if (emEmpurrao)
            return;

        switch (estado)
        {
            case Estado.Inativo:

                float taxa = (1f - sensibilidadeEmBusca) /
                             Mathf.Max(0.01f, tempoRecuperacao);

                sensibilidade = Mathf.MoveTowards(
                    sensibilidade,
                    1f,
                    taxa * Time.deltaTime
                );

                if (OuveJogador(sensibilidade))
                {
                    objetivo = areaDeSomJogador.transform;
                    sensibilidade = sensibilidadeEmBusca;

                    inimigo.isStopped = false;
                    inimigo.destination = areaDeSomJogador.Centro;
                    estado = Estado.Procurando;
                }
                break;

            case Estado.Procurando:

                if (OuveJogador(sensibilidadeEmBusca))
                {
                    objetivo = areaDeSomJogador.transform;
                    estado = Estado.Atacando;
                }
                else if (!inimigo.pathPending &&
                         inimigo.hasPath &&
                         inimigo.remainingDistance <= inimigo.stoppingDistance)
                {
                    inimigo.ResetPath();
                    objetivo = null;
                    estado = Estado.Inativo;
                }
                break;

            case Estado.Atacando:

                if (objetivo == null)
                {
                    inimigo.isStopped = false;
                    inimigo.ResetPath();
                    estado = Estado.Inativo;
                    break;
                }

                float distancia = Vector3.Distance(
                    transform.position,
                    objetivo.position
                );

                if (ataque != null && ataque.EmAtaque)
                {
                    // Continua parado durante a preparação e o ataque.
                    inimigo.isStopped = true;
                }
                else if (ataque != null && distancia <= distanciaAtaque)
                {
                    // Para e inicia o ataque quando chega perto.
                    inimigo.isStopped = true;
                    ataque.IniciarAtaque();
                }
                else
                {
                    // Continua perseguindo enquanto está longe.
                    inimigo.isStopped = false;
                    inimigo.destination = objetivo.position;
                }
                break;
        }
    }

    private bool OuveJogador(float sens)
    {
        if (areaDeSomJogador == null)
            return false;

        float alcance = areaDeSomJogador.RaioAtual * sens;

        if (alcance <= 0f)
            return false;

        float distanciaQuadrada =
            (areaDeSomJogador.Centro - transform.position).sqrMagnitude;

        return distanciaQuadrada <= alcance * alcance;
    }
    #region Comando externo
    public void Alertar()
    {
        if (areaDeSomJogador == null)
            return;

        objetivo = areaDeSomJogador.transform;
        inimigo.isStopped = false;
        inimigo.destination = areaDeSomJogador.Centro;
        estado = Estado.Atacando;
    }

    public void Empurrar(Vector3 velocidade, float duracao)
    {
        if (inimigo == null || !inimigo.isActiveAndEnabled || !inimigo.isOnNavMesh || duracao <= 0f)
            return;

        if (empurraoAtual != null)
            StopCoroutine(empurraoAtual);

        empurraoAtual = StartCoroutine(RotinaEmpurrao(velocidade, duracao));
    }

    private IEnumerator RotinaEmpurrao(Vector3 velocidade, float duracao)
    {
        emEmpurrao = true;
        inimigo.isStopped = true;
        inimigo.velocity = Vector3.zero;

        float tempo = 0f;
        while (tempo < duracao)
        {
            // A força diminui linearmente ao longo do tempo.
            float fator = 1f - tempo / duracao;
            inimigo.Move(velocidade * fator * Time.deltaTime);
            tempo += Time.deltaTime;
            yield return null;
        }

        inimigo.isStopped = false;
        emEmpurrao = false;
        empurraoAtual = null;
    }

    private void OnDisable()
    {
        // Evita deixar o inimigo travado se for desativado no meio de um empurrão.
        emEmpurrao = false;
        empurraoAtual = null;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || areaDeSomJogador == null)
            return;

        float sens = estado == Estado.Inativo
            ? sensibilidade
            : sensibilidadeEmBusca;

        if (estado == Estado.Inativo)
            Gizmos.color = Color.green;
        else if (estado == Estado.Procurando)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            areaDeSomJogador.Centro,
            areaDeSomJogador.RaioAtual * sens
        );

        Gizmos.DrawLine(transform.position, areaDeSomJogador.Centro);
    }
}