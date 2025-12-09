namespace SchoolColab.Data
{
	public class CreateTeacherDto
	{
		public string Name { get; set; }
		public string Degree { get; set; }
		public List<string> SelectedCourses { get; set; }
		public IFormFile? ProfileImage { get; set; }
	}
}
