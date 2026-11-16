
using UnityEngine;
using UnityEngine.InputSystem;

public class armas : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference acaoAtirar;
    [SerializeField] private InputActionReference acaoRecarregar;
    [SerializeField] private InputActionReference acaoTrocarArma;

    [Header("Referencias Script")]
    [SerializeField] private controleTiro controladorTiro;
    [SerializeField] private controleArmaVisual controladorVisual;
    [SerializeField] private controleSomArma controladorSom;

    [Header("Armas")]
    [SerializeField] private infoArmas dadosArmaEquipada;
    [SerializeField] private infoArmas dadosArmaSecundaria;

    private infoArmas dadosArmaPrincipal;

    private int municaoAtual;
    private int municaoArmaPrincipal;
    private int municaoArmaSecundaria;

    private bool armaSecundariaEquipada = false;
    private bool recarregando = false;

    private float proximoDisparo;

    [SerializeField] private bool debug = false;

    private void Awake()
    {
        if (dadosArmaEquipada == null)
        {
            Debug.LogError("Dados da arma principal não definidos.", this);
            enabled = false;
            return;
        }

        dadosArmaPrincipal = dadosArmaEquipada;

        if (dadosArmaPrincipal.automatica && dadosArmaPrincipal.cadencia <= 0)
        {
            Debug.LogError("A cadência da arma principal deve ser maior que zero.", this);
            enabled = false;
            return;
        }

        if (dadosArmaSecundaria != null &&
            dadosArmaSecundaria.automatica &&
            dadosArmaSecundaria.cadencia <= 0)
        {
            Debug.LogError("A cadência da arma secundária deve ser maior que zero.", this);
            enabled = false;
            return;
        }

        if (controladorTiro == null)
        {
            controladorTiro = GetComponent<controleTiro>();
        }

        if (controladorVisual == null)
        {
            controladorVisual = GetComponent<controleArmaVisual>();
        }

        // Inicializa a munição das duas armas.
        municaoAtual = dadosArmaEquipada.municao;
        municaoArmaPrincipal = municaoAtual;

        if (dadosArmaSecundaria != null)
        {
            municaoArmaSecundaria = dadosArmaSecundaria.municao;
        }

        AtualizarVisualSom();

        if (debug)
        {
            Debug.Log("Munição inicial: " + municaoAtual);
        }
    }

    private void OnEnable()
    {
        if (acaoAtirar != null)
            acaoAtirar.action.Enable();

        if (acaoRecarregar != null)
            acaoRecarregar.action.Enable();

        if (acaoTrocarArma != null)
            acaoTrocarArma.action.Enable();
    }

    private void OnDisable()
    {
        if (acaoAtirar != null)
            acaoAtirar.action.Disable();

        if (acaoRecarregar != null)
            acaoRecarregar.action.Disable();

        if (acaoTrocarArma != null)
            acaoTrocarArma.action.Disable();
    }

    private void Update()
    {
        if (dadosArmaEquipada == null || controladorTiro == null)
            return;

        // Troca a arma quando solicitado.
        if (acaoTrocarArma != null && acaoTrocarArma.action.triggered)
        {
            TrocarArma();
            return;
        }

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
            return;

        if (dadosArmaEquipada.automatica && Time.time < proximoDisparo)
            return;

        if (municaoAtual > 0)
        {
            municaoAtual--;

            controladorTiro.Atirar(dadosArmaEquipada.dano);

            if (controladorSom != null)
            {
                controladorSom.TocarDisparo(dadosArmaEquipada);
            }

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

    private void TrocarArma()
    {
        if (dadosArmaSecundaria == null)
        {
            if (debug)
            {
                Debug.LogWarning("Arma secundária não configurada.", this);
            }

            return;
        }

        if (!armaSecundariaEquipada)
        {
            // Salva a munição da arma principal.
            municaoArmaPrincipal = municaoAtual;

            // Equipa a secundária.
            dadosArmaEquipada = dadosArmaSecundaria;
            municaoAtual = municaoArmaSecundaria;

            armaSecundariaEquipada = true;
        }
        else
        {
            // Salva a munição da arma secundária.
            municaoArmaSecundaria = municaoAtual;

            // Equipa a principal.
            dadosArmaEquipada = dadosArmaPrincipal;
            municaoAtual = municaoArmaPrincipal;

            armaSecundariaEquipada = false;
        }

        // Reinicia o intervalo de disparo.
        proximoDisparo = 0f;

        AtualizarVisualSom();

        if (debug)
        {
            Debug.Log("Arma equipada: " + dadosArmaEquipada.nome);
            Debug.Log("Munição: " + municaoAtual);
        }
    }

    private void AtualizarVisualSom()
    {
        if (controladorVisual != null)
        {
            controladorVisual.Equipar(dadosArmaEquipada);

            // Atualiza o AudioSource para o modelo da nova arma.
            if (controladorSom != null)
            {
                AudioSource audioSourceArma = controladorVisual.ObterAudioSource();
                controladorSom.ConfigurarAudioSource(audioSourceArma);
            }
        }
        else if (debug)
        {
            Debug.LogWarning("Controlador visual da arma não encontrado.", this);
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

        if (controladorSom != null)
        {
            controladorSom.TocarRecarga(dadosArmaEquipada);
        }

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
