using Athena.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace Athena.Tests;

//Calculadora
public class CadastroFuncionarioUnitTests
{
    private readonly CadastroFuncionario _sut;

    public CadastroFuncionarioUnitTests()
    {
        _sut = new CadastroFuncionario();
    }

    [Fact]
    public void Dado_Cadastro_Funcionario_Corretamente_Dados_Funcionario_Cadastrado_Igual_Dados_Funcionario()
    {
        string expectedNome = "Juan Topini";
        string expectedFuncao = "Desenvolvedor";
        var funcionario = new Funcionario() { Nome = expectedNome, Funcao = expectedFuncao };

        // Act - Executação do código a ser testado
        var funcionarioCadastrado = _sut.Cadastrar(funcionario);

        // Assert - Verificação do resultado do código testado
        Assert.Equal(expectedNome, funcionarioCadastrado.Result.Nome);
        Assert.Equal(expectedFuncao, funcionarioCadastrado.Result.Funcao);
    }

    [Fact]
    public void Dado_Funcionario_Sem_Nome_Retorna_Excecao()
    {
        //AAA
        // Arrange - Preparação do código para teste
        var funcionario = new Funcionario();

        // Act - Executação do código a ser testado
        var funcionarioCadastrado = _sut.Cadastrar(funcionario);

        // Assert - Verificação do resultado do código testado
        Assert.Equal("'Nome' must not be empty.", funcionarioCadastrado.ErrorMessage);
    }

    [Fact]
    public void Dado_Funcionario_Sem_Funcao_Obrigatorio_Ter_Nome_Retorna_Excecao()
    {
        //AAA
        // Arrange - Preparação do código para teste
        var funcionario = new Funcionario() { Nome = "Qualquer Nome" };

        // Act - Executação do código a ser testado
        var funcionarioCadastrado = _sut.Cadastrar(funcionario);

        // Assert - Verificação do resultado do código testado
        Assert.Equal("'Funcao' must not be empty.", funcionarioCadastrado.ErrorMessage);
    }

    [Fact]
    public void Dado_Cadastro_Funcionario_Executado_Em_Duplicidade_Deve_Retornar_Erro()
    {
        //AAA
        // Arrange - Preparação do código para teste
        string expectedNome = "Juan Topini";
        string expectedFuncao = "Desenvolvedor";
        var funcionario = new Funcionario() { Nome = expectedNome, Funcao = expectedFuncao };

        // Act - Executação do código a ser testado
        var funcionarioCadastrado = _sut.Cadastrar(funcionario);
        var funcionarioCadastradoDuplicidade = _sut.Cadastrar(funcionario);

        // Assert - Verificação do resultado do código testado
        Assert.Equal("Funcionario ja cadastrado", funcionarioCadastradoDuplicidade.ErrorMessage);
    }
}

public class FuncionarioValidation : AbstractValidator<Funcionario>
{
    public FuncionarioValidation()
    {
        RuleFor(x => x.Nome).NotEmpty();
        RuleFor(x => x.Funcao).NotEmpty();
    }
}

public class Resultado<TResult> where TResult : class
{
    public static Resultado<TResult> ResultadoValido(TResult resultado)
    {
        return new Resultado<TResult>() { IsValid = true, Result = resultado };
    }

    public static Resultado<TResult> Erro(string mensagem)
    {
        return new Resultado<TResult>() { IsValid = false, ErrorMessage = mensagem };
    }

    public static Resultado<TResult> Erro(Exception exception)
    {
        return new Resultado<TResult>() { IsValid = false, ErrorMessage = exception.Message };
    }

    public TResult Result { get; set; }
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; }
}

public class CadastroFuncionario
{
    private readonly BaseDados baseDados;
    private readonly IValidator<Funcionario> validator;

    public CadastroFuncionario()
    {
        baseDados = new BaseDados();
        validator = new FuncionarioValidation();
    }

    public Resultado<Funcionario> Cadastrar(Funcionario funcionario)
    {
        try
        {
            ValidationResult result = validator.Validate(funcionario);
            if (!result.IsValid)
                return Resultado<Funcionario>.Erro(result.Errors.FirstOrDefault()!.ErrorMessage);

            var funcionarioCadastrado = baseDados.Insert(funcionario);
            return funcionarioCadastrado;
        }
        catch (Exception ex)
        {
            return Resultado<Funcionario>.Erro(ex);
        }
    }
}

public class BaseDados
{
    private readonly List<Funcionario> _list;

    public BaseDados()
    {
        _list = new List<Funcionario>();
    }

    public Resultado<Funcionario> Insert(Funcionario funcionario)
    {
        if (_list.Any(f => f.Nome == funcionario.Nome))
            return Resultado<Funcionario>.Erro("Funcionario ja cadastrado");

        funcionario.Id = Guid.NewGuid();
        _list.Add(funcionario);
        return Resultado<Funcionario>.ResultadoValido(funcionario);
    }
}