
using UnityEngine;
using UnityEngine.InputSystem;

public class armas : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference acaoAtirar;
    [SerializeField] private InputActionReference acaoRecarregar;

    [Header("Referencias Script")]
    [SerializeField] private controleTiro controladorTiro;
    [SerializeField] private controleArmaVisual controladorVisual;
    [SerializeField] private controleSomArma controladorSom;

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
            enabled = false; // tá aqui pra impedir divir por zero no calculo da cadência
            return;
        }

        if (controladorTiro == null)
        {
            controladorTiro = GetComponent<controleTiro>();
        }
        // procura o controlador visual caso não tenha referência
        if (controladorVisual == null)
        {
            controladorVisual = GetComponent<controleArmaVisual>();
        }

        // envia os dados da arma para o controlador visual
        if (controladorVisual != null)
        {
            controladorVisual.Equipar(dadosArmaEquipada);

            // Obtém o AudioSource do modelo instanciado.
            if (controladorSom != null)
            {
                AudioSource audioSourceArma = controladorVisual.ObterAudioSource();
                controladorSom.ConfigurarAudioSource(audioSourceArma);
            }
        }
        else
        {
            Debug.LogWarning("Controlador visual da arma não encontrado.", this);
        }

        // Verifica se o controlador de som foi atribuído.
        if (controladorSom == null && debug)
        {
            Debug.LogWarning("Controlador de som não definido no Inspector.", this);
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
            controladorSom.TocarDisparo(dadosArmaEquipada);

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
        controladorSom.TocarRecarga(dadosArmaEquipada);
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
