namespace StudCrud.Models
{
    // Student.cs
    public class Student
    {
        public int Id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public DateTime Dob { get; set; }
        public int YearOfStudy { get; set; }
        public int SubjectId { get; set; }

        // Navigation property for Subject
        public Subject Subject { get; set; }
    }

    // Subject.cs
    public class Subject
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }

        // Navigation property for Students
        public ICollection<Student> Students { get; set; }
    }

}
