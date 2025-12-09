using System.Security.Cryptography;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using SchoolColab.Data;
using SchoolColab.Models;
using SchoolColab.Services;

namespace SchoolColab.Controllers
{
    [ApiController]
    [Route("/api/teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly TeacherService _teacherService;
		private readonly Cloudinary _cloudinary;


		public TeacherController(TeacherService teacherService, Cloudinary cloudinary)
        {
            _teacherService = teacherService;
			_cloudinary = cloudinary;
		}

        [HttpGet]
        public async Task<IActionResult> GetAllTeachers()
        {
            var teachers = await _teacherService.GetAsync();
            return Ok(teachers);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTeacherById(string id)
		{
			var teacher = await _teacherService.GetAsync(id);
            if (teacher == null) return NotFound();
			return Ok(teacher);
		}

        [HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> AddTeacher([FromForm] CreateTeacherDto teacherDto)
        {
			string imageUrl = null;
			string publicId = null;
			string imageHash = null;

			if (teacherDto.ProfileImage != null)
			{
				string newHash = ComputeImageHash(teacherDto.ProfileImage);
				var uploadParams = new ImageUploadParams()
				{
					File = new FileDescription(teacherDto.ProfileImage.FileName, teacherDto.ProfileImage.OpenReadStream()),
					Folder = "teachers"
				};

				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				imageUrl = uploadResult.SecureUrl.ToString();
				publicId = uploadResult.PublicId;
				imageHash = newHash;
			}

			var teacher = new Teacher
			{
				Name = teacherDto.Name,
				Degree = teacherDto.Degree,
				SelectedCourses = teacherDto.SelectedCourses,
				ProfileImage = imageUrl,
				ImagePublicId = publicId,
				ImageHash = imageHash
			};

			await _teacherService.CreateAsync(teacher);
			return NoContent();
		}

		[HttpPut("{id}")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UpdateTeacher(string id, [FromForm] CreateTeacherDto teacherDto)
		{
			var isPresent = await _teacherService.GetAsync(id);
			if (isPresent == null)
			{
				return NotFound();
			}

			string imageUrl = null;
			string publicId = null;
			string imageHash = null;

			if (teacherDto.ProfileImage != null)
			{
				string newHash = ComputeImageHash(teacherDto.ProfileImage);

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
						File = new FileDescription(teacherDto.ProfileImage.FileName, teacherDto.ProfileImage.OpenReadStream()),
						Folder = "teachers"
					};

					var uploadResult = await _cloudinary.UploadAsync(uploadParams);

					imageUrl = uploadResult.SecureUrl.ToString();
					publicId = uploadResult.PublicId;
					imageHash = newHash;
				}
			}
			var teacher = new Teacher
			{
				Id = id,
				Name = teacherDto.Name,
				Degree = teacherDto.Degree,
				SelectedCourses = teacherDto.SelectedCourses,
				ProfileImage = imageUrl,
				ImagePublicId = publicId,
				ImageHash = imageHash
			};
			await _teacherService.UpdateAsync(id, teacher);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTeacher(string id)
		{
			var teacher = await _teacherService.GetAsync(id);
			if (teacher == null) return NotFound();
			await _teacherService.RemoveAsync(id);
			return NoContent();
		}

        [HttpGet("courses")]
        public async Task<IActionResult> GetTeachersByCourses([FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(course))
            {
                return BadRequest("course is required");
            }

            Console.WriteLine($"GetTeachersByCourses course='{course}'");
            List<Teacher> list = await _teacherService.GetTeachersByCourse(course);
            return Ok(list);
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
