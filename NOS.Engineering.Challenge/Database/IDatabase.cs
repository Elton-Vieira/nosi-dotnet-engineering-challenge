namespace NOS.Engineering.Challenge.Database;

public interface IDatabase<TOut, in TIn>
{

    Task<TOut?> Create(TIn item);
        Task<TOut?> Read(Guid id);
        Task<IEnumerable<TOut?>> ReadAll();
        Task<TOut?> Update(Guid id, TIn item);
        Task<Guid> Delete(Guid id);
        Task<Content?> AddGenres(Guid id, IEnumerable<string> genres);
        Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres);

}