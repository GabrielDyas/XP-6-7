
using UnityEngine;

public class controleSomArma : MonoBehaviour
{
    [SerializeField] private bool debug = false;

    private AudioSource audioSourceArma;

    // Recebe o AudioSource do modelo instanciado.
    public void ConfigurarAudioSource(AudioSource fonte)
    {
        audioSourceArma = fonte;

        if (audioSourceArma == null && debug)
        {
            Debug.LogWarning("AudioSource da arma não encontrado.", this);
        }
    }

    // Reproduz um som de disparo.
    public void TocarDisparo(infoArmas dadosArma)
    {
        TocarSomAleatorio(dadosArma.atirarSFX, "disparo");
    }

    // Reproduz um som de recarga.
    public void TocarRecarga(infoArmas dadosArma)
    {
        TocarSomAleatorio(dadosArma.recarregarSFX, "recarga");
    }

    // Seleciona e reproduz um clipe aleatório.
    private void TocarSomAleatorio(AudioClip[] clipes, string tipoSom)
    {
        if (clipes == null || clipes.Length == 0)
        {
            if (debug)
            {
                Debug.Log("Nenhum som de " + tipoSom + " configurado.", this);
            }
            return;
        }

        if (audioSourceArma == null)
        {
            if (debug)
            {
                Debug.LogWarning("AudioSource da arma não configurado.", this);
            }
            return;
        }

        // Conta quantos clipes são válidos.
        int quantidadeValida = 0;

        foreach (AudioClip clip in clipes)
        {
            if (clip != null)
            {
                quantidadeValida++;
            }
        }

        if (quantidadeValida == 0)
        {
            if (debug)
            {
                Debug.Log("A lista de sons está vazia.", this);
            }
            return;
        }

        // Sorteia um dos clipes válidos.
        int indiceAleatorio = Random.Range(0, quantidadeValida);

        foreach (AudioClip clip in clipes)
        {
            if (clip == null)
                continue;

            if (indiceAleatorio == 0)
            {
                audioSourceArma.PlayOneShot(clip);

                if (debug)
                {
                    Debug.Log("Reproduzindo som: " + clip.name, this);
                }

                return;
            }

            indiceAleatorio--;
        }
    }
}
