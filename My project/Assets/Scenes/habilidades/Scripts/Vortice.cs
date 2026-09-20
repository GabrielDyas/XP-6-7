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
            Vector3 direction = (transform.position - other.transform.position).normalized;
            other.transform.position += direction * velocidadeDeAtracao * Time.deltaTime;
            other.transform.RotateAround(transform.position, Vector3.up, velocidadeDeRotacao * Time.deltaTime);
        }
    }

    private IEnumerator Tempo()
    {
        yield return new WaitForSeconds(duracao);
        Destroy(gameObject);
    }


}
