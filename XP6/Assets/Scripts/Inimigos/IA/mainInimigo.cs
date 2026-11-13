
using UnityEngine;

public class mainInimigo : MonoBehaviour
{
    public float vida = 10f;

    [Header("Configuração de Drop")]
    [SerializeField] private string nomeTipoInimigo = "Inimigo base";
    [SerializeField] private ControladorHud CH;

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

    public void tomarDano(float dano)
    {
        if (morreu)
            return;

        vida -= dano;

        if (materialInimigo != null)
        {
            corAlterada = !corAlterada;

            materialInimigo.color = corAlterada
                ? Color.gray
                : corOriginal;
        }

        if (vida <= 0f)
        {
            morreu = true;

            // Acumula os pontos e verifica se atingiu a meta da Caça.
            bool iniciouCaca = Caca.AcumularPontos(Random.Range(10, 21));

            if (iniciouCaca)
            {
                // Dropar o item na posição do inimigo que atingiu a meta.
                if (dropsInimigos.Instancia != null)
                {
                    dropsInimigos.Instancia.DroparItemCaca(transform.position);
                }
            }

            // Solicita os drops ao GameManager.
            if (dropsInimigos.Instancia != null)
            {
                dropsInimigos.Instancia.DroparRecompensas(
                    nomeTipoInimigo,
                    transform.position
                );
            }

            if(CH != null)
            {
                CH.FimGame();
            }
            Destroy(gameObject);
            
        }
    }
}
