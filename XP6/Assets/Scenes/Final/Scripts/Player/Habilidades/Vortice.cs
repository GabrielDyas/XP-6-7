using UnityEngine;

public class Vortice : MonoBehaviour
{
    [SerializeField] private float velocidadeDeAtracao = 1f;
    [SerializeField] private float velocidadeDeRotacao = 100f;
    [SerializeField] private float duracao = 5f;

    private void Start() => Destroy(gameObject, duracao);

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Inimigo")) return;

        Vector3 direcao = (transform.position - other.transform.position).normalized;
        other.transform.position += direcao * velocidadeDeAtracao * Time.deltaTime;
        other.transform.RotateAround(transform.position, Vector3.up, velocidadeDeRotacao * Time.deltaTime);
    }
}
