namespace WebAPIV3.Data.Entity
{
    public class EntityClass
    {
        public class HRComm
        {
            public int EmployeeID { get; set; }
            public string? Subject { get; set; }
            public string? Description { get; set; }
            public int RequestType { get; set; }
            public int RequestQueryID { get; set; }
            public string? Status { get; set; }
        }

        public class ErrorResponse
        {
            public string? StatusCode { get; set; }
            public string? Message { get; set; }
        }

        
    }
}
