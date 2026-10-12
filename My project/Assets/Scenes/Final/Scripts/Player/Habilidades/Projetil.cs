using UnityEngine;

public class Projetil : MonoBehaviour
{
    public GameObject efeito;

    public void Start()
    {
        Collider collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terreno"))
        {
            if (efeito != null)
            {
                Instantiate(efeito, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }   
    }
}
