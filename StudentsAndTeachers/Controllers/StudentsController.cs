using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentsAndTeachers.Data;
using StudentsAndTeachers.Models;
using System.IO;

namespace StudentsAndTeachers.Controllers
{
    public class StudentsController : Controller
    {
        private readonly Context _context;
        private static bool imported = false;

        public StudentsController(Context context)
        {
            _context = context;    
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentIDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "studentid_desc" : "";
            ViewData["FirstNameSortParm"] = sortOrder == "firstname_asc" ? "firstname_desc" : "firstname_asc";
            ViewData["LastNameSortParm"] = sortOrder == "lastname_asc" ? "lastname_desc" : "lastname_asc";
            ViewData["StudentNumberSortParm"] = sortOrder == "studentnumber_asc" ? "studentnumber_desc" : "studentnumber_asc";
            ViewData["HasScholarshipSortParm"] = sortOrder == "hasscholarship_asc" ? "hasscholarship_desc" : "hasscholarship_asc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var students = from s in _context.Students
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                int hold;
                string[] names = searchString.Split(' ');
                if (Int32.TryParse(searchString, out hold))
                {
                    students = students.Where(s => s.StudentID == hold);
                } else if(names.Length == 2)
                {
                    students = students.Where(s => s.FirstName.Contains(names[0])
                                          && s.LastName.Contains(names[1]));

                } else
                {
                    students = students.Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString)
                                           || s.StudentNumber.Contains(searchString));
                }
                
            }
            switch (sortOrder)
            {
                case "lastname_asc":
                    students = students.OrderBy(s => s.LastName);
                    break;
                case "lastname_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "firstname_asc":
                    students = students.OrderBy(s => s.FirstName);
                    break;
                case "firstname_desc":
                    students = students.OrderByDescending(s => s.FirstName);
                    break;
                case "studentnumber_asc":
                    students = students.OrderBy(s => s.StudentNumber);
                    break;
                case "studentnumber_desc":
                    students = students.OrderByDescending(s => s.StudentNumber);
                    break;
                case "studentid_desc":
                    students = students.OrderByDescending(s => s.StudentID);
                    break;
                case "hasscholarship_asc":
                    students = students.OrderBy(s => s.HasScholarship);
                    break;
                case "hasscholarship_desc":
                    students = students.OrderByDescending(s => s.HasScholarship);
                    break;
                default:
                    students = students.OrderBy(s => s.StudentID);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }


        // GET: Students/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Students/Import
        [HttpPost, ActionName("Import")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(string s)
        {
            if (!imported)
            {
                imported = true;
                var readcsv = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data\\Students.csv"));
                string[] csvfilerecord = readcsv.Split('\n');
                int studentID = 0;
                foreach (var row in csvfilerecord.Skip(1))
                {
                    if (studentID == 0)
                    {
                        studentID++;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        bool schol = false;
                        if (cells[4].Equals("Yes"))
                        {
                            schol = true;
                        }
                        var student = new Student
                        {
                            StudentNumber = cells[1],
                            FirstName = cells[2],
                            LastName = cells[3],
                            HasScholarship = cells[4]
                        };


                        _context.Add(student);

                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            } else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentNumber,FirstName,LastName,HasScholarship")] Student student)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes, try again.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.SingleOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentID,StudentNumber,FirstName,LastName,HasScholarship")] Student student)
        {
            if (id != student.StudentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .SingleOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.SingleOrDefaultAsync(m => m.StudentID == id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }


        // GET: Students/DeleteAll
        public IActionResult DeleteAll()
        {
            return View();
        }

        // POST: Students/DeleteAll
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(string s)
        {
             _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [Student]");
            imported = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}
