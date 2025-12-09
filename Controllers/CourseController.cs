using Microsoft.AspNetCore.Mvc;
using SchoolColab.data;
using SchoolColab.Models;
using SchoolColab.Services;

namespace SchoolColab.Controllers
{
    [ApiController]
    [Route("/api/course")]
    public class CourseController : ControllerBase
    {
        private readonly CourseService courseService;

        public CourseController(CourseService courseService)
        {
            this.courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            List<Course> courses = await courseService.GetAllCourse();
            return Ok(courses);
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CreateCourseDto courseDto)
        {
            var course = new Course
            {
                Name = courseDto.Name,
                EntrolledStudentsCount = courseDto.EntrolledStudentsCount,
            };
            await courseService.AddCourse(course);
            return Ok("Course Created");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> IncrementStudentCount(string id)
        {
            await courseService.IncreaseCountEnrolled(id);
            return Ok("Course Count Incremented");
        }
    }
}
