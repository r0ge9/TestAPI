using TestAPI.Model.Enums;

namespace TestAPI.Model.Parameters
{
    public class UserParameter
    {
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        private int _pageSize =10;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
        public FieldsName sortField { get; set; } = 0;
        public bool isAsc { get; set; } = true;
        public string? nameFilter { get; set; } 
        public string? emailFilter { get; set; }
        public string? roleNameFilter { get; set; }
        public int? minAgeFilter { get; set; } 
        public int? maxAgeFilter { get; set; }



    }
}
