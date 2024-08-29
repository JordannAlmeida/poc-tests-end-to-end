namespace Domain.Model.Request
{
    public record RegisterBloodDonateRequest
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? UniqueCode { get; set; }
        public DateTime DateDonate { get; set; }
        public string? BloodType { get; set; }
        public string? RhFactor { get; set; }
        public float Quantity { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(UniqueCode) && !string.IsNullOrEmpty(BloodType) && !string.IsNullOrEmpty(RhFactor) && Quantity > 0;
        }
    }
}
