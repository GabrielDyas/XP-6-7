public abstract class EstadoBase
{
    protected readonly Movimento jogador;
    protected readonly MaquinaDeEstados maquina;

    protected InputSystem_Actions.PlayerActions Entrada => jogador.Controles.Player;

    protected EstadoBase(Movimento jogador, MaquinaDeEstados maquina)
    {
        this.jogador = jogador;
        this.maquina = maquina;
    }

    public virtual void Entrar() { }
    public virtual void Sair() { }

    public virtual void AtualizarLogica() { }

    // Chamado no FixedUpdate: mexe na física (Rigidbody).
    public virtual void AtualizarFisica() { }
}

public class MaquinaDeEstados
{
    public EstadoBase Atual { get; private set; }

    public void Iniciar(EstadoBase estadoInicial)
    {
        Atual = estadoInicial;
        Atual.Entrar();
    }

    public void Trocar(EstadoBase novoEstado)
    {
        if (novoEstado == Atual) return;

        Atual?.Sair();
        Atual = novoEstado;
        Atual.Entrar();
    }
}
