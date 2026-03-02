using Dapper;
using EmailNotificationsSender.Cache;
using EmailNotificationsSender.Dtos;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EmailNotificationsSender.Services;

public class EmailStatisticsService(string connectionString, RedisService cache, ILogger logger)
{
    private string cacheVersionKey = "users";
    public async Task<EmailStatisticsDto?> GetUserEmailStatistics(int userId)
    {
        var cacheVersion = await cache.GetCurrentCacheVersion(cacheVersionKey);
        var cacheKey = cache.GenerateCacheKey("userEmailStatistics", cacheVersion, userId );
        
        var cached = await cache.GetAsync<EmailStatisticsDto>(cacheKey);
        if (cached != null)
        {
            logger.LogInformation($"[CACHE HIT] user_id: {userId}");
            return cached;
        }

        logger.LogInformation($"[CACHE MISS] user_id: {userId}, идем в БД");

        var sql = @"
            SELECT 
                u.id as UserId,
                u.user_name,
                u.email,
                COUNT(em.id) as TotalEmails,
                COUNT(CASE WHEN em.status = 1 THEN 1 END) as SentCount,
                COUNT(CASE WHEN em.status = 0 THEN 1 END) as PendingCount,
                COUNT(CASE WHEN em.status = 2 THEN 1 END) as FailedCount,
                MAX(em.created_at) as LastEmailDate,
                AVG(EXTRACT(EPOCH FROM (em.sent_at - em.created_at))) as AvgSendTimeSeconds
            FROM ""Users"" u
            LEFT JOIN ""EmailMessages"" em ON u.id = em.user_id
            WHERE u.id = @UserId
            GROUP BY u.id, u.user_name, u.email";

        using var connection = new NpgsqlConnection(connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<EmailStatisticsDto>(sql, new { UserId = userId });

        if (result != null)
        {
            await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            logger.LogInformation($"[CACHE SET] user_id: {userId}, срок: 5 минут");
        }

        return result;
    }

    public async Task<List<UserEmailSummaryDto>> GetTopActiveUsers(int limit = 10)
    {
        var cacheVersion = await cache.GetCurrentCacheVersion(cacheVersionKey);
        var cacheKey = cache.GenerateCacheKey("topActiveUsers", cacheVersion, limit);
        
        var cached = await cache.GetAsync<List<UserEmailSummaryDto>>(cacheKey);
        if (cached != null)
        {
            logger.LogInformation($"[CACHE HIT] top_active_users, limit: {limit}");
            return cached;
        }

        logger.LogInformation($"[CACHE MISS] top_active_users, идем в БД");

        var sql = @"
            SELECT 
                u.id,
                u.user_name,
                u.email,
                COUNT(em.id) as EmailCount,
                COUNT(CASE WHEN em.status = 1 THEN 1 END) as SentCount,
                MAX(em.created_at) as LastActivity
            FROM ""Users"" u
            JOIN ""EmailMessages"" em ON u.id = em.user_id
            WHERE u.is_active = true
            GROUP BY u.id, u.user_name, u.email
            ORDER BY COUNT(em.id) DESC
            LIMIT @Limit";

        using var connection = new NpgsqlConnection(connectionString);
        var result = (await connection.QueryAsync<UserEmailSummaryDto>(
            sql, new { Limit = limit })).ToList();

        await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(3));
        logger.LogInformation($"[CACHE SET] top_active_users, limit: {limit}");

        return result;
    }
    
    /// <summary>
    /// заглушка для обновления данных в БД, нужно для тестирования инвалидации кеша
    /// </summary>
    public async Task UpdateDataAsync()
    {
        await cache.InvalidateCache(cacheVersionKey);
    }
}