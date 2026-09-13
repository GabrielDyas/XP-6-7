using UnityEngine;
using System.Collections;   

public class Vortice : MonoBehaviour
{
    public Collider vortice;
    public float velocidadeDeAtracao = 1f;
    public float velocidadeDeRotacao = 100f;
    public float duracao = 5f;


    private void Start()
    {
        StartCoroutine(Tempo());
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cubos"))
        {
            // Move o inimigo em direção ao centro do vórtice
            Vector3 direction = (transform.position - other.transform.position).normalized;
            other.transform.position += direction * velocidadeDeAtracao * Time.deltaTime;
            // Rotaciona o inimigo em torno do vórtice
            other.transform.RotateAround(transform.position, Vector3.up, velocidadeDeRotacao * Time.deltaTime);
        }
    }

    private IEnumerator Tempo()
    {
        yield return new WaitForSeconds(duracao);
        Destroy(gameObject);
    }


}
