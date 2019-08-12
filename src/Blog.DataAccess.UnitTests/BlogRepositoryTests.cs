using Blog.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Blog.DataAccess.UnitTests
{
    public class BlogRepositoryTests
    {
        [Fact]
        public void BlogRepositoryCtor_NullContext_Throws()
        {
            BlogRepository sut = null;
            Action act = () => sut = new BlogRepository(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReturnAllBlogObjects_Mock_DbContext()
        {
            var builder = new DbContextOptionsBuilder();
            var options = builder.Options;
            var blogContextMock = new Mock<BlogContext>(options);
            var blogs = new List<DataAccess.Blog>()
            {
                new Blog(),
                new Blog(),
            };
            var dbSetMock = new DbQueryMock<Blog>(blogs);

            blogContextMock
                .Setup(b => b.Blogs)
                .Returns(dbSetMock.Object);

            BlogRepository sut = new BlogRepository(blogContextMock.Object);
            var blogEntries = sut.GetAllBlogEntries();
            blogEntries.Should().HaveCount(2);
        }

        [Fact]
        public void InMemory_Repository_Integration()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName: "repositoryDb")
                .Options;

            using (var context = new BlogContext(options))
            {
                var blogRepository = new BlogRepository(context);
                var blogEntries = blogRepository.GetAllBlogEntries();
                blogEntries.Should().HaveCount(0);
                context.Blogs.Add(new DataAccess.Blog());
                context.SaveChanges();
                blogEntries = blogRepository.GetAllBlogEntries();
                blogEntries.Should().HaveCount(1);
            }
        }
        [Fact]
        public void InMemory_Repository_Integration2()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName: "repositoryDb")
                .Options;

            using (var context = new BlogContext(options))
            {
                var blogRepository = new BlogRepository(context);
                var blogEntries = blogRepository.GetAllBlogEntries();
                blogEntries.Should().HaveCount(0);
                context.Blogs.Add(new DataAccess.Blog());
                context.SaveChanges();
                blogEntries = blogRepository.GetAllBlogEntries();
                blogEntries.Should().HaveCount(1);
            }
        }

        [Fact]
        public void ReturnAllBlogObjects_Mock_Repository()
        {
            var blogRepository = new Mock<IBlogRepository>();
            blogRepository
                .Setup(b => b.GetAllBlogEntries())
                .Returns(new List<DataAccess.Blog> { new DataAccess.Blog() });
            var blogController = new BlogController(blogRepository.Object);
            var blogResult = blogController.Get().Result as OkObjectResult;
            blogResult.Should().NotBeNull();
            var returnedBlogEntries = blogResult.Value as IEnumerable<DataAccess.Blog>;
            returnedBlogEntries.Should().NotBeNull();
            returnedBlogEntries.Should().HaveCount(1);
        }

        [Fact]
        public void Inmemory_Controller_Integration()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName: "controllerDb")
                .Options;
            using (var context = new BlogContext(options))
            {
                var blogRepository = new BlogRepository(context);
                var controller = new BlogController(blogRepository);
                var objectResult = controller.Get().Result as OkObjectResult;
                var blogEntries = objectResult.Value as IEnumerable<DataAccess.Blog>;
                blogEntries.Should().HaveCount(0);
                context.Blogs.Add(new DataAccess.Blog());
                context.SaveChanges();
                objectResult = controller.Get().Result as OkObjectResult;
                blogEntries = objectResult.Value as IEnumerable<DataAccess.Blog>;
                blogEntries.Should().HaveCount(1);
            }
        }


    }
}
