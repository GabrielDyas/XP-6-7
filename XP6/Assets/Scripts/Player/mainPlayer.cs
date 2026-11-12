
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainPlayer : MonoBehaviour
{
    public float vida = 100f;
    public float vidaMaxima = 100f;
    public float cristais = 0f;

    private bool morreu = false;

    public void tomarDano(float dano)
    {
        if (morreu)
            return;

        vida -= dano;

        if (vida <= 0f)
        {
            morreu = true;

            Scene cenaAtual = SceneManager.GetActiveScene();
            SceneManager.LoadScene(cenaAtual.buildIndex);
        }
    }

    // Adiciona cristais ao jogador.
    public void AdicionarCristais(int quantidade)
    {
        if (quantidade <= 0)
            return;

        cristais += quantidade;
    }

    // Retorna a quantidade atual de cristais.
    public float GetCristais()
    {
        return cristais;
    }

    // Recupera a vida do jogador.
    public void RecuperarVida(float quantidade)
    {
        if (morreu || quantidade <= 0f)
            return;

        vida = Mathf.Min(vida + quantidade, vidaMaxima);
    }
}
