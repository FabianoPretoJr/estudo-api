using Microsoft.AspNetCore.Mvc;
using estudo_api.Models;
using estudo_api.Data;
using System.Linq;
using System;

namespace estudo_api.Controllers
{
    [Route("api/[controller]")] // Necessário para definir a rota, não é automático como em MVC
    [ApiController] // Necessário adicionar pra indicar a API e fazer algumas ações por baixo dos panos
    public class ProdutosController : ControllerBase // Tem menos recursos que o Controller, como poder mexer com HTML, o que não é necessário em API
    {
        private readonly ApplicationDbContext database;
        public ProdutosController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet] // Importante sempre passar o verbo http
        public IActionResult Get()
        {
            var proJson = database.Produtos.ToList();

            return Ok(proJson);

            // return Ok(new {nome = "Fabiano Preto", empresa = "GFT"}); // Retorna status code 200 e dados dentro do ()
            // Há outros como esse, exemplo badRequest e NotFound
            // Com o new é criado um JSON para ser retornado
        }

        [HttpGet("{id}")] // Mapeando rota
        public IActionResult Get(int id)
        {
            try // Usar try catch para fazer a tratativa do erro, em caso de se passar um id inválido
            {
                Produto proJson = database.Produtos.First(p => p.Id == id);
                return Ok(proJson);
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]ProdutoTemp pTemp)
        {
            Produto p = new Produto();

            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;
            database.Produtos.Add(p);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult(""); // Fazendo assim dá pra passar o status code que vc quiser
            //return Ok(new { info = "Você criou um novo produto", produto = pTemp}); // Aqui passa o objeto e o status code 200
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Produto produto = database.Produtos.First(p => p.Id == id);
                database.Produtos.Remove(produto);
                database.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }

        public class ProdutoTemp {
            public string Nome { get; set; }
            public float Preco { get; set; }
        }
    }
}