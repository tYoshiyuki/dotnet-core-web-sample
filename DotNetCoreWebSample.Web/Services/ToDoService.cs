using DotNetCoreWebSample.Web.Models;
using DotNetCoreWebSample.Web.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreWebSample.Web.Services
{
    public interface IToDoService
    {
        Task<Todo> Get(int id);
        Task<IList<Todo>> GetList();
        Task<int> Create(Todo todo);
        Task<int> Edit(Todo todo);
        Task<int> Delete(Todo todo);
        bool Exists(int id);
    }

    public class ToDoService : IToDoService
    {
        private readonly ITodoRepository _repository;

        public ToDoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<Todo>> GetList()
        {
            return await _repository.GetAsync();
        }

        public async Task<Todo> Get(int id)
        {
            return await _repository.FindAsync(id);
        }

        public async Task<int> Create(Todo todo)
        {
            _repository.Add(todo);
            return await _repository.SaveAsync();
        }

        public async Task<int> Edit(Todo todo)
        {
            _repository.Update(todo);
            return await _repository.SaveAsync();
        }

        public async Task<int> Delete(Todo todo)
        {
            _repository.Remove(todo);
            return await _repository.SaveAsync();
        }

        public bool Exists(int id)
        {
            return _repository.Find(id) != null;
        }
    }
}
