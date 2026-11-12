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
    public Vector3 posicaoNaTela = new Vector3(0.4f, -0.35f, 0.81f);
    [Header("Balanceamento")]
    public float dano;
    public int cadencia;
    public bool automatica;
    public int municao;
    [Header("SFX")]
    public AudioClip[] atirarSFX;
    public AudioClip[] recarregarSFX;
}