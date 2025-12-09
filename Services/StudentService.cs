using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SchoolColab.Data;
using SchoolColab.Models;

namespace SchoolColab.Services
{
    public class StudentService
    {
        private readonly IMongoCollection<Student> _studentsCollection;

        public StudentService(IOptions<databaseSetting> settings)
        {
            var setting = settings.Value;

            var client = new MongoClient(setting.ConnectionString);
            var database = client.GetDatabase(setting.DatabaseName);
            _studentsCollection = database.GetCollection<Student>("Students");

        }

        public async Task<List<Student>> GetAsync()
        {
            return await _studentsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Student> GetAsync(string id)
        {
            return await _studentsCollection.Find(student => student.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Student student)
        {
            await _studentsCollection.InsertOneAsync(student);
			// InsertOneAsync => Purpose of using async methods of mongodb is API can handle more requests at the same time
			// Runs without blocking your thread.
            // Returns a Task, so your application can continue doing other work
		}

		public async Task UpdateAsync(String id, Student student)
        {
            await _studentsCollection.ReplaceOneAsync(student => student.Id == id, student);
        }

        public async Task DeleteAsync(string id)
        {
            await _studentsCollection.DeleteOneAsync(student => student.Id == id);
        }
	}
}
