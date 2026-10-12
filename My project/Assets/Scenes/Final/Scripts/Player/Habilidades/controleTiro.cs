using UnityEngine;
using UnityEngine.InputSystem;

public class controleTiro : MonoBehaviour
{
    public Camera cameraPrincipal;
    public float distanciaMaxima = 100f;
    public float vidaDoRaioSegundos = 1f;

    // TEMPORÁRIO: o input de tiro é lido diretamente neste script.
    // Corrigir para que outro componente seja responsável pelos inputs.
    // Este script deverá apenas executar a lógica do tiro e fornecer o resultado,
    // resultado sendo se houve acerto e qual objeto foi atingido.
    public InputActionReference acaoAtirar;

    void OnEnable()
    {
        if (acaoAtirar != null)
            acaoAtirar.action.Enable();
    }

    void OnDisable()
    {
        if (acaoAtirar != null)
            acaoAtirar.action.Disable();
    }

    private void Start()
    {
        cameraPrincipal = Camera.main;
    }

    void Update()
    {
        // TEMPORÁRIO: esta checagem de input deve ser removida futuramente.
        // Outro componente chama a lógica de tiro deste script.
        if (acaoAtirar.action.triggered)
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

                if (inimigo!=null)
                {
                    inimigo.tomarDano(5f);
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
}