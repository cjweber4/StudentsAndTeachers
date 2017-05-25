using StudentsAndTeachers.Models;
using System;
using System.IO;
using System.Linq;

namespace StudentsAndTeachers.Data
{
    public static class DbInitializer
    {
        public static void Initialize(Context context)
        {
            context.Database.EnsureCreated();

        }
    }
}