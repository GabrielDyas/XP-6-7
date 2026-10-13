
using UnityEngine;

public class controleTiro : MonoBehaviour
{
    public Camera cameraPrincipal;
    public float distanciaMaxima = 100f;
    public float vidaDoRaioSegundos = 1f;

    private void Start()
    {
        cameraPrincipal = Camera.main;
    }

    // Executa o disparo quando solicitado pelo script armas.
    public void Atirar(float dano)
    {
        // Cria um vetor que representa o meio da tela
        Vector3 coordenadaDaTela = new Vector3(0.5f, 0.5f, 0f);

        // Cria o raio saindo da câmera
        Ray raioDoTiro = cameraPrincipal.ViewportPointToRay(coordenadaDaTela);

        // Faz o Raycast
        if (Physics.Raycast(raioDoTiro, out RaycastHit hit, distanciaMaxima))
        {
            Debug.Log("Acertou: " + hit.collider.gameObject.name);

            mainInimigo inimigo = hit.collider.GetComponentInParent<mainInimigo>();

            if (inimigo != null)
            {
                inimigo.tomarDano(dano);
            }
        }

        // Desenha o raio vermelho durante o tempo definido
        Debug.DrawRay(
            raioDoTiro.origin,
            raioDoTiro.direction * distanciaMaxima,
            Color.red,
            vidaDoRaioSegundos
        );
    }
}
