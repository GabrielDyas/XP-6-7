using UnityEngine;

public class mainInimigo : MonoBehaviour
{
    public float vida = 10f;

    private Material materialInimigo;
    private Color corOriginal;
    private bool corAlterada = false;

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
        vida -= dano;

        if (materialInimigo != null)
        {
            corAlterada = !corAlterada;

            materialInimigo.color = corAlterada
                ? Color.gray4
                : corOriginal;
        }

        if (vida <= 0f)
        {
            Destroy(gameObject);
        }
    }
}