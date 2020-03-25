using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentsEmployeesAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DepartmentsEmployeesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = 
                     @"Select e.Id, e.FirstName, e.LastName, e.DepartmentId, d.Id , d.DeptName  
                     FROM Employee e
                     Left Join Department d
                     On DepartmentId = d.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                              Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                              DeptName = reader.GetString(reader.GetOrdinal("DeptName"))

                            }

                        };
                        employees.Add(employee);
                    }
                    reader.Close();
                    return Ok(employees);
                }
            }
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select e.Id, e.FirstName, e.LastName, e.DepartmentId, d.Id , d.DeptName  
                     FROM Employee e
                     Left Join Department d
                     On DepartmentId = d.Id Where e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    if (reader.Read())
                    {
                         employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                DeptName = reader.GetString(reader.GetOrdinal("DeptName"))

                            }

                        };
                        reader.Close();
                    return Ok(employee);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }
    }
}