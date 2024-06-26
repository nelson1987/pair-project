namespace Athena.Domain;

//Classe de Mapeamento do Banco de Dados
public class Funcionario
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Funcao { get; set; }
    public DateTime Admissao { get; set; }
}