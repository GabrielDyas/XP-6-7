using UnityEngine;
using UnityEngine.InputSystem;

public class armas : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference acaoAtirar;
    [SerializeField] private InputActionReference acaoRecarregar;

    [Header("Referencias Script")]
    [SerializeField] private controleTiro controladorTiro;

    [Header("Arma equipada")]
    [SerializeField] private infoArmas dadosArmaEquipada;

    private int municaoAtual;
    private bool recarregando = false;

    private void Awake()
    {
        if (dadosArmaEquipada == null)
        {
            Debug.LogError("Dados da arma não definidos no Inspector.", this);
            enabled = false;
            return;
        }

        // Procura o controleTiro neste mesmo GameObject,
        // caso nenhuma referência tenha sido definida no Inspector.
        if (controladorTiro == null)
        {
            controladorTiro = GetComponent<controleTiro>();
        }

        // Começa com o carregador cheio.
        municaoAtual = dadosArmaEquipada.municao;

        Debug.Log("Munição total: " + dadosArmaEquipada.municao);
    }

    private void OnEnable()
    {
        if (acaoAtirar != null)
            acaoAtirar.action.Enable();

        if (acaoRecarregar != null)
            acaoRecarregar.action.Enable();
    }

    private void OnDisable()
    {
        if (acaoAtirar != null)
            acaoAtirar.action.Disable();

        if (acaoRecarregar != null)
            acaoRecarregar.action.Disable();
    }

    private void Update()
    {
        if (dadosArmaEquipada == null || controladorTiro == null)
            return;

        if (acaoRecarregar != null &&
            acaoRecarregar.action.triggered)
        {
            if (!recarregando)
                Recarregar();
            else
                Debug.Log("Já está recarregando.");

            return;
        }

        if (acaoAtirar == null || !acaoAtirar.action.triggered)
            return;

        if (recarregando)
        {
            Debug.Log("Não dá para atirar, recarregando.");
            return;
        }

        if (municaoAtual > 0)
        {
            // Consome uma bala e executa o disparo.
            municaoAtual--;

            controladorTiro.Atirar(dadosArmaEquipada.dano);

            Debug.Log("Munição restante: " + municaoAtual);
        }
    }

    private void Recarregar()
    {
        if (recarregando)
            return;

        if (municaoAtual >= dadosArmaEquipada.municao)
        {
            Debug.Log("O carregador já está cheio.");
            return;
        }

        recarregando = true;

        // Recarga instantânea.
        municaoAtual = dadosArmaEquipada.municao;

        Debug.Log("Arma recarregada! Munição: " + municaoAtual);

        recarregando = false;
    }
}