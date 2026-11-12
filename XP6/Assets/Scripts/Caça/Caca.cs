using System;
using Unity.AppUI.UI;
using UnityEngine;

public class Caca : MonoBehaviour
{
    public static int valorAlvo = 100;
    [SerializeField] public static int valorAtual = 0;

    [SerializeField] private GameObject inimigoEspecial;
    [SerializeField] private GameObject pontoDeInvocacao;



    public static void AcumularPontos(int pontos)
    {
        valorAtual += pontos;
    }

    private void Update()
    {
        if (valorAtual >= valorAlvo)
        {
            Instantiate(inimigoEspecial, pontoDeInvocacao.transform.position, Quaternion.identity);
            valorAtual = 0; // Reset the score after spawning the special enemy
        }
    }
}
