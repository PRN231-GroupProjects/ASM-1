using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Service.Common.Mapping;
using Service.Models;
using Service.Models.Dtos;

namespace Service.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> items, int page, int size)
    {
        return items.Skip((page - 1) * size).Take(size);
    }
    
    public static async Task<PaginatedList<TEntityDto>> ListPaginateWithSortAsync<TEntity, TEntityDto>(
        this IQueryable<TEntity> items,
        int? page,
        int? size,
        string? sortBy,
        string? sortOrder,
        IConfigurationProvider mapperConfiguration
       )
        where TEntityDto : BaseDto, IMapFrom<TEntity>
    {
        if (sortBy is null || !sortBy.MatchesPropertyName<TEntityDto>())
        {
            sortBy = nameof(BaseDto.Id);
        }
            
        sortOrder ??= "asc";
        var pageNumber = page is null or <= 0 ? 1 : page;
        var sizeNumber = size is null or <= 0 ? 10 : size;
            
        var count = await items.CountAsync();
        var list  = await items
            .OrderByCustom(sortBy, sortOrder)
            .Paginate(pageNumber.Value, sizeNumber.Value)
            .ToListAsync();

        var mapper = mapperConfiguration.CreateMapper();
        var result = mapper.Map<List<TEntityDto>>(list);
        return new PaginatedList<TEntityDto>(result, count, pageNumber.Value, sizeNumber.Value);
    }
    
    public static IQueryable<TEntity> OrderByCustom<TEntity>(this IQueryable<TEntity> items, string sortBy, string sortOrder)
    {
        var type = typeof(TEntity);
        var expression2 = Expression.Parameter(type, "t");
        var property = type.GetProperty(sortBy);
        var expression1 = Expression.MakeMemberAccess(expression2, property!);
        var lambda = Expression.Lambda(expression1, expression2);
        var result = Expression.Call(
            typeof(Queryable),
            sortOrder.Equals("desc") ? "OrderByDescending" : "OrderBy",
            new Type[] { type, property!.PropertyType },
            items.Expression,
            Expression.Quote(lambda));

        return items.Provider.CreateQuery<TEntity>(result);
    }
}