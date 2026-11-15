using UnityEngine;

public class Caca : MonoBehaviour
{
    public static int valorAlvo = 100;
    public static int valorAtual = 0;

    private static bool invocacaoPendente = false;

    [Header("Invocação")]
    [SerializeField] private GameObject inimigoEspecial;
    [SerializeField] private GameObject pontoDeInvocacao;

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
            return;

        GameObject especial = Instantiate(inimigoEspecial, pontoDeInvocacao.transform.position, Quaternion.identity);

        // Só o inimigo invocado pela Caça encerra o jogo ao morrer
        if (especial.TryGetComponent(out mainInimigo inimigo))
            inimigo.MarcarComoEspecial();
    }
}
