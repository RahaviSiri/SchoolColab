using MongoDB.Bson.Serialization.Attributes;

namespace SchoolColab.Models
{
    [BsonIgnoreExtraElements]
    public class Student
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public String Id { get; set; }

        [BsonElement("Student_Name")]
        public string Name { get; set; }

        [BsonElement("Student_Grade")]
        public string Grade { get; set; }

		[BsonElement("Student_Courses")]
        public List<string> StudentCourses { get; set; }

        [BsonElement("ProfileImageUrl")]
        public string? ProfileImage { get; set; }

        [BsonElement("ImageId")]
		public string? ImagePublicId { get; set; } // To delete already present image

        [BsonElement("ImageHash")]
		public string ImageHash { get; set; } // To check whether image already in cloudinary.

	}
}
