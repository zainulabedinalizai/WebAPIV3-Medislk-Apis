
using System;
using System.Data;
using System.IO;
using AspNetCore.Reporting;
//using Microsoft.Reporting.WebForms;


namespace WebAPIV3.Data.Report
{
    public class ReportService
    {
        private readonly string _reportPath;

        public ReportService()
        {
            _reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "EmployeeProfile.rdlc");
            if (!File.Exists(_reportPath))
            {
                throw new FileNotFoundException("Report file not found!", _reportPath);
            }
        }

        public byte[] GenerateReport(DataSet ds)
        {
            try
            {
                LocalReport report = new LocalReport(_reportPath);

                report.AddDataSource("EmployeeProfile", ds.Tables[0]);
                report.AddDataSource("LmsCourses", ds.Tables[1]);

                var result = report.Execute(RenderType.Pdf, 1, null, "");

                return result.MainStream;
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating RDLC report: " + ex.Message);
            }
        }
    }
}


