namespace ShelterApiJo.Models.DTOs
{
    public class NewAnimalDto
    {
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public int age { get; set; }
        public double weight { get; set; }
        public string ShelterId { get; set; } = string.Empty;
    }
}
