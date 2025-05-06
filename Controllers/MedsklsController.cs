using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using MySql.Data.MySqlClient;
using static WebAPIV3.Data.Entity.EntityClass;
using System.Data;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.ReportingServices.Diagnostics.Internal;
using DataSet = System.Data.DataSet;
using System.Collections;
using System.Text;
using WebAPIV3.Data.Report;
using static WebAPIV3.Data.Entity.MedsklsClass;
using MySqlX.XDevAPI.Common;
using Aspose.Pdf.Operators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Reporting.Map.WebForms.BingMaps;

namespace WebAPIV3.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MedsklsController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly SqlConnection _sqlCon;
        //private readonly MySqlConnection _mysqlCon;

        public MedsklsController(IConfiguration configuration)
        {
            // ✅ Initialize SQL Server Connection
            //string sqlConnectionString = configuration.GetConnectionString("Medskls");
            string sqlConnectionString = "Data Source=210.56.11.158; USER ID=SA; PASSWORD=Test@123; Initial Catalog=MedsklsDB; Encrypt=true; TrustServerCertificate=True; Integrated Security=False; Connection Timeout=500; Min Pool Size=20; Max Pool Size=500; Pooling=True;";
            if (string.IsNullOrEmpty(sqlConnectionString))
                throw new Exception("SQL Connection String is missing in appsettings.json");
            _sqlCon = new SqlConnection(sqlConnectionString);

            //// ✅ Initialize MySQL Connection
            //string? mysqlConnectionString = configuration.GetConnectionString("LMS");
            //if (string.IsNullOrEmpty(mysqlConnectionString))
            //    throw new Exception("MySQL Connection String is missing in appsettings.json");
            //_mysqlCon = new MySqlConnection(mysqlConnectionString);


        }


        [HttpPost]
        [Route("TestAccount")]
        public async Task<IActionResult> GetApplications([FromForm] RequestParamater param)
        {
            
            ErrorResponse errorResp = new ErrorResponse();

            try
            {
                if (param == null)
                    return BadRequest(new ErrorResponse { StatusCode = "9001", Message = "Invalid Request" });

                await _sqlCon.OpenAsync();

                using (var command = _sqlCon.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from Users";

                    var dataTable = new DataTable();
                    var dataSet = new DataSet();

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    // signle table return
                    if (dataTable.Rows.Count > 0)
                    {
                        // Convert DataTable to a list of dictionaries
                        var records = dataTable.AsEnumerable()
                            .Select(row => dataTable.Columns
                                .Cast<DataColumn>()
                                .ToDictionary(
                                    col => col.ColumnName,
                                    col => row[col] is DBNull ? null : row[col] // Handle DBNull
                                ))
                            .ToList();

                        return Ok(new
                        {
                            Success = true,
                            Data = records,
                            Count = records.Count
                        });
                    }

                    //// Convert all tables in the DataSet
                    //if (dataSet.Tables.Count > 1)
                    //{
                    //    var result = new Dictionary<string, object>();
                    //    foreach (DataTable table in dataSet.Tables)
                    //    {
                    //        result[table.TableName] = table.AsEnumerable()
                    //            .Select(row => table.Columns
                    //                .Cast<DataColumn>()
                    //                .ToDictionary(
                    //                    col => col.ColumnName,
                    //                    col => row[col] is DBNull ? null : row[col]
                    //                ))
                    //            .ToList();
                    //    }
                    //    return Ok(new
                    //    {
                    //        Success = true,
                    //        Data = result,
                    //        Count = result.Count
                    //    });
                    //}

                    else
                    {
                        return Ok(new
                        {
                            Success = false,
                            Message = "No records found",
                            StatusCode = "8004"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("UserRegistration")]
        public async Task<IActionResult> UserRegistration([FromForm] RequestParamater param)
        {

            ErrorResponse errorResp = new ErrorResponse();

            try
            {
                await _sqlCon.OpenAsync();

                string passwordForEncryption = "sA23(^A1&*%1)01Ax)@!21!@#$%^&*()7984651";
                string encryptedstring = StringCipher.Encrypt(param.Password, passwordForEncryption);

                string storedPassword = encryptedstring;
                string decryptedPassword = StringCipher.Decrypt(storedPassword, passwordForEncryption);

                var password = encryptedstring;

                //var dob = param.DOB;
                //string db2 = dob.ToString("MM-dd-yyyy");

                using (var command = _sqlCon.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[API_RegisterUser]";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Username", param.UserName);
                    command.Parameters.AddWithValue("@Email", param.Email);
                    command.Parameters.AddWithValue("@PasswordHash", password);
                    command.Parameters.AddWithValue("@FullName", param.FullName);
                    command.Parameters.AddWithValue("@DOB", param.DOB);
                    command.Parameters.AddWithValue("@Gender", param.Gender);
                    command.Parameters.AddWithValue("@Phone", param.Mobile);
                    command.Parameters.AddWithValue("@Address", param.PostalAddress);
                     
                    var dataTable = new DataTable();
                    var dataSet = new DataSet();

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    // signle table return
                    if (dataTable.Rows.Count > 0)
                    {
                        // Convert DataTable to a list of dictionaries
                        var records = dataTable.AsEnumerable()
                            .Select(row => dataTable.Columns
                                .Cast<DataColumn>()
                                .ToDictionary(
                                    col => col.ColumnName,
                                    col => row[col] is DBNull ? null : row[col] // Handle DBNull
                                ))
                            .ToList();

                        return Ok(new
                        {
                            Success = true,
                            Data = records,
                            Count = records.Count
                        });
                    }



                    else
                    {
                        return Ok(new
                        {
                            Success = false,
                            Message = "No records found",
                            StatusCode = "8004"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("GetUserLogin")]
        public async Task<IActionResult> GetUserLogin([FromForm] RequestParamater param)
        {

            ErrorResponse errorResp = new ErrorResponse();

            try
            {
                await _sqlCon.OpenAsync();

                string passwordForEncryption = "sA23(^A1&*%1)01Ax)@!21!@#$%^&*()7984651";
                string encryptedstring = StringCipher.Encrypt(param.Password, passwordForEncryption);

                
                var password = encryptedstring;

                using (var command = _sqlCon.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[API_Userlogin]";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Username", param.UserName);
                    command.Parameters.AddWithValue("@Password", password);

                    var dataTable = new DataTable();
                    var dataSet = new DataSet();

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    


                    // signle table return
                    if (dataTable.Rows.Count > 0)
                    {
                        string storedPassword = dataTable.Rows[0]["PasswordHash"].ToString();
                        string decryptedPassword = StringCipher.Decrypt(storedPassword, passwordForEncryption);

                        if (decryptedPassword != param.Password)
                        {
                            return Ok(new
                            {
                                Success = false,
                                Message = "Incorrect username or password",
                                StatusCode = "8005"
                            });
                        }
                        else
                        {
                            // Convert DataTable to a list of dictionaries
                            var records = dataTable.AsEnumerable()
                                .Select(row => dataTable.Columns
                                    .Cast<DataColumn>()
                                    .ToDictionary(
                                        col => col.ColumnName,
                                        col => row[col] is DBNull ? null : row[col] // Handle DBNull
                                    ))
                                .ToList();

                            return Ok(new
                            {
                                Success = true,
                                Data = records,
                                Count = records.Count
                            });
                        }
                            
                    }



                    else
                    {
                        return Ok(new
                        {
                            Success = false,
                            Message = "No records found",
                            StatusCode = "8004"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("GetUserList")]
        public async Task<IActionResult> GetUserList([FromForm] RequestParamater param)
        {

            ErrorResponse errorResp = new ErrorResponse();

            try
            {
                await _sqlCon.OpenAsync();

                using (var command = _sqlCon.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[API_UserList]";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@userID", param.UserID);

                    var dataTable = new DataTable();
                    var dataSet = new DataSet();

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    // signle table return
                    if (dataTable.Rows.Count > 0)
                    {
                        // Convert DataTable to a list of dictionaries
                        var records = dataTable.AsEnumerable()
                            .Select(row => dataTable.Columns
                                .Cast<DataColumn>()
                                .ToDictionary(
                                    col => col.ColumnName,
                                    col => row[col] is DBNull ? null : row[col] // Handle DBNull
                                ))
                            .ToList();

                        return Ok(new
                        {
                            Success = true,
                            Data = records,
                            Count = records.Count
                        });
                    }



                    else
                    {
                        return Ok(new
                        {
                            Success = false,
                            Message = "No records found",
                            StatusCode = "8004"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile([FromForm] RequestParamater param)
        {

            ErrorResponse errorResp = new ErrorResponse();

            try
            {
                await _sqlCon.OpenAsync();

                using (var command = _sqlCon.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[API_UserList]";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@userID", param.UserID);

                    var dataTable = new DataTable();
                    var dataSet = new DataSet();

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    // signle table return
                    if (dataTable.Rows.Count > 0)
                    {
                        // Convert DataTable to a list of dictionaries
                        var records = dataTable.AsEnumerable()
                            .Select(row => dataTable.Columns
                                .Cast<DataColumn>()
                                .ToDictionary(
                                    col => col.ColumnName,
                                    col => row[col] is DBNull ? null : row[col] // Handle DBNull
                                ))
                            .ToList();

                        return Ok(new
                        {
                            Success = true,
                            Data = records,
                            Count = records.Count
                        });
                    }



                    else
                    {
                        return Ok(new
                        {
                            Success = false,
                            Message = "No records found",
                            StatusCode = "8004"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("UpdateUserStatus")]
        public async Task<IActionResult> UpdateUserStatus([FromForm] RequestParamater param)
        {

            ErrorResponse errorResp = new ErrorResponse();

            try
            {
                await _sqlCon.OpenAsync();

                using (var command = _sqlCon.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[API_UpdateUserStatus]";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserID", param.UserID);
                    command.Parameters.AddWithValue("@StatusID", param.StatusID);
                    var dataTable = new DataTable();
                    var dataSet = new DataSet();

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    // signle table return
                    if (dataTable.Rows.Count > 0)
                    {
                        // Convert DataTable to a list of dictionaries
                        var records = dataTable.AsEnumerable()
                            .Select(row => dataTable.Columns
                                .Cast<DataColumn>()
                                .ToDictionary(
                                    col => col.ColumnName,
                                    col => row[col] is DBNull ? null : row[col] // Handle DBNull
                                ))
                            .ToList();

                        return Ok(new
                        {
                            Success = true,
                            Data = records,
                            Count = records.Count
                        });
                    }



                    else
                    {
                        return Ok(new
                        {
                            Success = false,
                            Message = "No records found",
                            StatusCode = "8004"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred",
                    Error = ex.Message
                });
            }
        }

    }
}
