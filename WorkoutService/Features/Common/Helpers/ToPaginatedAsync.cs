using Microsoft.EntityFrameworkCore;

namespace WorkoutService.Features.Common.Helpers
{
    public static class PaginationHelper
    {
        public static async Task<(List<T> Data, int Total, int TotalPages)> ToPaginatedAsync<T>(
            this IQueryable<T> query,
            int page,
            int perPage,
            CancellationToken cancellationToken = default)
        {
            var total = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(total / (double)perPage);
            var data = await query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .ToListAsync(cancellationToken);

            return (data, total, totalPages);
        }
}
}
