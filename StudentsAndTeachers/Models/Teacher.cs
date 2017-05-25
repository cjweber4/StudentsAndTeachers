using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentsAndTeachers.Models
{
    public class Teacher
    {
        
        public int TeacherID { get; set; }
        [Required(ErrorMessage = "The First Name field is required.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Use letters only please")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "The Last Name field is required.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Use letters only please")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "The Number of Students field is required.")]
        public int NumberOfStudents { get; set; }
    }
}
