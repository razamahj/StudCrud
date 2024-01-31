using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudCrud.Data;
using StudCrud.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace StudCrud.Controllers
{
    

    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Other actions...


        [HttpGet]
        public IActionResult Index()
        {
            var studentsWithSubjects = _context.Students
                .Include(student => student.Subject)  // Assuming a navigation property 'Subject' in the Student entity
                .ToList();

            return View("Index", studentsWithSubjects);
        }



        public IActionResult AddStudents()
        {
            return View();
        }



        [HttpPost]
        public IActionResult UploadStudents(IFormFile file)
        {

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    // Read the headers
                    var headers = reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        var student = new Student
                            {
                                First_name = values[0],
                                Last_name = values[1],
                                SubjectId = 1
                            };

                            if (DateTime.TryParseExact(values[2], "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
                            {
                                student.Dob = dob;
                            }
                            else
                            {
                                ModelState.AddModelError("Dob", $"Invalid date format: {values[2]}");
                                continue; // Skip this row and move to the next one
                            }

                            _context.Students.Add(student);
                        }
                      
                    }
                

                _context.SaveChanges();

                TempData["Message"] = "Students data uploaded successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error uploading students data: {ex.Message}";
            }

            return RedirectToAction("Index"); // Redirect to the Index view or wherever appropriate
        }


        [HttpPost]
        public IActionResult AddSubjects(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file");
                return View("Index"); // Redirect to the Index view or wherever appropriate
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split('\t'); // Assuming tab-separated values

                        var subject = new Subject
                        {
                            SubjectName = values[1]
                            // Add more properties as needed
                        };

                        _context.Subjects.Add(subject);
                    }
                }

                _context.SaveChanges();

                TempData["Message"] = "Subjects data uploaded successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error uploading subjects data: {ex.Message}";
            }

            return RedirectToAction("Index"); // Redirect to the Index view or wherever appropriate
        }



        public IActionResult Update(int id)
        {
            // Fetch the student from the database based on the id
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound(); 
            }

            ViewBag.Subjects = new SelectList(_context.Subjects.ToList(), "Id", "SubjectName");

            return View("Update", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Student student)
        {
            
                _context.Students.Update(student);
                _context.SaveChanges();

                TempData["Message"] = "Student updated successfully.";
                return RedirectToAction("Update");
         

            ViewBag.Subjects = new SelectList(_context.Subjects.ToList(), "Id", "SubjectName");
            return View("Update", student);
        }


    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound(); 
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}
