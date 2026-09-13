using UnityEngine;

public class Explosao : MonoBehaviour
{
    public float forcaExplosao = 10f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terreno"))
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cubos"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direcaoExplosao = other.transform.position - transform.position;
                rb.AddForce(direcaoExplosao.normalized * forcaExplosao, ForceMode.Impulse);
            }
        }
    }
}
