namespace UI_Layer.Dtos.NotificationsDto
{
    public class ResultNotificationDto
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public long Time { get; set; }
        public string? Type { get; set; }
        public int UserId { get; set; }
        public bool Entry { get; set; }
    }
}
