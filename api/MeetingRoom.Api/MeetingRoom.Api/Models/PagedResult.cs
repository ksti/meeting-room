namespace MeetingRoom.Api.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (Total + PageSize - 1) / PageSize;
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }
}
