
using UnityEngine;

public class controleTiro : MonoBehaviour
{
    [Header("Raycast")]
    public Camera cameraPrincipal;
    public float distanciaMaxima = 100f;

    [Header("Tracer")]
    [SerializeField] private LineRenderer prefabTracer;
    [SerializeField] private Transform origemTracer;
    public float vidaDoRaioSegundos = 0.08f;

    [Header("Debug")]
    [SerializeField] private bool debug = false;

    private void Start()
    {
        cameraPrincipal = Camera.main;
    }

    // Executa o disparo quando solicitado pelo script armas.
    public void Atirar(float dano)
    {
        if (cameraPrincipal == null)
            return;

        Vector3 coordenadaDaTela = new Vector3(0.5f, 0.5f, 0f);

        Ray raioDoTiro = cameraPrincipal.ViewportPointToRay(coordenadaDaTela);

        Vector3 pontoFinal = raioDoTiro.GetPoint(distanciaMaxima);

        // O Raycast encontra o objeto atingido.
        if (Physics.Raycast(raioDoTiro, out RaycastHit hit,
            distanciaMaxima, Physics.DefaultRaycastLayers))
        {
            pontoFinal = hit.point;

            if (debug)
            {
                Debug.Log("Acertou: " + hit.collider.gameObject.name);
            }

            mainInimigo inimigo = hit.collider.GetComponentInParent<mainInimigo>();

            if (inimigo != null)
            {
                inimigo.tomarDano(dano);
            }
        }

        // Define a origem visual do tracer.
        Vector3 origem = raioDoTiro.origin;

        if (origemTracer != null)
        {
            origem = origemTracer.position;
        }

        CriarTracer(origem, pontoFinal);

        if (debug)
        {
            Debug.DrawLine(origem, pontoFinal, Color.red, vidaDoRaioSegundos);
        }
    }

    private void CriarTracer(Vector3 origem, Vector3 destino)
    {
        if (prefabTracer == null)
            return;

        LineRenderer tracer = Instantiate(prefabTracer);

        tracer.useWorldSpace = true;
        tracer.positionCount = 2;

        tracer.SetPosition(0, origem);
        tracer.SetPosition(1, destino);

        Destroy(tracer.gameObject, vidaDoRaioSegundos);
    }
}
