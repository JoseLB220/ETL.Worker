using System;

namespace ETL.Worker.Domain.Models
{
    public class RecordModel
    {
        public int Id { get; set; }
        public string Source { get; set; } = "";
        public string UserName { get; set; } = "";
        public int Rating { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
