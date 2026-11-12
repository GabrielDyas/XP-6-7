
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class coletavelVida : MonoBehaviour
{
    private int quantidade;
    private bool coletado = false;

    // Recebe o valor configurado pelo Drop Manager.
    public void ConfigurarQuantidade(int valor)
    {
        quantidade = Mathf.Max(0, valor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (coletado)
            return;

        mainPlayer jogador = other.GetComponentInParent<mainPlayer>();

        if (jogador == null)
            return;

        // Não coleta se o jogador já estiver com a vida cheia.
        if (jogador.vida >= jogador.vidaMaxima)
            return;

        coletado = true;

        jogador.RecuperarVida(quantidade);

        Destroy(gameObject);
    }
}
