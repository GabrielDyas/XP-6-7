using UnityEngine;

[CreateAssetMenu(fileName = "NovaArma", menuName = "Criar nova arma")]
public class listaDeArmas : ScriptableObject
{
    [Header("Informações")]
    public string nome;
    public bool automatica;
    public int cadencia;
    public int municao;
    public float dano;
}
