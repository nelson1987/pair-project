using Athena.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FuncionariosController : ControllerBase
{
    private readonly ILogger<FuncionariosController> _logger;

    public FuncionariosController(ILogger<FuncionariosController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Read()
    {
        List<Funcionario> funcionario = new List<Funcionario>();
        return Ok(funcionario);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Funcionario funcionario)
    {
        return Ok(funcionario);
    }

    [HttpPatch("{id}")]
    public IActionResult Update([FromBody] Funcionario funcionario)
    {
        funcionario.Id = id;
        return Ok(funcionario);
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        return Ok();
    }
}