using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using JetBrains.Annotations;
public class Disparo : MonoBehaviour
{

    public GameObject pontoDeDisparo;
    public GameObject projetil; 
    public float velocidadeDoProjetil = 10f;
    private InputSystem_Actions input;

    public GameObject habilidadeVorticePrefab; // Prefab do vórtice
    public GameObject habilidadeDisparoPrefab; // Prefab do disparo

    private void Awake()
    {

        // Inicializar o sistema de entrada
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Player.Habilidade1.performed += ctx => Explosao();
        input.Player.Habilidade2.performed += ctx => Vortice();
        // Ativar as ações de entrada
        input.Enable();
    }

    private void Update()
    {
        if(input.Player.Habilidade1.triggered)
        {
            Explosao();
        }
        if(input.Player.Habilidade2.triggered)
        {
            Vortice();
        }
    }

    private void Explosao()
    {

        // Instanciar o projetil na posição do ponto de disparo
        GameObject novoProjetil = Instantiate(projetil, pontoDeDisparo.transform.position, pontoDeDisparo.transform.rotation);
        novoProjetil.GetComponent<Projetil>().efeito = habilidadeDisparoPrefab; // Atribuir o efeito do disparo ao projetil
        Rigidbody rbProjetil = novoProjetil.GetComponent<Rigidbody>();
        // Aplicar força ao projetil para movê-lo para frente
        rbProjetil.linearVelocity = pontoDeDisparo.transform.forward * velocidadeDoProjetil;
    }

    private void Vortice()
    {
        // Instanciar o projetil na posição do ponto de disparo
        GameObject novoProjetil = Instantiate(projetil, pontoDeDisparo.transform.position, pontoDeDisparo.transform.rotation);
        novoProjetil.GetComponent<Projetil>().efeito = habilidadeVorticePrefab; // Atribuir o efeito do vórtice ao projetil
        Rigidbody rbProjetil = novoProjetil.GetComponent<Rigidbody>();
        // Aplicar força ao projetil para movê-lo para frente
        rbProjetil.linearVelocity = pontoDeDisparo.transform.forward * velocidadeDoProjetil;
    }

}
