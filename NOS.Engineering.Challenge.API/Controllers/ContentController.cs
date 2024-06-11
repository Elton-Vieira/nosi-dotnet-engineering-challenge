using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Importei o namespace ILogger
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;

namespace NOS.Engineering.Challenge.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContentController : Controller
    {
        private readonly IContentsManager _manager;
        private readonly ILogger<ContentController> _logger; // Declarei ILogger para a classe ContentController

        public ContentController(IContentsManager manager, ILogger<ContentController> logger) // Injetei ILogger no construtor
        {
            _manager = manager;
            _logger = logger; // Atribui o ILogger ao campo _logger
        }

        // Comentei o codigo como é para descontinuar;
/*
        [HttpGet]
        public async Task<IActionResult> GetManyContents()
        {
            try
            {
                var contents = await _manager.GetManyContents().ConfigureAwait(false);
                if (!contents.Any())
                    return NotFound();
                
                return Ok(contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter conteúdos"); // Registre o erro utilizando o ILogger
                return StatusCode((int)HttpStatusCode.InternalServerError, "Erro ao obter conteúdos");
            }
        }
*/
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContent(Guid id)
        {
            try
            {
                var content = await _manager.GetContent(id).ConfigureAwait(false);
                if (content == null)
                    return NotFound();
                
                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter conteúdo com ID {ContentId}", id); // Registei o erro utilizando o ILogger
                return StatusCode((int)HttpStatusCode.InternalServerError, "Erro ao obter conteúdo");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] ContentInput content)
        {
            try
            {
                var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);
                return createdContent == null ? Problem() : Ok(createdContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conteúdo"); // Registei o erro utilizando o ILogger
                return StatusCode((int)HttpStatusCode.InternalServerError, "Erro ao criar conteúdo");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateContent(Guid id, [FromBody] ContentInput content)
        {
            try
            {
                var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);
                return updatedContent == null ? NotFound() : Ok(updatedContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar conteúdo com ID {ContentId}", id); // Registei o erro utilizando o ILogger
                return StatusCode((int)HttpStatusCode.InternalServerError, "Erro ao atualizar conteúdo");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(Guid id)
        {
            try
            {
                var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
                return Ok(deletedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir conteúdo com ID {ContentId}", id); // Registei o erro utilizando o ILogger
                return StatusCode((int)HttpStatusCode.InternalServerError, "Erro ao excluir conteúdo");
            }
        }
        
        [HttpPost("{id}/genre")]
        public async Task<IActionResult> AddGenres(Guid id, [FromBody] IEnumerable<string> genres)
        {
            if (genres == null || !genres.Any())
            {
                return BadRequest("Lista de gêneros não pode estar vazia.");
            }

            try
            {
                var content = await _manager.AddGenres(id, genres).ConfigureAwait(false);
                if (content == null)
                {
                    return NotFound();
                }

                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar gêneros ao conteúdo com ID {ContentId}", id); // Registei o erro utilizando o ILogger
                return StatusCode((int)HttpStatusCode.InternalServerError, "Erro ao adicionar gêneros ao conteúdo");
            }
        }
 
        [HttpDelete("{id}/genre")]
        public async Task<IActionResult> RemoveGenres(Guid id, [FromBody] IEnumerable<string> genres)
        {
            if (genres == null || !genres.Any())
            {
                return BadRequest("Lista de gêneros não pode estar vazia.");
            }

            try
            {
                var content = await _manager.RemoveGenres(id, genres).ConfigureAwait(false);
                if (content == null)
                {
                    return NotFound();
                }

                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover gêneros do conteúdo com ID {ContentId}", id); // Registei o erro utilizando o ILogger
                return StatusCode((int)HttpStatusCode.InternalServerError, "Erro ao remover gêneros do conteúdo");
            }
        }
        // Novo método para filtrar o conteúdo por título e/ou gênero
        [HttpGet("filter")]
        public async Task<IActionResult> FilterContents([FromQuery] string title, [FromQuery] string genre)
        {
            try
            {
                IEnumerable<Content?> contents;

                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(genre))
                {
                    // Filtrar por título e gênero
                    contents = await _manager.GetManyContents()
                        .Where(c => c.Title?.Contains(title, StringComparison.OrdinalIgnoreCase) == true &&
                                    c.Genres?.Contains(genre, StringComparer.OrdinalIgnoreCase) == true)
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                else if (!string.IsNullOrEmpty(title))
                {
                    // Filtrar apenas por título
                    contents = await _manager.GetManyContents()
                        .Where(c => c.Title?.Contains(title, StringComparison.OrdinalIgnoreCase) == true)
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                else if (!string.IsNullOrEmpty(genre))
                {
                    // Filtrar apenas por gênero
                    contents = await _manager.GetManyContents()
                        .Where(c => c.Genres?.Contains(genre, StringComparer.OrdinalIgnoreCase) == true)
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    return BadRequest("É necessário fornecer pelo menos um parâmetro de filtro.");
                }

                return Ok(contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao filtrar o conteúdo.");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Ocorreu um erro ao filtrar o conteúdo: {ex.Message}");
            }
        }
    }
}
