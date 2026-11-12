using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControladorHud : MonoBehaviour
{
    [System.Serializable]
    private class SlotHabilidade
    {
        public string nome = "Habilidade";
        public TMP_Text texto;

        [Header("Ícone")]
        [Tooltip("Arraste aqui o sprite (PNG) da habilidade")]
        public Sprite icone;
        public Vector2 tamanhoIcone = new Vector2(64f, 64f);
        [Tooltip("Posição do ícone em relação ao texto")]
        public Vector2 deslocamentoIcone = new Vector2(-80f, 0f);
        [Tooltip("Escurece o ícone em forma de relógio durante a recarga")]
        public bool mostrarRelogioDeRecarga = true;

        // Criados pelo script em tempo de execução
        [HideInInspector] public Image imagemIcone;
        [HideInInspector] public Image imagemRelogio;

        [HideInInspector] public int ultimoDecimos = -1;
        [HideInInspector] public float recargaTotal;
    }

    [Header("Referências (Player)")]
    [SerializeField] private armas arma;
    [SerializeField] private DisparoHabilidade habilidades;

    [Header("Munição")]
    [SerializeField] private TMP_Text textoMunicao;

    [Header("Habilidades")]
    [SerializeField] private SlotHabilidade explosao = new SlotHabilidade { nome = "Explosão" };
    [SerializeField] private SlotHabilidade vortice = new SlotHabilidade { nome = "Vórtice" };

    [Header("Aparência")]
    [SerializeField] private Color corPronta = Color.white;
    [SerializeField] private Color corEmRecarga = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private Color corDoRelogio = new Color(0f, 0f, 0f, 0.6f);

    private int ultimaMunicao = -1;
    private int ultimaCapacidade = -1;

    private void Awake()
    {
        if (arma == null)
            arma = FindAnyObjectByType<armas>();

        if (habilidades == null)
            habilidades = FindAnyObjectByType<DisparoHabilidade>();

        CriarImagens(explosao);
        CriarImagens(vortice);
    }

    private void Update()
    {
        AtualizarMunicao();

        if (habilidades == null)
            return;

        AtualizarSlot(explosao, habilidades.RecargaRestanteExplosao);
        AtualizarSlot(vortice, habilidades.RecargaRestanteVortice);
    }

    // Cria a Image do ícone (e a do relógio) a partir do Sprite
    private void CriarImagens(SlotHabilidade slot)
    {
        if (slot.icone == null)
            return;

        RectTransform pai = slot.texto != null
            ? (RectTransform)slot.texto.rectTransform.parent
            : (RectTransform)transform;

        // Ícone
        GameObject objIcone = new GameObject("Icone_" + slot.nome, typeof(RectTransform), typeof(Image));
        RectTransform rt = objIcone.GetComponent<RectTransform>();
        rt.SetParent(pai, false);

        if (slot.texto != null)
        {
            RectTransform rtTexto = slot.texto.rectTransform;
            Vector2 centroAncora = (rtTexto.anchorMin + rtTexto.anchorMax) * 0.5f;
            rt.anchorMin = centroAncora;
            rt.anchorMax = centroAncora;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = rtTexto.anchoredPosition + slot.deslocamentoIcone;
        }
        else
        {
            rt.anchoredPosition = slot.deslocamentoIcone;
        }
        rt.sizeDelta = slot.tamanhoIcone;

        slot.imagemIcone = objIcone.GetComponent<Image>();
        slot.imagemIcone.sprite = slot.icone;
        slot.imagemIcone.preserveAspect = true;
        slot.imagemIcone.raycastTarget = false;
        slot.imagemIcone.color = corPronta;

        // Relógio de recarga (mesmo sprite, escuro, preenchimento radial)
        if (slot.mostrarRelogioDeRecarga)
        {
            GameObject objRelogio = new GameObject("Relogio", typeof(RectTransform), typeof(Image));
            RectTransform rtR = objRelogio.GetComponent<RectTransform>();
            rtR.SetParent(rt, false);
            rtR.anchorMin = Vector2.zero;
            rtR.anchorMax = Vector2.one;
            rtR.offsetMin = Vector2.zero;
            rtR.offsetMax = Vector2.zero;

            slot.imagemRelogio = objRelogio.GetComponent<Image>();
            slot.imagemRelogio.sprite = slot.icone;
            slot.imagemRelogio.preserveAspect = true;
            slot.imagemRelogio.raycastTarget = false;
            slot.imagemRelogio.color = corDoRelogio;
            slot.imagemRelogio.type = Image.Type.Filled;
            slot.imagemRelogio.fillMethod = Image.FillMethod.Radial360;
            slot.imagemRelogio.fillOrigin = (int)Image.Origin360.Top;
            slot.imagemRelogio.fillClockwise = false;
            slot.imagemRelogio.fillAmount = 0f;
        }
    }

    private void AtualizarMunicao()
    {
        if (arma == null || textoMunicao == null)
            return;

        int atual = (int)arma.GetMunicao();
        int maxima = (int)arma.GetCapacidadeCarregador();

        if (atual == ultimaMunicao && maxima == ultimaCapacidade)
            return;

        ultimaMunicao = atual;
        ultimaCapacidade = maxima;

        textoMunicao.text = atual + " / " + maxima;
    }

    private void AtualizarSlot(SlotHabilidade slot, float restante)
    {
        bool pronta = restante <= 0f;

        // Descobre a duração total da recarga observando o maior valor restante
        if (pronta)
            slot.recargaTotal = 0f;
        else if (restante > slot.recargaTotal)
            slot.recargaTotal = restante;

        // Relógio atualizado todo frame para ficar suave
        if (slot.imagemRelogio != null)
        {
            slot.imagemRelogio.fillAmount =
                (pronta || slot.recargaTotal <= 0f) ? 0f : restante / slot.recargaTotal;
        }

        // Texto e cor só mudam a cada décimo de segundo
        int decimos = pronta ? 0 : Mathf.CeilToInt(restante * 10f);

        if (decimos == slot.ultimoDecimos)
            return;

        slot.ultimoDecimos = decimos;

        Color cor = pronta ? corPronta : corEmRecarga;

        if (slot.texto != null)
        {
            slot.texto.text = pronta
                ? slot.nome + ": PRONTA"
                : slot.nome + ": " + (decimos / 10f).ToString("0.0") + "s";
            slot.texto.color = cor;
        }

        if (slot.imagemIcone != null)
            slot.imagemIcone.color = cor;
    }
}