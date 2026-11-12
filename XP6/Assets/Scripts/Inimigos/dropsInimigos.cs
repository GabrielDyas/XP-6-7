
using UnityEngine;

[System.Serializable]
public class ConfiguracaoDropInimigo
{
    public string nomeInimigo = "Inimigo base";
    public bool ativo = true;

    [Range(0f, 100f)]
    public float chanceEmPorcentagem = 50f;

    [Min(0)]
    public int quantidade = 1;
}

public class dropsInimigos : MonoBehaviour
{
    public static dropsInimigos Instancia { get; private set; }

    [Header("Prefabs dos drops")]
    [SerializeField] private GameObject cristalObj;
    [SerializeField] private GameObject vidaObj;
    [SerializeField] private GameObject itemCacaObj;

    [Header("Probabilidades")]
    [SerializeField] private ConfiguracaoDropInimigo[] listaChancesCristal;
    [SerializeField] private ConfiguracaoDropInimigo[] listaChancesVida;

    [Header("Configurações")]
    [SerializeField] private Vector3 deslocamentoDrop = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private bool debug = false;

    private void Awake()
    {
        // Permite acessar o Drop Manager por outros scripts.
        if (Instancia != null && Instancia != this)
        {
            Debug.LogError("Existe mais de um Drop Manager na cena.", this);
            Destroy(this);
            return;
        }

        Instancia = this;
    }

    private void OnDestroy()
    {
        if (Instancia == this)
        {
            Instancia = null;
        }
    }

    // Solicita os drops de um inimigo.
    public void DroparRecompensas(string nomeInimigo, Vector3 posicao)
    {
        int quantidadeCristal = CalcularQuantidadeDrop(
            listaChancesCristal,
            nomeInimigo
        );

        int quantidadeVida = CalcularQuantidadeDrop(
            listaChancesVida,
            nomeInimigo
        );

        if (quantidadeCristal > 0)
        {
            CriarDropCristal(posicao, quantidadeCristal);
        }

        if (quantidadeVida > 0)
        {
            CriarDropVida(posicao, quantidadeVida);
        }
    }

    // Calcula se o inimigo vai dropar e retorna a quantidade.
    private int CalcularQuantidadeDrop(
        ConfiguracaoDropInimigo[] lista,
        string nomeInimigo
    )
    {
        ConfiguracaoDropInimigo configuracao = BuscarConfiguracao(
            lista,
            nomeInimigo
        );

        if (configuracao == null || !configuracao.ativo)
            return 0;

        if (configuracao.quantidade <= 0)
            return 0;

        float chance = Mathf.Clamp(
            configuracao.chanceEmPorcentagem,
            0f,
            100f
        );

        // Evita que 0% ou 100% dependam do sorteio.
        if (chance <= 0f)
            return 0;

        if (chance >= 100f)
            return configuracao.quantidade;

        // Sorteia um valor entre 0 e 100.
        float sorteio = Random.Range(0f, 100f);

        if (sorteio < chance)
        {
            return configuracao.quantidade;
        }

        return 0;
    }

    // Procura a configuração correspondente ao inimigo.
    private ConfiguracaoDropInimigo BuscarConfiguracao(
        ConfiguracaoDropInimigo[] lista,
        string nomeInimigo
    )
    {
        if (lista == null)
            return null;

        foreach (ConfiguracaoDropInimigo configuracao in lista)
        {
            if (configuracao != null &&
                configuracao.nomeInimigo == nomeInimigo)
            {
                return configuracao;
            }
        }

        if (debug)
        {
            Debug.LogWarning("Configuração de drop não encontrada: " + nomeInimigo, this);
        }

        return null;
    }

    // Instancia o prefab de cristal e configura seu valor.
    private void CriarDropCristal(Vector3 posicao, int quantidade)
    {
        if (cristalObj == null)
            return;

        GameObject objeto = Instantiate(
            cristalObj,
            posicao + deslocamentoDrop,
            Quaternion.identity
        );

        coletavelCristal coletavel = objeto.GetComponent<coletavelCristal>();

        if (coletavel == null)
        {
            Debug.LogError("O Prefab de cristal não possui coletavelCristal.", objeto);
            Destroy(objeto);
            return;
        }

        coletavel.ConfigurarQuantidade(quantidade);

        if (debug)
        {
            Debug.Log("Drop de cristais: " + quantidade);
        }
    }

    // Instancia o prefab de vida e configura seu valor.
    private void CriarDropVida(Vector3 posicao, int quantidade)
    {
        if (vidaObj == null)
            return;

        GameObject objeto = Instantiate(
            vidaObj,
            posicao + deslocamentoDrop,
            Quaternion.identity
        );

        coletavelVida coletavel = objeto.GetComponent<coletavelVida>();

        if (coletavel == null)
        {
            Debug.LogError("O Prefab de vida não possui coletavelVida.", objeto);
            Destroy(objeto);
            return;
        }

        coletavel.ConfigurarQuantidade(quantidade);

        if (debug)
        {
            Debug.Log("Drop de vida: " + quantidade);
        }
    }

    // Instancia o item que indica a localização da Caça.
    public void DroparItemCaca(Vector3 posicao)
    {
        if (itemCacaObj == null)
        {
            if (debug)
            {
                Debug.LogWarning("Prefab do item de Caça não definido.", this);
            }

            return;
        }

        Instantiate(
            itemCacaObj,
            posicao + deslocamentoDrop,
            Quaternion.identity
        );

        if (debug)
        {
            Debug.Log("Item de Caça instanciado.");
        }
    }

    // Permite que outros scripts alterem as configurações.

    public bool DefinirChanceCristal(string nomeInimigo, float chance)
    {
        return AlterarChance(listaChancesCristal, nomeInimigo, chance);
    }

    public bool DefinirQuantidadeCristal(string nomeInimigo, int quantidade)
    {
        return AlterarQuantidade(listaChancesCristal, nomeInimigo, quantidade);
    }

    public bool DefinirChanceVida(string nomeInimigo, float chance)
    {
        return AlterarChance(listaChancesVida, nomeInimigo, chance);
    }

    public bool DefinirQuantidadeVida(string nomeInimigo, int quantidade)
    {
        return AlterarQuantidade(listaChancesVida, nomeInimigo, quantidade);
    }

    private bool AlterarChance(
        ConfiguracaoDropInimigo[] lista,
        string nomeInimigo,
        float chance
    )
    {
        ConfiguracaoDropInimigo configuracao = BuscarConfiguracao(lista, nomeInimigo);

        if (configuracao == null)
            return false;

        configuracao.chanceEmPorcentagem = Mathf.Clamp(chance, 0f, 100f);
        return true;
    }

    private bool AlterarQuantidade(
        ConfiguracaoDropInimigo[] lista,
        string nomeInimigo,
        int quantidade
    )
    {
        ConfiguracaoDropInimigo configuracao = BuscarConfiguracao(lista, nomeInimigo);

        if (configuracao == null)
            return false;

        configuracao.quantidade = Mathf.Max(0, quantidade);
        return true;
    }
}
