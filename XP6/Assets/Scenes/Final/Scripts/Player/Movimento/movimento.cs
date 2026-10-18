using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movimento : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float multiplicadorCorrida = 1.5f;     
    [SerializeField] private float multiplicadorAgachado = 0.5f;  

    [Header("Pulo")]
    [SerializeField] private float forcaDoPulo = 5f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 10f;     
    [SerializeField] private float dashDuracao = 0.2f;  
    [SerializeField] private float dashCooldown = 1f;

    [Header("Chão")]
    [SerializeField] private Collider colliderDoPe; 
    [SerializeField] private LayerMask terreno;
    [SerializeField] private float distanciaChao = 0.1f;

    # region Acesso para os estados
    public float Velocidade => velocidade;
    public float MultiplicadorCorrida => multiplicadorCorrida;
    public float MultiplicadorAgachado => multiplicadorAgachado;
    public float DashForce => dashForce;
    public float DashDuracao => dashDuracao;

    public InputSystem_Actions Controles { get; private set; }
    public Rigidbody Rb { get; private set; }
    public MaquinaDeEstados Maquina { get; private set; }

    public EstadoBase Andando { get; private set; }
    public EstadoBase Correndo { get; private set; }
    public EstadoBase Agachado { get; private set; }
    public EstadoBase Dash { get; private set; }
    public EstadoBase Pulo { get; private set; }
    public EstadoBase Caindo { get; private set; }

    // Já usou o pulo extra no ar? (reseta ao aterrissar)
    public bool PuloNoArUsado { get; set; }

    private Transform cam;
    private float proximoDash;

    // ===== Leituras de input usadas por vários estados =====
    public Vector2 InputMovimento => Controles.Player.Move.ReadValue<Vector2>();

    // Andando/Agachado: apertar Shift indo para frente -> Correndo
    public bool QuerCorrer =>
        Controles.Player.Sprint.WasPressedThisFrame() && InputMovimento.y > 0.1f;

    // Andando/Agachado: apertar Shift indo para os lados ou para trás -> Dash
    public bool QuerDash =>
        Controles.Player.Sprint.WasPressedThisFrame()
        && InputMovimento.sqrMagnitude > 0.01f
        && InputMovimento.y <= 0.1f
        && Time.time >= proximoDash;

    // Correndo: apertar Shift de novo (movendo) -> Dash na direção do movimento
    public bool QuerDashCorrendo =>
        Controles.Player.Sprint.WasPressedThisFrame()
        && InputMovimento.sqrMagnitude > 0.01f
        && Time.time >= proximoDash;

    #endregion

    #region Ciclo de vida

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();

        // Se não foi arrastado no Inspector, tenta achar o filho "pe"
        if (colliderDoPe == null)
        {
            Transform pe = transform.Find("pe");
            if (pe != null) pe.TryGetComponent(out colliderDoPe);
        }
        if (colliderDoPe == null)
            Debug.LogError("[Movimento] Atribua o collider do 'pe' no campo 'Collider Do Pe'.", this);
        cam = Camera.main.transform;

        Controles = new InputSystem_Actions();
        Maquina = new MaquinaDeEstados();

        Andando = new EstadoAndando(this, Maquina);
        Correndo = new EstadoCorrendo(this, Maquina);
        Agachado = new EstadoAgachado(this, Maquina);
        Dash = new EstadoDash(this, Maquina);
        Pulo = new EstadoPulo(this, Maquina);
        Caindo = new EstadoCaindo(this, Maquina);
    }

    private void OnEnable() => Controles.Player.Enable();
    private void OnDisable() => Controles.Player.Disable();
    private void OnDestroy() => Controles.Dispose();

    private void Start()
    {
        Maquina.Iniciar(Andando); // estado inicial do diagrama
    }

    private void Update()
    {
        Maquina.Atual.AtualizarLogica();
        Girar();
    }

    private void FixedUpdate()
    {
        Maquina.Atual.AtualizarFisica();
    }

    #endregion

    #region Utilidades usadas pelos estados

    public void Mover(float multiplicador)
    {
        Vector3 direcao = DirecaoRelativaACamera(InputMovimento);
        DefinirVelocidadeHorizontal(direcao * (velocidade * multiplicador));
    }

    public void DefinirVelocidadeHorizontal(Vector3 v)
    {
        Rb.linearVelocity = new Vector3(v.x, Rb.linearVelocity.y, v.z);
    }

    public void Pular()
    {
        Vector3 v = Rb.linearVelocity;
        Rb.linearVelocity = new Vector3(v.x, forcaDoPulo, v.z);
    }

    public void IniciarCooldownDash() => proximoDash = Time.time + dashCooldown;

    public bool EstaNoChao()
    {
        if (colliderDoPe == null) return false;

        Bounds b = colliderDoPe.bounds;
        float metade = distanciaChao * 0.5f;
        Vector3 centro = b.center + Vector3.down * metade;
        Vector3 extensao = b.extents + Vector3.up * metade;
        return Physics.CheckBox(centro, extensao, Quaternion.identity, terreno, QueryTriggerInteraction.Ignore);
    }

    public Vector3 DirecaoRelativaACamera(Vector2 input)
    {
        Vector3 frente = cam.forward;
        Vector3 direita = cam.right;
        frente.y = 0;
        direita.y = 0;
        frente.Normalize();
        direita.Normalize();

        return (direita * input.x + frente * input.y).normalized;
    }

    #endregion

    private void Girar()
    {
        Vector3 direcaoCamera = cam.forward;
        direcaoCamera.y = 0;

        if (direcaoCamera != Vector3.zero)
        {
            Quaternion rotacao = Quaternion.LookRotation(direcaoCamera);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacao, Time.deltaTime * 10f);
        }
    }
}
