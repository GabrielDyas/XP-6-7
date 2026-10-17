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

    private InputSystem_Actions input;
    private Collider[] colidersDoDono;

    private void Awake()
    {
        input = new InputSystem_Actions();
        colidersDoDono = transform.root.GetComponentsInChildren<Collider>();

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
        GameObject obj = Instantiate(projetil, ponto.position, ponto.rotation);
        obj.GetComponent<Projetil>().Lancar(efeito, ponto.forward * velocidadeDoProjetil, colidersDoDono, ponto.rotation);
    }
}
