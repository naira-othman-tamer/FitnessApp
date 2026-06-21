using FCE.Domain.Enums;

namespace FCE.Domain
{
    public enum Gender { Male, Female }
   
    public enum ActivityLevel { Rookie, Beginner, Intermediate, Advance, TrueBeast }

    public class UserFitnessStats : BaseEntity //stage1 [Stores raw physical input variables]
    {
        public Guid userId { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public short Age { get; set; }
        public Gender gender { get; set; }
        public Goal goal { get; set; }
        public ActivityLevel activityLevel { get; set; }

        //public bool IsActive { get; set; }
        //public DateTime RecordedAt { get; set; } >> createdAt
    }
}
