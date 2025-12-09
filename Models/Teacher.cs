using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoDB.Bson;

namespace SchoolColab.Models
{
	[BsonIgnoreExtraElements]
	public class Teacher
    {

		[BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Teacher_Name")]
        public string Name { get; set; }

        [BsonElement("Teacher_Degree")]
	    public string Degree { get; set; }

        [BsonElement("Selected_Courses")]
	    public List<string> SelectedCourses { get; set; }

		[BsonElement("ProfileImageUrl")]
		public string? ProfileImage { get; set; }

		[BsonElement("ImageId")]
		public string? ImagePublicId { get; set; } 

		[BsonElement("ImageHash")]
		public string ImageHash { get; set; }
	}
}
