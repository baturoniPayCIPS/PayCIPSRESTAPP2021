using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Net;
using System.Data.SqlClient;


namespace PayCIPSAPPREST2021.Models
{
    public class clsBaseInputData
    {
        public string UserName { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string CallingIPAddress { get; set; }
    }

    public class clsBaseOutputData
    {
        public bool Status { get; set; }
        public string RespCode { get; set; }
        public string RespText { get; set; }
        public string RespTimeStamp
        {
            get
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                return date;
            }
        }
    }

    public class GeneralFunctions
    {
        public static Boolean GetDBConnection(SqlConnection db)
        {
            string mConnectionString;
            try
            {
                mConnectionString = ConfigurationManager.ConnectionStrings["PayCIPSAppDB"].ConnectionString;
                db.ConnectionString = mConnectionString;
                db.Open();
            }
            catch (Exception e)
            {
                GrabaLog(e.Message, "DB","","DBAccess","GetDBConnection");
                return false;
            }                   
            return true;
        }
        public static void GrabaLog(string Mensaje, string Bandera,string TerminalSerialNumber, string CallingModule, string CallingFunction)
        {
            string mFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePathtoSrv"].ToString();
            if (!Directory.Exists(mFilePath))                    
                Directory.CreateDirectory(mFilePath);

            string mFilePrefix = System.Configuration.ConfigurationManager.AppSettings["LogFilePrefixtoSrv"].ToString();
            string mFileName= mFilePath + "\\" + mFilePrefix + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            using (System.IO.TextWriter writer = File.AppendText(mFileName))
            {
                if (Bandera != "RAW") {
                    writer.Write(DateTime.Now.ToString("HH:mm:ss"));
                    writer.Write("\t");
                    writer.Write(CallingModule);
                    writer.Write("\t");
                    writer.Write(CallingFunction);
                    writer.Write("\t");
                    writer.Write(Bandera);
                    writer.Write("\t");
                    writer.Write(TerminalSerialNumber);
                    writer.Write("\t");
                }
                writer.WriteLine(Mensaje);                                
            }
        }
    }
}