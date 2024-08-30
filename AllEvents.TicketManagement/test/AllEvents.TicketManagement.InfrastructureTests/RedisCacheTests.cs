using AllEvents.TicketManagement.Application.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class RedisCacheFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public RedisCacheFixture()
        {
            var services = new ServiceCollection();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "TestInstance:";
            });

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
        }
    }


    public class RedisCacheTests : IClassFixture<RedisCacheFixture>
    {
        private readonly IDistributedCache _cache;

        public RedisCacheTests(RedisCacheFixture fixture)
        {
            _cache = fixture.ServiceProvider.GetRequiredService<IDistributedCache>();
        }

        [Fact]
        public async Task SetCacheAsync_SavesDataToRedis()
        {
            // Arrange
            var cacheKey = "test-key";
            var cacheValue = new { Name = "Test", Value = 123 };
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            // Act
            await _cache.SetCacheAsync(cacheKey, cacheValue, options, "TestPrefix");

            // Assert
            var cachedData = await _cache.GetCacheAsync<object>(cacheKey, "TestPrefix");
            Assert.NotNull(cachedData);

            // Deserialize and assert specific values
            var deserializedData = cachedData as dynamic;
            Assert.Equal("Test", (string)deserializedData.Name);
            Assert.Equal(123, (int)deserializedData.Value);
        }

        [Fact]
        public async Task RemoveCacheAsync_DeletesDataFromRedis()
        {
            // Arrange
            var cacheKey = "test-key";
            var cacheValue = new { Name = "Test", Value = 123 };
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetCacheAsync(cacheKey, cacheValue, options, "TestPrefix");

            // Act
            await _cache.RemoveCacheAsync(cacheKey, "TestPrefix");

            // Assert
            var cachedData = await _cache.GetCacheAsync<object>(cacheKey, "TestPrefix");
            Assert.Null(cachedData);
        }
    }

}
