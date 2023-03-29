using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<Todo> _data;

        public ToDoServiceTest()
        {
            _mock = new Mock<ITodoRepository>();

            _data = new List<Todo>
            {
                new() {Id = 1, Description = "Title 001", CreatedDate = DateTime.Now},
                new() {Id = 2, Description = "Title 002", CreatedDate = DateTime.Now},
                new() {Id = 3, Description = "Title 003", CreatedDate = DateTime.Now}
            };

            _mock.Setup(x => x.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int x) => _data.First(d => d.Id == x));

            _mock.Setup(_ => _.GetAsync())
                .ReturnsAsync(_data);

            _service = new ToDoService(_mock.Object);
        }

        [Fact]
        public void Get_正常系()
        {
            // Arrange・Act
            var result = _service.Get(1);

            // Assert
            _mock.Verify(_ => _.FindAsync(It.Is<int>(i => i == 1)), Times.Once());

            result.IsStructuralEqual(_data.First(x => x.Id == 1));
        }

        [Fact]
        public void GetList_正常系()
        {
            // Arrange・Act
            var result = _service.GetList();

            // Assert
            _mock.Verify(_ => _.GetAsync(), Times.Once());

            result.IsStructuralEqual(_data);
        }

        // TODO 別テストケースの作成
    }
}
