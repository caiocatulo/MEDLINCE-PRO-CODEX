namespace MrChip.MedLincePro.Api.ViewModels;

public sealed class ApiResponse<T>
{
    public bool Sucesso { get; init; }
    public string Mensagem { get; init; } = string.Empty;
    public T? Dados { get; init; }

    public static ApiResponse<T> Ok(T? dados, string mensagem = "Operação realizada com sucesso.") =>
        new() { Sucesso = true, Mensagem = mensagem, Dados = dados };

    public static ApiResponse<T> Falha(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };
}

public sealed class ApiResponse
{
    public bool Sucesso { get; init; }
    public string Mensagem { get; init; } = string.Empty;

    public static ApiResponse Ok(string mensagem = "Operação realizada com sucesso.") =>
        new() { Sucesso = true, Mensagem = mensagem };

    public static ApiResponse Falha(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };
}
