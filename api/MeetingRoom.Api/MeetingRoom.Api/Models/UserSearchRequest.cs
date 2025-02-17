using System.ComponentModel.DataAnnotations;

namespace MeetingRoom.Api.Models
{
    public class UserSearchRequest
    {
        public int Page { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than 1")]
        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public string? SortBy { get; set; } = "UpdatedAt";

        public bool SortDesc { get; set; } = true;

        public string? Status { get; set; }
    }
}
