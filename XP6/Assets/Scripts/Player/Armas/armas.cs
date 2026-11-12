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
    [SerializeField] private bool debug = false;

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
        if (debug)
        {
            Debug.Log("Munição total: " + dadosArmaEquipada.municao);
        }
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

        // Verifica primeiro se o jogador pediu para recarregar.
        if (acaoRecarregar != null &&acaoRecarregar.action.triggered)
        {
            if (!recarregando)
            {
                Recarregar();
            }
            else if (debug)
            {
                Debug.Log("Já está recarregando.");
            }
            return;
        }

        if (acaoAtirar == null || !acaoAtirar.action.triggered)
            return;

        if (recarregando)
        {
            if (debug)
            {
                Debug.Log("Não dá para atirar, recarregando.");
            }

            return;
        }

        if (municaoAtual > 0)
        {
            // Consome uma bala e executa o disparo.
            municaoAtual--;
            controladorTiro.Atirar(dadosArmaEquipada.dano);
            if (debug)
            {
                Debug.Log("Munição restante: " + municaoAtual);
            }
        }
    }

    private void Recarregar()
    {
        if (recarregando)
            return;

        if (municaoAtual >= dadosArmaEquipada.municao)
        {
            if (debug)
            {
                Debug.Log("O carregador já está cheio.");
            }
            return;
        }

        recarregando = true;

        // Recarga instantânea.
        municaoAtual = dadosArmaEquipada.municao;
        if (debug)
        {
            Debug.Log("Arma recarregada! Munição: " + municaoAtual);
        }
        recarregando = false;
    }



    public float GetMunicao()
    {
        return municaoAtual;
    }

    public float GetCapacidadeCarregador()
    {
        return dadosArmaEquipada.municao;
    }
}

