
//using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Configuration;
using System;
using System.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Net;
using System.IO;
using System.Net.Mail;
using System.Text;
using static WebAPIV3.Data.Entity.EntityClass;
using WebAPIV3.Data.Report;

namespace WebAPIV3.Controllers
{
    //public class HomeController : Controller
    //{
    //    public IActionResult Index()
    //    {
    //        return View();
    //    }
    //}

    [ApiController]
    [Route("api/")]
    public class HRCommunicationController : ControllerBase
    {
        private readonly SqlConnection _sqlCon;
        //private readonly MySqlConnection _mysqlCon;

        public HRCommunicationController(IConfiguration configuration)
        {
            // ✅ Initialize SQL Server Connection
            string? sqlConnectionString = configuration.GetConnectionString("Medskls");
            if (string.IsNullOrEmpty(sqlConnectionString))
                throw new Exception("SQL Connection String is missing in appsettings.json");
            _sqlCon = new SqlConnection(sqlConnectionString);

            //// ✅ Initialize MySQL Connection
            //string? mysqlConnectionString = configuration.GetConnectionString("LMS");
            //if (string.IsNullOrEmpty(mysqlConnectionString))
            //    throw new Exception("MySQL Connection String is missing in appsettings.json");
            //_mysqlCon = new MySqlConnection(mysqlConnectionString);
        }


        //[HttpGet("TestDB")]
        //public IActionResult TestDB()
        //{
        //    try
        //    {
        //        _mysqlCon.Open();
        //        using var cmd = new MySqlCommand("SELECT DATABASE();", _mysqlCon);
        //        var result = cmd.ExecuteScalar()?.ToString();
        //        _mysqlCon.Close();

        //        return Ok(new { message = "Connected Successfully", database = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}



        //[HttpPost("HRRequest")]
        //public async Task<IActionResult> HRCommunication([FromForm] HRComm param)
        //{
        //    try
        //    {
        //        if (param == null)
        //            return BadRequest(new ErrorResponse { StatusCode = "9001", Message = "Invalid Request" });

        //        await _sqlCon.OpenAsync();
        //        using var cmd = new SqlCommand("[API_Insert_HRCommunicationByEmployeeID]", _sqlCon)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        cmd.Parameters.AddWithValue("@EmployeeID", param.EmployeeID);
        //        cmd.Parameters.AddWithValue("@Subject", param.Subject);
        //        cmd.Parameters.AddWithValue("@Description", param.Description);
        //        cmd.Parameters.AddWithValue("@RequestType", param.RequestType);
        //        cmd.Parameters.AddWithValue("@RequestQueryID", param.RequestQueryID);
        //        cmd.Parameters.AddWithValue("@Status", param.Status);

        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        DataSet ds = new DataSet();
        //        da.Fill(ds);

        //        if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        //            return Ok(new ErrorResponse { StatusCode = "8004", Message = "Couldn't attempt at this time." });

        //        DataTable dt = ds.Tables[0];
        //        DataTable? dt1 = ds.Tables.Count > 1 ? ds.Tables[1] : null;

        //        string? employeeName = dt.Rows[0]["EmployeeName"].ToString();
        //        string? emailTo = dt.Rows[0]["ContactEmail"].ToString();

        //        StringBuilder emailBody = new StringBuilder();
        //        emailBody.AppendFormat($@"
        //            <p>Dear HR,</p>
        //            <p><strong>{employeeName}</strong> has submitted a request for {param.Subject}.</p>
        //            <p>Details:<br/>{param.Description}</p>");

        //        Hashtable paramArray = new()
        //        {
        //            ["[Address]"] = dt.Rows[0]["Address"].ToString(),
        //            ["[LogoURL]"] = dt.Rows[0]["LogoPath"].ToString().Replace("~", ""),
        //            ["[HREmail]"] = dt.Rows[0]["ContactEmail"].ToString(),
        //            ["[ContactNumber]"] = dt.Rows[0]["ContactNumber"].ToString(),
        //            ["[TemplateBody]"] = emailBody.ToString()
        //        };

                
        //        DataTable dt2 = new DataTable();
        //        byte[]? reportBytes = null;
        //        if (param.RequestType == 2)  // 🔹 Generate Report for Salary Change Request
        //        {
        //            dt2 = GetCourses(Convert.ToInt32(param.EmployeeID));
        //            DataSet reportData = new DataSet();
        //            ReportService reportService = new ReportService();

        //            if (dt1.Rows.Count >= 1)
        //            {
        //                reportData.Tables.Add(dt1.Copy());
        //            }

        //            if (dt2.Rows.Count > 1)
        //            {
        //                reportData.Tables.Add(dt2.Copy());
        //                reportBytes = reportService.GenerateReport(reportData);
        //            }
        //            else
        //            {
        //                return Ok(new ErrorResponse { StatusCode = "8005", Message = "-" + dt2.Rows[0][0].ToString() });

        //            }


        //        }
        //        return Ok(new ErrorResponse { StatusCode = "8005", Message = "-" + dt2.Rows[0][0].ToString() });

        //        // string[] recipients = { emailTo };
        //        //string[] recipients = { "khaqan.haider@tme.edu.pk" };

        //        // bool emailSent = SendEmail("HREmails.html", recipients, "Request Submitted by " + employeeName, emailBody.ToString(), reportBytes);


        //        //return emailSent
        //        //? Ok(new ErrorResponse { StatusCode = "8000", Message = "Added" })
        //        //: Ok(new ErrorResponse { StatusCode = "8004", Message = "Couldn't attempt at this time." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new ErrorResponse { StatusCode = "9000", Message = "General failure: " + ex.Message });
        //    }
        //}


        //private DataTable GetCourses(int employeeId)
        //{
        //    DataTable dt = new DataTable();

        //    try
        //    {
        //        _mysqlCon.Open();

        //        using var cmd = new MySqlCommand(@"
        //    SELECT DISTINCT 
        //        b.name AS Campus,
        //        CASE 
        //            WHEN cls.title IN ('Reception', 'Playgroup', 'Play Group', 'Junior Year', 'Advanced Year') THEN 'The Millennium Early Years'
        //            WHEN cls.title IN ('Grade-1', 'Grade-2', 'Grade-3', 'Grade-4', 'Grade-5') THEN 'The Millennium Primary Years'
        //            WHEN cls.title IN ('Grade-6', 'Grade-7', 'Grade-8', 'Grade-8(Matric)') THEN 'The Millennium Middle Years'
        //            WHEN cls.title IN ('Grade-9', 'Grade-10', 'IGCSE-I', 'IGCSE-II', 'IGCSE-III', 'MYP', 'Matriculation', 'O Level', 'O Level-1', 'O Level-2', 'O Level-3', 'O Levels/IGCSE') THEN 'The Millennium Upper Middle Years'
        //            WHEN cls.title IN ('A-I', 'A-II', 'A-Levels', 'A Levels') THEN 'The Millennium College Years'
        //        END AS AcademicTier,
        //        cls.title AS Class, 
        //        c.name AS Subject, 
        //        u.full_name, 
        //        u.emp_id
        //    FROM courses_owners co
        //    INNER JOIN `user` u ON co.course_owner = u.emp_id
        //    INNER JOIN `courses` c ON c.id = co.course_id
        //    INNER JOIN classes cls ON cls.class_id = c.class_id
        //    INNER JOIN branches b ON b.branch_id = co.branch
        //    WHERE u.emp_id = " + employeeId + @"
        //    ORDER BY b.name, cls.title, u.full_name", _mysqlCon);

        //        // ✅ Using Parameters to Prevent SQL Injection
        //        //cmd.Parameters.AddWithValue("@employeeId", employeeId);

        //        using MySqlDataAdapter da = new MySqlDataAdapter(cmd);
        //        da.Fill(dt);  // ✅ Fill DataTable with the query result

        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        dt.Columns.Add("ErrorMessage", typeof(string)); // Ensure the table has at least one column
        //        dt.Rows.Add("Error: " + ex.Message);
        //        return dt;
        //        //Console.WriteLine("Error: " + ex.Message);
        //        //return dt;
        //    }
        //    finally
        //    {
        //        if (_mysqlCon.State == ConnectionState.Open)
        //            _mysqlCon.Close();
        //    }
        //}

        //private bool SendEmail(string template, string[] recipients, string subject, string body, byte[] reportBytes)
        //{
        //    try
        //    {
        //        MailMessage mail = new()
        //        {
        //            From = new MailAddress("erp.notifications@millenniumschools.edu.pk", "TME"),
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        };
        //        mail.Bcc.Add(new MailAddress("waqas.ahmed@tme.edu.pk"));

        //        foreach (var recipient in recipients)
        //            mail.To.Add(new MailAddress(recipient));

        //        if (reportBytes != null)
        //        {
        //            MemoryStream ms = new MemoryStream(reportBytes);
        //            mail.Attachments.Add(new Attachment(ms, "EmployeeInfo.pdf", "application/pdf"));
        //        }

        //        //if (!string.IsNullOrEmpty(attachmentPath) && System.IO.File.Exists(attachmentPath))
        //        //    mail.Attachments.Add(new Attachment(attachmentPath));

        //        using SmtpClient smtp = new("smtp.office365.com", 587)
        //        {

        //            Port = 587,
        //            Credentials = new NetworkCredential("erp.notifications@tme.edu.pk", "!@#Millennium!@#"),
        //            EnableSsl = true,
        //            TargetName = "STARTTLS/smtp.office365.com"
        //        };

        //        smtp.Send(mail);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

    }
}
