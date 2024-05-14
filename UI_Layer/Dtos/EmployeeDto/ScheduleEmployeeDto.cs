namespace UI_Layer.Dtos.EmployeeDto
{
    public class ScheduleEmployeeDto
    {
        public int ScheduleID { get; set; }
        public long Starttime { get; set; }
        public long Endtime { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
    }
}
