namespace UI_Layer.Dtos.EmployeeImage
{
    public class EmployeeImageDto
    {

        public int UserId { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageMimeType { get; set; }

    }
}
