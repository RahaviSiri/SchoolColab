using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SchoolColab.Data;
using SchoolColab.Models;

namespace SchoolColab.Services
{
    public class TeacherService
    {
        private readonly IMongoCollection<Teacher> _teachersCollection;

        public TeacherService(IOptions<databaseSetting> settings)
        {

			var setting = settings.Value;

			var client = new MongoClient(setting.ConnectionString);
            var database = client.GetDatabase(setting.DatabaseName);
            _teachersCollection = database.GetCollection<Teacher>(setting.CollectionName);
        }

		// Task<T> as a promise that a value will be available later.
        // In database calls, network requests, or file I/O, you almost always use async/await with Task<T>.
		public async Task<List<Teacher>> GetAsync() {
            return await _teachersCollection.Find(_ => true).ToListAsync();
			//  return await _teachersCollection.Find(_ => true) return only pointer to database
			//  So to get actual resulst Convert query results into a List<Teacher> we use .ToListAsync()
		}

		public async Task<Teacher> GetAsync(string id) {
			return await _teachersCollection.Find(teacher => teacher.Id == id).FirstOrDefaultAsync();
			// Find the teacher with the specified id and return the first matching document or null if not found
		}

		public async Task CreateAsync(Teacher newTeacher) {
            await _teachersCollection.InsertOneAsync(newTeacher);
        }

        public async Task UpdateAsync(string id, Teacher updatedTeacher) {
            await _teachersCollection.ReplaceOneAsync(teacher => teacher.Id == id, updatedTeacher);
        }

        public async Task RemoveAsync(string id) {
            await _teachersCollection.DeleteOneAsync(teacher => teacher.Id == id);
        }

		public async Task<List<Teacher>> GetTeachersByCourse(string course)
		{
			// Filter: check if the teacher's Courses array contains the given course
			var filter = Builders<Teacher>.Filter.AnyEq(t => t.SelectedCourses, course);
			// AnyEq → checks if any element in Courses array equals "Math"

			// Execute the query and get the list
			var projection = Builders<Teacher>.Projection
			.Include(t => t.Name)
			.Include(t => t.SelectedCourses);

			var teachers = await _teachersCollection
				.Find(filter)
				.Project<Teacher>(projection)
				.ToListAsync();

			return teachers;
		}
	}
}
