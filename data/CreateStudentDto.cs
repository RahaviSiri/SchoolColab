
namespace SchoolColab.data
{
    public class CreateStudentDto
    {
		public string Name { get; set; }

		public string Grade { get; set; }

		public List<string> StudentCourses { get; set; }

		public IFormFile? ProfileImage { get; set; }
	}
}
