using UnityEngine;
using UnityEngine.AI;

public class IA_inimigo : MonoBehaviour
{
    [SerializeField] public Transform objetivo;
    private NavMeshAgent inimigo;

    void Start()
    {
        inimigo = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        inimigo.destination = objetivo.position;
    }
}