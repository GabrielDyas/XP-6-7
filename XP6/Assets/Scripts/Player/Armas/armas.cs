
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
    private float proximoDisparo;
    [SerializeField] private bool debug = false;

    private void Awake()
    {
        if (dadosArmaEquipada == null)
        {
            Debug.LogError("Dados da arma não definidos no Inspector.", this);
            enabled = false;
            return;
        }

        if (dadosArmaEquipada.automatica && dadosArmaEquipada.cadencia <= 0)
        {
            Debug.LogError("A cadência da arma automática deve ser maior que zero.", this);
            enabled = false;
            return;
        }

        if (controladorTiro == null)
        {
            controladorTiro = GetComponent<controleTiro>();
        }

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

        if (acaoRecarregar != null && acaoRecarregar.action.triggered)
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

        if (acaoAtirar == null)
            return;

        // ALTERADO: verifica o input dependendo do tipo de arma.
        bool solicitouDisparo;

        if (dadosArmaEquipada.automatica)
        {
            solicitouDisparo = acaoAtirar.action.IsPressed();
        }
        else
        {
            solicitouDisparo = acaoAtirar.action.triggered;
        }

        if (!solicitouDisparo)
            return;

        if (recarregando)
        {
            if (debug)
            {
                Debug.Log("Não dá para atirar, recarregando.");
            }
            return;
        }

        // NOVO: impede disparos automáticos antes do intervalo permitido.
        if (dadosArmaEquipada.automatica && Time.time < proximoDisparo)
            return;

        if (municaoAtual > 0)
        {
            municaoAtual--;

            controladorTiro.Atirar(dadosArmaEquipada.dano);

            // NOVO: calcula o intervalo entre os disparos automáticos.
            if (dadosArmaEquipada.automatica)
            {
                float intervaloEntreDisparos = 60f / dadosArmaEquipada.cadencia;
                proximoDisparo = Time.time + intervaloEntreDisparos;
            }

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
