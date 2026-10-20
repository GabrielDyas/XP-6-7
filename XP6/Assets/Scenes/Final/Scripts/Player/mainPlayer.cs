using UnityEngine;
using UnityEngine.SceneManagement;

public class mainPlayer : MonoBehaviour
{
    public float vida = 100f;

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
}