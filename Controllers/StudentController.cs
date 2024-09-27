using ADOCRUD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ADOCRUD.Controllers
{
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET ALL STUDENTS 
        [HttpGet]
        [Route("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            List<StudentModel> studentModels = new List<StudentModel>();
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLSTUDENT", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@action", 1);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StudentModel studentModel = new StudentModel
                    {
                        studentId = Convert.ToInt32(dt.Rows[i]["studentId"]),
                        studentName = dt.Rows[i]["studentName"].ToString(),
                        age = Convert.ToInt32(dt.Rows[i]["age"]),
                        town = dt.Rows[i]["town"].ToString(),
                        courseId = Convert.ToInt32(dt.Rows[i]["courseId"])
                    };
                    studentModels.Add(studentModel);
                }
            }
            return Ok(studentModels);
        }

        // GET STUDENT BY ID 
        [HttpGet]
        [Route("GetStudentById/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            StudentModel studentModel = new StudentModel();
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLSTUDENT", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@action", 2);
                cmd.Parameters.AddWithValue("@studentId", id);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    studentModel.studentId = Convert.ToInt32(dt.Rows[0]["studentId"]);
                    studentModel.studentName = dt.Rows[0]["studentName"].ToString();
                    studentModel.age = Convert.ToInt32(dt.Rows[0]["age"]);
                    studentModel.town = dt.Rows[0]["town"].ToString();
                    studentModel.courseId = Convert.ToInt32(dt.Rows[0]["courseId"]);
                    return Ok(studentModel);
                }
                return NotFound("Student not found.");
            }
        }

        // POST A STUDENT
        [HttpPost]
        [Route("AddStudent")]
        public async Task<IActionResult> AddStudent(StudentModel student)
        {
            if (student == null || string.IsNullOrEmpty(student.studentName) || student.age <= 0 ||
                string.IsNullOrEmpty(student.town) || student.courseId <= 0 || student.studentId <= 0)
            {
                return BadRequest("Invalid student data");
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLSTUDENT", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@action", 3);
                cmd.Parameters.AddWithValue("@studentId", student.studentId);
                cmd.Parameters.AddWithValue("@studentName", student.studentName);
                cmd.Parameters.AddWithValue("@age", student.age);
                cmd.Parameters.AddWithValue("@town", student.town);
                cmd.Parameters.AddWithValue("@courseId", student.courseId);

                con.Open();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.Close();

                if (rowsAffected > 0)
                {
                    return Ok("Student added successfully");
                }
                else
                {
                    return StatusCode(500, "Failed to add student");
                }
            }
        }

        // PUT STUDENT
        [HttpPut]
        [Route("UpdateStudent")]
        public async Task<IActionResult> UpdateStudent(StudentModel student)
        {
            if (student == null || string.IsNullOrEmpty(student.studentName) || student.age <= 0 ||
                string.IsNullOrEmpty(student.town) || student.courseId <= 0 || student.studentId <= 0)
            {
                return BadRequest("Invalid data");
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLSTUDENT", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@action", 4);
                cmd.Parameters.AddWithValue("@studentId", student.studentId);
                cmd.Parameters.AddWithValue("@studentName", student.studentName);
                cmd.Parameters.AddWithValue("@age", student.age);
                cmd.Parameters.AddWithValue("@town", student.town);
                cmd.Parameters.AddWithValue("@courseId", student.courseId);

                con.Open();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.Close();

                if (rowsAffected > 0)
                {
                    return Ok("Student updated successfully");
                }
                else
                {
                    return StatusCode(500, "Failed to update student");
                }
            }
        }

        // DELETE A STUDENT
        [HttpDelete]
        [Route("DeleteStudent/{studentId}")]
        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_AED_TBLSTUDENT", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@action", 5);
                cmd.Parameters.AddWithValue("@studentId", studentId);

                con.Open();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.Close();

                if (rowsAffected > 0)
                {
                    return Ok("Student deleted successfully");
                }
                else
                {
                    return StatusCode(500, "Failed to delete student");
                }
            }
        }
    }
}
