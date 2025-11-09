using ChainingAssertion;
using DotNetCoreWebSample.Web.Models;
using DotNetCoreWebSample.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotNetCoreWebSample.Web.Test.Repositories
{
    public class TodoRepositoryTest
    {
        private readonly TodoRepository _repository;
        private readonly IQueryable<Todo> _mockData;
        private readonly Mock<DbSet<Todo>> _mockDbSet;
        private readonly Mock<DotnetCoreWebSampleContext> _mockContext;

        public TodoRepositoryTest()
        {
            IList<Todo> dataList = new List<Todo>
            {
                new Todo { Id = 1, Description = "Title 001", CreatedDate = DateTime.Now },
                new Todo { Id = 2, Description = "Title 002", CreatedDate = DateTime.Now },
                new Todo { Id = 3, Description = "Title 003", CreatedDate = DateTime.Now }
            };

            _mockData = dataList.AsQueryable();

            _mockDbSet = new Mock<DbSet<Todo>>();
            
            // IQueryable のセットアップ
            _mockDbSet.As<IQueryable<Todo>>().Setup(m => m.Provider).Returns(new AsyncQueryProvider<Todo>(_mockData.Provider));
            _mockDbSet.As<IQueryable<Todo>>().Setup(m => m.Expression).Returns(_mockData.Expression);
            _mockDbSet.As<IQueryable<Todo>>().Setup(m => m.ElementType).Returns(_mockData.ElementType);
            _mockDbSet.As<IQueryable<Todo>>().Setup(m => m.GetEnumerator()).Returns(() => _mockData.GetEnumerator());
            _mockDbSet.As<IAsyncEnumerable<Todo>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => new AsyncEnumerator<Todo>(_mockData.GetEnumerator()));

            // Find のモック設定
            _mockDbSet.Setup(x => x.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => _mockData.FirstOrDefault(x => x.Id == (int)ids[0]));

            // FindAsync のモック設定
            _mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => _mockData.FirstOrDefault(x => x.Id == (int)ids[0]));

            _mockContext = new Mock<DotnetCoreWebSampleContext>();
            _mockContext.Setup(x => x.Todo).Returns(_mockDbSet.Object);
            _mockContext.Setup(x => x.Set<Todo>()).Returns(_mockDbSet.Object);
            _mockContext.Setup(x => x.SaveChanges()).Returns(1);
            _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _repository = new TodoRepository(_mockContext.Object);
        }

        // IAsyncQueryProvider の実装
        private class AsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
        {
            public IQueryable CreateQuery(Expression expression)
            {
                return new AsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new AsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return inner.Execute<TResult>(expression);
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new AsyncEnumerable<TResult>(expression);
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
#pragma warning restore CS1998
            {
                Type resultType = typeof(TResult);
                if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    resultType = resultType.GetGenericArguments()[0];
                    object result = Execute(expression);
                    return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                        ?.MakeGenericMethod(resultType)
                        .Invoke(null, new[] { result });
                }

                return Execute<TResult>(expression);
            }
        }

        // IAsyncEnumerable の実装
        private class AsyncEnumerable<T>(Expression expression) : EnumerableQuery<T>(expression), IAsyncEnumerable<T>, IQueryable<T>
        {

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
        }

        // IAsyncEnumerator の実装
        private class AsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
        {
            public T Current => inner.Current;

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(inner.MoveNext());
            }

            public ValueTask DisposeAsync()
            {
                inner.Dispose();
                return new ValueTask();
            }
        }

        [Fact]
        public void Get_正常系()
        {
            // Arrange・Act
            IList<Todo> result = _repository.Get();

            // Assert
            result.Count.Is(3);

            IList<Todo> expect = _mockData.ToList();
            foreach (Todo r in result)
            {
                expect.First(x => x.Id == r.Id).IsStructuralEqual(r);
            }
        }

        [Fact]
        public void Get_条件有_正常系()
        {
            // Arrange
            const int id = 1;
            Expression<Func<Todo, bool>> condition = t => t.Id == id;

            // Act
            IList<Todo> result = _repository.Get(condition);

            // Assert
            result.Count.Is(1);
            result.First().IsStructuralEqual(_mockData.First(x => x.Id == id));
        }

        [Fact]
        public void Add_正常系()
        {
            // Arrange
            Todo expect = new Todo { Id = 4, Description = "Title 004", CreatedDate = DateTime.Now };

            // Act
            _repository.Add(expect);

            // Assert
            _mockDbSet.Verify(x => x.Add(It.Is<Todo>(t => t.Id == expect.Id
                                                            && t.Description == expect.Description
                                                            && t.CreatedDate == expect.CreatedDate)), Times.Once);
        }

        [Fact]
        public void Find_正常系()
        {
            // Arrange
            const int id = 1;

            // Act
            Todo result = _repository.Find(id);

            // Assert
            result.IsStructuralEqual(_mockData.First(x => x.Id == id));
        }

        [Fact]
        public void Find_存在しないID_Nullを返す()
        {
            // Arrange
            const int notExistsId = 999;

            // Act
            Todo result = _repository.Find(notExistsId);

            // Assert
            result.IsNull();
        }

        [Fact]
        public async Task FindAsync_正常系_レコードが取得される()
        {
            // Arrange
            const int id = 1;

            // Act
            Todo result = await _repository.FindAsync(id);

            // Assert
            result.IsStructuralEqual(_mockData.First(x => x.Id == id));
            _mockDbSet.Verify(x => x.FindAsync(It.Is<object[]>(arr => arr.Length == 1 && (int)arr[0] == id)), Times.Once);
        }

        [Fact]
        public async Task FindAsync_存在しないID_Nullを返す()
        {
            // Arrange
            const int notExistsId = 999;

            // Act
            Todo result = await _repository.FindAsync(notExistsId);

            // Assert
            result.IsNull();
            _mockDbSet.Verify(x => x.FindAsync(It.Is<object[]>(arr => arr.Length == 1 && (int)arr[0] == notExistsId)), Times.Once);
        }

        [Fact]
        public void Update_正常系_レコードが更新される()
        {
            // Arrange
            Todo todo = new Todo { Id = 1, Description = "Updated Title 001", CreatedDate = DateTime.Now };

            // Act
            _repository.Update(todo);

            // Assert
            _mockDbSet.Verify(x => x.Update(It.Is<Todo>(t => t.Id == todo.Id 
                && t.Description == todo.Description)), Times.Once);
        }

        [Fact]
        public void Remove_正常系_レコードが削除される()
        {
            // Arrange
            Todo todo = new Todo { Id = 1, Description = "Title 001", CreatedDate = DateTime.Now };

            // Act
            _repository.Remove(todo);

            // Assert
            _mockDbSet.Verify(x => x.Remove(It.Is<Todo>(t => t.Id == todo.Id 
                && t.Description == todo.Description)), Times.Once);
        }

        [Fact]
        public void Save_正常系_変更が保存される()
        {
            // Act
            _repository.Save();

            // Assert
            _mockContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_正常系_変更が保存される()
        {
            // Act
            int result = await _repository.SaveAsync();

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            result.Is(1);
        }

        [Fact]
        public async Task SaveAsync_正常系_影響を受けた行数が返される()
        {
            // Arrange
            _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);

            // Act
            int result = await _repository.SaveAsync();

            // Assert
            result.Is(2);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_正常系_全件取得される()
        {
            // Act
            IList<Todo> result = await _repository.GetAsync();

            // Assert
            result.Count.Is(3);
            IList<Todo> expect = _mockData.ToList();
            foreach (Todo r in result)
            {
                expect.First(x => x.Id == r.Id).IsStructuralEqual(r);
            }
        }

        [Fact]
        public async Task GetAsync_条件有_正常系_条件に一致するレコードが取得される()
        {
            // Arrange
            const int id = 1;
            Expression<Func<Todo, bool>> condition = t => t.Id == id;

            // Act
            IList<Todo> result = await _repository.GetAsync(condition);

            // Assert
            result.Count.Is(1);
            result.First().IsStructuralEqual(_mockData.First(x => x.Id == id));
        }

        [Fact]
        public async Task GetAsync_条件有_一致なし_空のリストを返す()
        {
            // Arrange
            const int id = 999;
            Expression<Func<Todo, bool>> condition = t => t.Id == id;

            // Act
            IList<Todo> result = await _repository.GetAsync(condition);

            // Assert
            result.Count.Is(0);
        }

        [Fact]
        public void GetCount_正常系_レコード数が返される()
        {
            // Act
            int result = _repository.GetCount();

            // Assert
            result.Is(3);
        }

        [Fact]
        public async Task GetCountAsync_正常系_レコード数が返される()
        {
            // Act
            int result = await _repository.GetCountAsync();

            // Assert
            result.Is(3);
        }

    }
}
