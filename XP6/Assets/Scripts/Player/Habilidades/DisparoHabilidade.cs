using UnityEngine;
using UnityEngine.InputSystem;

public class DisparoHabilidade : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject pontoDeDisparo;
    [SerializeField] private GameObject projetil;

    [Header("Habilidades (efeito criado no impacto)")]
    [SerializeField] private GameObject habilidadeExplosaoPrefab; // Habilidade1 (explosão)
    [SerializeField] private GameObject habilidadeVorticePrefab; // Habilidade2 (vórtice)

    [Header("Tempo de recarga (segundos)")]
    [SerializeField] private float recargaExplosao = 10f;
    [SerializeField] private float recargaVortice = 10f;

    [Header("Projétil")]
    [SerializeField] private float velocidadeDoProjetil = 10f;

    [Header("Mira (centro da tela)")]
    [SerializeField] private float distanciaMaximaDaMira = 100f;
    [SerializeField] private LayerMask camadasDaMira = ~0;

    private static readonly RaycastHit[] hits = new RaycastHit[16];

    private InputSystem_Actions input;
    private Collider[] colidersDoDono;
    private Transform cam;

    // Instante do jogo (Time.time) a partir do qual cada habilidade pode ser usada de novo
    private float proximoUsoExplosao;
    private float proximoUsoVortice;

    // Segundos que faltam para a habilidade ficar pronta (0 = pronta). Útil para a UI.
    public float RecargaRestanteExplosao => Mathf.Max(0f, proximoUsoExplosao - Time.time);
    public float RecargaRestanteVortice => Mathf.Max(0f, proximoUsoVortice - Time.time);

    private void Awake()
    {
        input = new InputSystem_Actions();
        colidersDoDono = GetComponentsInChildren<Collider>();
        cam = Camera.main != null ? Camera.main.transform : null;

        bool ok = true;
        if (pontoDeDisparo == null)
        {
            Debug.LogError("[Disparo] 'Ponto De Disparo' não está atribuído no Inspector.", this);
            ok = false;
        }
        if (projetil == null)
        {
            Debug.LogError("[Disparo] 'Projetil' (prefab) não está atribuído no Inspector.", this);
            ok = false;
        }
        else if (!projetil.TryGetComponent<Projetil>(out _))
        {
            Debug.LogError("[Disparo] O prefab do projétil precisa ter o script Projetil.", this);
            ok = false;
        }
        if (cam == null)
            Debug.LogWarning("[Disparo] Nenhuma câmera com a tag MainCamera; o tiro vai sair na direção do ponto de disparo.", this);

        enabled = ok;
    }

    private void OnEnable()
    {
        input.Player.Habilidade1.performed += AoHabilidade1;
        input.Player.Habilidade2.performed += AoHabilidade2;
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Habilidade1.performed -= AoHabilidade1;
        input.Player.Habilidade2.performed -= AoHabilidade2;
        input.Player.Disable();
    }

    private void OnDestroy() => input.Dispose();

    private void AoHabilidade1(InputAction.CallbackContext ctx) =>
        Disparar(habilidadeExplosaoPrefab, recargaExplosao, ref proximoUsoExplosao);

    private void AoHabilidade2(InputAction.CallbackContext ctx) =>
        Disparar(habilidadeVorticePrefab, recargaVortice, ref proximoUsoVortice);

    private void Disparar(GameObject efeito, float recarga, ref float proximoUso)
    {
        // Nas telas de início e fim não dispara
        if (ControladorHud.instancia != null && !ControladorHud.instancia.JogoAtivo) return;

        // Ainda em recarga: ignora o aperto
        if (Time.time < proximoUso) return;

        if (efeito == null)
            Debug.LogWarning("[Disparo] Esta habilidade está sem prefab de efeito; nada acontece no impacto.", this);

        // A recarga só começa quando o disparo realmente acontece
        proximoUso = Time.time + recarga;

        Transform ponto = pontoDeDisparo.transform;
        Vector3 direcao = CalcularDirecaoDaMira(ponto.position);

        GameObject obj = Instantiate(projetil, ponto.position, Quaternion.LookRotation(direcao));
        obj.GetComponent<Projetil>().Lancar(efeito, direcao * velocidadeDoProjetil, colidersDoDono);
    }


    private Vector3 CalcularDirecaoDaMira(Vector3 origem)
    {
        if (cam == null) return pontoDeDisparo.transform.forward;

        Vector3 alvo = cam.position + cam.forward * distanciaMaximaDaMira;

        int n = Physics.RaycastNonAlloc(cam.position, cam.forward, hits,
                                        distanciaMaximaDaMira, camadasDaMira,
                                        QueryTriggerInteraction.Ignore);
        float menor = float.MaxValue;
        for (int i = 0; i < n; i++)
        {
            if (hits[i].collider.transform.IsChildOf(transform)) continue;
            if (hits[i].distance < menor)
            {
                menor = hits[i].distance;
                alvo = hits[i].point;
            }
        }

        Vector3 direcao = alvo - origem;

        if (Vector3.Dot(direcao, cam.forward) <= 0f) return cam.forward;

        return direcao.normalized;
    }
}