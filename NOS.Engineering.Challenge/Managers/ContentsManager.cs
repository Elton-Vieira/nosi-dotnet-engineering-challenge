using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    //Alterado por Elton Cabral 
    private readonly IDatabase<Content?, ContentDto> _database;

    public ContentsManager(IDatabase<Content?, ContentDto> database)
    {
        _database = database;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        return _database.ReadAll();
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        return _database.Create(content);
    }

    public Task<Content?> GetContent(Guid id)
    {
        return _database.Read(id);
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        return _database.Update(id, content);
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        return _database.Delete(id);
    }
    // Criado Por Elton Cabral
    // Adiciona gêneros a um conteúdo existente com o ID fornecido.
     public Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
    {
        var content = await _database.Read(id);
        if (content == null)
        {
            return null;
        }

        foreach (var genre in genres)
        {
            if (!content.Genres.Contains(genre, StringComparer.OrdinalIgnoreCase))
            {
                content.Genres.Add(genre);
            }
        }

        return _database.Update(id, new ContentDto
        {
            Title = content.Title,
            SubTitle = content.SubTitle,
            Description = content.Description,
            ImageUrl = content.ImageUrl,
            Duration = content.Duration,
            StartTime = content.StartTime,
            EndTime = content.EndTime,
            GenreList = content.Genres
        });
      
    }
    // Criado Por Elton Cabral
    // Remove gêneros de um conteúdo existente com o ID fornecido.
      public  Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
    {
        var content =  _database.Read(id);
        if (content == null)
        {
            return null;
        }

        foreach (var genre in genres)
        {
            if (content.Genres.Contains(genre, StringComparer.OrdinalIgnoreCase))
            {
                content.Genres.Remove(genre);
            }
        }

          return _database.Update(id, new ContentDto
        {
            Title = content.Title,
            SubTitle = content.SubTitle,
            Description = content.Description,
            ImageUrl = content.ImageUrl,
            Duration = content.Duration,
            StartTime = content.StartTime,
            EndTime = content.EndTime,
            GenreList = content.Genres
        });
     
    }
}