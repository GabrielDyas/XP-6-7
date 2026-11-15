using UnityEngine;

public class mainInimigo : MonoBehaviour
{
    public float vida = 10f;

    [Header("Configuração de Drop")]
    [SerializeField] private string nomeTipoInimigo = "Inimigo base";

    [Header("Fim de jogo")]
    [Tooltip("Se ligado, o jogo termina quando este inimigo morre. A Caça liga isso sozinha no inimigo especial.")]
    [SerializeField] private bool finalizaJogoAoMorrer = false;

    private Material materialInimigo;
    private Color corOriginal;
    private bool corAlterada = false;
    private bool morreu = false;

    private void Awake()
    {
        Renderer renderizador = GetComponentInChildren<Renderer>();

        if (renderizador != null)
        {
            materialInimigo = renderizador.material;
            corOriginal = materialInimigo.color;
        }
    }

    // Chamado pela Caça ao invocar o inimigo especial
    public void MarcarComoEspecial() => finalizaJogoAoMorrer = true;

    public void tomarDano(float dano)
    {
        if (morreu)
            return;

        vida -= dano;

        if (materialInimigo != null)
        {
            corAlterada = !corAlterada;
            materialInimigo.color = corAlterada ? Color.gray : corOriginal;
        }

        if (vida > 0f)
            return;

        morreu = true;

        // Soma pontos da Caça; ao atingir a meta, solta o item na posição do inimigo
        bool iniciouCaca = Caca.AcumularPontos(Random.Range(10, 21));

        dropsInimigos drops = dropsInimigos.Instancia;
        if (drops != null)
        {
            if (iniciouCaca)
                drops.DroparItemCaca(transform.position);

            drops.DroparRecompensas(nomeTipoInimigo, transform.position);
        }

        if (finalizaJogoAoMorrer && ControladorHud.instancia != null)
            ControladorHud.instancia.EndGame();

        Destroy(gameObject);
    }
}
