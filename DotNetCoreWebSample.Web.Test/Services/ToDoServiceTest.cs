using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChainingAssertion;
using DotNetCoreWebSample.Web.Models;
using DotNetCoreWebSample.Web.Repositories;
using DotNetCoreWebSample.Web.Services;
using Moq;
using Xunit;

namespace DotNetCoreWebSample.Web.Test.Services
{
    public class ToDoServiceTest
    {
        private readonly Mock<ITodoRepository> _mock;
        private readonly ToDoService _service;

        public ToDoServiceTest()
        {
            _mock = new Mock<ITodoRepository>();

            var data = new List<Todo>
            {
                new Todo {Id = 1, Description = "Title 001", CreatedDate = DateTime.Now},
                new Todo {Id = 2, Description = "Title 002", CreatedDate = DateTime.Now},
                new Todo {Id = 3, Description = "Title 003", CreatedDate = DateTime.Now}
            };

            _mock.Setup(_ => _.FindAsync(It.IsAny<int>()))
                .Returns<object>(t => Task.FromResult(data.First(d => d.Id == (int)t)));

            _service = new ToDoService(_mock.Object);
        }

        [Fact]
        public void Get_正常系()
        {
            // Arrange・Act
            var result = _service.Get(1);

            // Assert
            _mock.Verify(_ => _.FindAsync(It.Is<int>(i => i == 1)), Times.Once());
            result.Id.Is(1);
        }

        // TODO 別テストケースの作成
    }
}
