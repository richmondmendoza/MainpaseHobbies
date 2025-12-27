using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public class PaginatedDto<T> where T : class, new()
    {
        public PaginatedDto()
        {

        }
        private readonly IQueryable<T> _query;
        private readonly IEnumerable<T> _enumerable;

        public int CurrentPage { get; set; }

        public int Pages { get; set; }

        public int Size { get; set; }

        public int Total { get; set; }

        public List<T> Items { get; set; }

        public PaginatedDto(IQueryable<T> query, int page, int size)
        {
            if (query == null)
            {
                Items = Empty();
                return;
            }

            _query = query;
            CurrentPage = page;
            Size = size;

            Total = GetTotal();
            Pages = GetTotalPages();
            Items = ToPagedList();
        }

        public PaginatedDto(IEnumerable<T> query, int page, int size)
        {
            if (query == null)
            {
                Items = Empty();
                return;
            }

            _enumerable = query;
            CurrentPage = page;
            Size = size;

            Total = GetTotal(false);
            Pages = GetTotalPages();
            Items = ToPagedList(false);
        }

        private List<T> ToPagedList(bool isQueryable = true)
        {
            if (isQueryable)
                return _query
                    .Skip((CurrentPage - 1) * Size)
                    .Take(Size)
                    .ToList();


            return _enumerable.Skip((CurrentPage - 1) * Size)
                .Take(Size)
                .ToList();
        }

        private int GetTotal(bool isQueryable = true)
        {
            if (isQueryable)
                return _query.Count();

            return _enumerable.Count();
        }

        private int GetTotalPages()
        {
            return (int)Math.Ceiling((double)Total / Size);
        }

        private List<T> Empty()
        {
            return new List<T>();
        }
    }


    public static class QueryableExtensions
    {
        public static PaginatedDto<T> Paginate<T>(this IQueryable<T> query, int page, int size)
            where T : class, new()
        {
            return new PaginatedDto<T>(query, page, size);
        }

        public static PaginatedDto<T> Paginate<T>(this IEnumerable<T> query, int? page, int size)
            where T : class, new()
        {
            return new PaginatedDto<T>(query, page ?? 0, size);
        }
    }
    public class PageModelParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string Order { get; set; }
        public string Sort { get; set; }
        public string Filter { get; set; }
    }
}
