using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using Xunit;

namespace NOS.Engineering.Challenge.API.Tests.Unit.Managers
{
    public class ContentsManagerTests
    {
       [Fact]
        public async Task GetManyContents_ReturnsContentsFromDatabase()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
            var expectedContents = new List<Content?>
            {
                new Content(Guid.NewGuid(), "Title 1", "Subtitle 1", "Description 1", "ImageUrl 1", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string>{"Genre 1", "Genre 2"}),
                new Content(Guid.NewGuid(), "Title 2", "Subtitle 2", "Description 2", "ImageUrl 2", 150, DateTime.Now, DateTime.Now.AddHours(3), new List<string>{"Genre 3", "Genre 4"}),
            };

            mockDatabase.Setup(db => db.ReadAll()).ReturnsAsync(expectedContents);

            var manager = new ContentsManager(mockDatabase.Object);

            // Act
            var actualContents = await manager.GetManyContents();

            // Assert
            Assert.Equal(expectedContents, actualContents);
        }

        [Fact]
        public async Task CreateContent_ReturnsCreatedContent()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
            var contentDto = new ContentDto
            {
                Title = "Title",
                SubTitle = "Subtitle",
                Description = "Description",
                ImageUrl = "ImageUrl",
                Duration = 120,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2),
                GenreList = new List<string>{"Genre 1", "Genre 2"}
            };
            var createdContent = new Content(Guid.NewGuid(), contentDto.Title, contentDto.SubTitle, contentDto.Description, contentDto.ImageUrl, contentDto.Duration, contentDto.StartTime.Value, contentDto.EndTime.Value, contentDto.GenreList);

            mockDatabase.Setup(db => db.Create(contentDto)).ReturnsAsync(createdContent);

            var manager = new ContentsManager(mockDatabase.Object);

            // Act
            var result = await manager.CreateContent(contentDto);

            // Assert
            Assert.Equal(createdContent, result);
        }

        [Fact]
        public async Task GetContent_ReturnsContentById()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
            var contentId = Guid.NewGuid();
            var content = new Content(contentId, "Title", "Subtitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string>{"Genre 1", "Genre 2"});

            mockDatabase.Setup(db => db.Read(contentId)).ReturnsAsync(content);

            var manager = new ContentsManager(mockDatabase.Object);

            // Act
            var result = await manager.GetContent(contentId);

            // Assert
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task UpdateContent_ReturnsUpdatedContent()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
            var contentId = Guid.NewGuid();
            var contentDto = new ContentDto
            {
                Title = "Updated Title",
                SubTitle = "Updated Subtitle",
                Description = "Updated Description",
                ImageUrl = "Updated ImageUrl",
                Duration = 150,
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(3),
                GenreList = new List<string>{"Genre 3", "Genre 4"}
            };
            var updatedContent = new Content(contentId, contentDto.Title, contentDto.SubTitle, contentDto.Description, contentDto.ImageUrl, contentDto.Duration, contentDto.StartTime.Value, contentDto.EndTime.Value, contentDto.GenreList);

            mockDatabase.Setup(db => db.Update(contentId, contentDto)).ReturnsAsync(updatedContent);

            var manager = new ContentsManager(mockDatabase.Object);

            // Act
            var result = await manager.UpdateContent(contentId, contentDto);

            // Assert
            Assert.Equal(updatedContent, result);
        }

        [Fact]
        public async Task DeleteContent_ReturnsDeletedContentId()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
            var contentId = Guid.NewGuid();

            mockDatabase.Setup(db => db.Delete(contentId)).ReturnsAsync(contentId);

            var manager = new ContentsManager(mockDatabase.Object);

            // Act
            var result = await manager.DeleteContent(contentId);

            // Assert
            Assert.Equal(contentId, result);
        }

        [Fact]
        public async Task AddGenres_ReturnsUpdatedContentWithGenresAdded()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
            var contentId = Guid.NewGuid();
            var existingContent = new Content(contentId, "Title 1", "Subtitle 1", "Description 1", "ImageUrl 1", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string>{"Genre 1", "Genre 2"});
            var genresToAdd = new List<string>{"Genre 3", "Genre 4"};
            var updatedContentDto = new ContentDto
            {
                Title = existingContent.Title,
                SubTitle = existingContent.SubTitle,
                Description = existingContent.Description,
                ImageUrl = existingContent.ImageUrl,
                Duration = existingContent.Duration,
                StartTime = existingContent.StartTime,
                EndTime = existingContent.EndTime,
                GenreList = new List<string>{"Genre 1", "Genre 2", "Genre 3", "Genre 4"}
            };

            mockDatabase.Setup(db => db.Read(contentId)).ReturnsAsync(existingContent);
            mockDatabase.Setup(db => db.Update(contentId, updatedContentDto)).ReturnsAsync(existingContent);

            var manager = new ContentsManager(mockDatabase.Object);

            // Act
            var updatedContent = await manager.AddGenres(contentId, genresToAdd);

            // Assert
            Assert.Equal(existingContent, updatedContent);
        }

        [Fact]
        public async Task RemoveGenres_ReturnsUpdatedContentWithGenresRemoved()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
            var contentId = Guid.NewGuid();
            var existingContent = new Content(contentId, "Title 1", "Subtitle 1", "Description 1", "ImageUrl 1", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string>{"Genre 1", "Genre 2"});
            var genresToRemove = new List<string>{"Genre 1"};
            var updatedContentDto = new ContentDto
            {
                Title = existingContent.Title,
                SubTitle = existingContent.SubTitle,
                Description = existingContent.Description,
                ImageUrl = existingContent.ImageUrl,
                Duration = existingContent.Duration,
                StartTime = existingContent.StartTime,
                EndTime = existingContent.EndTime,
                GenreList = new List<string>{"Genre 2"}
            };

            mockDatabase.Setup(db => db.Read(contentId)).ReturnsAsync(existingContent);
            mockDatabase.Setup(db => db.Update(contentId, updatedContentDto)).ReturnsAsync(existingContent);

            var manager = new ContentsManager(mockDatabase.Object);

            // Act
            var updatedContent = await manager.RemoveGenres(contentId, genresToRemove);

            // Assert
            Assert.Equal(existingContent, updatedContent);
        }
    }
}
