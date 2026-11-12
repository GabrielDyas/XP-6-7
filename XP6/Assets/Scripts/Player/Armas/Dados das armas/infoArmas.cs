using UnityEngine;

[CreateAssetMenu(
    fileName = "NovaArma",
    menuName = "Criar nova arma"
)]
public class infoArmas : ScriptableObject
{
    [Header("Geral")]
    public string nome;
    [Header("Modelo")]
    public GameObject modeloArma; 
    public Vector3 posicaoNaTela = Vector3.zero; 
    [Header("Balanceamento")]
    public float dano;
    public int cadencia;
    public bool automatica;
    public int municao;
}