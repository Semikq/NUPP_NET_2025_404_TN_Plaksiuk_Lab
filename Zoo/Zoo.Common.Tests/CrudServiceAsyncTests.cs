using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Zoo.Common;

namespace Zoo.Common.Tests
{
    public class CrudServiceAsyncTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly List<string> _tempFiles = new List<string>();

        public CrudServiceAsyncTests()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.json");
            _tempFiles.Add(_testFilePath);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewElement_ReturnsTrue()
        {
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var entity = new TestEntity("Test1", 10);

            var result = await service.CreateAsync(entity);

            Assert.True(result);
            var allItems = await service.ReadAllAsync();
            Assert.Single(allItems);
            Assert.Contains(entity, allItems);
        }

        [Fact]
        public async Task CreateAsync_ShouldNotAddDuplicateId_ReturnsFalse()
        {
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id, "Test1", 10);
            var entity2 = new TestEntity(id, "Test2", 20);

            await service.CreateAsync(entity1);
            var result = await service.CreateAsync(entity2);

            Assert.False(result);
            var allItems = await service.ReadAllAsync();
            Assert.Single(allItems);
            Assert.Equal("Test1", allItems.First().Name);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnElement_WhenExists()
        {
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var entity = new TestEntity("Test1", 10);
            await service.CreateAsync(entity);

            var result = await service.ReadAsync(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal("Test1", result.Name);
            Assert.Equal(10, result.Value);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnNull_WhenNotExists()
        {
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var nonExistentId = Guid.NewGuid();

            var result = await service.ReadAsync(nonExistentId);

            Assert.Null(result);
        }

        [Fact]
        public async Task ReadAllAsync_ShouldReturnAllElements()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var entity1 = new TestEntity("Test1", 10);
            var entity2 = new TestEntity("Test2", 20);
            var entity3 = new TestEntity("Test3", 30);

            await service.CreateAsync(entity1);
            await service.CreateAsync(entity2);
            await service.CreateAsync(entity3);

            // Act
            var result = await service.ReadAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains(entity1, result);
            Assert.Contains(entity2, result);
            Assert.Contains(entity3, result);
        }

        [Fact]
        public async Task ReadAllAsync_ShouldReturnEmpty_WhenNoElements()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);

            // Act
            var result = await service.ReadAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ReadAllAsync_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            for (int i = 1; i <= 10; i++)
            {
                await service.CreateAsync(new TestEntity($"Test{i}", i));
            }

            // Act
            var page1 = await service.ReadAllAsync(1, 3);
            var page2 = await service.ReadAllAsync(2, 3);
            var page3 = await service.ReadAllAsync(3, 3);
            var page4 = await service.ReadAllAsync(4, 3);

            // Assert
            Assert.Equal(3, page1.Count());
            Assert.Equal(3, page2.Count());
            Assert.Equal(3, page3.Count());
            Assert.Single(page4);
        }

        [Fact]
        public async Task ReadAllAsync_WithPagination_ShouldReturnEmpty_WhenPageOutOfRange()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            await service.CreateAsync(new TestEntity("Test1", 10));

            // Act
            var result = await service.ReadAllAsync(10, 5);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingElement_ReturnsTrue()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var entity = new TestEntity("Test1", 10);
            await service.CreateAsync(entity);

            var updatedEntity = new TestEntity(entity.Id, "UpdatedTest", 20);

            // Act
            var result = await service.UpdateAsync(updatedEntity);

            // Assert
            Assert.True(result);
            var readEntity = await service.ReadAsync(entity.Id);
            Assert.NotNull(readEntity);
            Assert.Equal("UpdatedTest", readEntity.Name);
            Assert.Equal(20, readEntity.Value);
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotUpdateNonExistentElement_ReturnsFalse()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var nonExistentEntity = new TestEntity(Guid.NewGuid(), "Test", 10);

            // Act
            var result = await service.UpdateAsync(nonExistentEntity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveExistingElement_ReturnsTrue()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var entity = new TestEntity("Test1", 10);
            await service.CreateAsync(entity);

            // Act
            var result = await service.RemoveAsync(entity);

            // Assert
            Assert.True(result);
            var allItems = await service.ReadAllAsync();
            Assert.Empty(allItems);
            var readEntity = await service.ReadAsync(entity.Id);
            Assert.Null(readEntity);
        }

        [Fact]
        public async Task RemoveAsync_ShouldNotRemoveNonExistentElement_ReturnsFalse()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var nonExistentEntity = new TestEntity(Guid.NewGuid(), "Test", 10);

            // Act
            var result = await service.RemoveAsync(nonExistentEntity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SaveAsync_ShouldSaveToFile()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var entity1 = new TestEntity("Test1", 10);
            var entity2 = new TestEntity("Test2", 20);
            await service.CreateAsync(entity1);
            await service.CreateAsync(entity2);

            // Act
            var result = await service.SaveAsync();

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(_testFilePath));
            
            // Перевірка, що файл містить дані
            var fileContent = await File.ReadAllTextAsync(_testFilePath);
            Assert.Contains("Test1", fileContent);
            Assert.Contains("Test2", fileContent);
        }

        [Fact]
        public async Task CreateAndLoadAsync_ShouldLoadFromFile()
        {
            // Arrange
            var service1 = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var entity1 = new TestEntity("Test1", 10);
            var entity2 = new TestEntity("Test2", 20);
            await service1.CreateAsync(entity1);
            await service1.CreateAsync(entity2);
            await service1.SaveAsync();

            // Act - створюємо новий сервіс і завантажуємо дані
            var service2 = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);

            // Assert
            var allItems = await service2.ReadAllAsync();
            Assert.Equal(2, allItems.Count());
            Assert.True(allItems.Any(e => e.Name == "Test1"));
            Assert.True(allItems.Any(e => e.Name == "Test2"));
        }

        [Fact]
        public async Task CreateAndLoadAsync_ShouldHandleNonExistentFile()
        {
            // Arrange
            var nonExistentFile = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.json");
            _tempFiles.Add(nonExistentFile);

            // Act
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(nonExistentFile);

            // Assert
            var allItems = await service.ReadAllAsync();
            Assert.Empty(allItems);
        }

        [Fact]
        public async Task CreateAndLoadAsync_ShouldHandleEmptyFile()
        {
            // Arrange
            await File.WriteAllTextAsync(_testFilePath, "");

            // Act
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);

            // Assert
            var allItems = await service.ReadAllAsync();
            Assert.Empty(allItems);
        }

        [Fact]
        public void GetEnumerator_ShouldReturnAllElements()
        {
            // Arrange
            var service = CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath).Result;
            var entity1 = new TestEntity("Test1", 10);
            var entity2 = new TestEntity("Test2", 20);
            service.CreateAsync(entity1).Wait();
            service.CreateAsync(entity2).Wait();

            // Act
            var items = new List<TestEntity>();
            foreach (var item in service)
            {
                items.Add(item);
            }

            // Assert
            Assert.Equal(2, items.Count);
            Assert.Contains(entity1, items);
            Assert.Contains(entity2, items);
        }

        [Fact]
        public async Task ConcurrentOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var service = await CrudServiceAsync<TestEntity>.CreateAndLoadAsync(_testFilePath);
            var tasks = new List<Task>();

            // Act - виконуємо багато операцій паралельно
            for (int i = 0; i < 100; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    var entity = new TestEntity($"Test{index}", index);
                    await service.CreateAsync(entity);
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            var allItems = await service.ReadAllAsync();
            Assert.Equal(100, allItems.Count());
        }


        public void Dispose()
        {
            // Cleanup - видаляємо тимчасові файли
            foreach (var file in _tempFiles)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                    // Ігноруємо помилки видалення
                }
            }
        }
    }
}

