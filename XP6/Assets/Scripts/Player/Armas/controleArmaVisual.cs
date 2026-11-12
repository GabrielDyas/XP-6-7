using UnityEngine;

public class controleArmaVisual : MonoBehaviour
{
    [Header("Referência")]
    [SerializeField] private Transform pontoArma;

    private GameObject modeloInstanciado;

    public void Equipar(infoArmas dadosArma)
    {
        if (pontoArma == null)
        {
            Debug.LogError("Ponto da arma não definido.", this);
            return;
        }

        if (dadosArma == null || dadosArma.modeloArma == null)
        {
            Debug.LogError("Modelo da arma não definido.", this);
            return;
        }

        if (modeloInstanciado != null)
        {
            Debug.Log("Modelo nao esta nulo");
            Destroy(modeloInstanciado);
        }

        modeloInstanciado = Instantiate(
            dadosArma.modeloArma,
            pontoArma
        );
        modeloInstanciado.transform.localPosition = dadosArma.posicaoNaTela;
    }
    // Referência para o controleSomArma
    public AudioSource ObterAudioSource()
    {
        if (modeloInstanciado == null)
            return null;

        return modeloInstanciado.GetComponentInChildren<AudioSource>(true);
    }

}