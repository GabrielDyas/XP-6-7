
using UnityEngine;

public class musicaGlobal : MonoBehaviour
{
    [Header("Músicas")]
    [SerializeField] private AudioClip[] musicas = new AudioClip[3];

    private AudioSource audioSource;

    private static musicaGlobal instancia;

    private int indiceAtual = -1;

    private void Awake()
    {
        // Impede que existam dois gerenciadores de música.
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;

        // Mantém a música ao trocar de cena.
        // DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource não encontrado.", this);
            enabled = false;
            return;
        }

        // Configura o áudio como global (2D).
        audioSource.spatialBlend = 0f;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        TocarProximaMusica();
    }

    private void Update()
    {
        // Verifica se a música atual terminou.
        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            TocarProximaMusica();
        }
    }

    private void TocarProximaMusica()
    {
        // Procura a próxima música disponível.
        for (int i = 0; i < musicas.Length; i++)
        {
            indiceAtual++;

            // Volta para a primeira música ao chegar ao final.
            if (indiceAtual >= musicas.Length)
            {
                indiceAtual = 0;
            }

            if (musicas[indiceAtual] != null)
            {
                audioSource.clip = musicas[indiceAtual];
                audioSource.Play();

                return;
            }
        }

        Debug.LogWarning("Nenhuma música foi configurada.", this);

        audioSource.Stop();
        audioSource.clip = null;
    }
}
