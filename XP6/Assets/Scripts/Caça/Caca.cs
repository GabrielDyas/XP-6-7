
using UnityEngine;

public class Caca : MonoBehaviour
{
    public static int valorAlvo = 100;
    public static int valorAtual = 0;

    private static bool invocacaoPendente = false;

    [Header("Invocação")]
    [SerializeField] private GameObject inimigoEspecial;
    [SerializeField] private GameObject pontoDeInvocacao;

    [Header("Debug")]
    [SerializeField] private bool debug = false;

    public static bool AcumularPontos(int pontos)
    {
        valorAtual += pontos;

        if (valorAtual >= valorAlvo)
        {
            valorAtual = 0;
            invocacaoPendente = true;

            return true;
        }

        return false;
    }

    private void Update()
    {
        if (!invocacaoPendente)
            return;

        invocacaoPendente = false;

        if (pontoDeInvocacao == null || inimigoEspecial == null)
        {
            Debug.LogError("Inimigo especial ou ponto de invocação não definido.", this);
            return;
        }

        // Instancia o inimigo especial no ponto definido.
        Instantiate(inimigoEspecial,pontoDeInvocacao.transform.position,Quaternion.identity);

        if (debug)
        {
            Debug.Log("Caça iniciada! Inimigo especial instanciado.");
        }
    }
}
