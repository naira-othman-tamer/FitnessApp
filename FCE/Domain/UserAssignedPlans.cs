namespace FCE.Domain
{
    public class UserAssignedPlans
    {
        public int Id { get; set; }
        public int ExternalPlanId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool isActive { get; set; }

    }
}
