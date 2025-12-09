using MongoDB.Bson.Serialization.Attributes;

namespace SchoolColab.Models
{
    [BsonIgnoreExtraElements]
    public class Course
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Course_Name")]
        public string Name { get; set; }

        [BsonElement("NoOfStudentsEnrolled")]
        public int EntrolledStudentsCount { get; set; }
    }
}
