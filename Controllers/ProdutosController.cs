using Microsoft.AspNetCore.Mvc;
using estudo_api.Models;
using estudo_api.Data;
using System.Linq;
using System;

namespace estudo_api.Controllers
{
    [Route("api/v1/[controller]")] // Necessário para definir a rota, não é automático como em MVC
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
            if (pTemp.Preco <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O preço do produto não pode ser menor ou igual a 0"});
            }

            if (pTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O nome do produto precisa ter mais de um caracter"});
            }

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

        [HttpPatch]
        public IActionResult Patch([FromBody] Produto produto)
        {
            if(produto.Id > 0)
            {
                try
                {
                    var p = database.Produtos.First(p => p.Id == produto.Id);

                    if(p != null)
                    {
                        p.Nome = produto.Nome != null ? produto.Nome : p.Nome; // condicao ? faz algo : faz outra coisa
                        p.Preco = produto.Preco != 0 ? produto.Preco : p.Preco;
                        database.SaveChanges();

                        return Ok();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Produto não encontrado");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Produto não encontrado");
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id do produto é inválido"});
            }
        }

        public class ProdutoTemp {
            public string Nome { get; set; }
            public float Preco { get; set; }
        }
    }
}