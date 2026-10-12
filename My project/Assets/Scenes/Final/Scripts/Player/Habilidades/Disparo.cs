using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Disparo : MonoBehaviour
{

    public GameObject pontoDeDisparo;
    public GameObject projetil; 
    public float velocidadeDoProjetil = 10f;
    public InputActionAsset inputActions; // Referência ao InputActionAsset

    public GameObject habilidadeVorticePrefab; // Prefab do vórtice
    public GameObject habilidadeDisparoPrefab; // Prefab do disparo



    private void Start()
    {
        // Ativar as ações de entrada
        inputActions.Enable();
    }

    private void Update()
    {
        // Verificar se a ação de disparo foi realizada
        if (inputActions.FindAction("Q").triggered)
        {
            Disparar();
        }
        // Verificar se a ação de vórtice foi realizada
        if (inputActions.FindAction("E").triggered)
        {
            Vortice();
        }
    }

    private void Disparar()
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
