using UnityEngine;

public class controleTiro : MonoBehaviour
{
    public Camera cameraPrincipal;
    public float distanciaMaxima = 100f;

    private void Start()
    {
        cameraPrincipal = Camera.main;
    }
    void Update()
    {
        // Cria um vetor que representa o meio da tela
        Vector3 coordenadaDaTela = new Vector3(0.5f, 0.5f, 0f);

        // Cria o raio saindo da câmera
        Ray raioDoTiro = cameraPrincipal.ViewportPointToRay(coordenadaDaTela);

        // Faz o Raycast
        if (Physics.Raycast(raioDoTiro, out RaycastHit hit, distanciaMaxima))
        {
            Debug.Log("Acertou: " + hit.collider.gameObject.name);
        }

        // não sei porque não está desenhando para mim
        Debug.DrawRay(raioDoTiro.origin, raioDoTiro.direction * distanciaMaxima);
    }
}
