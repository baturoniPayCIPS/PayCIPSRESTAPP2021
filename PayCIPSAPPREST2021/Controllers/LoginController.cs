using PayCIPSAPPREST2021.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace PayCIPSAPPREST2021.Controllers
{
    public class LoginController : ApiController
    {
        public class clsLoginInputData : clsBaseInputData
        {
            public string Password { get; set; }
            public string AppVersionNumber { get; set; }
        }

        public class clsLoginOutputData : clsBaseOutputData
        {
            public clsComercio Comercio { get; set; }
            public clsMenu[] Menu { get; set; }
        }

        public class clsComercio
        {
            public string Id { get; set; }
            public string Tipo { get; set; }
            public string Nombre { get; set; }
            public string CLABE { get; set; }
            public string CIE { get; set; }
            public clsTarjeta Tarjeta { get; set; }
        }

        public class clsTarjeta
        {
            public string PAN { get; set; }
            public string FecExp { get; set; }
            public string Tipo { get; set; }
            public bool Status { get; set; }

        }
        public class clsAdquirente
        {
            public bool TPV { get; set; }
            public bool Tokenizacion { get; set; }
            public bool Formulario { get; set; }
        }

        public class clsSPEI
        {
            public bool Enviar { get; set; }
            public bool Recibir { get; set; }
        }

        public class clsCoDi
        {
            public bool CobroPresencial { get; set; }
            public bool CobroRemoto { get; set; }
        }

        public class clsPagoEfectivo
        {
            public bool Oxxo24 { get; set; }
            public bool PayNet { get; set; }
            public bool CIE { get; set; }

        }
        public class clsMenu
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public string Etiqueta { get; set; }
            public string Titulo { get; set; }
            public string Image { get; set; }
            public string Order { get; set; }
            public string OnClick { get; set; }
            public string Param { get; set; }
            public string ColorBG { get; set; }
            public string ColorFG { get; set; }
        }
        // POST: api/Login
        public clsLoginOutputData Post(string TerminalSerialNumber, [FromBody] clsLoginInputData value)
        {
            clsLoginOutputData mLoginOutput = new clsLoginOutputData();
            
            if (value is null)
            {
                GeneralFunctions.GrabaLog("01 Acceso Rechazado. El documento JSON de entrada NO es válido." ,"<-", TerminalSerialNumber, "/api/Login", "POST");
                mLoginOutput.RespCode = "01"; 
                mLoginOutput.RespText = "Acceso Rechazado";
                mLoginOutput.Status = false;
                return mLoginOutput;
            }
            GeneralFunctions.GrabaLog(JsonConvert.SerializeObject(value),"->", TerminalSerialNumber, "/api/Login", "POST");
            ExecDB(value.UserName, value.Password, TerminalSerialNumber, value.Latitud, value.Longitud, value.CallingIPAddress, value.AppVersionNumber, mLoginOutput);
            GeneralFunctions.GrabaLog(JsonConvert.SerializeObject(mLoginOutput), "<-", TerminalSerialNumber, "/api/Login", "POST");

            return mLoginOutput;
        }

        private bool ExecDB(string UserName, string Password, string TerminalSerialNumber, string Latitud, string Longitud, string IPAddress, string APPVerNo, clsLoginOutputData Salida) {
            SqlConnection myDBConnection= new SqlConnection();
            DataSet rs = new DataSet();

            if (!GeneralFunctions.GetDBConnection(myDBConnection))
            {
                GeneralFunctions.GrabaLog("PAYCIPS_APP_USUARIO_SUP: Error conectando con Base de Datos." , "ERR", TerminalSerialNumber, "/api/Login", "ExecDB");
                Salida.Status = false;
                Salida.RespCode = "91";
                Salida.RespText = "Acceso Denegado. Intente más tarde.";
                rs.Dispose();
                myDBConnection.Close();
                myDBConnection.Dispose();
                return false;
            }
            SqlCommand mObjCmd = new SqlCommand();
            mObjCmd.CommandType = CommandType.StoredProcedure;
            mObjCmd.CommandText = "PAYCIPS_APP_USUARIO_SUP";
            mObjCmd.Parameters.AddWithValue("@USUARIO", UserName);
            mObjCmd.Parameters.AddWithValue("@USERPASS", Password);
            mObjCmd.Parameters.AddWithValue("@TERMSERIALNO", TerminalSerialNumber);
            mObjCmd.Parameters.AddWithValue("@LAT", Latitud);
            mObjCmd.Parameters.AddWithValue("@LON", Longitud);
            mObjCmd.Parameters.AddWithValue("@IP", IPAddress);
            mObjCmd.Parameters.AddWithValue("@APPVER", APPVerNo);
            mObjCmd.Connection = myDBConnection;

            SqlDataAdapter myCommand = new SqlDataAdapter(mObjCmd);
            try
            {
                myCommand.Fill(rs, "Resultado");
            }
            catch (Exception e)
            {
                string mDummy = e.Message;
                GeneralFunctions.GrabaLog("PAYCIPS_APP_USUARIO_SUP: " + mDummy,"ERR", TerminalSerialNumber, "/api/Login", "ExecDB");
                Salida.Status = false;
                Salida.RespCode = "99";
                Salida.RespText = mDummy;
                rs.Dispose();
                myDBConnection.Close();
                myDBConnection.Dispose();
                return false;
            }
            //Aquí se llena la información que viene de la base de datos
            Salida.Status = false;
            Salida.RespCode = "00";
            Salida.RespText = "Acceso Permitido";
            rs.Dispose();
            myDBConnection.Close();
            myDBConnection.Dispose();
            return true;
        }

    }
}
