namespace MrChip.MedLincePro.Business.Common;

public sealed class ResultadoOperacao
{
    public bool Sucesso { get; init; }
    public string Mensagem { get; init; } = string.Empty;

    public static ResultadoOperacao Ok(string mensagem = "Operação realizada com sucesso.") =>
        new() { Sucesso = true, Mensagem = mensagem };

    public static ResultadoOperacao Falha(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };
}

public sealed class ResultadoOperacao<T>
{
    public bool Sucesso { get; init; }
    public string Mensagem { get; init; } = string.Empty;
    public T? Dados { get; init; }

    public static ResultadoOperacao<T> Ok(T? dados, string mensagem = "Operação realizada com sucesso.") =>
        new() { Sucesso = true, Mensagem = mensagem, Dados = dados };

    public static ResultadoOperacao<T> Falha(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };
}
