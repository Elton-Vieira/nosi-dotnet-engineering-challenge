using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;
 //Alterado por Elton Cabral 
public interface IContentsManager
{
    Task<IEnumerable<Content?>> GetManyContents();
    Task<Content?> CreateContent(ContentDto content);
    Task<Content?> GetContent(Guid id);
    Task<Content?> UpdateContent(Guid id, ContentDto content);
    Task<Guid> DeleteContent(Guid id);
     // Criado Por Elton Cabral
    // Adiciona gêneros a um conteúdo existente com o ID fornecido.
    Task<Content?> AddGenres(Guid id, IEnumerable<string> genres);
    // Criado Por Elton Cabral
    // Remove gêneros de um conteúdo existente com o ID fornecido.
    Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres);
}