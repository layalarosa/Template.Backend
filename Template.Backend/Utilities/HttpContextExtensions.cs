using Microsoft.EntityFrameworkCore;

namespace Template.Backend.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParametersInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double amount = await queryable.CountAsync();
            httpContext.Response.Headers.Append("total-amount-of-records", amount.ToString());
        }
    }
}
