using UnityEngine;

public class AreaDeSom : MonoBehaviour
{
    [Header("Componentes")]
    [Tooltip("Rigidbody do objeto. Se deixado em branco, busca automaticamente.")]
    [SerializeField] private Rigidbody rb;

    [Header("Configurações do Raio")]
    [Tooltip("Raio da área quando o objeto está totalmente parado.")]
    [SerializeField] private float minRadius = 0f;

    [Tooltip("Raio máximo da área quando o objeto atingir a velocidade limite.")]
    [SerializeField] private float maxRadius = 10f;

    [Tooltip("Velocidade do Rigidbody necessária para atingir o raio máximo.")]
    [SerializeField] private float maxSpeed = 15f;

    [Tooltip("Velocidade de transição para o raio crescer/diminuir suavemente. Defina 0 para instantâneo.")]
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Visualização & Colisão (Opcionais)")]
    [Tooltip("Objeto filho que representa visualmente a área (ex: Círculo ou Esfera).")]
    [SerializeField] private Transform areaVisual;

    [Tooltip("Collider do tipo esfera/gatilho para ajustar a área de colisão dinamicamente.")]
    [SerializeField] private SphereCollider areaCollider;

    private float currentRadius;

    public float RaioAtual => currentRadius;
    public Vector3 Centro => transform.position;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        currentRadius = minRadius;
    }

    private void Update()
    {
        if (rb == null) return;


        float currentSpeed = rb.linearVelocity.magnitude;

        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

        float targetRadius = Mathf.Lerp(minRadius, maxRadius, speedRatio);

        if (smoothSpeed > 0f)
        {
            currentRadius = Mathf.Lerp(currentRadius, targetRadius, Time.deltaTime * smoothSpeed);
        }
        else
        {
            currentRadius = targetRadius;
        }

        if (areaVisual != null)
        {
            float diameter = currentRadius * 2f;
            areaVisual.localScale = new Vector3(diameter, areaVisual.localScale.y, diameter);
        }

        if (areaCollider != null)
        {
            areaCollider.radius = currentRadius;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Application.isPlaying ? currentRadius : minRadius);
    }
}