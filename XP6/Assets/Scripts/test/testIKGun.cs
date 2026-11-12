
using UnityEngine;
using UnityEngine.InputSystem;

public class testIKGun : MonoBehaviour
{
    [Header("Targets da arma")]
    [SerializeField] private Transform targetArmaDireita;
    [SerializeField] private Transform targetArmaEsquerda;

    [Header("Targets do Two Bone IK")]
    [SerializeField] private Transform targetIKDireita;
    [SerializeField] private Transform targetIKEsquerda;

    [Header("Recuo")]
    [SerializeField] private InputActionReference acaoRecuo;
    [SerializeField] private Transform objetoRecuo;
    [SerializeField] private float intensidadeRecuo = 2.5f;
    [SerializeField] private float forcaRetorno = 100f;
    [SerializeField] private float amortecimento = 18f;

    private Vector3 posicaoInicial;
    private Vector3 velocidadeRecuo;

    private void Awake()
    {
        if (objetoRecuo != null)
            posicaoInicial = objetoRecuo.localPosition;
    }
    private void Update()
    {
        if (objetoRecuo == null)
            return;

        // Aplica o impulso do disparo
        if (acaoRecuo != null && acaoRecuo.action.WasPressedThisFrame())
        {
            velocidadeRecuo += Vector3.back * intensidadeRecuo;
        }

        // Calcula a força de retorno
        Vector3 deslocamento = posicaoInicial - objetoRecuo.localPosition;

        Vector3 forcaMola = deslocamento * forcaRetorno;

        // Calcula o amortecimento
        Vector3 forcaAmortecimento = -velocidadeRecuo * amortecimento;

        // Soma as forças
        Vector3 aceleracao = forcaMola + forcaAmortecimento;

        // Atualiza velocidade e posição
        velocidadeRecuo += aceleracao * Time.deltaTime;
        objetoRecuo.localPosition += velocidadeRecuo * Time.deltaTime;
    }

    private void LateUpdate()
    {
        // Mão direita
        if (targetArmaDireita != null && targetIKDireita != null)
        {
            targetIKDireita.SetPositionAndRotation(
                targetArmaDireita.position,
                targetArmaDireita.rotation);
        }

        // Mão esquerda
        if (targetArmaEsquerda != null && targetIKEsquerda != null)
        {
            targetIKEsquerda.SetPositionAndRotation(
                targetArmaEsquerda.position,
                targetArmaEsquerda.rotation);
        }
    }
}
