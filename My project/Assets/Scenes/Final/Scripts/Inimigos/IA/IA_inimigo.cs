using UnityEngine;
using UnityEngine.AI;

public class IA_inimigo : MonoBehaviour
{
    [SerializeField] public Transform objetivo;
    private NavMeshAgent inimigo;

    void Start()
    {
        inimigo = GetComponent<NavMeshAgent>();

        GameObject player = GameObject.FindWithTag("Player");
        objetivo = player.transform;
    }
    private void Update()
    {
        inimigo.destination = objetivo.position;
    }
}