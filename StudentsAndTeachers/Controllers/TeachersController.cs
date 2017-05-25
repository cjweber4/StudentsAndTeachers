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
    public class TeachersController : Controller
    {
        private readonly Context _context;
        private static bool imported = false;

        public TeachersController(Context context)
        {
            _context = context;    
        }

        // GET: Teachers
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TeacherIDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "teacherid_desc" : "";
            ViewData["FirstNameSortParm"] = sortOrder == "firstname_asc" ? "firstname_desc" : "firstname_asc";
            ViewData["LastNameSortParm"] = sortOrder == "lastname_asc" ? "lastname_desc" : "lastname_asc";
            ViewData["NumberOfStudentsSortParm"] = sortOrder == "numstudents_asc" ? "numstudents_desc" : "numstudents_asc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var teachers = from t in _context.Teachers
                           select t;
            if (!String.IsNullOrEmpty(searchString))
            {
                int hold;
                string[] names = searchString.Split(' ');
                if (Int32.TryParse(searchString, out hold))
                {
                    teachers = teachers.Where(t => t.TeacherID == hold);
                }
                else if (names.Length == 2)
                {
                    teachers = teachers.Where(t => t.FirstName.Contains(names[0])
                                          && t.LastName.Contains(names[1]));

                }
                else
                {
                    teachers = teachers.Where(t => t.LastName.Contains(searchString)
                                           || t.FirstName.Contains(searchString));
                }

            }
            switch (sortOrder)
            {
                case "lastname_asc":
                    teachers = teachers.OrderBy(t => t.LastName);
                    break;
                case "lastname_desc":
                    teachers = teachers.OrderByDescending(t => t.LastName);
                    break;
                case "firstname_asc":
                    teachers = teachers.OrderBy(t => t.FirstName);
                    break;
                case "firstname_desc":
                    teachers = teachers.OrderByDescending(t => t.FirstName);
                    break;
                case "teacherid_desc":
                    teachers = teachers.OrderByDescending(t => t.TeacherID);
                    break;
                case "numstudents_asc":
                    teachers = teachers.OrderBy(t => t.NumberOfStudents);
                    break;
                case "numstudents_desc":
                    teachers = teachers.OrderByDescending(t => t.NumberOfStudents);
                    break;
                default:
                    teachers = teachers.OrderBy(t => t.TeacherID);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Teacher>.CreateAsync(teachers.AsNoTracking(), page ?? 1, pageSize));

        }


        // GET: Teachers/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Teachers/Import
        [HttpPost, ActionName("Import")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(string s)
        {
            if (!imported)
            {
                imported = true;

                var readcsv2 = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data\\Teachers.csv"));
                string[] csvfilerecord2 = readcsv2.Split('\n');
                foreach (var row in csvfilerecord2.Skip(1))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        var teacher = new Teacher
                        {
                            FirstName = cells[1],
                            LastName = cells[2],
                        };

                        _context.Add(teacher);

                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            } else
            {
                return RedirectToAction("Index");

            }
        }


        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,NumberOfStudents")] Teacher teacher)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    _context.Add(teacher);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes, try again.");
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.SingleOrDefaultAsync(m => m.TeacherID == id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeacherID,FirstName,LastName,NumberOfStudents")] Teacher teacher)
        {
            if (id != teacher.TeacherID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.TeacherID))
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
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .SingleOrDefaultAsync(m => m.TeacherID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.SingleOrDefaultAsync(m => m.TeacherID == id);
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.TeacherID == id);
        }

        // GET: Teachers/DeleteAll
        public IActionResult DeleteAll()
        {
            return View();
        }

        // POST: Teachers/DeleteAll
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(string s)
        {
            _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [Teacher]");
            imported = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}
