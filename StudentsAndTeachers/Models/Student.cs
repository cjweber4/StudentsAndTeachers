using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentsAndTeachers.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        [RegularExpression(@"^[0-9a-zA-Z ]+$", ErrorMessage = "Use letters and numbers only please")]
        [Required(ErrorMessage = "The Student Number field is required.")]
        public string StudentNumber { get; set; }
        [Required(ErrorMessage = "The First Name field is required.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Use letters only please")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "The Last Name field is required.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Use letters only please")]
        public string LastName { get; set; }
        [Required]
        public string HasScholarship { get; set; }
    }
}