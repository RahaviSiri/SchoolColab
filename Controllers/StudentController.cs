using System.Security.Cryptography;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using SchoolColab.data;
using SchoolColab.Models;
using SchoolColab.Services;

namespace SchoolColab.Controllers
{
	[ApiController]
	[Route("api/student")]
	public class StudentController : ControllerBase
	{
		private readonly StudentService _studentService;
		private readonly Cloudinary _cloudinary;

		public StudentController(StudentService _studentService, Cloudinary _cloudinary)
		{
			this._studentService = _studentService;
			this._cloudinary = _cloudinary;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllStudents()
		{
			List<Student> students = await _studentService.GetAsync();
			return Ok(students);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetStudentById(string id)
		{
			Student student = await _studentService.GetAsync(id);
			return Ok(student);
		}

		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateStudent([FromForm] CreateStudentDto createStudent)
		{
			string imageUrl = null;
			string publicId = null;
			string imageHash = null;

			if (createStudent.ProfileImage != null)
			{
				string newHash = ComputeImageHash(createStudent.ProfileImage);

				var uploadParams = new ImageUploadParams()
				{
					File = new FileDescription(createStudent.ProfileImage.FileName, createStudent.ProfileImage.OpenReadStream()),
					Folder = "students"
				};

				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				imageUrl = uploadResult.SecureUrl.ToString();
				publicId = uploadResult.PublicId;
				imageHash = newHash;
			}

			var student = new Student
			{
				Name = createStudent.Name,
				Grade = createStudent.Grade,
				StudentCourses = createStudent.StudentCourses,
				ProfileImage = imageUrl,
				ImagePublicId = publicId,
				ImageHash = imageHash

			};
			await _studentService.CreateAsync(student);
			return Ok("Student Created");
		}


		[HttpPut("{id}")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UpdateStudent(string id, [FromForm] CreateStudentDto createStudent)
		{
			var isPresent = await _studentService.GetAsync(id);
			if (isPresent == null)
			{
				return NotFound();
			}

			string imageUrl = null;
			string publicId = null;
			string imageHash = null;

			if (createStudent.ProfileImage != null)
			{
				string newHash = ComputeImageHash(createStudent.ProfileImage);

				// If same image → don't upload
				if (isPresent.ImageHash == newHash)
				{
					imageUrl = isPresent.ProfileImage;
					publicId = isPresent.ImagePublicId;
					imageHash = isPresent.ImageHash;
				}
				else
				{
					// Different image → delete old, upload new
					if (!string.IsNullOrEmpty(isPresent.ImagePublicId))
					{
						var deletionParams = new DeletionParams(isPresent.ImagePublicId);
						await _cloudinary.DestroyAsync(deletionParams);
					}

					var uploadParams = new ImageUploadParams()
					{
						File = new FileDescription(createStudent.ProfileImage.FileName, createStudent.ProfileImage.OpenReadStream()),
						Folder = "students"
					};

					var uploadResult = await _cloudinary.UploadAsync(uploadParams);

					imageUrl = uploadResult.SecureUrl.ToString();
					publicId = uploadResult.PublicId;
					imageHash = newHash;
				}
			}

			var student = new Student
			{
				Id = id,
				Name = createStudent.Name,
				Grade = createStudent.Grade,
				StudentCourses = createStudent.StudentCourses,
				ProfileImage = imageUrl,
				ImagePublicId = publicId,
				ImageHash = imageHash
			};
			await _studentService.UpdateAsync(id, student);
			return Ok("Student Updated");
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteStudent(string id)
		{
			var isPresent = await _studentService.GetAsync(id);
			if (isPresent == null)
			{
				return NotFound();
			}
			await _studentService.DeleteAsync(id);
			return Ok("Student Deleted");
		}

	private string ComputeImageHash(IFormFile file)
		{
			using var sha = SHA256.Create();
			using var stream = file.OpenReadStream();
			var hashBytes = sha.ComputeHash(stream);
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
		}
	}
}
