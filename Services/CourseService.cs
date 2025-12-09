using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SchoolColab.Data;
using SchoolColab.Models;

namespace SchoolColab.Services
{
    public class CourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;

        public CourseService(IOptions<databaseSetting> settings)
        {
            var setting = settings.Value;

            var client = new MongoClient(setting.ConnectionString);
            var database = client.GetDatabase(setting.DatabaseName);
            _courseCollection = database.GetCollection<Course>("Courses");
        }

        public async Task AddCourse(Course course)
        {
            await _courseCollection.InsertOneAsync(course);
        }

		public async Task<List<Course>> GetAllCourse()
		{
			List<Course> courses =  await _courseCollection.Find(_ => true).ToListAsync();
            return courses;
		}

        public async Task IncreaseCountEnrolled(string id)
        {
            var filter = Builders<Course>.Filter.Eq(c => c.Id, id);
            var update = Builders<Course>.Update.Inc(c => c.EntrolledStudentsCount, 1);

            await _courseCollection.UpdateOneAsync(filter, update);
        }
	}
}
