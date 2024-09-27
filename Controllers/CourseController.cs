using ADOCRUD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace ADOCRUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CourseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET ALL COURSES
        [HttpGet]
        [Route("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            List<CourseModel> courses = new List<CourseModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLCOURSES", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", 1);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    courses.Add(new CourseModel
                    {
                        courseId = Convert.ToInt32(row["courseId"]),
                        courseTitle = row["courseTitle"].ToString(),
                        maxStudents = Convert.ToInt32(row["maxStudents"])
                    });
                }
            }
            return Ok(courses);
        }

        // GET COURSE BY ID
        [HttpGet]
        [Route("GetCourseById/{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            CourseModel course = new CourseModel();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLCOURSES", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", 2);
                cmd.Parameters.AddWithValue("@courseId", id);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    course.courseId = Convert.ToInt32(row["courseId"]);
                    course.courseTitle = row["courseTitle"].ToString();
                    course.maxStudents = Convert.ToInt32(row["maxStudents"]);
                    return Ok(course);
                }
            }
            return NotFound("Course not found");
        }

        // GET STUDENTS BY COURSE ID 
        [HttpGet]
        [Route("GetStudentsByCourseId/{courseId}")]
        public async Task<IActionResult> GetStudentsByCourseId(int courseId)
        {
            List<StudentDto> studentDtos = new List<StudentDto>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("GetStudentsByCourse", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@courseId", courseId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0 || (dt.Rows.Count == 1 && Convert.ToInt32(dt.Rows[0]["InvalidCourseId"]) == -1))
                {
                    return NotFound("Invalid courseId or no students found for this course.");
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StudentDto studentDto = new StudentDto();
                    studentDto.studentId = Convert.ToInt32(dt.Rows[i]["studentId"]);
                    studentDto.studentName = dt.Rows[i]["studentName"].ToString();
                    studentDtos.Add(studentDto);
                }
            }

            return Ok(studentDtos);
        }

        // ADD A COURSE
        [HttpPost]
        [Route("AddCourse")]
        public async Task<IActionResult> AddCourse(CourseModel course)
        {
            if (course == null || string.IsNullOrEmpty(course.courseTitle) || course.maxStudents < 0)
            {
                return BadRequest("Invalid course data");
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLCOURSES", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", 3);
                cmd.Parameters.AddWithValue("@courseId", course.courseId);
                cmd.Parameters.AddWithValue("@courseTitle", course.courseTitle);
                cmd.Parameters.AddWithValue("@maxStudents", course.maxStudents);
                con.Open();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.Close();

                if (rowsAffected > 0)
                {
                    return Ok("Course added successfully");
                }
            }
            return StatusCode(500, "Failed to add course");
        }

        // UPDATE A COURSE
        [HttpPut]
        [Route("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse(CourseModel course)
        {
            if (course == null || string.IsNullOrEmpty(course.courseTitle) || course.courseId <= 0 || course.maxStudents < 0)
            {
                return BadRequest("Invalid course data");
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLCOURSES", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", 4);
                cmd.Parameters.AddWithValue("@courseId", course.courseId);
                cmd.Parameters.AddWithValue("@courseTitle", course.courseTitle);
                cmd.Parameters.AddWithValue("@maxStudents", course.maxStudents);
                con.Open();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.Close();

                if (rowsAffected > 0)
                {
                    return Ok("Course updated successfully");
                }
            }
            return StatusCode(500, "Failed to update course");
        }

        // DELETE A COURSE
        [HttpDelete]
        [Route("DeleteCourse/{courseId}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            if (courseId <= 0)
            {
                return BadRequest("Invalid course ID");
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLCOURSES", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", 5);
                cmd.Parameters.AddWithValue("@courseId", courseId);
                con.Open();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.Close();

                if (rowsAffected > 0)
                {
                    return Ok("Course deleted successfully");
                }
            }
            return StatusCode(500, "Failed to delete course");
        }


    }
}
