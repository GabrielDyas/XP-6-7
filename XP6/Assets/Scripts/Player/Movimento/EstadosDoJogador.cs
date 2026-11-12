using UnityEngine;

public class EstadoAndando : EstadoBase
{
    public EstadoAndando(Movimento j, MaquinaDeEstados m) : base(j, m) { }

    public override void AtualizarLogica()
    {
        if (!jogador.EstaNoChao())            { maquina.Trocar(jogador.Caindo);   return; }
        if (Entrada.Jump.WasPressedThisFrame()) { maquina.Trocar(jogador.Pulo);     return; }
        if (Entrada.Crouch.WasPressedThisFrame()) { maquina.Trocar(jogador.Agachado); return; }
        if (jogador.QuerCorrer)               { maquina.Trocar(jogador.Correndo); return; }
        if (jogador.QuerDash)                 { maquina.Trocar(jogador.Dash);     return; }
    }

    public override void AtualizarFisica() => jogador.Mover(1f);
}


public class EstadoCorrendo : EstadoBase
{
    public EstadoCorrendo(Movimento j, MaquinaDeEstados m) : base(j, m) { }

    public override void AtualizarLogica()
    {
        if (!jogador.EstaNoChao())              { maquina.Trocar(jogador.Caindo); return; }
        if (Entrada.Jump.WasPressedThisFrame()) { maquina.Trocar(jogador.Pulo);   return; }
        if (jogador.QuerDashCorrendo)           { maquina.Trocar(jogador.Dash);   return; }


        if (jogador.InputMovimento.sqrMagnitude < 0.01f)
        {
            maquina.Trocar(jogador.Andando);
            return;
        }
    }

    public override void AtualizarFisica() => jogador.Mover(jogador.MultiplicadorCorrida);
}

public class EstadoAgachado : EstadoBase
{
    public EstadoAgachado(Movimento j, MaquinaDeEstados m) : base(j, m) { }


    public override void AtualizarLogica()
    {
        if (!jogador.EstaNoChao())              { maquina.Trocar(jogador.Caindo);   return; }
        if (Entrada.Jump.WasPressedThisFrame()) { maquina.Trocar(jogador.Pulo);     return; }
        if (!Entrada.Crouch.IsPressed())        { maquina.Trocar(jogador.Andando);  return; } 
        if (jogador.QuerCorrer)                 { maquina.Trocar(jogador.Correndo); return; }
        if (jogador.QuerDash)                   { maquina.Trocar(jogador.Dash);     return; } 
    }

    public override void AtualizarFisica() => jogador.Mover(jogador.MultiplicadorAgachado);
}

public class EstadoDash : EstadoBase
{
    private Vector3 direcao;
    private float tempo;

    public EstadoDash(Movimento j, MaquinaDeEstados m) : base(j, m) { }

    public override void Entrar()
    {
        tempo = 0f;
        direcao = jogador.DirecaoRelativaACamera(jogador.InputMovimento);
        if (direcao == Vector3.zero) direcao = jogador.transform.forward;
        jogador.IniciarCooldownDash();
    }

    public override void AtualizarLogica()
    {
        if (Entrada.Jump.WasPressedThisFrame()) { maquina.Trocar(jogador.Pulo); return; }

        tempo += Time.deltaTime;
        if (tempo >= jogador.DashDuracao)       { maquina.Trocar(jogador.Caindo); return; }
    }

    public override void AtualizarFisica() =>
        jogador.DefinirVelocidadeHorizontal(direcao * jogador.DashForce);

    public override void Sair() =>
        jogador.DefinirVelocidadeHorizontal(direcao * jogador.Velocidade);
}


public class EstadoPulo : EstadoBase
{
    public EstadoPulo(Movimento j, MaquinaDeEstados m) : base(j, m) { }

    public override void Entrar()
    {
        jogador.Pular();
        maquina.Trocar(jogador.Caindo);
    }
}


public class EstadoCaindo : EstadoBase
{
    public EstadoCaindo(Movimento j, MaquinaDeEstados m) : base(j, m) { }

    public override void AtualizarLogica()
    {
        if (Entrada.Jump.WasPressedThisFrame() && !jogador.PuloNoArUsado)
        {
            jogador.PuloNoArUsado = true;
            maquina.Trocar(jogador.Pulo);
            return;
        }

        if (jogador.EstaNoChao() && jogador.Rb.linearVelocity.y <= 0.1f)
        {
            jogador.PuloNoArUsado = false;
            maquina.Trocar(jogador.Andando);
            return;
        }
    }
}
