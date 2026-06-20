namespace FCE.Domain
{
    public enum Gender { Male, Female }
    public class UserFitnessStats //stage1 [Stores raw physical input variables]
    {
        public Guid userId { get; set; }
        public double weight { get; set; }
        public double Height { get; set; }
        public short age { get; set; }
        public Gender gender { get; set; }
        public string goal { get; set; }
        public string activityResult { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
