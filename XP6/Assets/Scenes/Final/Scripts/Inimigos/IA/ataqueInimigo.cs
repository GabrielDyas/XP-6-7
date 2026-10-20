using System.Collections;
using UnityEngine;

public class ataqueInimigo : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private float tempoPreparacao = 2f;
    [SerializeField] private float duracaoHitbox = 1f;
    [SerializeField] private float dano = 10f;

    [Header("Área de dano")]
    [SerializeField] private Vector3 tamanhoHitbox = new Vector3(3f, 2f, 4f);
    [SerializeField] private float distanciaFrente = 2f;
    [SerializeField] private float alturaHitbox = 1f;
    [SerializeField] private Material materialArea;

    private bool emAtaque = false;

    public bool EmAtaque => emAtaque;

    public void IniciarAtaque()
    {
        if (!emAtaque)
            StartCoroutine(ExecutarAtaque());
    }

    private IEnumerator ExecutarAtaque()
    {
        emAtaque = true;

        // Espera antes de atacar.
        yield return new WaitForSeconds(tempoPreparacao);

        // Cria a área vermelha na frente do inimigo.
        GameObject area = GameObject.CreatePrimitive(PrimitiveType.Cube);
        area.name = "AreaDanoInimigo";

        area.transform.position = transform.position
            + transform.forward * distanciaFrente
            + Vector3.up * alturaHitbox;

        area.transform.rotation = transform.rotation;
        area.transform.localScale = tamanhoHitbox;

        // A hitbox não deve bloquear fisicamente o jogador.
        area.GetComponent<BoxCollider>().isTrigger = true;

        Renderer renderizador = area.GetComponent<Renderer>();
        renderizador.material = materialArea;

        float tempo = 0f;
        bool danoAplicado = false;

        // Verifica a área durante os 5 segundos.
        while (tempo < duracaoHitbox)
        {
            if (!danoAplicado)
            {
                Collider[] colisoes = Physics.OverlapBox(
                    area.transform.position,
                    area.transform.localScale / 2f,
                    area.transform.rotation,
                    ~0,
                    QueryTriggerInteraction.Collide
                );

                foreach (Collider col in colisoes)
                {
                    mainPlayer jogador = col.GetComponentInParent<mainPlayer>();

                    if (jogador != null)
                    {
                        danoAplicado = true;
                        jogador.tomarDano(dano);
                        break;
                    }
                }
            }

            tempo += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Remove a área e o material criado para ela.
        Destroy(area);

        emAtaque = false;
    }
}