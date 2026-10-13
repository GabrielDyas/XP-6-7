
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class armas : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference acaoAtirar;
    [SerializeField] private InputActionReference acaoRecarregar;

    [Header("Referencias Script")]
    [SerializeField] private controleTiro controladorTiro;

    [Header("Municao")]
    [SerializeField] private int capacidadeCarregador = 10;
    private int municaoAtual;
    private bool recarregando = false;

    [UnitHeaderInspectable("Dano")]
    [SerializeField] public float dano = 5f;

    private void Awake()
    {
        // Procura o controleTiro neste mesmo GameObject,
        // caso nenhuma referência tenha sido definida no Inspector.
        if (controladorTiro == null)
        {
            Debug.Log("controladorTiro não definido no inspetor");
            controladorTiro = GetComponent<controleTiro>();
        }

        // Começa o jogo com o carregador cheio.
        municaoAtual = capacidadeCarregador;

        Debug.Log("Munição total: " + capacidadeCarregador);
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
        if (controladorTiro == null)
        {
            Debug.Log("referência do controladorTiro nula");
            return;
        }

        // Verifica primeiro se o jogador pediu para recarregar.
        if (acaoRecarregar != null && acaoRecarregar.action.triggered && !recarregando) {
            Recarregar();
            return;
        }
        if (acaoRecarregar.action.triggered && recarregando) {
            Debug.Log("Já está recarregando");
            
        }
        if (acaoAtirar.action.triggered)
        {
            if (recarregando) 
            {
                Debug.Log("Não dá pra atirar, recarregando");
            }
            if (municaoAtual > 0f)
            {
                // Consome uma bala e executa o disparo.
                municaoAtual--;

                controladorTiro.Atirar(dano);

                Debug.Log("Munição restante: " + municaoAtual);
            }
        }
    }

    private void Recarregar()
    {
        if (recarregando)
            return;

        // Não recarrega se o carregador já estiver cheio.
        if (municaoAtual >= capacidadeCarregador)
        {
            Debug.Log("O carregador já está cheio.");
            return;
        }

        recarregando = true;

        // Recarga instantânea: enche o carregador imediatamente.
        municaoAtual = capacidadeCarregador;

        Debug.Log("Arma recarregada! Munição: " + municaoAtual);

        recarregando = false;
    }
}
