
using UnityEngine;

public class IKPe : MonoBehaviour
{
    [Header("Target do IK")]
    [SerializeField] private Transform targetPeIK;

    [Header("Raycast")]
    [SerializeField] private float distanciaRaio = 3f;
    [SerializeField] private LayerMask Camada;

    [Header("Movimento do pé")]
    [SerializeField] private float alcancePasso = 0.5f;
    [SerializeField] private float duracaoPasso = 0.2f;
    [SerializeField] private float distanciaFrente = 0.2f; 
    [SerializeField] private float alturaPasso = 0.2f; 

    private Vector3 posicaoChao;
    private Vector3 normalChao = Vector3.up;

    private Vector3 inicioPasso;
    private Vector3 destinoPasso;
    private float tempoPasso;
    private bool movendoPe = false;

    private void Update()
    {
        // Procura o terreno abaixo da perna
        bool encontrouTerreno = Physics.Raycast(
            transform.position,
            Vector3.down,
            out RaycastHit hit,
            distanciaRaio,
            Camada
        );

        // Verifica se encontrou o terreno
        if (encontrouTerreno && hit.collider.CompareTag("Terreno"))
        {
            posicaoChao = hit.point;
            normalChao = hit.normal;
        }
        else
        {
            encontrouTerreno = false;
        }

        // Executa o movimento do pé
        if (movendoPe)
        {
            tempoPasso += Time.deltaTime;

            float progresso = Mathf.Clamp01(tempoPasso / duracaoPasso);

            targetPeIK.position = Vector3.Lerp(
                inicioPasso,
                destinoPasso,
                progresso
            );

            // eleva o pé no meio do movimento, criando um arco
            float elevacao = Mathf.Sin(progresso * Mathf.PI) * alturaPasso;

            // adiciona a elevação à posição calculada pelo Lerp
            targetPeIK.position += normalChao * elevacao;

            if (progresso >= 1f)
            {
                movendoPe = false;
            }

            return;
        }

        // Verifica se precisa iniciar um novo passo
        if (encontrouTerreno)
        {
            float distancia = Vector3.Distance(
                targetPeIK.position,
                posicaoChao
            );

            if (distancia > alcancePasso)
            {
                inicioPasso = targetPeIK.position;

                // calcula a direção do pé atual até o ponto encontrado no chão
                Vector3 direcaoPasso = Vector3.ProjectOnPlane(
                    posicaoChao - targetPeIK.position,
                    normalChao
                ).normalized;

                // coloca o destino um pouco além do ponto encontrado
                destinoPasso = posicaoChao + direcaoPasso * distanciaFrente;

                tempoPasso = 0f;
                movendoPe = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origem = transform.position;

        // Desenha o Raycast
        Gizmos.color = Color.green;

        Gizmos.DrawLine(
            origem,
            origem + Vector3.down * distanciaRaio
        );

        // Desenha o ponto encontrado e o alcance
        if (Physics.Raycast(origem, Vector3.down, out RaycastHit hit,
            distanciaRaio, Camada))
        {
            if (hit.collider.CompareTag("Terreno"))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(hit.point, 0.04f);
                Gizmos.DrawWireSphere(hit.point, alcancePasso);

                // mostra o destino estimado do próximo passo
                Vector3 direcao = Vector3.ProjectOnPlane(
                    hit.point - (targetPeIK != null ? targetPeIK.position : transform.position),
                    hit.normal
                ).normalized;

                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(hit.point + direcao * distanciaFrente, 0.04f);
            }
        }
    }
}
