using Microsoft.EntityFrameworkCore;

namespace ProgressTrackingService.Features.Common.Helpers;

public static class PaginationHelper
{
    public record PaginatedResult<T>(List<T> Data, int TotalCount, int TotalPages, bool IsValidRequest);

    public static async Task<PaginatedResult<T>> ToPaginatedAsync<T>(
        this IQueryable<T> query,
        int page,
        int PageSize,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        PageSize = PageSize < 1 ? 10 : PageSize;
        var total = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)PageSize);
        var data = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<T>(data, total, totalPages, IsValidRequest: true);
    }
}
