using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorHud : MonoBehaviour
{
    public static ControladorHud instancia;

    // Sobrevive ao recarregamento da cena (usado para pular a tela de início ao reiniciar)
    private static bool reiniciando;

    [System.Serializable]
    private class SlotHabilidade
    {
        public string nome = "Habilidade";

        [Tooltip("Arraste o sprite (PNG) da habilidade")]
        public Sprite icone;

        [Tooltip("Image da própria habilidade (objeto dentro de Game > Habilidades). Se vazio, o script procura pelo nome da habilidade.")]
        public Image imagem;

        [HideInInspector] public Image preenchimento;
        [HideInInspector] public float recargaTotal;
        [HideInInspector] public bool emRecarga;
    }

    [Header("Referências (Player)")]
    [SerializeField] private armas arma;
    [SerializeField] private DisparoHabilidade habilidades;

    [Header("Telas")]
    [SerializeField] private GameObject Inicio;
    [SerializeField] private GameObject FimDeJogo;
    [SerializeField] private GameObject Game;
    [Tooltip("Ao reiniciar, entra direto no jogo sem passar pela tela de início")]
    [SerializeField] private bool pularInicioAoReiniciar = true;

    public static GameObject FimDeJogoStatic => instancia != null ? instancia.FimDeJogo : null;

    [Header("Armas")]
    [SerializeField] private GameObject Armas;
    [SerializeField] private TMP_Text textoMunicao;

    [Header("Habilidades")]
    [SerializeField] private GameObject Habilidades;
    [SerializeField] private SlotHabilidade explosao = new SlotHabilidade { nome = "Explosão" };
    [SerializeField] private SlotHabilidade vortice = new SlotHabilidade { nome = "Vórtice" };

    [Header("Aparência")]
    [SerializeField] private Color corPronta = Color.white;
    [SerializeField] private Color corEscurecida = new Color(0.25f, 0.25f, 0.25f, 0.85f);

    [HideInInspector] public bool iniciado = false;

    // Frame em que o jogo começou (evita que o clique do botão vire tiro no mesmo frame)
    private int frameInicio = -1;

    // Os outros scripts (armas, habilidades, movimento...) consultam isso antes de reagir ao input
    public bool JogoAtivo => iniciado && Time.frameCount > frameInicio;

    private int ultimaMunicao = -1;
    private int ultimaCapacidade = -1;

    private void Awake()
    {
        instancia = this;
        iniciado = false;

        if (arma == null)
            arma = FindAnyObjectByType<armas>();

        if (habilidades == null)
            habilidades = FindAnyObjectByType<DisparoHabilidade>();

        Preparar(explosao);
        Preparar(vortice);

        // Estado inicial: tela de início, jogo pausado
        if (Inicio != null) Inicio.SetActive(true);
        if (Game != null) Game.SetActive(false);
        if (FimDeJogo != null) FimDeJogo.SetActive(false);
        Time.timeScale = 0f;

        if (reiniciando && pularInicioAoReiniciar)
            IniciarJogo();

        reiniciando = false;
    }

    private void Update()
    {
        // Update roda mesmo com timeScale = 0, por isso o cursor é controlado aqui
        AtualizarCursor();

        if (!iniciado)
            return;

        AtualizarMunicao();

        if (habilidades == null)
            return;

        AtualizarSlot(explosao, habilidades.RecargaRestanteExplosao);
        AtualizarSlot(vortice, habilidades.RecargaRestanteVortice);
    }

    private void AtualizarCursor()
    {
        Cursor.lockState = iniciado ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !iniciado;
    }

    // ---------- Fluxo do jogo ----------

    public void IniciarJogo()
    {
        iniciado = true;
        frameInicio = Time.frameCount;

        if (Inicio != null) Inicio.SetActive(false);
        if (FimDeJogo != null) FimDeJogo.SetActive(false);
        if (Game != null) Game.SetActive(true);

        Time.timeScale = 1f;
    }

    public void FimGame()
    {
        if (!iniciado)
            return;

        iniciado = false;

        if (Inicio != null) Inicio.SetActive(false);
        if (Game != null) Game.SetActive(false);
        if (FimDeJogo != null) FimDeJogo.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ReiniciarJogo()
    {
        reiniciando = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ---------- Munição ----------

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

    // ---------- Ícones das habilidades ----------

    private void Preparar(SlotHabilidade slot)
    {
        if (slot.icone == null)
        {
            Debug.LogWarning("[HUD] Sprite não definido para " + slot.nome, this);
            return;
        }

        // Sem Image atribuída: procura, dentro de Habilidades, o objeto com o nome da habilidade
        if (slot.imagem == null)
            slot.imagem = ProcurarImagem(slot.nome);

        if (slot.imagem == null)
        {
            Debug.LogWarning("[HUD] Não achei a Image da habilidade '" + slot.nome +
                             "'. Arraste-a no campo 'Imagem' (dentro de Game > Habilidades).", this);
            return;
        }

        slot.imagem.sprite = slot.icone;
        slot.imagem.type = Image.Type.Simple;
        slot.imagem.preserveAspect = true;
        slot.imagem.raycastTarget = false;
        slot.imagem.color = corPronta;

        GameObject objPreench = new GameObject("Preenchimento", typeof(RectTransform), typeof(Image));
        RectTransform rtP = objPreench.GetComponent<RectTransform>();
        rtP.SetParent(slot.imagem.rectTransform, false);
        rtP.anchorMin = Vector2.zero;
        rtP.anchorMax = Vector2.one;
        rtP.offsetMin = Vector2.zero;
        rtP.offsetMax = Vector2.zero;

        slot.preenchimento = objPreench.GetComponent<Image>();
        slot.preenchimento.sprite = slot.icone;
        slot.preenchimento.preserveAspect = true;
        slot.preenchimento.raycastTarget = false;
        slot.preenchimento.color = corPronta;
        slot.preenchimento.type = Image.Type.Filled;
        slot.preenchimento.fillMethod = Image.FillMethod.Radial360;
        slot.preenchimento.fillOrigin = (int)Image.Origin360.Top;
        slot.preenchimento.fillClockwise = true;
        slot.preenchimento.fillAmount = 1f;
        slot.preenchimento.enabled = false;
    }

    // Procura (sem diferenciar acentos/maiúsculas) um filho de Habilidades com o nome da habilidade
    private Image ProcurarImagem(string nome)
    {
        Transform raiz = Habilidades != null ? Habilidades.transform
                       : Game != null ? Game.transform
                       : transform;

        string alvo = Normalizar(nome);

        foreach (Transform filho in raiz.GetComponentsInChildren<Transform>(true))
        {
            if (filho == raiz) continue;
            if (Normalizar(filho.name) != alvo) continue;

            // Usa a Image que já existe ou cria o componente no próprio objeto
            if (!filho.TryGetComponent(out Image img))
                img = filho.gameObject.AddComponent<Image>();
            return img;
        }
        return null;
    }

    private static string Normalizar(string texto)
    {
        string d = texto.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();
        foreach (char c in d)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(char.ToLowerInvariant(c));
        return sb.ToString();
    }

    private void AtualizarSlot(SlotHabilidade slot, float restante)
    {
        if (slot.imagem == null || slot.preenchimento == null)
            return;

        if (restante <= 0f)
        {
            if (slot.emRecarga)
            {
                slot.emRecarga = false;
                slot.recargaTotal = 0f;
                slot.imagem.color = corPronta;
                slot.preenchimento.enabled = false;
            }
            return;
        }

        if (!slot.emRecarga)
        {
            slot.emRecarga = true;
            slot.recargaTotal = restante;
            slot.imagem.color = corEscurecida;
            slot.preenchimento.enabled = true;
        }

        if (restante > slot.recargaTotal)
            slot.recargaTotal = restante;

        slot.preenchimento.fillAmount = 1f - (restante / slot.recargaTotal);
    }
}
