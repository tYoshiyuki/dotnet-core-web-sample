using DotNetCoreWebSample.Web.Models;

namespace DotNetCoreWebSample.Web.Repositories
{
    public interface ITodoRepository : IRepository<Todo>
    {

    }

    public class TodoRepository : Repository<Todo>, ITodoRepository
    {
        public TodoRepository(DotnetCoreWebSampleContext db) : base(db) { }
    }
}
