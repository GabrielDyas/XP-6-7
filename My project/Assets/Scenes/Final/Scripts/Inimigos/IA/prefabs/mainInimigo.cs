using UnityEngine;

public class mainInimigo : MonoBehaviour
{
    public float vida = 10f;
    public void tomarDano(float dano) {
        vida -= dano;       
    }
    private void FixedUpdate()
    {
        if (vida <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
