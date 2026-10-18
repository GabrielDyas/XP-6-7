using UnityEngine;
using UnityEngine.InputSystem;

public class DisparoHabilidade : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject pontoDeDisparo;
    [SerializeField] private GameObject projetil; // prefab com Projetil + Rigidbody + Collider

    [Header("Habilidades (efeito criado no impacto)")]
    [SerializeField] private GameObject habilidadeExplosaoPrefab; // Habilidade1 (explosão)
    [SerializeField] private GameObject habilidadeVorticePrefab; // Habilidade2 (vórtice)

    [Header("Projétil")]
    [SerializeField] private float velocidadeDoProjetil = 10f;

    [Header("Mira (centro da tela)")]
    [SerializeField] private float distanciaMaximaDaMira = 100f;
    [SerializeField] private LayerMask camadasDaMira = ~0;

    private static readonly RaycastHit[] hits = new RaycastHit[16];

    private InputSystem_Actions input;
    private Collider[] colidersDoDono;
    private Transform cam;

    private void Awake()
    {
        input = new InputSystem_Actions();
        colidersDoDono = GetComponentsInChildren<Collider>();
        cam = Camera.main != null ? Camera.main.transform : null;

        // Avisa no Console exatamente o que está faltando
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

    private void AoHabilidade1(InputAction.CallbackContext ctx) => Disparar(habilidadeExplosaoPrefab);
    private void AoHabilidade2(InputAction.CallbackContext ctx) => Disparar(habilidadeVorticePrefab);

    // Uma única lógica para todas as habilidades: só muda o efeito do impacto
    private void Disparar(GameObject efeito)
    {
        if (efeito == null)
            Debug.LogWarning("[Disparo] Esta habilidade está sem prefab de efeito; nada acontece no impacto.", this);

        Transform ponto = pontoDeDisparo.transform;
        Vector3 direcao = CalcularDirecaoDaMira(ponto.position);

        GameObject obj = Instantiate(projetil, ponto.position, Quaternion.LookRotation(direcao));
        obj.GetComponent<Projetil>().Lancar(efeito, direcao * velocidadeDoProjetil, colidersDoDono);
    }

    // Raio do centro da tela -> ponto que o jogador está olhando.
    // A direção do tiro vai do ponto de disparo até esse ponto.
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
            if (hits[i].collider.transform.IsChildOf(transform)) continue; // ignora o próprio jogador
            if (hits[i].distance < menor)
            {
                menor = hits[i].distance;
                alvo = hits[i].point;
            }
        }

        Vector3 direcao = alvo - origem;

        // Alvo atrás do ponto de disparo (parede colada): usa a direção da câmera
        if (Vector3.Dot(direcao, cam.forward) <= 0f) return cam.forward;

        return direcao.normalized;
    }
}
