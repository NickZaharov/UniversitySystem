
namespace UniversitySystem
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int StudentId { get; set; }
        public int LogId { get; set; }
        public string Value { get; set; }

        public Student Student { get; set; }
        public Log Log { get; set; }
    }
}
