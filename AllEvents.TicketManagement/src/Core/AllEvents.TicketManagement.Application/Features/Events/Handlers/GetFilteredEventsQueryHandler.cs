using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Extensions;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

public class GetFilteredEventsQueryHandler : IRequestHandler<GetFilteredEventsQuery, PagedResult<EventModel>>
{
    private readonly IAllEventsDbContext _dbContext;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetFilteredEventsQueryHandler> _logger;
    private const string CacheKeyPrefix = "Event";

    public GetFilteredEventsQueryHandler(IAllEventsDbContext dbContext, IDistributedCache cache, ILogger<GetFilteredEventsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<EventModel>> Handle(GetFilteredEventsQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"{CacheKeyPrefix}_{request.PageIndex}_{request.PageSize}_{request.Title}_{request.Category}_{request.SortBy}_{request.Ascending}";

        var cachedResult = await _cache.GetCacheAsync<PagedResult<EventModel>>(cacheKey, CacheKeyPrefix);
        if (cachedResult != null)
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return cachedResult;
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);

        var query = new EventQuery(_dbContext.Events);

        if (!string.IsNullOrEmpty(request.Title))
        {
            query.Search(request.Title);
        }

        if (request.Category.HasValue)
        {
            query.ForCategory(request.Category.Value);
        }

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            query.SortBy(request.SortBy, request.Ascending);
        }

        var events = await query.ToListAsync(request.PageIndex, request.PageSize);
        var totalEvents = await query.CountAsync();

        var eventModels = events.Select(e => new EventModel
        {
            EventId = e.EventId,
            Title = e.Title,
            Location = e.Location,
            Price = e.Price,
            Category = e.Category,
        }).ToList();

        var result = new PagedResult<EventModel>(eventModels, totalEvents, request.PageIndex + 1, request.PageSize);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        await _cache.SetCacheAsync(cacheKey, result, cacheOptions, CacheKeyPrefix);

        return result;
    }
}
