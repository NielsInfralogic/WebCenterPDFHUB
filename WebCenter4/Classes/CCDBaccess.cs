using System;
using System.Data;
using System.Data.SqlClient;

using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Web;
using System.Web.Caching;
using WebCenter4.Classes;
using System.Collections.Generic;
using System.Globalization;
/*
#define STATUSNUMBER_MISSING			0
#define STATUSNUMBER_POLLING			5
#define STATUSNUMBER_POLLINGERROR		6
#define STATUSNUMBER_POLLED				10
#define STATUSNUMBER_RESAMPLING			15
#define STATUSNUMBER_RESAMPLINGERROR	16
#define STATUSNUMBER_READY				20
#define STATUSNUMBER_TRANSMITTING		25
#define STATUSNUMBER_TRANSMISSIONERROR	26
#define STATUSNUMBER_TRANSMITTED		30
#define STATUSNUMBER_ASSEMBLING			35
#define STATUSNUMBER_ASSEMBLINGERROR	36
#define STATUSNUMBER_ASSEMBLED			40
#define STATUSNUMBER_IMAGING			45
#define STATUSNUMBER_IMAGINGERROR		46
#define STATUSNUMBER_IMAGED				50
#define STATUSNUMBER_VERIFYING			55
#define STATUSNUMBER_VERIFYERROR		56
#define STATUSNUMBER_VERIFIED			60
#define STATUSNUMBER_KILLED				99

#define APPROVAL_NOTAPPROVED			0
#define	APPROVAL_APPROVED				1
#define	APPROVAL_REJECTED				2*/




namespace WebCenter4.Classes
{
    /// <summary>
    /// Summary description for DBaccess.
    /// </summary>
    public class CCDBaccess
    {
        SqlConnection connection;
        SqlConnection connection2;

        public CCDBaccess()
        {
            connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            connection2 = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        }

        public void CloseAll()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
            if (connection2.State == ConnectionState.Open)
                connection2.Close();
        }

        public DataTable GetNamesCollection(String dataSetName, String sStoredProcedureName, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable(dataSetName);
            //	DataSet dataSet = new DataSet(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));
            SqlCommand command = new SqlCommand(sStoredProcedureName, connection2);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["ID"] = reader.GetInt32(0);
                    newRow["Name"] = reader.GetString(1);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //	dataSet.Tables.Add(tblResults);
            //	return dataSet;
            return tblResults;
        }

        public DataTable GetHardProofCollection(out string errmsg)
        {
            errmsg = "";
            DataTable dataTable = new DataTable();

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter("SELECT DISTINCT ProofID AS ID,ProofName AS Name FROM FlatProofConfigurations ORDER BY ProofName", connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }

        public DataTable GetPublicationEditionsCollection(out string errmsg)
        {
            errmsg = "";
            DataTable dataTable = new DataTable();

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter("SELECT DISTINCT PublicationID, EditionID FROM PublicationEditions WHERE PublicationID>0 AND EditionID>0 ORDER BY PublicationID, EditionID", connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }

        public DataTable GetPublicationSectionsCollection(out string errmsg)
        {
            errmsg = "";
            DataTable dataTable = new DataTable();

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter("SELECT DISTINCT PublicationID, SectionID FROM PublicationSections WHERE PublicationID>0 AND SectionID>0 ORDER BY PublicationID, SectionID", connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }



        public DataTable GetPublicationTemplateCollection(out string errmsg)
        {
            errmsg = "";
            DataTable dataTable = new DataTable();

            if (TableExists("PublicationTemplateOptions", out errmsg) != 1)
                return dataTable;

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter("SELECT DISTINCT PublicationID, TemplateID, SeparateRuns FROM PublicationTemplateOptions WHERE PublicationID>0 AND TemplateID>0 ORDER BY PublicationID, TemplateID", connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }



        public DataTable GetUnknownFilesCollection(out string errmsg)
        {
            errmsg = "";
            DataTable dataTable = new DataTable();

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter("SELECT DISTINCT FileName,InputFolder,FileTime FROM UnknownFiles ORDER BY InputFolder,FileName", connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }


        public DataTable GetPublicationCollection(out string errmsg)
        {
            errmsg = "";
            bool hasDeadlineField = FieldExists("PublicationNames", "Deadline", out errmsg) == 1;
            errmsg = "";

            DataTable dataTable = new DataTable();

            string sql = "SELECT DISTINCT PublicationID AS ID,Name,ISNULL(PageFormatID,0) AS PageFormatID,ISNULL(TrimToFormat,0) AS TrimToFormat,ISNULL(LatestHour,0) AS LatestHour,ISNULL(DefaultProofID,0) AS DefaultProofID,ISNULL(DefaultHardProofID,0) AS DefaultHardProofID,ISNULL(DefaultApprove,0) AS DefaultApprove,ISNULL(CustomerID,0) AS CustomerID, ISNULL(DefaultTemplateID,1) AS DefaultPressID FROM PublicationNames WITH (NOLOCK) ORDER BY Name";
            if (hasDeadlineField)
                sql = "SELECT DISTINCT PublicationID AS ID,Name,ISNULL(PageFormatID,0) AS PageFormatID,ISNULL(TrimToFormat,0) AS TrimToFormat,ISNULL(LatestHour,0) AS LatestHour,ISNULL(DefaultProofID,0) AS DefaultProofID,ISNULL(DefaultHardProofID,0) AS DefaultHardProofID,ISNULL(DefaultApprove,0) AS DefaultApprove,ISNULL(CustomerID,0) AS CustomerID,ISNULL(Deadline,0) AS Deadline, ISNULL(DefaultTemplateID,1) AS DefaultPressID FROM PublicationNames WITH (NOLOCK) ORDER BY Name";

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter(sql, connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }




        public DataTable GetRipSetupCollection(int type, out string errmsg)
        {

            errmsg = "";
            //		bool hasDeadlineField = FieldExists("PublicationNames","Deadline", out errmsg) == 1;
            if (type == 2 && TableExists("PreflightSetupNames", out errmsg) != 1)
                return null;
            if (type == 3 && TableExists("InksaveSetupNames", out errmsg) != 1)
                return null;

            DataTable dataTable = new DataTable();

            string sql = "SELECT DISTINCT RipSetupID AS ID,Name FROM RipSetupNames ORDER BY ID";
            if (type == 2)
                sql = "SELECT DISTINCT PreflightSetupID AS ID,Name FROM PreflightSetupNames ORDER BY ID";
            else if (type == 3)
                sql = "SELECT DISTINCT InksaveSetupID AS ID,Name FROM InksaveSetupNames ORDER BY ID";

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter(sql, connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }

        public DataTable GetNamesExCollection(String dataSetName, String sStoredProcedureName, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Type", Type.GetType("System.Int32"));

            SqlCommand command = new SqlCommand(sStoredProcedureName, connection2);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["ID"] = reader.GetInt32(0);
                    newRow["Name"] = reader.GetString(1);
                    newRow["Type"] = reader.GetInt32(2); // LocationID for Presses !!!!
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //dataSet.Tables.Add(tblResults);
            return tblResults;
        }

        public DataTable GetPressCollection(out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable("PressNames");

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Type", Type.GetType("System.Int32"));

            SqlCommand command = new SqlCommand("SELECT PressID,PressName,LocationID FROM PressNames ORDER BY PressName", connection2);

            string pressesToIgnore = (string)HttpContext.Current.Application["PressesToIgnore"];
            string[] pressignorelist = pressesToIgnore.Split(',');

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    int pressID = reader.GetInt32(0);
                    string pressName = reader.GetString(1);
                    int locationID = reader.GetInt32(2);

                    bool found = false;
                    foreach (string s in pressignorelist)
                    {
                        if (s == pressName)
                        {
                            found = true;
                        }
                    }
                    if (found == true)
                        continue;

                    newRow = tblResults.NewRow();
                    newRow["ID"] = pressID;
                    newRow["Name"] = pressName;
                    newRow["Type"] = locationID;

                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //dataSet.Tables.Add(tblResults);
            return tblResults;
        }

        public DataTable GetPressGroupCollection(out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable("PressGroupNames");
            //	DataSet dataSet = new DataSet();

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("PressIDList", Type.GetType("System.String"));

            SqlCommand command = new SqlCommand("SELECT PressGroupID,PressGroupName,PressID  FROM PressGroupNames ORDER BY PressGroupID,PressID", connection2);

            string pressesToIgnore = (string)HttpContext.Current.Application["PressesToIgnore"];
            string[] pressignorelist = pressesToIgnore.Split(',');

            // Mark the Command as a SPROC
            command.CommandType = CommandType.Text;

            DataRow newRow = null;
            SqlDataReader reader = null;

            int currentPressGroupID = 0;
            string currentPressGroupName = "";
            string pressIDList = "";
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int pressGroupID = reader.GetInt32(0);
                    string pressGroupName = reader.GetString(1);
                    int pressID = reader.GetInt32(2);

                    bool found = false;
                    foreach (string s in pressignorelist)
                    {
                        if (s == pressGroupName)
                        {
                            found = true;
                        }
                    }
                    if (found == true)
                        continue;


                    if (pressGroupID != currentPressGroupID && currentPressGroupID > 0)
                    {
                        newRow = tblResults.NewRow();
                        newRow["ID"] = currentPressGroupID;
                        newRow["Name"] = currentPressGroupName;
                        newRow["PressIDList"] = pressIDList;
                        tblResults.Rows.Add(newRow);
                        pressIDList = "";
                    }
                    if (pressIDList != "")
                        pressIDList += ",";
                    pressIDList += pressID.ToString();
                    currentPressGroupID = pressGroupID;
                    currentPressGroupName = pressGroupName;

                }
                if (currentPressGroupID > 0)
                {
                    newRow = tblResults.NewRow();
                    newRow["ID"] = currentPressGroupID;
                    newRow["Name"] = currentPressGroupName;
                    newRow["PressIDList"] = pressIDList;
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //dataSet.Tables.Add(tblResults);
            return tblResults;
        }


        public DataTable GetTemplateCollection(out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable();

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Press", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Nup", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("IsDummy", Type.GetType("System.Boolean"));

            SqlCommand command = new SqlCommand("SELECT TemplateID,TemplateName,P.PressName,PagesAcross*PagesDown,PlateCut  FROM TemplateConfigurations AS T INNER JOIN PressNames AS P ON T.PressID=P.PressID", connection2);

            string pressesToIgnore = (string)HttpContext.Current.Application["PressesToIgnore"];
            string[] pressignorelist = pressesToIgnore.Split(',');


            // Mark the Command as a SPROC
            command.CommandType = CommandType.Text;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    int templateID = reader.GetInt32(0);
                    string templateName = reader.GetString(1);
                    string pressName = reader.GetString(2);

                    bool found = false;
                    foreach (string s in pressignorelist)
                    {
                        if (s == pressName)
                        {
                            found = true;
                        }
                    }
                    if (found == true)
                        continue;

                    newRow = tblResults.NewRow();
                    newRow["ID"] = templateID;
                    newRow["Name"] = templateName;
                    newRow["Press"] = pressName;
                    newRow["Nup"] = reader.GetInt32(3);
                    int n = reader.GetInt32(4);
                    newRow["IsDummy"] = (n & 0x02) > 0 ? true : false;
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //dataSet.Tables.Add(tblResults);
            return tblResults;
        }

        public DataTable GetAliasCollection(out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable();
            //DataSet dataSet = new DataSet();

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Type", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("LongName", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("ShortName", Type.GetType("System.String"));

            string sql = "SELECT DISTINCT Type,LongName,ShortName FROM InputAliases WHERE ShortName<>'' AND LongName<>''";
            SqlCommand command = new SqlCommand(sql, connection2);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.Text;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["Type"] = reader.GetString(0);
                    newRow["LongName"] = reader.GetString(1);
                    newRow["ShortName"] = reader.GetString(2);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //dataSet.Tables.Add(tblResults);
            return tblResults;
        }

        public string LookupInkAlias(string longName, int pressID, out string errmsg)
        {
            errmsg = "";
            string alias = longName;
            string sql = "SELECT DISTINCT ShortName FROM Inkaliases WHERE Type='Publication' AND ShortName<>'' AND LongName='" + longName + "' AND PressID=" + pressID.ToString();
            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    alias = reader.GetString(0);
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

            }
            finally
            {

                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return alias;

        }

        public DataTable GetInkAliasCollection(out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable();

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Type", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("LongName", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("ShortName", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("PressID", Type.GetType("System.Int32"));

            string sql = "SELECT DISTINCT Type,LongName,ShortName,PressID FROM Inkaliases WHERE ShortName<>'' AND LongName<>'' AND PressID<>0";
            SqlCommand command = new SqlCommand(sql, connection2);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.Text;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["Type"] = reader.GetString(0);
                    newRow["LongName"] = reader.GetString(1);
                    newRow["ShortName"] = reader.GetString(2);
                    newRow["PressID"] = reader.GetInt32(3);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //dataSet.Tables.Add(tblResults);
            return tblResults;
        }


        public DataTable GetColorCollection(String dataSetName, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable(dataSetName);
            //	DataSet dataSet = new DataSet(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("C", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("M", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Y", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("K", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("ColorOrder", Type.GetType("System.Int32"));

            //connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand command = new SqlCommand("GetColorNames", connection2);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["ID"] = reader.GetInt32(0);
                    newRow["Name"] = reader.GetString(1);
                    newRow["C"] = reader.GetInt32(2);
                    newRow["M"] = reader.GetInt32(3);
                    newRow["Y"] = reader.GetInt32(4);
                    newRow["K"] = reader.GetInt32(5);
                    newRow["ColorOrder"] = reader.GetInt32(6);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //	dataSet.Tables.Add(tblResults);
            return tblResults;
        }

        public DataTable GetStatusCollection(String dataSetName, int nStatusType, out string errmsg)
        {
            errmsg = "";

            DataTable tblResults = new DataTable(dataSetName);
            //DataSet dataSet = new DataSet(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("StatusNumber", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("StatusName", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("StatusColor", Type.GetType("System.String"));

            SqlCommand command = new SqlCommand("GetStatusNames", connection2);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Clear();
            SqlParameter param;
            param = command.Parameters.Add("@StatusType", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = nStatusType;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["StatusNumber"] = reader.GetInt32(0);
                    newRow["StatusName"] = reader.GetString(1);
                    newRow["StatusColor"] = reader.GetString(2);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //	dataSet.Tables.Add(tblResults);
            return tblResults;
        }

        public DataTable GetEventCollection(String dataSetName, out string errmsg)
        {
            errmsg = "";

            DataTable tblResults = new DataTable(dataSetName);
            //	DataSet dataSet = new DataSet(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));

            SqlCommand command = new SqlCommand("SELECT DISTINCT EventNumber,EventName FROM EventCodes", connection2);
            command.CommandType = CommandType.Text;


            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["ID"] = reader.GetInt32(0);
                    newRow["Name"] = reader.GetString(1);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //dataSet.Tables.Add(tblResults);
            return tblResults;
        }


        public bool GetAllowedProductions(ref ProdList prodList, out string errmsg)
        {
            errmsg = "";

            prodList.Clear();

            string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            string locationsallowed = (string)HttpContext.Current.Session["LocationsAllowed"];
            string[] loclist = locationsallowed.Split(',');

            string pressesallowed = (string)HttpContext.Current.Session["PressesAllowed"];
            string[] presslist = pressesallowed.Split(',');

            SqlCommand command = new SqlCommand("SELECT DISTINCT ProductionID,PublicationID,PubDate,LocationID,PressID FROM PageTable WITH (NOLOCK) ORDER BY PubDate", connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int productionID = reader.GetInt32(0);
                    int publicationID = reader.GetInt32(1);
                    DateTime pubDate = reader.GetDateTime(2);
                    int locationID = reader.GetInt32(3);
                    int pressID = reader.GetInt32(4);

                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == Globals.GetNameFromID("PublicationNameCache", publicationID))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    if (locationsallowed != "*" && locationsallowed != "")
                    {
                        bool found = false;
                        foreach (string sp in loclist)
                        {
                            if (sp == Globals.GetNameFromID("LocationNameCache", locationID))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }
                    if (pressesallowed != "*" && pressesallowed != "")
                    {
                        bool found = false;
                        foreach (string sp in presslist)
                        {
                            if (sp == Globals.GetNameFromID("PressNameCache", locationID))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    prodList.Add(productionID, publicationID, pubDate);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        private bool IsInList(string[] array, string toFind)
        {
            foreach (string s in array)
            {
                if (s == toFind)
                    return true;
            }
            return false;
        }

        /*        public DataTable CacheTreeState(out string errmsg)
                {
                    errmsg = "";
                    DataTable tblResults = new DataTable("TreeeState");
                    DataColumn newColumn;
                    newColumn = tblResults.Columns.Add("Location", Type.GetType("System.String"));
                    newColumn = tblResults.Columns.Add("PubDate", Type.GetType("System.DateTime"));
                    newColumn = tblResults.Columns.Add("Publication", Type.GetType("System.String"));
                    newColumn = tblResults.Columns.Add("Edition", Type.GetType("System.String"));
                    newColumn = tblResults.Columns.Add("Section", Type.GetType("System.String"));
                    newColumn = tblResults.Columns.Add("MinStatus", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("MaxStatus", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("NeedApproval", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("AllApproved", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("AnyUniquePage", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("AnyImaging", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("AnyReady", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("AnyError", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("AnyOnHold", Type.GetType("System.Int32"));
                    newColumn = tblResults.Columns.Add("HasBeenImaged", Type.GetType("System.Int32"));

                    string sql;
                    string s = (string)HttpContext.Current.Session["SelectedLocation"];
                    if (s != "*" && s != "")
                        sql = "SELECT LocationID,Pubdate,PublicationID,EditionID,SectionID,MinStatus,MaxStatus,NeedApproval,AllApproved,AnyUniquePage,AnyImaging,AnyReady,AnyError,AnyOnHold,MiscInt1 FROM TreeeState WHERE PressRunID=0 AND LocationID=" + Globals.GetIDFromName("LocationNameCache", s);
                    else
                        sql = "SELECT LocationID,Pubdate,PublicationID,EditionID,SectionID,MinStatus,MaxStatus,NeedApproval,AllApproved,AnyUniquePage,AnyImaging,AnyReady,AnyError,AnyOnHold,MiscInt1 FROM TreeeState WHERE PressRunID=0";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.CommandType = CommandType.Text;

                    DataRow newRow = null;
                    SqlDataReader reader = null;

                    try
                    {
                        if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                            connection.Open();
                        reader = command.ExecuteReader();

                        int rowCount = reader.FieldCount;

                        while (reader.Read())
                        {
                            newRow = tblResults.NewRow();

                            newRow["Location"] = Globals.GetNameFromID("LocationNameCache", reader.GetInt32(0));
                            newRow["PubDate"] = reader.GetDateTime(1);
                            newRow["Publication"] = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(2));
                            newRow["Edition"] = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(3));
                            newRow["Section"] = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(4));
                            newRow["MinStatus"] = reader.GetInt32(5);
                            newRow["MaxStatus"] = reader.GetInt32(6);
                            newRow["NeedApproval"] = reader.GetInt32(7);
                            newRow["AllApproved"] = reader.GetInt32(8);
                            newRow["AnyUniquePage"] = reader.GetInt32(9);
                            newRow["AnyImaging"] = reader.GetInt32(10);
                            newRow["AnyReady"] = reader.GetInt32(11);
                            newRow["AnyError"] = reader.GetInt32(12);
                            newRow["AnyOnHold"] = reader.GetInt32(13);
                            newRow["HasBeenImaged"] = reader.GetInt32(14);
                            tblResults.Rows.Add(newRow);
                        }
                    }
                    catch (Exception ex)
                    {
                        errmsg = ex.Message;
                    }
                    finally
                    {
                        // always call Close when done reading.
                        if (reader != null)
                            reader.Close();
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }

                    return tblResults;
                }

         */
        // ProductionID, PublicationID,Pubdate, IssueID,EditionID,SectionID,PressID,Hold
        public DataTable GetActiveProductionCollection(String dataSetName, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Production", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Publication", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Pubdate", Type.GetType("System.DateTime"));
            newColumn = tblResults.Columns.Add("Issue", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Edition", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Press", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("PageCount", Type.GetType("System.Int32"));                    // In Section
            newColumn = tblResults.Columns.Add("Status", Type.GetType("System.Int32"));                         // In Section
            newColumn = tblResults.Columns.Add("Weeknumber", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Customer", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("TimedFrom", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PressSectionNumber", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Comment", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("PlanType", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("MaxStatus", Type.GetType("System.Int32"));                      // In Section
            newColumn = tblResults.Columns.Add("PagesReceived", Type.GetType("System.Int32"));                  // In Section
            newColumn = tblResults.Columns.Add("PageCountInPublication", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PagesReceivedInPublication", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PageCountInEdition", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PagesReceivedInEdition", Type.GetType("System.Int32"));

            newColumn = tblResults.Columns.Add("StatusPublication", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("MaxStatusPublication", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("StatusEdition", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("MaxStatusEdition", Type.GetType("System.Int32"));

            newColumn = tblResults.Columns.Add("ErrorEvent", Type.GetType("System.Int32"));

            newColumn = tblResults.Columns.Add("ErrorEventPublication", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("ErrorEventEdition", Type.GetType("System.Int32"));

            newColumn = tblResults.Columns.Add("VisioLinkStatus", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PComment", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("EComment", Type.GetType("System.String"));

            SqlCommand command = new SqlCommand("GetActiveProductions", connection);
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter param;
            command.Parameters.Clear();
            string s;

            param = command.Parameters.Add("@LocationID", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
            param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);


            param = command.Parameters.Add("@UseTreeState", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = 0;

            param = command.Parameters.Add("@GetPageCounts", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = (int)HttpContext.Current.Application["ShowPageCountInTree"] > 0 ? 1 : 0;

            param = command.Parameters.Add("@PublisherID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PublisherNameCache", (string)HttpContext.Current.Session["SelectedPublisher"]);

            param = command.Parameters.Add("@ChannelID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("ChannelNameCache", (string)HttpContext.Current.Session["SelectedChannel"]);

            DataRow newRow = null;
            SqlDataReader reader = null;

            string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            string sSectionsAllowed = (string)HttpContext.Current.Session["SectionsAllowed"];
            string[] sSectionsAllowedList = sSectionsAllowed.Split(',');

            string locationsallowed = (string)HttpContext.Current.Session["LocationsAllowed"];
            string[] loclist = locationsallowed.Split(',');

            string pressesallowed = (string)HttpContext.Current.Session["PressesAllowed"];
            string[] presslist = pressesallowed.Split(',');

            string sEditionsAllowed = (string)HttpContext.Current.Session["EditionsAllowed"];
            string[] sEditionsAllowedList = sEditionsAllowed.Split(new char[] { ',' });

            //            string sPublishersAllowed = (string)HttpContext.Current.Session["PublishersAllowed"];
            //           string[] sPublishersAllowedList = sPublishersAllowed.Split(new char[] { ',' });

            DateTime tNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();

                int rowCount = reader.FieldCount;

                while (reader.Read())
                {

                    string thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(0));
                    DateTime thisPubDate = reader.GetDateTime(1);
                    string thisIssue = Globals.GetNameFromID("IssueNameCache", reader.GetInt32(2));
                    string thisEdition = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(3));
                    string thisSection = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(4));
                    int thisPressID = reader.GetInt32(5);
                    int thisPageCount = reader.GetInt32(6);  // Not used!!!!!!!
                    string thisProduction = reader.GetString(7).Trim();
                    int thisStatus = reader.GetInt32(8);

                    int thisWeekNumber = 0;
                    string thisCustomer = "";
                    if (reader.FieldCount > 9)
                    {
                        thisWeekNumber = reader.GetInt32(9);
                        thisCustomer = reader.GetString(10).Trim();
                    }

                    int thisTimedFrom = 0;
                    if (reader.FieldCount > 11)
                        thisTimedFrom = reader.GetInt32(11);

                    int thisPressSectionNumber = 0;
                    if (reader.FieldCount > 12)
                        thisPressSectionNumber = reader.GetInt32(12);

                    string thisComment = "";
                    if (reader.FieldCount > 13)
                        thisComment = reader.GetString(13).Trim();

                    int thisPlanType = 1;
                    if (reader.FieldCount > 14)
                        thisPlanType = reader.GetInt32(14);


                    int thisMaxStatus = 0;
                    if (reader.FieldCount > 15)
                        thisMaxStatus = reader.GetInt32(15);

                    int thisTotalPageCount = 0;
                    int thisTotalPagesReceived = 0;
                    int thisErrorEvent = 0;
                    string scomment = "";
                    string ecomment = "";
                    if ((int)HttpContext.Current.Application["ShowPageCountInTree"] > 0)
                    {
                        if (reader.FieldCount > 16)
                            thisTotalPageCount = reader.GetInt32(16);

                        if (reader.FieldCount > 17)
                            thisTotalPagesReceived = reader.GetInt32(17);

                        if (reader.FieldCount > 18)
                            thisErrorEvent = reader.GetInt32(18);
                        if (reader.FieldCount > 19)
                            scomment = reader.GetString(19);
                        if (reader.FieldCount > 20)
                            ecomment = reader.GetString(20);
                    }
                    else
                    {
                        if (reader.FieldCount > 16)
                            thisErrorEvent = reader.GetInt32(16);
                        if (reader.FieldCount > 17)
                            scomment = reader.GetString(17);
                        if (reader.FieldCount > 18)
                            ecomment = reader.GetString(18);

                    }
                    if ((bool)HttpContext.Current.Session["HideOld"] == true || (DateTime)HttpContext.Current.Session["PubdateFilter"] == DateTime.MaxValue)
                    {
                        if (DateTime.Compare(thisPubDate, tNow) < 0)
                            continue;
                    }

                    if ((DateTime)HttpContext.Current.Session["PubdateFilter"] != DateTime.MinValue && (DateTime)HttpContext.Current.Session["PubdateFilter"] != DateTime.MaxValue)
                    {
                        if (thisPubDate != (DateTime)HttpContext.Current.Session["PubdateFilter"])
                            continue;
                    }

                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    if (locationsallowed != "*" && locationsallowed != "")
                    {
                        bool found = false;
                        foreach (string sp in loclist)
                        {
                            // Get Location from pressID
                            if ((bool)HttpContext.Current.Application["LocationIsPress"] == false)
                            {
                                int thisLocationID = Globals.GetTypeFromID("PressNameCache", thisPressID);

                                if (sp == Globals.GetNameFromID("LocationNameCache", thisLocationID))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            else
                            {
                                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                                {
                                    if (sp == Globals.GetNameFromID("PressGroupNameCache", thisPressID))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (sp == Globals.GetNameFromID("PressNameCache", thisPressID))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (found == false)
                            continue;
                    }


                    if (pressesallowed != "*" && pressesallowed != "")
                    {
                        bool found = false;
                        foreach (string sp in presslist)
                        {
                            if ((bool)HttpContext.Current.Application["UsePressGroups"])
                            {
                                if (Globals.IsPressInGroup(thisPressID, Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"])))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (sp == Globals.GetNameFromID("PressNameCache", thisPressID))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    if (sEditionsAllowed != "" && sEditionsAllowed != "*")
                    {
                        if (IsInList(sEditionsAllowedList, thisEdition) == false)
                            continue;
                    }

                    if (sSectionsAllowed != "" && sSectionsAllowed != "*")
                    {
                        if (IsInList(sSectionsAllowedList, thisSection) == false)
                            continue;
                    }

                    newRow = tblResults.NewRow();
                    newRow["Publication"] = thisPublication;
                    newRow["Pubdate"] = thisPubDate;
                    newRow["Issue"] = thisIssue;
                    newRow["Edition"] = thisEdition;
                    newRow["Section"] = thisSection;
                    //                    newRow["Press"] = (bool)HttpContext.Current.Application["UsePressGroups"] ? Globals.GetNameFromID("PressGroupNameCache", thisPressID) : Globals.GetNameFromID("PressNameCache", thisPressID);
                    newRow["Press"] = Globals.GetNameFromID("PressNameCache", thisPressID);

                    newRow["Production"] = thisProduction;
                    newRow["Status"] = thisStatus;
                    newRow["MaxStatus"] = thisMaxStatus;
                    newRow["Weeknumber"] = thisWeekNumber;
                    newRow["Customer"] = thisCustomer;
                    newRow["TimedFrom"] = thisTimedFrom;
                    newRow["PressSectionNumber"] = thisPressSectionNumber;
                    newRow["Comment"] = thisComment;
                    newRow["PlanType"] = thisPlanType;

                    newRow["PageCount"] = thisTotalPageCount; // thisPageCount;
                    newRow["PagesReceived"] = thisTotalPagesReceived;

                    newRow["PageCountInPublication"] = 0;
                    newRow["PagesReceivedInPublication"] = 0;
                    newRow["PageCountInEdition"] = 0;
                    newRow["PagesReceivedInEdition"] = 0;
                    newRow["StatusPublication"] = 0;
                    newRow["MaxStatusPublication"] = 0;
                    newRow["StatusEdition"] = 0;
                    newRow["MaxStatusEdition"] = 0;
                    newRow["ErrorEvent"] = thisErrorEvent;
                    newRow["PComment"] = scomment;
                    newRow["EComment"] = ecomment;
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            //	dataSet.Tables.Add(tblResults);
            return tblResults;
        }

        public ArrayList GetPubDateList(out string errmsg)
        {
            return GetPubDateList("", out errmsg);
        }

        public ArrayList GetPubDateList(string pressIdList, out string errmsg)
        {
            ArrayList weeks = new ArrayList();
            ArrayList weekDates = new ArrayList();
            return GetPubDateList(pressIdList, (bool)HttpContext.Current.Session["HideOld"], ref weeks, ref weekDates, out errmsg);
        }

        public ArrayList GetPubDateList(string pressIdList, bool hideOld, ref ArrayList weeklist, ref ArrayList weeklistDate, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();
            weeklist.Clear();
            weeklistDate.Clear();

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;


            string sql;
            if (pressIdList != "" && pressIdList != "*")
                sql = "SELECT DISTINCT PublicationID, PubDate FROM PageTable WITH (NOLOCK) WHERE Active=1 AND Dirty=0 AND PressID IN (" + pressIdList + ") ORDER BY PubDate DESC, PublicationID";
            else
                sql = "SELECT DISTINCT PublicationID, PubDate FROM PageTable WITH (NOLOCK) WHERE Active=1 AND Dirty=0 ORDER BY PubDate DESC, PublicationID";

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            //			DataRow newRow = null;
            SqlDataReader reader = null;

            DateTime tNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(0));
                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }
                    DateTime dt = reader.GetDateTime(1);


                    if (hideOld == true)
                        if (DateTime.Compare(dt, tNow) < 0)
                            continue;



                    bool founddt = false;
                    foreach (DateTime existingDT in al)
                    {
                        if (existingDT == dt)
                        {
                            founddt = true;
                            break;
                        }
                    }

                    if (founddt == false)
                        al.Add(dt);

                    int week = cal.GetWeekOfYear(dt, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                    founddt = false;
                    foreach (int existingweek in weeklist)
                    {
                        if (existingweek == week)
                        {
                            founddt = true;
                            break;
                        }
                    }

                    if (founddt == false)
                    {
                        weeklist.Add(week);
                        weeklistDate.Add(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return al;
        }

        public DataTable GetPressRunCollection(out string errmsg)
        {
            errmsg = "";
            return GetPressRunCollection(DateTime.MinValue, out errmsg);
        }

        public string GetSectionsInPressRun(int pressRunID, out string errmsg)
        {
            errmsg = "";
            string sectionList = "";

            SqlCommand command = new SqlCommand("SELECT DISTINCT SectionID FROM PageTable WHERE Dirty=0 AND PressRunID=" + pressRunID, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (sectionList != "")
                        sectionList += ",";
                    sectionList += Globals.GetNameFromID("SectionNameCache", reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return sectionList;
        }


        public bool GetPagesArrived(int productionID, out int pages, out string errmsg)
        {
            errmsg = "";
            pages = 0;

            SqlCommand command = new SqlCommand("SELECT COUNT(DISTINCT MasterCopySeparationSet) FROM PageTable WITH (NOLOCK) WHERE Status>=10 AND ProductionID=" + productionID, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    pages = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool GetPublicationFromProduction(int productionID, out int publicationID, out DateTime pubDate, out string errmsg)
        {
            errmsg = "";
            publicationID = 0;
            pubDate = DateTime.MinValue;

            SqlCommand command = new SqlCommand("SELECT TOP 1 PublicationID,PubDate FROM PageTable WITH (NOLOCK) WHERE ProductionID=" + productionID, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    publicationID = reader.GetInt32(0);
                    pubDate = reader.GetDateTime(1);

                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }



        public DataTable GetPressRunCollection(DateTime pubDateFiler, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable();


            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Location", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Press", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Pubdate", Type.GetType("System.DateTime"));
            newColumn = tblResults.Columns.Add("Publication", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Edition", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("DeadLine", Type.GetType("System.DateTime"));
            newColumn = tblResults.Columns.Add("Priority", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Copies", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Hold", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("Pages", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PressRunID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("OrderNumber", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Comment", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("PlanType", Type.GetType("System.Int32"));

            SqlCommand command = new SqlCommand("spPressRunPageList", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Clear();
            SqlParameter param;
            param = command.Parameters.Add("@LocationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = -1;
            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = -1;

            param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);

            param = command.Parameters.Add("@PublisherID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PublisherNameCache", (string)HttpContext.Current.Session["SelectedPublisher"]);

            if (pubDateFiler > DateTime.MinValue)
            {
                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = pubDateFiler;
            }

            DataRow newRow = null;
            SqlDataReader reader = null;
            DateTime tNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);


            string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();


                while (reader.Read())
                {
                    int fieldCount = reader.FieldCount;

                    string thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(0));
                    string thisLocation = Globals.GetNameFromID("LocationNameCache", reader.GetInt32(1));
                    string thisPress = Globals.GetNameFromID("PressNameCache", reader.GetInt32(2));
                    DateTime thisPubDate = reader.GetDateTime(3);
                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    if (pubDateFiler > DateTime.MinValue)
                        if (thisPubDate != pubDateFiler)
                            continue;

                    if ((bool)HttpContext.Current.Session["PressRunHideOld"] == true)
                    {
                        if (DateTime.Compare(thisPubDate, tNow) < 0)
                            continue;
                    }

                    newRow = tblResults.NewRow();

                    newRow["Publication"] = thisPublication;
                    newRow["Location"] = thisLocation;
                    newRow["Press"] = thisPress;
                    newRow["Pubdate"] = thisPubDate;
                    //					newRow["Issue"] =  Globals.GetNameFromID("IssueNameCache",reader.GetInt32(4));
                    newRow["Edition"] = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(5));

                    //	if (Global.databaseVersion == 2)
                    //		newRow["Section"] = reader.GetInt32(6).ToString();
                    //	else
                    newRow["Section"] = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(6));

                    DateTime dt = new DateTime();
                    try
                    {
                        dt = reader.GetDateTime(7);
                    }
                    catch
                    {
                        dt = new DateTime(1975, 1, 1, 0, 0, 0);
                    }
                    newRow["DeadLine"] = dt;

                    newRow["Priority"] = reader.GetInt32(8);
                    newRow["Copies"] = reader.GetInt32(9);
                    newRow["Hold"] = reader.GetInt32(10);
                    newRow["Pages"] = reader.GetInt32(11);
                    if (fieldCount >= 13)
                        newRow["PressRunID"] = reader.GetInt32(12);
                    //reader.GetInt32(12);    // OressRunID
                    newRow["OrderNumber"] = "";
                    if (fieldCount >= 14)
                        newRow["OrderNumber"] = reader.GetString(13);       // MiscString3 ChannelIDList

                    if (fieldCount >= 15)
                    {
                        string s = reader.GetString(14);
                        if (s != "" && (bool)HttpContext.Current.Application["PressRunShowInkComment"])
                            newRow["Press"] += "(" + s + ")";
                    }

                    newRow["Comment"] = "";
                    if (fieldCount >= 16)
                    {
                        newRow["Comment"] = reader.GetString(15);
                    }

                    newRow["PlanType"] = 1;
                    if (fieldCount >= 17)
                    {
                        newRow["PlanType"] = reader.GetInt32(16);
                    }

                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblResults;
        }

        public DataTable GetThumbnailPageCollection(bool hideApproved, bool hideCommon, bool bUnfiltered, out string errmsg)
        {
            bool bAllCopies = false;

            errmsg = "";

            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spThumbnailPageList", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable("PageTable");

            DataColumn newColumn;

            newColumn = tblMain.Columns.Add("Production", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Publication", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PubDate", Type.GetType("System.DateTime"));
            newColumn = tblMain.Columns.Add("Issue", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Edition", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Page", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Color", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Status", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ExternalStatus", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Version", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Approval", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("CopyNumber", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Pagination", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("LastError", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Comment", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ProofStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Location", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PageType", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MasterCopySeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Active", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("UniquePage", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Hold", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PlanPageName", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PageIndex", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("EmailStatus", Type.GetType("System.UInt32"));
            newColumn = tblMain.Columns.Add("Deadline", Type.GetType("System.DateTime"));
            newColumn = tblMain.Columns.Add("InputTime", Type.GetType("System.DateTime"));
            newColumn = tblMain.Columns.Add("Locked", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Alert", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("UniquePublicationID", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ColorStatus", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PdfMaster", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("FileStatusLow", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FileStatusHigh", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FileStatusPrint", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("ColorWarningStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ColorMessage", Type.GetType("System.String"));

            newColumn = tblMain.Columns.Add("MessageStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MessageIsRead", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ThumbnailSize", Type.GetType("System.Int32"));

            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;

                string s = (string)HttpContext.Current.Session["SelectedPublication"];
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("PublicationNameCache", s);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? tDefaultPubDate : (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@LocationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                if ((bool)HttpContext.Current.Application["LocationIsPress"])
                    param.Value = -1;
                else
                    param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("LocationNameCache", (string)HttpContext.Current.Session["SelectedLocation"]);

                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                    param.Value = 0;

                param = command.Parameters.Add("@Approved", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideApproved ? 1 : -1);

                param = command.Parameters.Add("@Unique", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideCommon ? 1 : -1);

                param = command.Parameters.Add("@CopyNumber", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bAllCopies ? 0 : 1;

                param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                int thisMasterCopySeparationSet = 0;
                string thisColor = "";
                int prevMasterCopySeparationSet = -1;

                int nColors = 0;
                int nMinStatus = 100;
                int nMinExtStatus = 100;
                bool bHasError = false;
                bool bIsApproved = true;
                int nMaxVersion = 0;
                bool bIsFirst = true;
                bool bIsProofed = true;
                bool bIsPartiallyProofed = false;
                int nMinProofStatus = 100;
                bool bIsPartiallyPolled = false;
                int thisActive = 0;
                int thisPageType = 0;
                bool bIsActive = false;
                //  string s;
                string thisPublication = "";
                DateTime tMaxInputTime = new DateTime();
                tMaxInputTime = DateTime.MinValue;

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                string sSectionsAllowed = (string)HttpContext.Current.Session["SectionsAllowed"];
                string[] sSectionsAllowedList = sSectionsAllowed.Split(',');

                string sEditionsAllowed = (string)HttpContext.Current.Session["EditionsAllowed"];
                string[] sEditionsAllowedList = sEditionsAllowed.Split(new char[] { ',' });



                string thisEdition = "";
                string thisSection = "";
                string thisIssue = "";
                DateTime thisPubDate;
                int thisPagination = 0;


                while (reader.Read())
                {

                    int idx = 0;
                    int fieldCount = reader.FieldCount;

                    thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(idx++));

                    Global.logging.WriteLog("GetThumbnailPageCollection() " + thisPublication);

                    if (pubsallowed != "" && pubsallowed != "*")
                    {
                        if (IsInList(publist, thisPublication) == false)
                            continue;
                    }

                    thisMasterCopySeparationSet = reader.GetInt32(idx++);	// CopyColorSet

                    if (Global.databaseVersion == 2)
                        thisColor = Globals.GetNameFromID("ColorNameCache", reader.GetInt32(idx++));
                    else
                        thisColor = reader.GetString(idx++);

                    thisPagination = reader.GetInt32(idx++);
                    thisActive = reader.GetInt32(idx++);
                    thisPubDate = reader.GetDateTime(idx++);
                    thisIssue = Globals.GetNameFromID("IssueNameCache", reader.GetInt32(idx++));
                    thisEdition = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(idx++));
                    thisSection = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(idx++));

                    if (sEditionsAllowed != "" && sEditionsAllowed != "*")
                    {
                        if (IsInList(sEditionsAllowedList, thisEdition) == false)
                            continue;
                    }

                    if (sSectionsAllowed != "" && sSectionsAllowed != "*")
                    {
                        if (IsInList(sSectionsAllowedList, thisSection) == false)
                            continue;
                    }

                    if (thisMasterCopySeparationSet != prevMasterCopySeparationSet)
                    {
                        // Store previous page
                        if (bIsFirst == false)
                            tblMain.Rows.Add(newRow);

                        bIsFirst = false;

                        // New page begins - reset color string
                        nColors = 0;
                        nMinStatus = 100;
                        nMinProofStatus = 100;
                        bHasError = false;
                        bIsApproved = true;
                        bIsProofed = true;
                        nMaxVersion = 0;
                        tMaxInputTime = DateTime.MinValue;
                        bIsActive = false;
                        bIsPartiallyProofed = false;
                        bIsPartiallyPolled = false;

                        newRow = tblMain.NewRow();
                        newRow["Color"] = "";
                        newRow["ColorStatus"] = "";
                        newRow["FileStatusLow"] = 0;
                        newRow["FileStatusHigh"] = 0;
                        newRow["FileStatusPrint"] = 0;

                        // nou used (yet..)
                        newRow["ColorWarningStatus"] = 0;
                        newRow["ColorMessage"] = "";
                        newRow["MessageStatus"] = 0;
                        newRow["MessageIsRead"] = 0;
                        newRow["ThumbnailSize"] = 0;

                    }

                    newRow["MasterCopySeparationSet"] = thisMasterCopySeparationSet;
                    newRow["PdfMaster"] = thisMasterCopySeparationSet;
                    newRow["Pagination"] = thisPagination;

                    bIsActive = thisActive == 1 ? true : false;
                    newRow["Active"] = bIsActive ? 1 : 0;	// int !


                    newRow["Publication"] = thisPublication;
                    newRow["UniquePublicationID"] = Globals.GetIDFromName("PublicationNameCache", thisPublication);
                    newRow["PubDate"] = thisPubDate;
                    newRow["Production"] = thisPublication + " " + thisPubDate.ToShortDateString();
                    newRow["Issue"] = thisIssue;
                    newRow["Edition"] = thisEdition;
                    newRow["Section"] = thisSection;

                    newRow["Page"] = reader.GetString(idx++);

                    int nStatus = reader.GetInt32(idx++);
                    if ((bool)HttpContext.Current.Application["AllowPartialProofs"])
                    {
                        if (nStatus < nMinStatus && nStatus > 0 && bIsActive)
                            nMinStatus = nStatus;
                    }
                    else
                    {
                        if (nStatus < nMinStatus && bIsActive)
                            nMinStatus = nStatus;
                    }
                    if (bIsActive)
                    {
                        if (nColors != 0)
                        {
                            newRow["Color"] += ";";
                            newRow["ColorStatus"] += ";";
                        }
                        newRow["Color"] += thisColor;
                        newRow["ColorStatus"] += nStatus.ToString();
                        nColors++;
                    }

                    int nRest = nStatus - (nStatus / 10) * 10;
                    if (nRest == 6 && bIsActive)
                        bHasError = true;

                    String st = Globals.GetStatusName(bHasError ? nStatus : nMinStatus, 0);
                    newRow["Status"] = st == "" ? "N/A" : st;

                    nStatus = reader.GetInt32(idx++);
                    if (nStatus < nMinExtStatus && bIsActive)
                        nMinExtStatus = nStatus;

                    if (nStatus >= 10)
                        bIsPartiallyPolled = true;

                    nRest = nStatus - (nStatus / 10) * 10;
                    if (nRest == 6 && bIsActive)
                        nMinExtStatus = nStatus;

                    st = Globals.GetStatusName(nMinExtStatus, 1);
                    newRow["ExternalStatus"] = st == "" ? "N/A" : st;

                    newRow["Version"] = reader.GetInt32(idx++);

                    int nApproval = reader.GetInt32(idx++); ;

                    if ((nApproval == 0 || nApproval == 2) && bIsActive)
                        bIsApproved = false;
                    newRow["Approval"] = nApproval;

                    newRow["CopyNumber"] = reader.GetInt32(idx++);
                    newRow["LastError"] = reader.GetString(idx++);

                    string sComment = reader.GetString(idx++);
                    newRow["Comment"] = sComment;

                    int prf = reader.GetInt32(idx++);
                    if (prf < 1 && bIsActive)
                        bIsProofed = false;

                    if (prf > 6 && bIsActive)
                        bIsPartiallyProofed = true;

                    if (prf < nMinProofStatus && bIsActive)
                        nMinProofStatus = prf;

                    newRow["ProofStatus"] = bIsProofed ? nMinProofStatus : (bIsPartiallyProofed ? 5 : 0);		// Use 5 as partial proofed indicator
                    newRow["Location"] = Globals.GetNameFromID("LocationNameCache", reader.GetInt32(idx++));
                    newRow["PageType"] = reader.GetInt32(idx++);
                    newRow["Hold"] = reader.GetInt32(idx++);
                    newRow["UniquePage"] = reader.GetInt32(idx++);


                    newRow["PlanPageName"] = "";
                    newRow["PageIndex"] = (int)newRow["Pagination"];
                    newRow["EmailStatus"] = 0;
                    newRow["Deadline"] = new DateTime(1975, 1, 1, 0, 0, 0);
                    newRow["InputTime"] = new DateTime(1975, 1, 1, 0, 0, 0);


                    newRow["Locked"] = 0;
                    newRow["Alert"] = 0;

                    reader.GetInt32(idx++); // Dummy read PressID

                    reader.GetInt32(idx++); // Dummy read PressRunID

                    newRow["PlanPageName"] = reader.GetString(idx++);
                    newRow["PageIndex"] = reader.GetInt32(idx++);

                    if (!reader.IsDBNull(idx))
                        newRow["EmailStatus"] = (uint)reader.GetInt32(idx);
                    idx++;

                    if (!reader.IsDBNull(idx))
                        newRow["Deadline"] = reader.GetDateTime(idx);
                    idx++;

                    if (!reader.IsDBNull(idx))
                        newRow["InputTime"] = reader.GetDateTime(idx);
                    idx++;

                    if (!reader.IsDBNull(idx))      // OutputPriority
                    {
                        int n = reader.GetInt32(idx);
                        newRow["Locked"] = n == -1 ? 1 : 0;
                    }
                    idx++;

                    if (!reader.IsDBNull(idx))
                    {
                        newRow["FileStatusLow"] = reader.GetInt32(idx) == 10 ? 1 : 0;
                    }
                    idx++;


                    if (!reader.IsDBNull(idx))
                    {
                        newRow["FileStatusHigh"] = reader.GetInt32(idx) == 10 ? 1 : 0;
                    }
                    idx++;

                    if (!reader.IsDBNull(idx))
                    {
                        newRow["FileStatusPrint"] = reader.GetInt32(idx) == 10 ? 1 : 0;
                    }
                    idx++;

                    prevMasterCopySeparationSet = thisMasterCopySeparationSet;

                }
                if (bIsFirst == false)
                    tblMain.Rows.Add(newRow);

            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                Global.logging.WriteLog("GetThumbnailPageCollection() " + errmsg);

                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            GetUniquePagePublicationNames(ref tblMain, out errmsg);

            return tblMain;
        }

        public bool GetUniquePagePublicationNames(ref DataTable dt, out string errmsg)
        {
            errmsg = "";

            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["UniquePage"] != 1)
                {
                    string sql = string.Format("SELECT TOP 1 PublicationID FROM PageTable WHERE UniquePage=1 AND MasterCopySeparationSet={0} AND Dirty=0", (int)row["MasterCopySeparationSet"]);
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.CommandType = CommandType.Text;

                    SqlDataReader reader = null;
                    try
                    {
                        if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                            connection.Open();

                        reader = command.ExecuteReader();

                        if (reader.Read())
                            row["UniquePublicationID"] = reader.GetInt32(0);
                    }
                    catch (Exception ex)
                    {
                        errmsg = "!!  " + ex.Message;
                        return false;
                    }
                    finally
                    {
                        // always call Close when done reading.
                        if (reader != null)
                            reader.Close();
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }

            }

            return true;
        }

        public bool GetMasterSetPageCollection(ref List<int> masterList, ref List<string> pagenameList, bool hideApproved, bool hideCommon, bool bUnfiltered, out string errmsg)
        {
            masterList.Clear();
            pagenameList.Clear();
            ArrayList al = new ArrayList();

            errmsg = "";

            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spMasterSetPageList", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? tDefaultPubDate : (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@Approved", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideApproved ? 1 : -1);

                param = command.Parameters.Add("@Unique", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideCommon ? 1 : -1);

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                string thisPublication = "";

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                while (reader.Read())
                {

                    int idx = 0;

                    thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(idx++));

                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    masterList.Add(reader.GetInt32(idx++));
                    pagenameList.Add(reader.GetString(idx++));

                }
            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        public DataTable GetStatCollection(int reportType, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand("spProgressReport", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable();

            DataColumn newColumn;

            newColumn = tblMain.Columns.Add("PublicationID", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PubDate", Type.GetType("System.DateTime"));
            newColumn = tblMain.Columns.Add("IssueID", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("EditionID", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("SectionID", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Page", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("MinStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MinApprove", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Time", Type.GetType("System.DateTime"));
            newColumn = tblMain.Columns.Add("DeadLine", Type.GetType("System.DateTime"));

            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                    param.Value = 0;

                param = command.Parameters.Add("@ReportType", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = reportType;

                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                {
                    param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                }

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                while (reader.Read())
                {
                    int idx = 0;
                    int publicationID = reader.GetInt32(idx++);

                    string thisPublication = Globals.GetNameFromID("PublicationNameCache", publicationID);
                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }
                    newRow = tblMain.NewRow();

                    newRow["PublicationID"] = publicationID;
                    newRow["PubDate"] = reader.GetDateTime(idx++);
                    newRow["EditionID"] = reader.GetInt32(idx++);
                    newRow["IssueID"] = reader.GetInt32(idx++);
                    newRow["SectionID"] = reader.GetInt32(idx++);
                    newRow["Page"] = reader.GetString(idx++);
                    newRow["MinStatus"] = reader.GetInt32(idx++);
                    newRow["MinApprove"] = reader.GetInt32(idx++);
                    newRow["Time"] = reader.GetDateTime(idx++);

                    if (reader.FieldCount > idx)
                        newRow["DeadLine"] = reader.GetDateTime(idx++);
                    else
                        newRow["DeadLine"] = DateTime.MinValue;

                    tblMain.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return tblMain;
        }




        public DataSet GetStatCollectionExcelEx(bool getEdition, bool getSection,
            bool getFtpLog,
            bool getPreflightLog,
            bool getRipLog,
            bool getColorLog,
            bool getReadyTime,
            bool getViewTime,
            bool getInputTime,
            bool getApproveTime,
            bool getOutputTime,
            out int ftploglength,
            out int preflightloglength,
            out int riploglength,
            out int colorloglength,
            bool orderByPlate,

            out string errmsg)
        {
            errmsg = "";
            ftploglength = 0;
            preflightloglength = 0;
            riploglength = 0;
            colorloglength = 0;

            SqlCommand command = new SqlCommand("spProgressReportExcel", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable();
            DataSet dataSet = new DataSet();

            DataColumn newColumn;

            if (orderByPlate)
            {
                newColumn = tblMain.Columns.Add("Sheet", Type.GetType("System.Int32"));
                newColumn = tblMain.Columns.Add("Side", Type.GetType("System.String"));
            }

            if (getEdition)
                newColumn = tblMain.Columns.Add("Edition", Type.GetType("System.String"));
            if (getSection)
                newColumn = tblMain.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Page", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Version", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Status", Type.GetType("System.String"));
            if (getFtpLog)
            {
                newColumn = tblMain.Columns.Add("FTPtime", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("FTPstatus", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("FTPmessage", Type.GetType("System.String"));
            }
            if (getPreflightLog)
            {
                newColumn = tblMain.Columns.Add("Preflighttime", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("Preflightstatus", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("Preflightmessage", Type.GetType("System.String"));
            }
            if (getRipLog)
            {
                newColumn = tblMain.Columns.Add("RIPtime", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("RIPstatus", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("RIPmessage", Type.GetType("System.String"));
            }

            if (getInputTime)
                newColumn = tblMain.Columns.Add("InputTime", Type.GetType("System.String"));

            if (getColorLog)
            {
                newColumn = tblMain.Columns.Add("Colorstatus", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("Colormessage", Type.GetType("System.String"));
            }

            if (getViewTime)
            {
                newColumn = tblMain.Columns.Add("ViewTime", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("ViewUser", Type.GetType("System.String"));
            }

            if (getApproveTime)
            {
                newColumn = tblMain.Columns.Add("ApproveTime", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("Approved", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("ApproveUser", Type.GetType("System.String"));
            }

            if (getReadyTime)
            {
                newColumn = tblMain.Columns.Add("ReadyTime", Type.GetType("System.String"));
            }

            if (getOutputTime)
            {
                newColumn = tblMain.Columns.Add("OutputTime", Type.GetType("System.String"));
            }
            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@OrderByPlate", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = orderByPlate ? 1 : 0;

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                int nPrevSide = 0;
                int nSide = 0;

                while (reader.Read())
                {
                    int idx = 0;
                    int publicationID = reader.GetInt32(idx++);

                    string thisPublication = Globals.GetNameFromID("PublicationNameCache", publicationID);
                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    /*if (orderByPlate && nPrevSide != nSide) 
                    {
                        newRow = tblMain.NewRow();
                        tblMain.Rows.Add(newRow);
                    }
                    nPrevSide = nSide;*/

                    newRow = tblMain.NewRow();

                    //newRow["Publication"] = thisPublication;
                    reader.GetDateTime(idx++);
                    //newRow["PubDate"] = reader.GetDateTime(idx++);
                    int nEdition = reader.GetInt32(idx++);
                    if (getEdition)
                        newRow["Edition"] = Globals.GetNameFromID("EditionNameCache", nEdition);
                    reader.GetInt32(idx++); // dummy read over issue..
                    int nSection = reader.GetInt32(idx++);
                    if (getSection)
                        newRow["Section"] = Globals.GetNameFromID("SectionNameCache", nSection);

                    string s = reader.GetString(idx++);
                    //newRow["Page"] = s; // CHANGED TO PAGINATION - LAST COLUMN..
                    newRow["Status"] = Globals.GetStatusName(reader.GetInt32(idx++), 0);
                    newRow["Version"] = reader.GetInt32(idx++);


                    DateTime dt = reader.GetDateTime(idx++);
                    if (getInputTime)
                        newRow["InputTime"] = dt.Year > 2000 ? dt.ToString() : "";

                    int approval = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);
                    dt = reader.GetDateTime(idx++);
                    if (getApproveTime)
                    {
                        newRow["ApproveTime"] = dt.Year > 2000 ? dt.ToString() : "";
                        switch (approval)
                        {
                            case 2:
                                newRow["Approved"] = Global.rm.GetString("txtRejected");
                                break;
                            case 0:
                                newRow["Approved"] = Global.rm.GetString("txtNotApproved");
                                break;
                            default:
                                newRow["Approved"] = Global.rm.GetString("txtApproved");
                                break;
                        }
                        newRow["ApproveUser"] = s;
                    }

                    dt = reader.GetDateTime(idx++);
                    if (getReadyTime)
                        newRow["ReadyTime"] = dt.Year > 2000 ? dt.ToString() : "";

                    dt = reader.GetDateTime(idx++);
                    if (getOutputTime)
                        newRow["OutputTime"] = dt.Year > 2000 ? dt.ToString() : "";

                    int n = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);
                    dt = reader.GetDateTime(idx++);

                    if (getFtpLog)
                    {
                        newRow["FTPtime"] = dt.Year > 2000 ? dt.ToString() : "";
                        newRow["FTPstatus"] = Globals.GetNameFromID("EventNameCache", n);
                        newRow["FTPmessage"] = s;
                        if (s.Length > ftploglength)
                            ftploglength = s.Length;
                    }

                    n = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);
                    dt = reader.GetDateTime(idx++);

                    if (getPreflightLog)
                    {
                        newRow["Preflighttime"] = dt.Year > 2000 ? dt.ToString() : "";
                        newRow["Preflightstatus"] = Globals.GetNameFromID("EventNameCache", n);
                        int i = s.IndexOf("?");
                        if (i != -1)
                            s = s.Substring(i + 1);
                        i = s.IndexOf("Report for");
                        if (i != -1)
                            s = s.Substring(0, i);
                        s.Trim();

                        newRow["Preflightmessage"] = s;
                        if (s.Length > preflightloglength)
                            preflightloglength = s.Length;

                    }

                    n = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);
                    dt = reader.GetDateTime(idx++);
                    if (getRipLog)
                    {
                        newRow["RIPtime"] = dt.Year > 2000 ? dt.ToString() : "";
                        newRow["RIPstatus"] = Globals.GetNameFromID("EventNameCache", n);
                        newRow["RIPmessage"] = s;
                        if (s.Length > riploglength)
                            riploglength = s.Length;
                    }

                    n = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);

                    if (getColorLog)
                    {
                        newRow["Colorstatus"] = Globals.GetNameFromID("EventNameCache", n);
                        newRow["Colormessage"] = s;
                        if (s.Length > colorloglength)
                            colorloglength = s.Length;

                    }

                    dt = reader.GetDateTime(idx++);
                    s = reader.GetString(idx++);

                    if (getViewTime)
                    {
                        newRow["ViewTime"] = dt.Year > 2000 ? dt.ToString() : "";
                        newRow["ViewUser"] = s;
                    }
                    // Get
                    int nPage = reader.GetInt32(idx++);
                    newRow["Page"] = nPage;


                    if (orderByPlate)
                    {
                        int nSheet = reader.GetInt32(idx++);
                        newRow["Sheet"] = nSheet;

                        nSide = reader.GetInt32(idx++);

                        if (nSide == 0)
                            newRow["Side"] = "Front";
                        else
                            newRow["Side"] = "Back";
                    }

                    tblMain.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                dataSet = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (dataSet != null)
                dataSet.Tables.Add(tblMain);

            return dataSet;
        }

        private string Time2String(DateTime dt)
        {
            if (dt.Year < 2000)
                return "";
            CultureInfo culture = new CultureInfo(Global.culture);
            return dt.ToString((string)HttpContext.Current.Application["ExcelTimeFormat"], culture);
        }



        public DataTable GetStatCollectionExcel(bool getEdition, bool getSection, bool getFtpLog,
            bool getPreflightLog, bool getRipLog, bool getColorLogout, bool getReadyTime,
            bool getViewTime, bool orderByPlate, bool showAfterdeadline, bool showCMYKInk,
            ref int Cinkmean, ref int Minkmean, ref int Yinkmean, ref int Kinkmean /*, ref double fAreaMean*/, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand("spProgressReportExcel", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable();

            Cinkmean = 0;
            Minkmean = 0;
            Yinkmean = 0;
            Kinkmean = 0;
            // fAreaMean = 0.0;

            int Pagesmean = 0;

            DataColumn newColumn;

            if (orderByPlate)
            {
                newColumn = tblMain.Columns.Add("Sheet", Type.GetType("System.Int32"));
                newColumn = tblMain.Columns.Add("Side", Type.GetType("System.String"));
            }

            if (getEdition)
                newColumn = tblMain.Columns.Add("Edition", Type.GetType("System.String"));
            if (getSection)
                newColumn = tblMain.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Page", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Version", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Status", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("InputTime", Type.GetType("System.String"));

            if (showAfterdeadline)
            {
                newColumn = tblMain.Columns.Add("InputTimeAfterDeadline", Type.GetType("System.String"));
            }

            newColumn = tblMain.Columns.Add("ViewTime", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ViewUser", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Approved", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ApproveUser", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ApproveTime", Type.GetType("System.String"));
            if (getReadyTime)
            {
                newColumn = tblMain.Columns.Add("ReadyTime", Type.GetType("System.String"));
            }

            newColumn = tblMain.Columns.Add("OutputTime", Type.GetType("System.String"));
            if (getFtpLog)
            {
                newColumn = tblMain.Columns.Add("FTPstatus", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("FTPmessage", Type.GetType("System.String"));
            }
            if (getPreflightLog)
            {
                newColumn = tblMain.Columns.Add("Preflightstatus", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("Preflightmessage", Type.GetType("System.String"));
            }
            if (getRipLog)
            {
                newColumn = tblMain.Columns.Add("RIPstatus", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("RIPmessage", Type.GetType("System.String"));
            }
            if (getColorLogout)
            {
                newColumn = tblMain.Columns.Add("Colorstatus", Type.GetType("System.String"));
                newColumn = tblMain.Columns.Add("Colormessage", Type.GetType("System.String"));
            }

            if (showCMYKInk)
            {
                newColumn = tblMain.Columns.Add("Cink", Type.GetType("System.Int32"));
                newColumn = tblMain.Columns.Add("Mink", Type.GetType("System.Int32"));
                newColumn = tblMain.Columns.Add("Yink", Type.GetType("System.Int32"));
                newColumn = tblMain.Columns.Add("Kink", Type.GetType("System.Int32"));

            }

            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@OrderByPlate", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = orderByPlate ? 1 : 0;


                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                {
                    param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                }

                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                    param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                else
                    param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                while (reader.Read())
                {
                    int idx = 0;
                    int publicationID = reader.GetInt32(idx++);

                    string thisPublication = Globals.GetNameFromID("PublicationNameCache", publicationID);
                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }
                    newRow = tblMain.NewRow();

                    //newRow["Publication"] = thisPublication;
                    reader.GetDateTime(idx++);
                    //newRow["PubDate"] = reader.GetDateTime(idx++);
                    int n = reader.GetInt32(idx++);
                    if (getEdition)
                        newRow["Edition"] = Globals.GetNameFromID("EditionNameCache", n);
                    reader.GetInt32(idx++); // dummy read over issue..
                    n = reader.GetInt32(idx++);
                    if (getSection)
                        newRow["Section"] = Globals.GetNameFromID("SectionNameCache", n);

                    string s = reader.GetString(idx++);
                    //newRow["Page"] = s; // CHANGED TO PAGINATION - LAST COLUMN..
                    int status = reader.GetInt32(idx++);
                    newRow["Status"] = Globals.GetStatusName(status, 0);
                    newRow["Version"] = reader.GetInt32(idx++);

                    DateTime dtInput = reader.GetDateTime(idx++);
                    newRow["InputTime"] = Time2String(dtInput); // dtInput.Year > 2000 ? dtInput.ToString((string)HttpContext.Current.Application["ExcelTimeFormat"], Format) : "";


                    int approval = reader.GetInt32(idx++);
                    switch (approval)
                    {
                        case 2:
                            newRow["Approved"] = Global.rm.GetString("txtRejected");
                            break;
                        case 0:
                            newRow["Approved"] = Global.rm.GetString("txtNotApproved");
                            break;
                        default:
                            newRow["Approved"] = Global.rm.GetString("txtApproved");
                            break;
                    }

                    newRow["ApproveUser"] = reader.GetString(idx++);

                    DateTime dtApprove = reader.GetDateTime(idx++);


                    DateTime dtReady = reader.GetDateTime(idx++);
                    if (getReadyTime)
                    {
                        newRow["ReadyTime"] = Time2String(dtReady);
                    }

                    DateTime dtOutput = reader.GetDateTime(idx++);
                    newRow["OutputTime"] = Time2String(dtOutput);

                    n = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);
                    DateTime dt = reader.GetDateTime(idx++);

                    if (getFtpLog)
                    {
                        if (dt.Year > 2000)
                        {
                            newRow["InputTime"] = Time2String(dt);
                        }
                        newRow["FTPstatus"] = Time2String(dt);
                        newRow["FTPmessage"] = Globals.GetNameFromID("EventNameCache", n) + " " + s;
                    }

                    if (dtApprove < dtInput && dtApprove.Year > 2000 && dtInput.Year > 2000)
                        dtApprove = dtInput;

                    if (dtApprove > dtOutput && dtApprove.Year > 2000 && dtOutput.Year > 2000)
                        dtApprove = dtOutput;

                    newRow["ApproveTime"] = Time2String(dtApprove);

                    n = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);
                    dt = reader.GetDateTime(idx++);

                    if (getPreflightLog)
                    {
                        newRow["Preflightstatus"] = Globals.GetNameFromID("EventNameCache", n);
                        int i = s.IndexOf("?");
                        if (i != -1)
                            s = s.Substring(i + 1);
                        i = s.IndexOf("Report for");
                        if (i != -1)
                            s = s.Substring(0, i);
                        s.Trim();

                        newRow["Preflightmessage"] = s;
                    }

                    n = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);
                    dt = reader.GetDateTime(idx++);

                    if (getRipLog)
                    {
                        newRow["RIPstatus"] = Globals.GetNameFromID("EventNameCache", n);
                        newRow["RIPmessage"] = s;
                    }

                    n = reader.GetInt32(idx++);
                    s = reader.GetString(idx++);

                    if (getColorLogout)
                    {
                        newRow["Colorstatus"] = Globals.GetNameFromID("EventNameCache", n);
                        newRow["Colormessage"] = s;
                    }

                    newRow["ViewTime"] = "";
                    newRow["ViewUser"] = "";
                    dt = reader.GetDateTime(idx++);
                    s = reader.GetString(idx++);

                    if (getViewTime)
                    {
                        newRow["ViewTime"] = Time2String(dt);
                        newRow["ViewUser"] = s;
                    }
                    // Get
                    n = reader.GetInt32(idx++);	// PageIndex
                    newRow["Page"] = n;

                    reader.GetInt32(idx++); // Pagination
                    DateTime deadline = reader.GetDateTime(idx++);

                    if (orderByPlate)
                    {
                        n = reader.GetInt32(idx++);
                        newRow["Sheet"] = n;

                        n = reader.GetInt32(idx++);
                        if (n == 0)
                            newRow["Side"] = "Front";
                        else
                            newRow["Side"] = "Back";

                    }

                    newRow["InputTimeAfterDeadline"] = "";
                    if (showAfterdeadline)
                    {
                        dtInput = reader.GetDateTime(idx++);
                        if (reader.FieldCount > idx)
                        {
                            string s1 = reader.GetString(idx++);
                            if (s1.Length > 3)
                                s1 = s1.Substring(0, 3);
                            string s2 = (dtInput < dtOutput) ? " (" + Global.rm.GetString("txtImaged") + ")" :
                                " (" + Global.rm.GetString("txtNotImaged") + ")";

                            newRow["InputTimeAfterDeadline"] = Time2String(dtInput);
                        }
                    }
                    else
                    {
                        reader.GetDateTime(idx++);
                        if (reader.FieldCount > idx)
                            reader.GetString(idx++);
                    }

                    if (showCMYKInk)
                    {
                        newRow["Cink"] = 0;
                        newRow["Mink"] = 0;
                        newRow["Yink"] = 0;
                        newRow["Kink"] = 0;


                        if (reader.FieldCount > idx)
                            s = reader.GetString(idx++);
                        else
                            s = "";

                        string[] clrs = s.Split(',');
                        if (clrs.Length >= 4)
                        {
                            newRow["Cink"] = Globals.TryParse(clrs[0], 0);
                            newRow["Mink"] = Globals.TryParse(clrs[1], 0);
                            newRow["Yink"] = Globals.TryParse(clrs[2], 0);
                            newRow["Kink"] = Globals.TryParse(clrs[3], 0);

                            Cinkmean += Globals.TryParse(clrs[0], 0);
                            Minkmean += Globals.TryParse(clrs[1], 0);
                            Yinkmean += Globals.TryParse(clrs[2], 0);
                            Kinkmean += Globals.TryParse(clrs[3], 0);
                            ++Pagesmean;
                        }

                        //   if (clrs.Length >= 5 && fAreaMean == 0.0)
                        //       fAreaMean = Globals.TryParseDouble(clrs[4], 0.0);

                    }

                    if (status == 0)
                    {
                        newRow["Version"] = 0;
                        newRow["InputTime"] = "";

                        newRow["Approved"] = "";
                        newRow["ApproveUser"] = "";
                        if (getReadyTime)
                            newRow["ReadyTime"] = "";
                        newRow["OutputTime"] = "";
                        newRow["ApproveTime"] = "";
                        if (getViewTime)
                        {
                            newRow["ViewTime"] = "";
                            newRow["ViewUser"] = "";
                        }
                        if (getColorLogout)
                        {
                            newRow["Colormessage"] = "";
                            newRow["Colorstatus"] = "";
                        }
                        if (showAfterdeadline)
                            newRow["InputTimeAfterDeadline"] = "";

                        if (showCMYKInk)
                        {
                            newRow["Cink"] = 0;
                            newRow["Mink"] = 0;
                            newRow["Yink"] = 0;
                            newRow["Kink"] = 0;
                        }
                    }

                    tblMain.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (Pagesmean > 0)
            {
                Cinkmean = (int)(Math.Round((double)Cinkmean / (double)Pagesmean));
                Minkmean = (int)(Math.Round((double)Minkmean / (double)Pagesmean));
                Yinkmean = (int)(Math.Round((double)Yinkmean / (double)Pagesmean));
                Kinkmean = (int)(Math.Round((double)Kinkmean / (double)Pagesmean));
            }
            return tblMain;
        }

        public DataTable GetMonthlyCollectionExcel(out string errmsg)
        {
            errmsg = "";

            DataTable dt = new DataTable();

            SqlCommand command = new SqlCommand("spMonthlyReport", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter(command);

            SqlParameter param = command.Parameters.Add("@DaysToKeep", SqlDbType.Int);
            param.Value = 30;

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
            if ((bool)HttpContext.Current.Application["UsePressGroups"])
                param.Value = 0;

            if ((bool)HttpContext.Current.Application["UsePressGroups"])
            {
                param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
            }

            param = command.Parameters.Add("@UserName", SqlDbType.VarChar, 50);
            param.Direction = ParameterDirection.Input;
            param.Value = (string)HttpContext.Current.Session["UserName"];

            try
            {
                sqlDataAdaptor.Fill(dt);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dt;

        }

        public int GetReadviewNextPage(int mastercopyseparationset, out string pageName, out int approved, out string errmsg)
        {
            errmsg = "";
            approved = 1;
            pageName = "0";
            int ret = 0;

            string sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Pagination FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND P.PubDate=P2.PubDate AND P.Dirty=0 AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID AND P2.Dirty=0  WHERE P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.Pagination < P.Pagination AND P.ProofStatus = 20 AND P.Status >= 10 AND P.PageType<2  ORDER BY P.Pagination";
            if ((bool)HttpContext.Current.Application["NextButtonOrderByPageIndex"])
                sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.PageIndex FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND P.PubDate=P2.PubDate AND P.Dirty=0 AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID AND P2.Dirty=0 WHERE P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.PageIndex < P.PageIndex AND P.ProofStatus = 20 AND P.Status >= 10 AND  P.PageType<2 ORDER BY P.PageIndex";
            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ret = reader.GetInt32(0);
                    pageName = reader.GetString(1);
                    approved = reader.GetInt32(2);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return ret;
        }

        public int GetReadviewPrevPage(int mastercopyseparationset, out string pageName, out int approved, out string errmsg)
        {
            errmsg = "";
            approved = 1;
            pageName = "0";
            int ret = 0;
            string sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Pagination FROM PageTable P WITH (NOLOCK)  INNER JOIN PageTable P2 WITH (NOLOCK) ON P.PublicationID=P2.PublicationID AND P.PubDate=P2.PubDate AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>=10 AND P.ProofStatus=20 AND P.Active>0 AND P.PageType<2 AND P.Dirty=0 AND P2.Dirty=0 AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.Pagination > P.Pagination AND  ORDER BY P.Pagination DESC";
            if ((bool)HttpContext.Current.Application["NextButtonOrderByPageIndex"])
                sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.PageIndex FROM PageTable P WITH (NOLOCK)  INNER JOIN PageTable P2 WITH (NOLOCK) ON P.PublicationID=P2.PublicationID AND P.PubDate=P2.PubDate AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>=10 AND P.ProofStatus=20 AND P.Active>0 AND P.PageType<2 AND P.Dirty=0 AND P2.Dirty=0 AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.PageIndex > P.PageIndex  ORDER BY P.PageIndex DESC";

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ret = reader.GetInt32(0);
                    pageName = reader.GetString(1);
                    approved = reader.GetInt32(2);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return ret;
        }

        public int GetNextPage(int mastercopyseparationset, bool needapprovalonly, out int version, out string pageName, out int approved, out bool isMono, out string errmsg)
        {
            errmsg = "";
            approved = 1;
            pageName = "0";
            version = 0;
            isMono = false;
            int ret = 0;
            string sql;
            if ((bool)HttpContext.Current.Application["NextButtonOrderByPageIndex"])
            {
                sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Version,P.PageIndex FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND  P.Pubdate=P2.Pubdate AND  P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>=10 AND P.ProofStatus>=10 AND P.Active>0 AND P.PageType<2 AND P.Dirty=0 AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.PageIndex < P.PageIndex  ORDER BY P.PageIndex, P.Version DESC";
                if (needapprovalonly)
                    sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Version,P.PageIndex FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID  AND P.Pubdate=P2.Pubdate AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>=10 AND P.ProofStatus>=10 AND P.Active>0 AND P.PageType<2 AND P.Dirty=0 AND (P.Approved=0 OR P.Approved = 2) AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.PageIndex < P.PageIndex  ORDER BY P.PageIndex,P.Version DESC";
            }
            else
            {
                sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Version,P.Pagination FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND P.Pubdate=P2.Pubdate  AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>=10 AND P.ProofStatus>=10 AND P.Active>0 AND P.PageType<2 AND P.Dirty=0 AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.Pagination < P.Pagination  ORDER BY P.Pagination, P.Version DESC";
                if (needapprovalonly)
                    sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Version,P.Pagination FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND P.Pubdate=P2.Pubdate  AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>=10 AND P.ProofStatus>=10 AND P.Active>0 AND P.PageType<2 AND P.Dirty=0 AND (P.Approved=0 OR P.Approved = 2) AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.Pagination < P.Pagination  ORDER BY P.Pagination,P.Version DESC";
            }
            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ret = reader.GetInt32(0);
                    pageName = reader.GetString(1);
                    approved = reader.GetInt32(2);
                    version = reader.GetInt32(3);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (ret > 0)
            {
                int colors = 0;
                if (GetNumberOfPageColors(ret, out colors, out errmsg))
                    isMono = colors == 1;
            }

            return ret;
        }

        public int GetPrevPage(int mastercopyseparationset, bool needapprovalonly, out int version, out string pageName, out int approved, out bool isMono, out string errmsg)
        {
            errmsg = "";
            approved = 1;
            pageName = "0";
            int ret = 0;
            version = 0;
            isMono = false;
            string sql;
            if ((bool)HttpContext.Current.Application["NextButtonOrderByPageIndex"])
            {
                sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Version,P.PageIndex FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND P.Pubdate=P2.Pubdate AND P.Dirty=0 AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>0 AND P.ProofStatus>6 AND P.Active>0 AND P.PageType<2 AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.PageIndex > P.PageIndex ORDER BY P.PageIndex DESC, P.Version DESC";
                if (needapprovalonly)
                    sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Version,P.PageIndex FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND P.Pubdate=P2.Pubdate AND P.Dirty=0 AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>0 AND P.ProofStatus>6 AND P.Active>0 AND P.PageType<2 AND (P.Approved=0 OR P.Approved = 2) AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.PageIndex > P.PageIndex ORDER BY P.PageIndex DESC,P.Version DESC";
            }
            else
            {
                sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Version,P.Pagination FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND P.Pubdate=P2.Pubdate AND P.Dirty=0 AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>0 AND P.ProofStatus>6 AND P.Active>0 AND P.PageType<2 AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.Pagination > P.Pagination ORDER BY P.Pagination DESC, P.Version DESC";
                if (needapprovalonly)
                    sql = "SELECT DISTINCT P.MasterCopySeparationSet,P.PageName,P.Approved,P.Version,P.Pagination FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PublicationID=P2.PublicationID AND P.Pubdate=P2.Pubdate AND P.Dirty=0 AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>0 AND P.ProofStatus>6 AND P.Active>0 AND P.PageType<2 AND (P.Approved=0 OR P.Approved = 2) AND P2.MasterCopySeparationSet=" + mastercopyseparationset.ToString() + " AND P2.Pagination > P.Pagination ORDER BY P.Pagination DESC,P.Version DESC";

            }
            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ret = reader.GetInt32(0);
                    pageName = reader.GetString(1);
                    approved = reader.GetInt32(2);
                    version = reader.GetInt32(3);

                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (ret > 0)
            {
                int numberOfColors = 0;
                if (GetNumberOfPageColors(ret, out numberOfColors, out errmsg))
                    isMono = numberOfColors == 1;
            }
            return ret;
        }


        public int GetNextFlat(int copyFlatSeparationSet, out string errmsg)
        {
            errmsg = "";
            int ret = 0;
            string sql = "SELECT DISTINCT P.CopyFlatSeparationSet FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PressRunID=P2.PressRunID AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>=10 AND (P.InkStatus=10 OR P.FlatProofStatus=10) AND P.Active>0  AND P2.CopyFlatSeparationSet=" + copyFlatSeparationSet.ToString() + " AND (P2.SheetNumber*2+P2.SheetSide)=(P.SheetNumber*2+P.SheetSide)-1";
            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ret = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return ret;
        }

        public int GetPreviousFlat(int copyFlatSeparationSet, out string errmsg)
        {
            errmsg = "";
            int ret = 0;
            string sql = "SELECT DISTINCT P.CopyFlatSeparationSet FROM PageTable P WITH (NOLOCK) INNER JOIN PageTable P2 WITH (NOLOCK) ON  P.PressRunID=P2.PressRunID AND P.SectionID=P2.SectionID AND P.EditionID=P2.EditionID WHERE P.Status>=10 AND (P.InkStatus=10 OR P.FlatProofStatus=10) AND P.Active>0  AND P2.CopyFlatSeparationSet=" + copyFlatSeparationSet.ToString() + " AND (P2.SheetNumber*2+P2.SheetSide)=(P.SheetNumber*2+P.SheetSide)+1";
            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ret = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return ret;
        }



        public bool GetOrderList(ref ArrayList allOrders, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand("SELECT DISTINCT PublicationID,CustomOrderNumber,CustomOrderNumberPressRun, CustomOrderStatus,CustomOrderTime,CustomerID,PressID,PubDate,EditionID,SectionIDList,PagesInSectionList,Inserted,PressTime,ISNULL(Comment,''),ISNULL(Circulation,0),ISNULL(Circulation2,0) FROM CustomOrderNumbers ORDER BY CustomOrderNumber", connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            allOrders.Clear();

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                int orderID = 1;

                while (reader.Read())
                {

                    int publicationID = reader.GetInt32(0);
                    string publicationName = Globals.GetNameFromID("PublicationNameCache", publicationID);

                    string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];

                    bool addOrder = false;
                    if (pubsallowed == "*")
                        addOrder = true;
                    else
                    {
                        string[] publist = pubsallowed.Split(',');
                        foreach (string sp in publist)
                        {
                            if (sp == publicationName)
                            {
                                addOrder = true;
                                break;
                            }
                        }
                    }

                    if (addOrder)
                    {
                        OrderEntry order = new OrderEntry();
                        order.m_orderID = orderID++;
                        order.m_publicationID = publicationID;

                        order.m_customerOrderNumber = reader.GetString(1);
                        order.m_customerOrderNumberPressRun = reader.GetString(2);
                        order.m_customerOrderStatus = reader.GetString(3);
                        order.m_customerOrderTime = reader.GetDateTime(4);
                        order.m_customerID = reader.GetInt32(5);
                        order.m_pressID = reader.GetInt32(6);

                        order.m_pubDate = reader.GetDateTime(7);
                        order.m_editionID = reader.GetInt32(8);

                        order.m_sectionIDList = reader.GetString(9);
                        order.m_pagesInSectionList = reader.GetString(10);

                        order.m_inserted = reader.GetInt32(11) > 0;
                        order.m_pressTime = reader.GetDateTime(12);
                        order.m_comment = reader.GetString(13);
                        order.m_circulation = reader.GetInt32(14);
                        order.m_circulation2 = reader.GetInt32(15);

                        allOrders.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        public bool GetJobDetailsForEdit(int productionID, ref ArrayList allOrders, out string errmsg)
        {
            errmsg = "";

            string sqlStmt = string.Format("SELECT DISTINCT ISNULL(PR.OrderNumber,''),P.PublicationID,P.PubDate, P.EditionID,P.SectionID, P.PressRunID, P.PressSectionNumber, P.PressID, P.TemplateID, P.CustomerID, P.PressTime,PRS.Comment, COUNT(DISTINCT P.PageIndex),PRS.SequenceNumber,PRS.TimedEditionFrom,PRS.TimedEditionTo,PRS.FromZone,P.MiscString3,MAX(P.Pagination) FROM PageTable AS P WITH (NOLOCK) INNER JOIN ProductionNames AS PR WITH (NOLOCK) ON PR.ProductionID=P.ProductionID INNER JOIN PressRunID AS PRS WITH (NOLOCK) ON PRS.PressRunID=P.PressRunID WHERE P.PageType<=2 AND P.Active>0 AND P.ProductionID={0} GROUP BY P.PublicationID,P.PubDate,PRS.SequenceNumber, P.EditionID,P.SectionID, P.PressRunID, P.PressSectionNumber, P.PressID, P.TemplateID, PR.OrderNumber,P.CustomerID,P.PressTime,PRS.Comment,PRS.TimedEditionFrom,PRS.TimedEditionTo,PRS.FromZone,P.MiscString3 ORDER BY PRS.SequenceNumber,P.EditionID,P.SectionID, P.PressRunID, P.PressSectionNumber,PRS.TimedEditionFrom,PRS.TimedEditionTo,PRS.FromZone", productionID);

            SqlCommand command = new SqlCommand(sqlStmt, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            allOrders.Clear();

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                int orderID = 1;

                int thisEditionID = 0;
                OrderEntry order = null;
                while (reader.Read())
                {

                    string orderNumber = reader.GetString(0);
                    int publicationID = reader.GetInt32(1);
                    DateTime pubDate = reader.GetDateTime(2);
                    int editionID = reader.GetInt32(3);
                    int sectionID = reader.GetInt32(4);
                    int pressRunID = reader.GetInt32(5);
                    int pressSectionNumber = reader.GetInt32(6);
                    int pressID = reader.GetInt32(7);
                    int templateID = reader.GetInt32(8);
                    int customerID = reader.GetInt32(9);
                    DateTime pressTime = reader.GetDateTime(10);
                    string orderComment = reader.GetString(11);
                    int numberOfPages = reader.GetInt32(12);
                    reader.GetInt32(13);	// Skip sequencenumber
                    int timedEditionFromID = reader.GetInt32(14);
                    int timedEditionToID = reader.GetInt32(15);
                    int timedEditionSequence = reader.GetInt32(16);
                    string miscString3 = reader.GetString(17);

                    int highestpagenumber = reader.GetInt32(18);

                    if ((highestpagenumber % 2) > 0)
                        highestpagenumber++;

                    if (numberOfPages < highestpagenumber)
                        numberOfPages = highestpagenumber;


                    if (thisEditionID != editionID)
                    {
                        order = new OrderEntry();

                        order.m_orderID = orderID++;
                        order.m_publicationID = publicationID;
                        order.m_customerOrderNumber = orderNumber;
                        order.m_customerOrderStatus = "";
                        order.m_customerOrderTime = DateTime.Now;
                        order.m_customerID = customerID;
                        order.m_pressID = pressID;

                        order.m_pubDate = pubDate;
                        order.m_editionID = editionID;

                        order.m_sectionIDList = sectionID.ToString();
                        order.m_pagesInSectionList = numberOfPages.ToString();

                        order.m_inserted = false;
                        order.m_pressTime = pressTime;
                        order.m_comment = orderComment;
                        order.m_timedEditionFrom = timedEditionFromID;
                        order.m_timedEditionTo = timedEditionToID;
                        order.m_miscString3 = miscString3;
                        order.m_timedEditionSequence = timedEditionSequence;


                        thisEditionID = editionID;


                        allOrders.Add(order);
                    }
                    else
                    {
                        order.m_sectionIDList += "," + sectionID.ToString();
                        order.m_pagesInSectionList += "," + numberOfPages.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool GetEditionJobDetailsForEdit(int publicationID, DateTime pubDate, int editionID, int sectionID, ref ArrayList editionPageList, out string errmsg)
        {
            errmsg = "";
            editionPageList.Clear();

            SqlCommand command = new SqlCommand("spGetMasterEditionList", connection);

            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = null;

            command.Parameters.Clear();
            SqlParameter param;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = publicationID;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Direction = ParameterDirection.Input;
            param.Value = pubDate;

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = editionID;


            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = sectionID;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    editionPageList.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool GetEditionUniqueFlags(ref ArrayList arrUniqueList, int pressRunID, int editionID, int sectionID, out string errmsg)
        {
            errmsg = "";
            arrUniqueList.Clear();

            string sqlStmt = string.Format("SELECT DISTINCT PageIndex, UniquePage FROM PageTable WITH (NOLOCK) WHERE PressRunID={0} AND EditionID={1} AND SectionID={2} AND Active>0 AND PageType>=2", pressRunID, editionID, sectionID);
            if (sectionID == 0)
                sqlStmt = string.Format("SELECT DISTINCT PageIndex, UniquePage FROM PageTable  WITH (NOLOCK) WHERE PressRunID={0} AND EditionID={1} AND Active>0 AND PageType>=2", pressRunID, editionID);

            SqlCommand command = new SqlCommand(sqlStmt, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    bool ret = reader.GetInt32(0) > 0 ? true : false;
                    arrUniqueList.Add(ret);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }



        public ArrayList GetReadViewMasterSetCollection(bool hideApproved, bool hideCommon, bool bUnfiltered, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();
            DataTable dataTablePages = GetThumbnailPageCollection(false, false, bUnfiltered, out errmsg);
            int prevPagination = 0;
            int prevProofStatus = 0;
            int prevMasterCopySeparationSet = 0;
            int prevPageType = 0;

            int pagination = 0;
            foreach (DataRow pagerow in dataTablePages.Rows)
            {
                pagination = (int)pagerow["Pagination"];

                if (pagination == 1)
                {
                    int m2 = (int)pagerow["MasterCopySeparationSet"];
                    string s = "0_" + m2.ToString();
                    al.Add(s);
                    prevPagination = 0;
                    continue;
                }
                else
                {
                    if (pagination % 2 == 0) // Left page
                    {
                        prevPageType = (int)pagerow["PageType"];
                        if (prevPageType == 1)
                        {
                            // Panorama
                            int m1 = (int)pagerow["MasterCopySeparationSet"];
                            string pagename = "0";
                            int appr = 1;
                            int m2 = GetReadviewNextPage((int)pagerow["MasterCopySeparationSet"], out pagename, out appr, out errmsg);//(int)pagerow["MasterCopySeparationSet"];

                            string s = m1.ToString() + "_" + m2.ToString();
                            al.Add(s);

                            prevPagination = 0;
                            continue;

                        }
                        else
                        {
                            prevMasterCopySeparationSet = (int)pagerow["MasterCopySeparationSet"];
                            prevPagination = pagination;
                            continue;
                        }
                    }
                    else // Right page..
                    {
                        int m2 = (int)pagerow["MasterCopySeparationSet"];

                        string s = prevMasterCopySeparationSet.ToString() + "_" + m2.ToString();
                        al.Add(s);

                        prevPagination = 0;
                        continue;

                    }
                }
            }
            if (pagination > 0)
            {
                string s = prevMasterCopySeparationSet.ToString() + "_0";
                al.Add(s);
            }

            return al;
        }

        public DataSet GetReadViewCollection(bool hideApproved, bool hideCommon, bool bUnfiltered, out string errmsg)
        {
            errmsg = "";

            DataTable dataTablePages = GetThumbnailPageCollection(false, false, bUnfiltered, out errmsg);

            DataTable tblMain = new DataTable("PageView");
            DataSet dataSet = new DataSet("PageView");
            DataColumn newColumn;

            newColumn = tblMain.Columns.Add("Production", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Publication", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PubDate", Type.GetType("System.DateTime"));
            newColumn = tblMain.Columns.Add("Issue", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Edition", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Page", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Page2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Color", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Color2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Status", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ExternalStatus", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Version", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Version2", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Approval", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("CopyNumber", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Pagination", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Pagination2", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("LastError", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Comment", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ProofStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ProofStatus2", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Location", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PageType", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MasterCopySeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MasterCopySeparationSet2", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Active", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Active2", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("UniquePage", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("UniquePage2", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Hold", Type.GetType("System.Int32"));

            DataRow newRow = null;

            string prevPage = "";
            string prevColor = "";
            int prevVersion = 1;
            int prevPagination = 0;
            int prevProofStatus = 0;
            int prevMasterCopySeparationSet = 0;
            int prevActive = 0;
            int prevUniquePage = 0;
            int prevPageType = 0;
            int prevHold = 0;
            int prevCopyNumber = 1;
            string prevStatus = "";
            string prevExternalStatus = "";
            int prevApproval = 0;
            string prevLastError = "";
            string prevComment = "";

            string prevProduction = "";
            string prevPublication = "";
            DateTime prevPubDate = DateTime.Now;
            string prevIssue = "";
            string prevEdition = "";
            string prevSection = "";
            string prevLocation = "";


            int pagination = 0;
            foreach (DataRow pagerow in dataTablePages.Rows)
            {
                pagination = (int)pagerow["Pagination"];

                if (pagination == 1)
                {
                    newRow = tblMain.NewRow();
                    newRow["Production"] = (string)pagerow["Production"];
                    newRow["Publication"] = (string)pagerow["Publication"];
                    newRow["PubDate"] = (DateTime)pagerow["PubDate"];
                    newRow["Issue"] = (string)pagerow["Issue"];
                    newRow["Edition"] = (string)pagerow["Edition"];
                    newRow["Section"] = (string)pagerow["Section"];
                    newRow["Location"] = (string)pagerow["Location"];

                    newRow["Page"] = "0";
                    newRow["Page2"] = (string)pagerow["Page"];
                    newRow["Color"] = (string)pagerow["Color"];
                    newRow["Color2"] = (string)pagerow["Color"];
                    newRow["Status"] = (string)pagerow["Status"];
                    newRow["ExternalStatus"] = (string)pagerow["ExternalStatus"];
                    newRow["Version"] = (int)pagerow["Version"];
                    newRow["Version2"] = (int)pagerow["Version"];
                    newRow["Approval"] = (int)pagerow["Approval"];
                    newRow["CopyNumber"] = (int)pagerow["CopyNumber"];
                    newRow["Pagination"] = 0;
                    newRow["Pagination2"] = (int)pagerow["Pagination"];
                    newRow["LastError"] = (string)pagerow["LastError"];
                    newRow["Comment"] = (string)pagerow["Comment"];
                    newRow["ProofStatus"] = (int)pagerow["ProofStatus"];
                    newRow["ProofStatus2"] = (int)pagerow["ProofStatus"];

                    newRow["PageType"] = (int)pagerow["PageType"];
                    newRow["MasterCopySeparationSet"] = 0;
                    newRow["MasterCopySeparationSet2"] = (int)pagerow["MasterCopySeparationSet"];
                    newRow["Active"] = (int)pagerow["Active"];
                    newRow["Active2"] = (int)pagerow["Active"];
                    newRow["UniquePage"] = (int)pagerow["UniquePage"];
                    newRow["UniquePage2"] = (int)pagerow["UniquePage"];
                    newRow["Hold"] = (int)pagerow["Hold"];

                    tblMain.Rows.Add(newRow);
                    prevPagination = 0;
                    continue;
                }
                else
                {
                    if (pagination % 2 == 0) // Left page
                    {
                        prevPageType = (int)pagerow["PageType"];
                        if (prevPageType == 1)
                        {
                            newRow = tblMain.NewRow();
                            newRow["Production"] = (string)pagerow["Production"];
                            newRow["Publication"] = (string)pagerow["Publication"];
                            newRow["PubDate"] = (DateTime)pagerow["PubDate"];
                            newRow["Issue"] = (string)pagerow["Issue"];
                            newRow["Edition"] = (string)pagerow["Edition"];
                            newRow["Section"] = (string)pagerow["Section"];
                            newRow["Page"] = (string)pagerow["Page"];
                            int n = pagination + 1;
                            try
                            {
                                n = Convert.ToInt32(newRow["Page"]) + 1;
                            }
                            catch
                            {
                            }
                            newRow["Page2"] = n.ToString();
                            newRow["Color"] = (string)pagerow["Color"];
                            newRow["Color2"] = (string)pagerow["Color"];


                            newRow["Status"] = (string)pagerow["Status"];
                            newRow["ExternalStatus"] = (string)pagerow["ExternalStatus"];

                            newRow["Version"] = (int)pagerow["Version"];
                            newRow["Version2"] = (int)pagerow["Version"];

                            newRow["Approval"] = (int)pagerow["Approval"];


                            newRow["CopyNumber"] = (int)pagerow["CopyNumber"];

                            newRow["Pagination"] = (int)pagerow["Pagination"];
                            newRow["Pagination2"] = (int)pagerow["Pagination"] + 1;

                            newRow["LastError"] = (string)pagerow["LastError"];
                            newRow["Comment"] = (string)pagerow["Comment"];

                            newRow["ProofStatus"] = (int)pagerow["ProofStatus"];
                            newRow["ProofStatus2"] = (int)pagerow["ProofStatus"];

                            newRow["Location"] = (string)pagerow["Location"];

                            newRow["PageType"] = (int)pagerow["PageType"];
                            newRow["MasterCopySeparationSet"] = (int)pagerow["MasterCopySeparationSet"];
                            string pagename = "0";
                            int appr = 1;
                            newRow["MasterCopySeparationSet2"] = GetReadviewNextPage((int)pagerow["MasterCopySeparationSet"], out pagename, out appr, out errmsg);//(int)pagerow["MasterCopySeparationSet"];

                            newRow["Active"] = (int)pagerow["Active"];
                            newRow["Active2"] = (int)pagerow["Active"];
                            newRow["UniquePage"] = (int)pagerow["UniquePage"];
                            newRow["UniquePage2"] = (int)pagerow["UniquePage"];
                            newRow["Hold"] = (int)pagerow["Hold"];

                            tblMain.Rows.Add(newRow);
                            prevPagination = 0;
                            continue;



                        }
                        else
                        {
                            prevProduction = (string)pagerow["Production"];
                            prevPublication = (string)pagerow["Publication"];
                            prevPubDate = (DateTime)pagerow["PubDate"];
                            prevIssue = (string)pagerow["Issue"];
                            prevEdition = (string)pagerow["Edition"];
                            prevSection = (string)pagerow["Section"];
                            prevLocation = (string)pagerow["Location"];

                            prevPage = (string)pagerow["Page"];
                            prevColor = (string)pagerow["Color"];
                            prevVersion = (int)pagerow["Version"];
                            prevPagination = pagination;
                            prevProofStatus = (int)pagerow["ProofStatus"];
                            prevMasterCopySeparationSet = (int)pagerow["MasterCopySeparationSet"];
                            prevActive = (int)pagerow["Active"];
                            prevUniquePage = (int)pagerow["UniquePage"];

                            prevHold = (int)pagerow["Hold"];
                            prevCopyNumber = (int)pagerow["CopyNumber"];
                            prevStatus = (string)pagerow["Status"];
                            prevExternalStatus = (string)pagerow["ExternalStatus"];
                            prevApproval = (int)pagerow["Approval"];
                            prevLastError = (string)pagerow["LastError"];
                            prevComment = (string)pagerow["Comment"];
                            continue;
                        }
                    }
                    else // Right page..
                    {
                        newRow = tblMain.NewRow();
                        newRow["Production"] = (string)pagerow["Production"];
                        newRow["Publication"] = (string)pagerow["Publication"];
                        newRow["PubDate"] = (DateTime)pagerow["PubDate"];
                        newRow["Issue"] = (string)pagerow["Issue"];
                        newRow["Edition"] = (string)pagerow["Edition"];
                        newRow["Section"] = (string)pagerow["Section"];
                        newRow["Page"] = prevPage;
                        newRow["Page2"] = (string)pagerow["Page"];
                        newRow["Color"] = prevColor;
                        newRow["Color2"] = (string)pagerow["Color"];

                        int nStatus1 = Globals.GetStatusID(prevStatus, 0);
                        int nStatus2 = Globals.GetStatusID((string)pagerow["Status"], 0);

                        newRow["Status"] = Globals.GetStatusName(nStatus1 < nStatus2 ? nStatus1 : nStatus2, 0);

                        nStatus1 = Globals.GetStatusID(prevExternalStatus, 1);
                        nStatus2 = Globals.GetStatusID((string)pagerow["ExternalStatus"], 0);

                        newRow["ExternalStatus"] = Globals.GetStatusName(nStatus1 < nStatus2 ? nStatus1 : nStatus2, 1);

                        newRow["Version"] = prevVersion;
                        newRow["Version2"] = (int)pagerow["Version"];

                        newRow["Approval"] = (int)pagerow["Approval"];
                        if (prevApproval == 0 || prevApproval == 2)
                            newRow["Approval"] = prevApproval;

                        newRow["CopyNumber"] = (int)pagerow["CopyNumber"];

                        newRow["Pagination"] = prevPagination;
                        newRow["Pagination2"] = (int)pagerow["Pagination"];

                        newRow["LastError"] = prevLastError + "," + (string)pagerow["LastError"];
                        newRow["Comment"] = prevComment + "," + (string)pagerow["Comment"];

                        newRow["ProofStatus"] = prevProofStatus;
                        newRow["ProofStatus2"] = (int)pagerow["ProofStatus"];

                        newRow["Location"] = (string)pagerow["Location"];

                        newRow["PageType"] = prevPageType;
                        newRow["MasterCopySeparationSet"] = prevMasterCopySeparationSet;
                        newRow["MasterCopySeparationSet2"] = (int)pagerow["MasterCopySeparationSet"];

                        newRow["Active"] = prevActive;
                        newRow["Active2"] = (int)pagerow["Active"];
                        newRow["UniquePage"] = prevUniquePage;
                        newRow["UniquePage2"] = (int)pagerow["UniquePage"];
                        newRow["Hold"] = (int)pagerow["Hold"];

                        tblMain.Rows.Add(newRow);
                        prevPagination = 0;
                        continue;
                    }
                }
            }
            if (pagination > 0)
            {
                newRow = tblMain.NewRow();
                newRow["Production"] = prevProduction;
                newRow["Publication"] = prevPublication;
                newRow["PubDate"] = prevPubDate;
                newRow["Issue"] = prevIssue;
                newRow["Edition"] = prevEdition;
                newRow["Section"] = prevSection;
                newRow["Page"] = prevPage;
                newRow["Page2"] = "0";
                newRow["Color"] = prevColor;
                newRow["Color2"] = prevColor;
                newRow["Status"] = prevStatus;
                newRow["ExternalStatus"] = prevExternalStatus;
                newRow["Version"] = prevVersion;
                newRow["Version2"] = prevVersion;
                newRow["Approval"] = prevApproval;
                newRow["CopyNumber"] = prevCopyNumber;
                newRow["Pagination"] = pagination;
                newRow["Pagination2"] = 0;
                newRow["LastError"] = prevLastError;
                newRow["Comment"] = prevComment;
                newRow["ProofStatus"] = prevProofStatus;
                newRow["ProofStatus2"] = prevProofStatus;
                newRow["Location"] = prevLocation;
                newRow["PageType"] = prevPageType;
                newRow["MasterCopySeparationSet"] = prevMasterCopySeparationSet;
                newRow["MasterCopySeparationSet2"] = 0;
                newRow["Active"] = prevActive;
                newRow["Active2"] = prevActive;
                newRow["UniquePage"] = prevUniquePage;
                newRow["UniquePage2"] = prevUniquePage;
                newRow["Hold"] = prevHold;

                tblMain.Rows.Add(newRow);
            }

            dataSet.Tables.Add(tblMain);

            return dataSet;
        }

        public DataSet GetPageSequenceForReadView(int mastercopysseparationset, out string errmsg)
        {
            errmsg = "";
            SqlCommand command = new SqlCommand("spProofGetReadViews", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable();
            DataSet dataSet = new DataSet();

            DataColumn newColumn;

            newColumn = tblMain.Columns.Add("Pagination", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PageName", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MasterCopySeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Status", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ProofStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PageType", Type.GetType("System.Int32"));
            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@CurrentMasterCopySeparationSet", SqlDbType.Int);
                param.Value = mastercopysseparationset;

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;

                    newRow = tblMain.NewRow();
                    newRow["Pagination"] = reader.GetInt32(idx++);
                    newRow["PageName"] = reader.GetString(idx++);
                    newRow["MasterCopySeparationSet"] = reader.GetInt32(idx++);
                    newRow["Status"] = reader.GetInt32(idx++);
                    newRow["ProofStatus"] = reader.GetInt32(idx++);
                    newRow["PageType"] = reader.GetInt32(idx++);
                    tblMain.Rows.Add(newRow);
                }

            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                dataSet = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (dataSet != null)
                dataSet.Tables.Add(tblMain);

            return dataSet;

        }


        public DataTable GetThumbnailPageCollection2(bool hideApproved, bool hideCommon, bool bUnfiltered, out string errmsg)
        {
            string spress = (string)HttpContext.Current.Session["SelectedPress"];
            errmsg = "";

            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spThumbnailPageList2", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable("PrepollPageTable");
            //DataSet dataSet = new DataSet("PrepollPageTable");

            DataColumn newColumn;

            newColumn = tblMain.Columns.Add("MasterCopySeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FTPStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FTPMessage", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PreflightStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PreflightMessage", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("RIPStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("RIPMessage", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ColorStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ColorMessage", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("InkSaveStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("InkSaveMessage", Type.GetType("System.String"));

            newColumn = tblMain.Columns.Add("MessageStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MessageIsRead", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ThumbnailSize", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PdfMaster", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MaxStatus", Type.GetType("System.Int32"));
            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? tDefaultPubDate : (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@Approved", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideApproved ? 1 : -1);

                param = command.Parameters.Add("@Unique", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideCommon ? 1 : -1);

                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);

                param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                int thisMasterCopySeparationSet = 0;
                int prevMasterCopySeparationSet = -1;

                string thisPublication = "";

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                bool bIsFirst = true; ;
                while (reader.Read())
                {
                    int idx = 0;
                    int fieldCount = reader.FieldCount;
                    thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(idx++));
                    /*
                                        if (pubsallowed != "*")
                                        {
                                            bool found = false;
                                            foreach (string sp in publist)
                                            {
                                                if (sp == thisPublication)
                                                {
                                                    found = true;
                                                    break;
                                                }
                                            }
                                            if (found == false)
                                                continue;
                                        }
                    */
                    thisMasterCopySeparationSet = reader.GetInt32(idx++);	// CopyColorSet

                    if (thisMasterCopySeparationSet != prevMasterCopySeparationSet)
                    {
                        // Store previous page
                        if (bIsFirst == false)
                            tblMain.Rows.Add(newRow);

                        bIsFirst = false;

                        newRow = tblMain.NewRow();
                        newRow["FTPStatus"] = 0;
                        newRow["FTPMessage"] = "";
                        newRow["PreflightStatus"] = 0;
                        newRow["PreflightMessage"] = "";
                        newRow["RIPStatus"] = 0;
                        newRow["RIPMessage"] = "";
                        newRow["ColorStatus"] = 0;
                        newRow["ColorMessage"] = "";
                        newRow["InkSaveStatus"] = 0;
                        newRow["InkSaveMessage"] = "";
                        newRow["MessageStatus"] = 0;
                        newRow["MessageIsRead"] = 0;
                        newRow["ThumbnailSize"] = 0;
                        newRow["PdfMaster"] = 0;
                        newRow["MaxStatus"] = 0;

                    }

                    newRow["MasterCopySeparationSet"] = thisMasterCopySeparationSet;

                    int nProcessType = reader.GetInt32(idx++);
                    int nStatus = reader.GetInt32(idx++);
                    string sMessage = reader.GetString(idx++);
                    reader.GetDateTime(idx++); // pubdate
                    reader.GetInt32(idx++);
                    reader.GetInt32(idx++); // ed
                    reader.GetInt32(idx++); // sec
                    reader.GetInt32(idx++); // pageindex

                    newRow["PdfMaster"] = 0;
                    if (fieldCount >= 11)
                    {
                        if (!reader.IsDBNull(idx))
                            newRow["PdfMaster"] = reader.GetInt32(idx); // pdfMaster
                        idx++;
                    }
                    newRow["MaxStatus"] = 0;
                    if (fieldCount >= 12)
                    {
                        if (!reader.IsDBNull(idx))
                            newRow["MaxStatus"] = reader.GetInt32(idx); // MAX(Status)
                        idx++;
                    }

                    if (nProcessType == 0 || nProcessType == 190)
                    {
                        newRow["FTPStatus"] = nStatus;
                        newRow["FTPMessage"] = sMessage;
                    }
                    else if (nProcessType == 1 || nProcessType == 2)
                    {
                        newRow["PreflightStatus"] = nStatus;

                        int i = sMessage.IndexOf("?");
                        if (i != -1)
                            sMessage = sMessage.Substring(i + 1);
                        i = sMessage.IndexOf("Report for");
                        if (i != -1)
                            sMessage = sMessage.Substring(0, i);
                        sMessage.Trim();


                        newRow["PreflightMessage"] = sMessage;
                    }
                    else if (nProcessType == 4)
                    {
                        newRow["InkSaveStatus"] = nStatus;
                        newRow["InkSaveMessage"] = sMessage;
                    }
                    else if (nProcessType == 10)
                    {
                        newRow["RIPStatus"] = nStatus;
                        newRow["RIPMessage"] = sMessage;
                    }
                    else if (nProcessType == 50)
                    {
                        newRow["ColorStatus"] = nStatus;
                        newRow["ColorMessage"] = sMessage;
                    }

                    if (nProcessType == 340 && sMessage != "")
                    {
                        int nSize = 0;
                        if (Int32.TryParse(sMessage, out nSize))
                            newRow["ThumbnailSize"] = nSize;
                    }

                    prevMasterCopySeparationSet = thisMasterCopySeparationSet;

                }
                if (bIsFirst == false)
                    tblMain.Rows.Add(newRow);

            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                Global.logging.WriteLog("GetThumbnailPageCollection2() " + errmsg);

                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblMain;
        }



        public DataTable GetThumbnailPageCollection2Channels(bool hideApproved, bool hideCommon, bool bUnfiltered, out string errmsg)
        {
            string spress = (string)HttpContext.Current.Session["SelectedPress"];
            errmsg = "";

            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spThumbnailPageList2", connection)
            {

                // Mark the Command as a SPROC
                CommandType = CommandType.StoredProcedure
            };

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable("PrepollPageTable");
            //DataSet dataSet = new DataSet("PrepollPageTable");

            DataColumn newColumn;

            newColumn = tblMain.Columns.Add("MasterCopySeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FileStatusLow", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FileStatusHigh", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FileStatusPrint", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("ColorStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ColorMessage", Type.GetType("System.String"));

            newColumn = tblMain.Columns.Add("MessageStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MessageIsRead", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("ThumbnailSize", Type.GetType("System.Int32"));


            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? tDefaultPubDate : (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@Approved", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideApproved ? 1 : -1);

                param = command.Parameters.Add("@Unique", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideCommon ? 1 : -1);

                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                    param.Value = 0;

                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                {
                    param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                }

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                int thisMasterCopySeparationSet = 0;
                int prevMasterCopySeparationSet = -1;

                string thisPublication = "";

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                bool bIsFirst = true; ;
                while (reader.Read())
                {
                    int idx = 0;

                    thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(idx++));

                    thisMasterCopySeparationSet = reader.GetInt32(idx++);

                    if (thisMasterCopySeparationSet != prevMasterCopySeparationSet)
                    {
                        // Store previous page
                        if (bIsFirst == false)
                            tblMain.Rows.Add(newRow);

                        bIsFirst = false;

                        newRow = tblMain.NewRow();
                        newRow["FileStatusLow"] = 0;
                        newRow["FileStatusHigh"] = 0;
                        newRow["FileStatusPrint"] = 0;

                        // Not used
                        newRow["ColorStatus"] = 0;
                        newRow["ColorMessage"] = "";

                        newRow["MessageStatus"] = 0;
                        newRow["MessageIsRead"] = 0;
                        newRow["ThumbnailSize"] = 0;
                        // Not used


                    }

                    newRow["MasterCopySeparationSet"] = thisMasterCopySeparationSet;

                    newRow["FileStatusLow"] = reader.GetInt32(idx++);
                    newRow["FileStatusHigh"] = reader.GetInt32(idx++);
                    newRow["FileStatusPrint"] = reader.GetInt32(idx++);
                    // Global.logging.WriteLog("GetThumbnailPageCollection2Channels() returned " + ((int)newRow["FileStatusLow"]).ToString() + "  " + ((int)newRow["FileStatusHigh"]).ToString() + "  " + ((int)newRow["FileStatusPrint"]).ToString());
                    prevMasterCopySeparationSet = thisMasterCopySeparationSet;

                }
                if (bIsFirst == false)
                    tblMain.Rows.Add(newRow);

            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                Global.logging.WriteLog("GetThumbnailPageCollection2() " + errmsg);

                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblMain;
        }


        public DateTime GetInputTime(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            DateTime tInputTime = new DateTime();
            tInputTime = DateTime.MinValue;
            string sql = string.Format("SELECT MAX(InputTime) FROM PageTable WITH (NOLOCK) WHERE MasterCopySeparationSet={0} AND Active>0 AND Dirty=0 AND PageType<3 AND Status>0", masterCopySeparationSet);

            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    tInputTime = reader.GetDateTime(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return DateTime.MinValue;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return tInputTime;
        }


        public DataTable GetFlatPageCollection(bool bUnfiltered, bool hideApproved, bool hideCommon, bool bAllCopies, out int maxCopyNumber, out string errmsg)
        {
            maxCopyNumber = 1;
            errmsg = "";

            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spFlatPageList", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable("PageTable");


            DataColumn newColumn;

            newColumn = tblMain.Columns.Add("Production", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Publication", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PubDate", Type.GetType("System.DateTime"));
            newColumn = tblMain.Columns.Add("Issue", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Edition", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Page", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Color", Type.GetType("System.String"));

            newColumn = tblMain.Columns.Add("Status", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ExternalStatus", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Version", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Approval", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("CopyNumber", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Pagination", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("LastError", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Comment", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ProofStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Location", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PageType", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("MasterCopySeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FlatSeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Active", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("UniquePage", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PagePositions", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PagesOnPlate", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PlateRotation", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PageRotations", Type.GetType("System.String"));

            newColumn = tblMain.Columns.Add("SheetNumber", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("SheetSide", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("IncomingPageRotationEven", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("IncomingPageRotationOdd", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("PagesAcross", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PagesDown", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("Hold", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("StatusC", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("StatusM", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("StatusY", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("StatusK", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("StatusSpot", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("StatusPDF", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("CopyFlatSeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("PressRunID", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FlatProofStatus", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("FlatRotation", Type.GetType("System.Int32"));
            //newColumn = tblMain.Columns.Add("FlatVersion",Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("UniquePlate", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("PlateName", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("ExtStatus", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("ForcedPlate", Type.GetType("System.Int32"));

            newColumn = tblMain.Columns.Add("PageVersion", Type.GetType("System.Int32"));


            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? tDefaultPubDate : (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@LocationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                if ((bool)HttpContext.Current.Application["LocationIsPress"])
                    param.Value = -1;
                else
                    param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("LocationNameCache", (string)HttpContext.Current.Session["SelectedLocation"]);

                //param.Value =  bUnfiltered ? -1 : Globals.GetIDFromName("LocationNameCache",(string)HttpContext.Current.Session["SelectedLocation"]);


                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                    param.Value = 0;

                param = command.Parameters.Add("@Approved", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideApproved ? 1 : -1);

                param = command.Parameters.Add("@Unique", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : (hideCommon ? 1 : -1);

                param = command.Parameters.Add("@CopyNumber", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bAllCopies ? 0 : 1;

                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                {
                    param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                }


                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                string thisColor = "";
                int thisFlatSeparationSet = 0;
                int prevFlatSeparationSet = -1;
                int thisSeparationSet = 0;
                int prevSeparationSet = -1;

                int nColors = 0;
                int nMinStatus = 100;
                int nMinExtStatus = 100;
                int nMinLogStatus = 0;
                bool bHasError = false;
                bool bIsApproved = true;
                bool bIsAutoApproved = true;
                bool bIsDisapproved = false;
                int nMaxVersion = 0;
                int nMaxVersionFlat = 0;
                int nMaxVersionPage = 0;

                bool bIsFirst = true;
                bool bIsProofed = true;
                int thisActive = 0;
                int thisPageType = 0;
                bool bIsActive = false;
                bool bIsUnique = false;
                bool bIsPlateUnique = false;
                bool bIsPlateForced = false;
                string thisPublication = "";

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                int nSheetNumber = 0;
                int nSheetSide = 1;
                int nFlatRotation = 0;
                while (reader.Read())
                {
                    int idx = 0;

                    thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(idx++));

                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    thisFlatSeparationSet = reader.GetInt32(idx++);	// FlatSeparationSet
                    thisSeparationSet = reader.GetInt32(idx++);		// CopyColorSet
                    if (thisSeparationSet != prevSeparationSet)
                    {
                        // Store previous page
                        if (bIsFirst == false)
                            tblMain.Rows.Add(newRow);

                        bIsFirst = false;

                        // New page begins - reset color string
                        nColors = 0;
                        nMinStatus = 100;
                        bHasError = false;
                        bIsApproved = true;
                        bIsAutoApproved = true;
                        bIsDisapproved = false;
                        bIsProofed = true;
                        nMaxVersionPage = 0;
                        //nMaxVersion = 0;
                        bIsActive = false;
                        bIsUnique = false;
                        newRow = tblMain.NewRow();
                        newRow["Color"] = "";
                        newRow["StatusC"] = 100;
                        newRow["StatusM"] = 100;
                        newRow["StatusY"] = 100;
                        newRow["StatusK"] = 100;
                        newRow["StatusSpot"] = 100;
                        newRow["StatusPDF"] = 100;

                    }

                    // Trace sheet number and side
                    if (thisFlatSeparationSet != prevFlatSeparationSet)
                    {
                        nSheetSide = 1 - nSheetSide;

                        if (nSheetSide == 0)
                            nSheetNumber++;

                        nMaxVersion = 0;
                        bIsPlateUnique = false;
                        bIsPlateForced = false;
                        prevFlatSeparationSet = thisFlatSeparationSet;
                    }

                    newRow["FlatSeparationSet"] = thisFlatSeparationSet;
                    newRow["CopyFlatSeparationSet"] = thisFlatSeparationSet / 100;

                    thisColor = Globals.GetNameFromID("ColorNameCache", reader.GetInt32(idx++));

                    newRow["Pagination"] = reader.GetInt32(idx++);

                    thisActive = reader.GetInt32(idx++);
                    bIsActive = thisActive == 1 ? true : false;
                    newRow["Active"] = bIsActive ? 1 : 0;	// int !

                    newRow["Publication"] = thisPublication;

                    DateTime tm = reader.GetDateTime(idx++);
                    newRow["PubDate"] = tm;

                    newRow["Production"] = thisPublication + " " + tm.ToShortDateString();

                    newRow["Issue"] = Globals.GetNameFromID("IssueNameCache", reader.GetInt32(idx++));
                    newRow["Edition"] = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(idx++));
                    newRow["Section"] = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(idx++));
                    newRow["Page"] = reader.GetString(idx++);

                    int nStatus = reader.GetInt32(idx++);

                    int nExtStatus = reader.GetInt32(idx++);


                    if (bIsActive)
                    {
                        switch (thisColor)
                        {
                            case "C":
                                if ((int)newRow["StatusC"] > nStatus)
                                    newRow["StatusC"] = nStatus;
                                break;
                            case "M":
                                if ((int)newRow["StatusM"] > nStatus)
                                    newRow["StatusM"] = nStatus;
                                break;
                            case "Y":
                                if ((int)newRow["StatusY"] > nStatus)
                                    newRow["StatusY"] = nStatus;
                                break;
                            case "K":
                                if ((int)newRow["StatusK"] > nStatus)
                                    newRow["StatusK"] = nStatus;
                                break;

                            case "PDF":
                            case "PDFmono":
                                if ((int)newRow["StatusPDF"] > nStatus)
                                    newRow["StatusPDF"] = nStatus;
                                break;
                            default:
                                if ((int)newRow["StatusSpot"] > nStatus)
                                    newRow["StatusSpot"] = nStatus;
                                break;
                        }
                    }

                    if (bIsActive)
                    {
                        string sc = (string)newRow["Color"];

                        if (sc.IndexOf(thisColor) == -1)
                        {
                            if (nColors != 0)
                            {
                                newRow["Color"] += ";";
                            }
                            newRow["Color"] += thisColor;
                            nColors++;
                        }

                    }

                    int nVersion = reader.GetInt32(idx++);
                    if (nVersion > nMaxVersion && bIsActive)
                        nMaxVersion = nVersion;
                    if (nVersion > nMaxVersionPage && bIsActive)
                        nMaxVersionPage = nVersion;
                    newRow["Version"] = nMaxVersion;				// Flat version !
                    newRow["PageVersion"] = nMaxVersionPage;            // Page version !


                    int nApproval = reader.GetInt32(idx++);

                    if ((nApproval == 0 || nApproval == 2) && bIsActive)
                        bIsApproved = false;
                    if (nApproval != -1 && bIsActive)
                        bIsAutoApproved = false;
                    if (nApproval == 2 && bIsActive)
                        bIsDisapproved = true;


                    newRow["Approval"] = bIsDisapproved ? 2 : (bIsAutoApproved ? -1 : (bIsApproved ? 1 : 0));

                    int nCopyNumber = reader.GetInt32(idx++);
                    newRow["CopyNumber"] = nCopyNumber;

                    if (maxCopyNumber < nCopyNumber)
                        maxCopyNumber = nCopyNumber;


                    newRow["LastError"] = reader.GetString(idx++);

                    string sComment = reader.GetString(idx++);
                    newRow["Comment"] += sComment + " ";

                    int prf = reader.GetInt32(idx++);
                    if (prf < 1 && bIsActive)
                        bIsProofed = false;
                    newRow["ProofStatus"] = bIsProofed ? 10 : 0;
                    newRow["Location"] = Globals.GetNameFromID("LocationNameCache", reader.GetInt32(idx++));
                    newRow["PageType"] = reader.GetInt32(idx++);

                    int nRest = nStatus - (nStatus / 10) * 10;
                    if (nRest == 6 && bIsActive)
                        bHasError = true;

                    if (nStatus < nMinStatus && bIsActive && (int)newRow["PageType"] <= 1)
                        nMinStatus = nStatus;

                    String st = Globals.GetStatusName(bHasError ? nStatus : nMinStatus, 0);
                    newRow["Status"] = st == "" ? "N/A" : st;

                    if (nExtStatus < nMinExtStatus && bIsActive && (int)newRow["PageType"] <= 1)
                        nMinExtStatus = nStatus;

                    nRest = nExtStatus - (nExtStatus / 10) * 10;
                    if (nRest == 6 && bIsActive)
                        nMinExtStatus = nStatus;

                    st = Globals.GetStatusName(nMinExtStatus, 1);
                    newRow["ExternalStatus"] = st == "" ? "N/A" : st;
                    newRow["ExtStatus"] = nMinExtStatus;


                    string sPagePositions = reader.GetString(idx++);
                    newRow["PagePositions"] = sPagePositions;
                    string sPageRotationList = reader.GetString(idx++);
                    string sPageRotationBackList = reader.GetString(idx++);

                    newRow["PlateRotation"] = 0;
                    newRow["SheetNumber"] = reader.GetInt32(idx++);
                    newRow["SheetSide"] = reader.GetInt32(idx++);
                    newRow["IncomingPageRotationEven"] = reader.GetInt32(idx++);
                    newRow["IncomingPageRotationOdd"] = reader.GetInt32(idx++);

                    newRow["PageRotations"] = "";
                    string[] sPagePositionArr = sPagePositions.Split(',');

                    if ((int)newRow["SheetSide"] == 0) // Front
                    {
                        string[] sRotArr = sPageRotationList.Split(',');
                        for (int w = 0; w < sPagePositionArr.Length; w++)
                        {
                            int thisPos = Convert.ToInt32(sPagePositionArr[w]);
                            if (sRotArr.Length >= thisPos)
                            {
                                if ((string)newRow["PageRotations"] != "")
                                    newRow["PageRotations"] += ",";
                                newRow["PageRotations"] += sRotArr[thisPos - 1];
                            }
                            else
                                newRow["PageRotations"] = "0";
                        }
                    }
                    else
                    {
                        string[] sRotArr = sPageRotationBackList.Split(',');
                        for (int w = 0; w < sPagePositionArr.Length; w++)
                        {
                            int thisPos = Convert.ToInt32(sPagePositionArr[w]);
                            if (sRotArr.Length >= thisPos)
                            {
                                if ((string)newRow["PageRotations"] != "")
                                    newRow["PageRotations"] += ",";
                                newRow["PageRotations"] += sRotArr[thisPos - 1];
                            }
                            else
                                newRow["PageRotations"] = "0";
                        }
                    }

                    newRow["PagesAcross"] = reader.GetInt32(idx++);
                    newRow["PagesDown"] = reader.GetInt32(idx++);
                    newRow["PagesOnPlate"] = (int)newRow["PagesAcross"] * ((int)newRow["PagesDown"]);

                    newRow["Hold"] = reader.GetInt32(idx++);

                    int bUnique = reader.GetInt32(idx++);

                    if (bUnique == 1)
                    {
                        bIsUnique = true;
                        bIsPlateUnique = true;
                    }
                    if (bUnique == 2)
                        bIsPlateForced = true;


                    newRow["UniquePage"] = bIsUnique;
                    newRow["UniquePlate"] = bIsPlateUnique;
                    newRow["ForcedPlate"] = bIsPlateForced;
                    newRow["MasterCopySeparationSet"] = reader.GetInt32(idx++);
                    newRow["PressRunID"] = reader.GetInt32(idx++);

                    if (reader.FieldCount > idx)
                        newRow["FlatProofStatus"] = reader.GetInt32(idx++);
                    else
                        newRow["FlatProofStatus"] = 10;

                    if (reader.FieldCount > idx)
                        newRow["FlatRotation"] = reader.GetInt32(idx++);
                    else
                        newRow["FlatRotation"] = 0;

                    // Dummy read PageIndex
                    if (reader.FieldCount > idx)
                        reader.GetInt32(idx++);

                    newRow["PlateName"] = "";
                    // Dummy read PageIndex
                    if (reader.FieldCount > idx)
                        newRow["PlateName"] = reader.GetString(idx++);


                    prevFlatSeparationSet = thisFlatSeparationSet;
                    prevSeparationSet = thisSeparationSet;

                }
                if (bIsFirst == false)
                    tblMain.Rows.Add(newRow);

            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblMain;
        }

        public DataTable GetTablePageStatusCollection(bool hideDone, out string errmsg)
        {
            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);
            SqlCommand command = new SqlCommand("spPageStatusList", connection);
            command.CommandType = CommandType.StoredProcedure;

            DataTable tblMain = new DataTable("StatusTable");
            DataColumn newColumn;
            DataRow newRow = null;
            SqlDataReader reader = null;
            string s = "";

            newColumn = tblMain.Columns.Add("Ed", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Sec", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Page", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("FTP", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PRE", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("INK", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("RIP", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Rdy", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Appr", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("CTP", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Bend", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Preset", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("XXX", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Ed2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Sec2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Page2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("FTP2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("PRE2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("INK2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("RIP2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Rdy2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Appr2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("CTP2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Bend2", Type.GetType("System.String"));
            newColumn = tblMain.Columns.Add("Preset2", Type.GetType("System.String"));



            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                    param.Value = 0;

                param = command.Parameters.Add("@HideDone", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = hideDone;

                param = command.Parameters.Add("@EmulateBender", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = 0;

                param = command.Parameters.Add("@EmulatePreset", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = 0;

                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                {
                    param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                }

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;

                    newRow = tblMain.NewRow();
                    newRow["Ed"] = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(idx++));
                    newRow["Sec"] = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(idx++));

                    s = reader.GetString(idx++);
                    if (s.Length == 1)
                        s = "  " + s;
                    else if (s.Length == 2)
                        s = " " + s;
                    newRow["Page"] = s;

                    int FTP = reader.GetInt32(idx++);
                    int PRE = reader.GetInt32(idx++);
                    int INK = reader.GetInt32(idx++);
                    int RIP = reader.GetInt32(idx++);
                    int Rdy = reader.GetInt32(idx++);
                    int Appr = reader.GetInt32(idx++);
                    int CTP = reader.GetInt32(idx++);
                    int Bend = reader.GetInt32(idx++);
                    int Preset = reader.GetInt32(idx++);

                    FTP += (reader.GetInt32(idx++)) * 256;
                    PRE += (reader.GetInt32(idx++)) * 256;
                    INK += (reader.GetInt32(idx++)) * 256;
                    RIP += (reader.GetInt32(idx++)) * 256;
                    Rdy += (reader.GetInt32(idx++)) * 256;
                    Appr += (reader.GetInt32(idx++)) * 256;
                    CTP += (reader.GetInt32(idx++)) * 256;
                    Bend += (reader.GetInt32(idx++)) * 256;
                    Preset += (reader.GetInt32(idx++)) * 256;

                    newRow["FTP"] = FTP.ToString() + ";" + reader.GetString(idx++);
                    newRow["PRE"] = PRE.ToString() + ";" + reader.GetString(idx++);
                    newRow["INK"] = INK.ToString() + ";" + reader.GetString(idx++);
                    newRow["RIP"] = RIP.ToString() + ";" + reader.GetString(idx++);
                    newRow["Rdy"] = Rdy.ToString() + ";";
                    newRow["Appr"] = Appr.ToString() + ";";
                    newRow["CTP"] = CTP.ToString() + ";" + reader.GetString(idx++);
                    newRow["Bend"] = Bend.ToString() + ";";
                    newRow["Preset"] = Preset.ToString() + ";";

                    DateTime ftpTime = reader.GetDateTime(idx++);
                    DateTime preTime = reader.GetDateTime(idx++);
                    DateTime inkTime = reader.GetDateTime(idx++);
                    DateTime ripTime = reader.GetDateTime(idx++);
                    DateTime rdyTime = reader.GetDateTime(idx++);
                    DateTime apprTime = reader.GetDateTime(idx++);
                    DateTime ctpTime = reader.GetDateTime(idx++);
                    DateTime bendTime = reader.GetDateTime(idx++);
                    DateTime presetTime = reader.GetDateTime(idx++);

                    newRow["FTP"] = (string)newRow["FTP"] + ";" + Globals.DateTime2StringShort(ftpTime);
                    newRow["PRE"] = (string)newRow["PRE"] + ";" + Globals.DateTime2StringShort(preTime);
                    newRow["INK"] = (string)newRow["INK"] + ";" + Globals.DateTime2StringShort(inkTime);
                    newRow["RIP"] = (string)newRow["RIP"] + ";" + Globals.DateTime2StringShort(ripTime);
                    newRow["Rdy"] = (string)newRow["Rdy"] + ";" + Globals.DateTime2StringShort(rdyTime);
                    newRow["Appr"] = (string)newRow["Appr"] + ";" + Globals.DateTime2StringShort(apprTime);
                    newRow["CTP"] = (string)newRow["CTP"] + ";" + Globals.DateTime2StringShort(ctpTime);
                    newRow["Bend"] = (string)newRow["Bend"] + ";" + Globals.DateTime2StringShort(bendTime);
                    newRow["Preset"] = (string)newRow["Preset"] + ";" + Globals.DateTime2StringShort(presetTime);

                    newRow["XXX"] = "";

                    tblMain.Rows.Add(newRow);
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblMain;

        }

        public DataTable GetTablePageCollection(bool bUnfiltered, bool bAllCopies, out string errmsg)
        {
            errmsg = "";
            bool hideApproved = false;
            bool hideCommon = false;

            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spTablePageList", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblMain = new DataTable("PageTable");

            DataColumn newColumn;

            DateTime tDummyDate = new DateTime(1975, 1, 1, 0, 0, 0);

            string[] colNames = {   "Edition",  "Section",  "Page",         "Color",        "Status",       "Version",
                                    "Approval",     "Hold",     "Priority", "Template", "Device",   "ExternalStatus","CopyNumber", "Pagination",    "Press",    "LastError",    "Comment",
                                    "DeadLine",     "ProofStatus", "Location", "SheetNumber", "SheetSide", "PagePositions", "PageType", "PagesPerPlate",
                                    "InputTime", "ApproveTime", "OutputTime", "VerifyTime", "Active", "Unique", "MasterCopySeparationSet", "SeparationSet",
                                    "FlatSeparationSet", "FlatSeparation"};
            string[] colTypes = {       "String",   "String",   "String",       "String",       "String",       "Int32",
                                    "Int32",        "String",   "Int32",    "String",   "String",   "String",       "Int32",        "Int32",        "String",   "String",       "String",
                                    "String",       "Int32",    "String",   "Int32",    "String",   "String",       "Int32",        "Int32",
                                    "String",       "String",   "String",   "String",   "Boolean",  "Boolean",      "Int32",        "Int32",
                                    "Int32",        "Int64"};

            /*			bool loadDefafaults= false;
                        if (HttpContext.Current.Session["ColumnOrder"] == null)
                            loadDefafaults = true;
                        if (loadDefafaults == false && (string)HttpContext.Current.Session["ColumnOrder"] == "")
                            loadDefafaults = true;
                        if (loadDefafaults)
                        {
                            string s = "";
                            foreach (string s1 in colNames)
                            {
                                if (s != "")
                                    s += ",";
                                s += s1;
                            }
                            HttpContext.Current.Session["ColumnOrder"] = s;
                        }

             * */
            string colOrder = (string)HttpContext.Current.Application["ColumnOrder"];
            string[] cols = colOrder.Split(',');

            foreach (string thiscol in cols)
            {
                if (thiscol == "Separation")    // Only for single sep view..
                    continue;
                //                if (thiscol == "Color")	// Only for single sep view..
                //                   continue;

                for (int i = 0; i < colNames.Length; i++)
                {
                    if (colNames[i].ToUpper() == thiscol.ToUpper())
                    {
                        newColumn = tblMain.Columns.Add(colNames[i], Type.GetType("System." + colTypes[i]));
                        break;
                    }
                }
            }

            DataRow newRow = null;
            //			DataRow newRowSeps = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = -1;

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                if ((string)HttpContext.Current.Session["SelectedEdition"] != "" && (string)HttpContext.Current.Session["SelectedEdition"] != "*")
                    param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);
                else
                    param.Value = -1;

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                if ((string)HttpContext.Current.Session["SelectedSection"] != "" && (string)HttpContext.Current.Session["SelectedSection"] != "*")
                    param.Value = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);
                else
                    param.Value = -1;

                param = command.Parameters.Add("@LocationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("LocationNameCache", (string)HttpContext.Current.Session["SelectedLocation"]);

                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                    param.Value = 0;

                param = command.Parameters.Add("@Approved", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = (bool)HttpContext.Current.Session["HideApproved"] ? 1 : -1;


                param = command.Parameters.Add("@Unique", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = (bool)HttpContext.Current.Session["HideCommon"] ? 1 : -1;

                param = command.Parameters.Add("@ActiveOnly", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = (bool)HttpContext.Current.Session["HideFinished"] ? 1 : -1;

                param = command.Parameters.Add("@CopyNumber", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bAllCopies ? 0 : 1;

                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                {
                    param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                }

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                int thisMasterCopySeparationSet = 0;
                int thisCopySeparationSet = 0;
                int thisSeparationSet = 0;

                int prevSeparationSet = -1;
                int nColors = 0;
                int n;
                int nMinStatus = 100;
                int nMinExtStatus = 100;
                int nMaxPriority = 0;
                bool bHasError = false;
                bool bIsHold = false;
                bool bIsApproved = true;
                int nMaxVersion = 0;
                bool bIsFirst = true;
                bool bIsProofed = true;
                int thisActive = 0;

                bool bIsActive = false;
                string thisPublication = "";
                string s;

                string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
                string[] publist = pubsallowed.Split(',');

                while (reader.Read())
                {
                    int idx = 0;
                    thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(idx++));

                    thisMasterCopySeparationSet = reader.GetInt32(idx++);   // CopyColorSet
                    thisSeparationSet = reader.GetInt32(idx++);             // ColorSet

                    if (thisSeparationSet != prevSeparationSet)
                    {
                        // Store previous page
                        if (bIsFirst == false)
                            tblMain.Rows.Add(newRow);

                        bIsFirst = false;

                        // New page begins - reset color string
                        nColors = 0;
                        nMinStatus = 100;
                        nMinExtStatus = 100;
                        nMaxPriority = 0;
                        bHasError = false;
                        bIsHold = false;
                        bIsApproved = true;
                        bIsProofed = true;
                        nMaxVersion = 0;

                        bIsActive = false;

                        newRow = tblMain.NewRow();
                        newRow["Color"] = "";
                    }

                    //					newRow["Publication"] = thisPublication;
                    newRow["MasterCopySeparationSet"] = thisMasterCopySeparationSet;
                    newRow["SeparationSet"] = thisSeparationSet;

                    reader.GetInt64(idx++); // Dummy read of Separation

                    newRow["FlatSeparationSet"] = reader.GetInt32(idx++);
                    newRow["FlatSeparation"] = reader.GetInt64(idx++);

                    thisActive = reader.GetInt32(idx++);
                    bIsActive = thisActive == 1 ? true : false;

                    newRow["Active"] = bIsActive ? 1 : 0;   // int !

                    DateTime tm = reader.GetDateTime(idx++);
                    //				newRow["PubDate"] = tm.ToShortDateString();;

                    reader.GetInt32(idx++); // Dummy read issue
                    newRow["Edition"] = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(idx++));
                    newRow["Section"] = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(idx++));

                    newRow["Page"] = reader.GetString(idx++);

                    string thisColor = Globals.GetNameFromID("ColorNameCache", reader.GetInt32(idx++));
                    if (bIsActive)
                    {
                        if (nColors != 0)
                            newRow["Color"] += ";";
                        newRow["Color"] += thisColor;
                        nColors++;
                    }

                    int nStatus = reader.GetInt32(idx++);               // OK					
                    if (nStatus < nMinStatus && bIsActive)
                        nMinStatus = nStatus;

                    int nRest = nStatus - (nStatus / 10) * 10;
                    if (nRest == 6 && bIsActive)
                        bHasError = true;

                    String st = Globals.GetStatusName(bHasError ? nStatus : nMinStatus, 0);
                    newRow["Status"] = st == "" ? "N/A" : st;

                    nStatus = reader.GetInt32(idx++);
                    if (nStatus < nMinExtStatus && bIsActive)
                        nMinExtStatus = nStatus;
                    nRest = nStatus - (nStatus / 10) * 10;
                    if (nRest == 6 && bIsActive)
                        nMinExtStatus = nStatus;
                    st = Globals.GetStatusName(nMinExtStatus, 1);
                    newRow["ExternalStatus"] = st == "" ? "N/A" : st;

                    n = reader.GetInt32(idx++);
                    if (n > nMaxVersion && bIsActive)
                        nMaxVersion = n;
                    newRow["Version"] = nMaxVersion;

                    int nApproval = reader.GetInt32(idx++);
                    if ((nApproval == 0 || nApproval == 2) && bIsActive)
                        bIsApproved = false;
                    newRow["Approval"] = nApproval;

                    n = reader.GetInt32(idx++);
                    if (n == 1 && bIsActive)
                        bIsHold = true;
                    newRow["Hold"] = bIsHold ? "On hold" : "Released";

                    n = reader.GetInt32(idx++);
                    if (n > nMaxPriority && bIsActive)
                        nMaxPriority = n;
                    newRow["Priority"] = nMaxPriority;

                    newRow["Template"] = reader.GetString(idx++);
                    newRow["Device"] = Globals.GetNameFromID("DeviceNameCache", reader.GetInt32(idx++));

                    newRow["CopyNumber"] = reader.GetInt32(idx++);
                    newRow["Pagination"] = reader.GetInt32(idx++);
                    newRow["Press"] = Globals.GetNameFromID("PressNameCache", reader.GetInt32(idx++));

                    string sTime = "";
                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }

                    newRow["InputTime"] = sTime;

                    sTime = "";
                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }
                    newRow["ApproveTime"] = sTime;

                    sTime = "";
                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }
                    newRow["OutputTime"] = sTime;

                    sTime = "";
                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }
                    newRow["VerifyTime"] = sTime;

                    newRow["LastError"] = reader.GetString(idx++);

                    s = reader.GetString(idx++);
                    newRow["Comment"] += s + " ";

                    sTime = "";
                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }
                    newRow["DeadLine"] = sTime;

                    n = reader.GetInt32(idx++);
                    if (n < 1 && bIsActive)
                        bIsProofed = false;
                    newRow["ProofStatus"] = bIsProofed ? 10 : 0;

                    newRow["Location"] = Globals.GetNameFromID("LocationNameCache", reader.GetInt32(idx++));
                    newRow["SheetNumber"] = reader.GetInt32(idx++);

                    n = reader.GetInt32(idx++);
                    newRow["SheetSide"] = n == 0 ? "Front" : "Back";
                    newRow["PageType"] = reader.GetInt32(idx++);
                    newRow["Unique"] = reader.GetInt32(idx++);
                    newRow["PagePositions"] = reader.GetString(idx++);
                    newRow["PagesPerPlate"] = reader.GetInt32(idx++);

                    prevSeparationSet = thisSeparationSet;
                }
                if (bIsFirst == false)
                    tblMain.Rows.Add(newRow);

            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblMain;
        }

        public DataTable GetTablePageSeparationCollection(bool bUnfiltered, bool bAllCopies, out string errmsg)
        {
            errmsg = "";
            //bool hideCommon = false;
            //bool hideApproved = false;

            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            //	connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand command = new SqlCommand("spTablePageList", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;

            DataTable tblSeps = new DataTable("PageTable");
            DataColumn newColumn;

            DateTime tDummyDate = new DateTime(1975, 1, 1, 0, 0, 0);


            string[] colNames = {       "Edition",  "Section",  "Page",         "Color",        "Status",       "Version",
                                    "Approval",     "Hold",     "Priority", "Template", "Device",   "ExternalStatus","CopyNumber", "Pagination",    "Press",    "LastError",    "Comment",
                                    "DeadLine",     "ProofStatus", "Location", "SheetNumber", "SheetSide", "PagePositions", "PageType", "PagesPerPlate",
                                    "InputTime", "ApproveTime", "OutputTime", "VerifyTime", "Active", "Unique", "MasterCopySeparationSet", "SeparationSet",
                                    "FlatSeparationSet", "FlatSeparation", "Separation"};
            string[] colTypes = {   "String",   "String",   "String",       "String",       "String",       "Int32",
                                    "Int32",        "String",   "Int32",    "String",   "String",   "String",       "Int32",        "Int32",        "String",   "String",       "String",
                                    "String",       "Int32",    "String",   "Int32",    "String",   "String",       "Int32",        "Int32",
                                    "String",       "String",   "String",   "String",   "Boolean",  "Boolean",      "Int32",        "Int32",
                                    "Int32",        "Int64",    "Int64"};


            /*	bool loadDefafaults= false;
                if (HttpContext.Current.Application["ColumnOrder"] == null)
                    loadDefafaults = true;
                if (loadDefafaults == false && (string)HttpContext.Current.Application["ColumnOrder"] == "")
                    loadDefafaults = true;
                if (loadDefafaults)
                {
                    string s = "";
                    foreach (string s1 in colNames)
                    {
                        if (s != "")
                            s += ",";
                        s += s1;
                    }
                    HttpContext.Current.Application["ColumnOrder"] = s;
                }
             */

            string colOrder = (string)HttpContext.Current.Application["ColumnOrder"];
            string[] cols = colOrder.Split(',');



            bool hasSeparation = false;
            foreach (string thiscol in cols)
            {
                for (int i = 0; i < colNames.Length; i++)
                {
                    if (thiscol == "Separation")
                        hasSeparation = true;
                    if (colNames[i].ToUpper() == thiscol.ToUpper())
                    {
                        newColumn = tblSeps.Columns.Add(colNames[i], Type.GetType("System." + colTypes[i]));
                        break;
                    }
                }
            }

            if (hasSeparation == false)
                newColumn = tblSeps.Columns.Add("Separation", Type.GetType("System.Int64"));


            DataRow newRowSeps = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;


                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];

                param = command.Parameters.Add("@IssueID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = -1; //

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                if ((string)HttpContext.Current.Session["SelectedEdition"] != "" && (string)HttpContext.Current.Session["SelectedEdition"] != "*")
                    param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);
                else
                    param.Value = -1;

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                if ((string)HttpContext.Current.Session["SelectedSection"] != "" && (string)HttpContext.Current.Session["SelectedSection"] != "*")
                    param.Value = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);
                else
                    param.Value = -1;

                param = command.Parameters.Add("@LocationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("LocationNameCache", (string)HttpContext.Current.Session["SelectedLocation"]);

                param = command.Parameters.Add("@PressID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                    param.Value = 0;

                param = command.Parameters.Add("@Approved", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = (bool)HttpContext.Current.Session["HideApproved"] ? 1 : -1;

                param = command.Parameters.Add("@Unique", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = (bool)HttpContext.Current.Session["HideCommon"] ? 1 : -1;

                param = command.Parameters.Add("@ActiveOnly", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = (bool)HttpContext.Current.Session["HideFinished"] ? 1 : -1;

                param = command.Parameters.Add("@CopyNumber", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = bAllCopies ? 0 : 1;

                if ((bool)HttpContext.Current.Application["UsePressGroups"])
                {
                    param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Input;
                    param.Value = bUnfiltered ? -1 : Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
                }

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                int n;
                int thisActive = 0;
                bool bIsActive = false;
                string thisPublication = "";

                while (reader.Read())
                {
                    int idx = 0;
                    thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(idx++));
                    newRowSeps = tblSeps.NewRow();

                    newRowSeps["MasterCopySeparationSet"] = reader.GetInt32(idx++); // CopyColorSet
                    newRowSeps["SeparationSet"] = reader.GetInt32(idx++);
                    newRowSeps["Separation"] = reader.GetInt64(idx++);
                    newRowSeps["FlatSeparationSet"] = reader.GetInt32(idx++);
                    newRowSeps["FlatSeparation"] = reader.GetInt64(idx++);

                    thisActive = reader.GetInt32(idx++);
                    bIsActive = thisActive == 1 ? true : false;
                    newRowSeps["Active"] = bIsActive;       // boolean					// HideColor=0 AND IgnoreRec=0

                    DateTime tm = reader.GetDateTime(idx++);
                    //					newRowSeps["PubDate"] = tm.ToShortDateString();;

                    // dummy read Issue
                    reader.GetInt32(idx++);
                    //					newRowSeps["Issue"] =  Globals.GetNameFromID("IssueNameCache",reader.GetInt32(idx++));
                    newRowSeps["Edition"] = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(idx++));
                    newRowSeps["Section"] = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(idx++));

                    newRowSeps["Page"] = reader.GetString(idx++);
                    newRowSeps["Color"] = Globals.GetNameFromID("ColorNameCache", reader.GetInt32(idx++));

                    int nStatus = reader.GetInt32(idx++);               // OK
                    newRowSeps["Status"] = Globals.GetStatusName(nStatus, 0);

                    nStatus = reader.GetInt32(idx++);
                    newRowSeps["ExternalStatus"] = Globals.GetStatusName(nStatus, 1);
                    newRowSeps["Version"] = reader.GetInt32(idx++);
                    newRowSeps["Approval"] = reader.GetInt32(idx++); ;

                    n = reader.GetInt32(idx++);
                    newRowSeps["Hold"] = n == 1 ? "On hold" : "Released";

                    newRowSeps["Priority"] = reader.GetInt32(idx++);
                    newRowSeps["Template"] = reader.GetString(idx++);
                    newRowSeps["Device"] = Globals.GetNameFromID("DeviceNameCache", reader.GetInt32(idx++));
                    newRowSeps["CopyNumber"] = reader.GetInt32(idx++);
                    newRowSeps["Pagination"] = reader.GetInt32(idx++);
                    newRowSeps["Press"] = Globals.GetNameFromID("PressNameCache", reader.GetInt32(idx++));

                    string sTime = "";

                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }

                    newRowSeps["InputTime"] = sTime;

                    sTime = "";

                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }
                    newRowSeps["ApproveTime"] = sTime;

                    sTime = "";
                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }
                    newRowSeps["OutputTime"] = sTime;
                    sTime = "";
                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }
                    newRowSeps["VerifyTime"] = sTime;

                    newRowSeps["LastError"] = reader.GetString(idx++);
                    newRowSeps["Comment"] = reader.GetString(idx++);

                    sTime = "";
                    try
                    {
                        DateTime dtm = reader.GetDateTime(idx++);
                        sTime = dtm.Year >= 2000 ? dtm.ToShortTimeString() : "";
                    }
                    catch
                    {
                    }

                    newRowSeps["DeadLine"] = sTime;
                    newRowSeps["ProofStatus"] = reader.GetInt32(idx++);
                    newRowSeps["Location"] = Globals.GetNameFromID("LocationNameCache", reader.GetInt32(idx++));
                    newRowSeps["SheetNumber"] = reader.GetInt32(idx++);

                    n = reader.GetInt32(idx++);
                    newRowSeps["SheetSide"] = n == 0 ? "Front" : "Back";
                    newRowSeps["PageType"] = reader.GetInt32(idx++);
                    newRowSeps["Unique"] = reader.GetInt32(idx++);
                    newRowSeps["PagePositions"] = reader.GetString(idx++);
                    newRowSeps["PagesPerPlate"] = reader.GetInt32(idx++);

                    tblSeps.Rows.Add(newRowSeps);
                }

            }
            catch (Exception ex)
            {
                errmsg = "!!  " + ex.Message;
                tblSeps = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }


            return tblSeps;
        }

        public DataTable GetMessages(string userName, out string errmsg)
        {
            return GetMessages(userName, 0, DateTime.MinValue, false, out errmsg);
        }

        public DataTable GetMessages(string userName, int publicationID, DateTime pubDate, bool unreadOnly, out string errmsg)
        {
            errmsg = "";
            DataTable dt = new DataTable();

            SqlCommand command = new SqlCommand("spGetMessages", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter(command);

            SqlParameter param = command.Parameters.Add("@UserName", SqlDbType.VarChar, 50);
            param.Value = userName;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;

            param = command.Parameters.Add("@UnreadOnly", SqlDbType.Int);
            param.Value = unreadOnly ? 1 : 0;

            try
            {
                sqlDataAdaptor.Fill(dt);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dt;
        }

        public string GetPrePollMessage(int masterCopySeparationSet, int eventCode, out string errmsg)
        {
            errmsg = "";
            string message = "";

            string sql = string.Format("SELECT TOP 1 [Message] FROM PrePollPageTable WITH (NOLOCK)  WHERE [Event]={0} AND MasterCopySeparationSet={1}", eventCode, masterCopySeparationSet);

            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    message = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return "";
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return message;
        }


        public bool GetMessageDetails(int messageID, out int severity, out int isRead, out string sender, out string receiver, out string message, out DateTime eventTime, out DateTime pubDate, out string title, out int publicationID, out string errmsg)
        {
            errmsg = "";
            severity = 0;
            isRead = 0;
            sender = "";
            receiver = "";
            message = "";
            eventTime = DateTime.MinValue;
            pubDate = DateTime.MinValue;
            publicationID = 0;
            title = "";

            string sql = string.Format("SELECT TOP 1 Severity, IsRead, Sender,Receiver,Message, EventTime,Title,PubDate,PublicationID FROM Messages WHERE MessageID={0}", messageID);

            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    severity = reader.GetInt32(0);
                    isRead = reader.GetInt32(1);
                    sender = reader.GetString(2);
                    receiver = reader.GetString(3);
                    message = reader.GetString(4);
                    eventTime = reader.GetDateTime(5);
                    title = reader.GetString(6);
                    pubDate = reader.GetDateTime(7);
                    publicationID = reader.GetInt32(8);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool GetPageStat(string location, string press, DateTime tPubDate, string publication, string issue, string edition, string section,
            out int nPagesArrived, out int nPagesApproved, out int nPagesWithError,
            out int FTPcount, out int PREcount, out int INKcount, out int RIPcount, out int nPageCountTotal,
            out string errmsg)
        {
            nPagesArrived = 0;
            nPagesApproved = 0;
            nPagesWithError = 0;
            nPageCountTotal = 0;

            FTPcount = 0;
            PREcount = 0;
            INKcount = 0;
            RIPcount = 0;

            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            //	connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand command = new SqlCommand("spPressRunStat", connection);

            command.Parameters.Clear();
            SqlParameter param;

            param = command.Parameters.Add("@LocationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("LocationNameCache", location);

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PressNameCache", press);

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Direction = ParameterDirection.Input;
            param.Value = tPubDate;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PublicationNameCache", publication);

            param = command.Parameters.Add("@IssueID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = 1;//Globals.GetIDFromName("IssueNameCache", issue);

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("EditionNameCache", edition);

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("SectionNameCache", section);

            param = command.Parameters.Add("@GetPrePollStat", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = (bool)HttpContext.Current.Application["PressRunPrePollStatus"] ? 1 : 0;

            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    nPagesArrived = reader.GetInt32(0);
                    nPagesApproved = reader.GetInt32(1);
                    nPagesWithError = reader.GetInt32(2);
                    nPageCountTotal = reader.GetInt32(3);
                    reader.GetInt32(4);
                    if (reader.FieldCount > 5)
                    {
                        FTPcount = reader.GetInt32(5);
                        PREcount = reader.GetInt32(6);
                        INKcount = reader.GetInt32(7);
                        RIPcount = reader.GetInt32(8);
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool GetReportStat(string location,
            out int nPages,
            out int nPagesArrived, out DateTime tFirstArrivedPage, out DateTime tLastArrivedPage,
            out int nPagesApproved, out DateTime tFirstApprovedPage, out DateTime tLastApprovedPage,
            out int nPagesOutput, out DateTime tFirstOutputPage, out DateTime tLastOutputPage,
            out int nTotalPlates, out int nTotalPlatesDone, out int nTotalPlatesUsed, out DateTime tDeadLine,
            out DateTime tDeadLine2,
             out int nTotalPlatesUpdated, out int nTotalPlatesDamaged,
            out string errmsg)
        {
            nPages = 0;
            nPagesArrived = 0;
            tFirstArrivedPage = DateTime.MinValue;
            tLastArrivedPage = DateTime.MinValue;

            nPagesApproved = 0;
            tFirstApprovedPage = DateTime.MinValue;
            tLastApprovedPage = DateTime.MinValue;

            nPagesOutput = 0;
            tFirstOutputPage = DateTime.MinValue;
            tLastOutputPage = DateTime.MinValue;

            nTotalPlates = 0;
            nTotalPlatesDone = 0;
            nTotalPlatesUsed = 0;
            nTotalPlatesUpdated = 0;
            nTotalPlatesDamaged = 0;
            tDeadLine = DateTime.MinValue;
            tDeadLine2 = DateTime.MinValue;


            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            //	connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand command = new SqlCommand("spPressRunStatEx", connection);

            command.Parameters.Clear();
            SqlParameter param;

            param = command.Parameters.Add("@LocationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("LocationNameCache", location);

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
            if ((bool)HttpContext.Current.Application["UsePressGroups"])
                param.Value = 0;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Direction = ParameterDirection.Input;
            param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

            param = command.Parameters.Add("@IssueID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);

            if ((bool)HttpContext.Current.Application["spParamExists_spPressRunStatEx_IgnoreDirty"])
            {
                param = command.Parameters.Add("@IgnoreDirty", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = false;
            }
            if ((bool)HttpContext.Current.Application["UsePressGroups"])
            {
                param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
            }

            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int i = 0;
                    nPages = reader.GetInt32(i++);
                    nPagesArrived = reader.GetInt32(i++);
                    tFirstArrivedPage = reader.GetDateTime(i++);
                    tLastArrivedPage = reader.GetDateTime(i++);
                    nPagesApproved = reader.GetInt32(i++);
                    tFirstApprovedPage = reader.GetDateTime(i++);
                    tLastApprovedPage = reader.GetDateTime(i++);
                    nPagesOutput = reader.GetInt32(i++);
                    tFirstOutputPage = reader.GetDateTime(i++);
                    tLastOutputPage = reader.GetDateTime(i++);
                    nTotalPlates = reader.GetInt32(i++);
                    nTotalPlatesDone = reader.GetInt32(i++);
                    nTotalPlatesUsed = reader.GetInt32(i++);
                    if (reader.FieldCount > i)
                        tDeadLine = reader.GetDateTime(i++);
                    if (reader.FieldCount > i)
                        tDeadLine2 = reader.GetDateTime(i++);
                    if (reader.FieldCount > i)
                        nTotalPlatesUpdated = reader.GetInt32(i++);
                    if (reader.FieldCount > i)
                        nTotalPlatesDamaged = reader.GetInt32(i++);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public DataTable GetReportStatVersions(out string errmsg)
        {
            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            //	connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand command = new SqlCommand("spProgressReportVersions", connection);

            command.Parameters.Clear();
            SqlParameter param;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Direction = ParameterDirection.Input;
            param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];

            param = command.Parameters.Add("@IssueID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = 1;// Globals.GetIDFromName("IssueNameCache", (string)HttpContext.Current.Session["SelectedIssue"]);

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            int nid = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);
            param.Value = nid; //Globals.GetIDFromName("SectionNameCache",(string)HttpContext.Current.Session["SelectedSection"]);

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PressNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
            if ((bool)HttpContext.Current.Application["UsePressGroups"])
                param.Value = 0;

            if ((bool)HttpContext.Current.Application["UsePressGroups"])
            {
                param = command.Parameters.Add("@PressGroupID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PressGroupNameCache", (string)HttpContext.Current.Session["SelectedPress"]);
            }


            command.CommandType = CommandType.StoredProcedure;

            DataTable dataTable = new DataTable();
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(command);

            try
            {
                sqlAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }

            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return dataTable;
        }

        public DataTable GetPageHistory(int masterCopySeparationSet, bool allColors, out string errmsg)
        {
            errmsg = "";
            SqlCommand command = new SqlCommand("spGetPageHistory", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            SqlParameter param;

            param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = masterCopySeparationSet;

            param = command.Parameters.Add("@HistoryMode", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = 1;

            param = command.Parameters.Add("@AllColors", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = allColors ? 1 : 0;

            DataTable tblResults = new DataTable();

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Time", Type.GetType("System.DateTime"));
            newColumn = tblResults.Columns.Add("Action", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Version", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Message", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Comment", Type.GetType("System.String"));

            SqlDataReader reader = null;
            DataRow newRow = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    newRow = tblResults.NewRow();

                    newRow["Time"] = reader.GetDateTime(idx++);
                    reader.GetString(idx++);
                    newRow["Action"] = reader.GetString(idx++);
                    newRow["Version"] = reader.GetInt32(idx++);
                    reader.GetString(idx++);
                    reader.GetString(idx++);

                    newRow["Message"] = reader.GetString(idx++);
                    reader.GetInt32(idx++);
                    reader.GetInt32(idx++);
                    newRow["Comment"] = reader.GetString(idx++);



                    tblResults.Rows.Add(newRow);
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return tblResults;

        }

        // If Section="" all sections are returned
        public bool GetPlatesDone(string location, string press, string publication, DateTime tPubDate, string issue, string edition, string section, out int nPlateInTotal, out int nPlatesDone, out int nPlateWithError, out string devices,
            out int nPlatesBend, out int nPlatesSorted, out string errmsg)
        {
            nPlateInTotal = 0;
            nPlatesDone = 0;
            nPlateWithError = 0;
            devices = "";
            nPlatesBend = 0;
            nPlatesSorted = 0;

            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spGetPlateList", connection);

            command.Parameters.Clear();
            SqlParameter param;

            /*param = command.Parameters.Add("@LocationID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = Globals.GetIDFromName("LocationNameCache", location);*/

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PressNameCache", press);

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PublicationNameCache", publication);

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Direction = ParameterDirection.Input;
            param.Value = tPubDate;

            param = command.Parameters.Add("@IssueID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = 1;//Globals.GetIDFromName("IssueNameCache", issue);

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("EditionNameCache", edition);

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;

            //if (Global.databaseVersion == 2)
            //	param.Value = 0;
            //else
            param.Value = Globals.GetIDFromName("SectionNameCache", section);

            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    nPlateInTotal = reader.GetInt32(0);
                    nPlatesDone = reader.GetInt32(1);
                    nPlateWithError = reader.GetInt32(2);
                    if (reader.FieldCount > 3)
                        devices = reader.GetString(3);
                    if (reader.FieldCount > 4)
                        nPlatesBend = reader.GetInt32(4);
                    if (reader.FieldCount > 5)
                        nPlatesSorted = reader.GetInt32(5);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool GetExportStatus(string publication, DateTime tPubDate, string edition, string section, ref List<ChannelProgress> channelProgressList, out string errmsg)
        {
            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spGetChannelProgressForReport", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            SqlParameter param;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = Globals.GetIDFromName("PublicationNameCache", publication);

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = tPubDate;

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = edition != "" ? Globals.GetIDFromName("EditionNameCache", edition) : 0;

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Value = section != "" ? Globals.GetIDFromName("SectionNameCache", section) : 0;


            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    channelProgressList.Add(new ChannelProgress()
                    {
                        ChannelID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Pages = reader.GetInt32(2),
                        PagesSent = reader.GetInt32(3),
                        PagesWithError = reader.GetInt32(4),
                        Alias = reader.GetString(5)

                    });
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog($"ERROR: GetExportDone - {errmsg}");
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        public bool GetExportDone(string publication, DateTime tPubDate,  string edition, string section, out int nPagesTotal, out int nPagesExported, out int nPagesExportedWithError, out string errmsg)
        {
            nPagesTotal = 0;
            nPagesExported = 9999;
            nPagesExportedWithError = 0;
         
            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            Global.logging.WriteLog($"GetExportDone called {publication}, {tPubDate}, {edition}, {section}");

            SqlCommand command = new SqlCommand("spGetChannelProgressForReport", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            SqlParameter param;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = Globals.GetIDFromName("PublicationNameCache", publication);

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = tPubDate;

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = edition != "" ? Globals.GetIDFromName("EditionNameCache", edition) : 0;

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Value = section != "" ? Globals.GetIDFromName("SectionNameCache", section) : 0;


            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (nPagesTotal == 0)
                        nPagesTotal = reader.GetInt32(2);

                    int n = reader.GetInt32(3);
                    if (n < nPagesExported)
                        nPagesExported = n;

                    n = reader.GetInt32(4);
                    if (n > nPagesExportedWithError)
                        nPagesExportedWithError = n;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog($"ERROR: GetExportDone - {errmsg}");
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (nPagesExported == 9999)
                nPagesExported = 0;
            return true;
        }
        public bool UpdateApproval(string userName, int masterCopySeparationSet, int approvalValue, out string errmsg)
        {
            return UpdateApproval(userName, masterCopySeparationSet, approvalValue, "", out errmsg);
        }

        public bool UpdateApproval(string userName, int masterCopySeparationSet, int approvalValue, string comment, out string errmsg)
        {
            errmsg = "";
            bool result = false;
            int retries = 10;

            while (--retries >= 0 && result == false)
            {
                result = UpdateApprovalEx(userName, masterCopySeparationSet, approvalValue, comment, out errmsg);
                if (result == false)
                    System.Threading.Thread.Sleep(1000);
            }

            if (result == false)
                Global.logging.WriteLog("ERROR: UpdateApproval() - " + errmsg);

            return result;
        }

        public bool UpdateApprovalEx(string userName, int masterCopySeparationSet, int approvalValue, string comment, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spUpdateApproval";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = masterCopySeparationSet;
            param = command.Parameters.Add("@ApprovalValue", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = approvalValue;

            param = command.Parameters.Add("@Username", SqlDbType.VarChar, 50);
            param.Direction = ParameterDirection.Input;
            param.Value = userName;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if ((bool)HttpContext.Current.Session["SetCommentOnDisapproval"] == false)
                return true;


            if ((bool)HttpContext.Current.Application["SetCommentInPrePollPageTable"] == false)
                return SetComment(masterCopySeparationSet, comment, out errmsg);
            else
                return InsertPrepollLog(350, masterCopySeparationSet, comment, out errmsg);
        }

        public bool SetComment(int masterCopySeparationSet, string comment, out string errmsg)
        {
            errmsg = "";

            if ((bool)HttpContext.Current.Session["SetCommentOnDisapproval"] == false)
                return true;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;


            command.CommandText = "UPDATE PageTable SET Comment='" + comment + "' WHERE MasterCopySeparationSet=" + masterCopySeparationSet;
            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }




        public bool ResetApproval(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "UPDATE PageTable SET Approved=0 WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString() + " AND Approved <> -1";
            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool UpdateFlatApproval(string userName, int nCopyFlatSeparationSet, int approvalValue, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spUpdateFlatApproval";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@CopyFlatSeparationSet", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = nCopyFlatSeparationSet;
            param = command.Parameters.Add("@ApprovalValue", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = approvalValue;
            param = command.Parameters.Add("@Username", SqlDbType.VarChar, 50);
            param.Direction = ParameterDirection.Input;
            param.Value = userName;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool UpdateCopyFlatHold(int copyFlatSeparationSet, int nHoldValue, int colorID, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;


            if (nHoldValue == 0 && (bool)HttpContext.Current.Application["UpdateApproveTimeOnRelease"])
            {
                command.CommandText = "UPDATE PageTable SET Hold=@HoldValue,ApproveTime=GETDATE() WHERE CopyFlatSeparationSet=@CopyFlatSeparationSet";
                if (colorID != 0)
                    command.CommandText = "UPDATE PageTable SET Hold=@HoldValue,ApproveTime=GETDATE() WHERE CopyFlatSeparationSet=@CopyFlatSeparationSet AND ColorID=" + colorID.ToString();

            }
            else
            {
                command.CommandText = "UPDATE PageTable SET Hold=@HoldValue WHERE CopyFlatSeparationSet=@CopyFlatSeparationSet";
                if (colorID != 0)
                    command.CommandText = "UPDATE PageTable SET Hold=@HoldValue WHERE CopyFlatSeparationSet=@CopyFlatSeparationSet AND ColorID=" + colorID.ToString();

            }
            command.CommandType = CommandType.Text;

            SqlParameter param;
            param = command.Parameters.Add("@CopyFlatSeparationSet", SqlDbType.Int);
            param.Value = copyFlatSeparationSet;
            param = command.Parameters.Add("@HoldValue", SqlDbType.Int);
            param.Value = nHoldValue;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            UpdateCopyFlatHoldFlatmaster(copyFlatSeparationSet, nHoldValue, colorID, out errmsg);

            if ((bool)HttpContext.Current.Application["RunCustomRelease"])
                CustomRelease(copyFlatSeparationSet, nHoldValue, colorID, out errmsg);

            return true;
        }

        public bool CustomRelease(int copyFlatSeparationSet, int nHoldValue, int colorID, out string errmsg)
        {
            errmsg = "";

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = CustomRelease2(copyFlatSeparationSet, nHoldValue, colorID, out errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }

        public bool CustomRelease2(int copyFlatSeparationSet, int nHoldValue, int colorID, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spCustomRelease";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@CopyFlatSeparationSet", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = copyFlatSeparationSet;
            param = command.Parameters.Add("@Hold", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = nHoldValue;

            if ((bool)HttpContext.Current.Application["StoredProcParamExists_spCustomRelease_ColorID"])
            {
                param = command.Parameters.Add("@ColorID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = colorID;
            }

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


        public bool UpdateCopyFlatHoldFlatmaster(int copyFlatSeparationSet, int nHoldValue, int colorID, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            string miscString2 = GetMiscString2(copyFlatSeparationSet, out errmsg);
            bool useMiscString2 = Globals.TryParse(miscString2, 0) > 0;
            Global.logging.WriteLog("UpdateCopyFlatHoldFlatmaster() : miscString2=" + miscString2);

            if (useMiscString2 == false)
                return true;

            if (nHoldValue == 0 && (bool)HttpContext.Current.Application["UpdateApproveTimeOnRelease"])
            {
                if ((bool)HttpContext.Current.Application["FieldExists_PageTable_FlatMaster"])
                    if (colorID == 0)
                        command.CommandText = "UPDATE PageTable SET Hold=@HoldValue,ApproveTime=GETDATE() WHERE FlatMaster=@MiscString2";
                    else
                        command.CommandText = "UPDATE PageTable SET Hold=@HoldValue,ApproveTime=GETDATE() WHERE FlatMaster=@MiscString2 AND ColorID=" + colorID.ToString();
                else
                    if (colorID == 0)
                    command.CommandText = "UPDATE PageTable SET Hold=@HoldValue,ApproveTime=GETDATE() WHERE MiscString2=@MiscString2";
                else
                    command.CommandText = "UPDATE PageTable SET Hold=@HoldValue,ApproveTime=GETDATE() WHERE MiscString2=@MiscString2 AND ColorID=" + colorID.ToString();
            }
            else
            {
                if ((bool)HttpContext.Current.Application["FieldExists_PageTable_FlatMaster"])
                    if (colorID == 0)
                        command.CommandText = "UPDATE PageTable SET Hold=@HoldValue WHERE FlatMaster=@MiscString2 AND ColorID=" + colorID.ToString();
                    else
                        command.CommandText = "UPDATE PageTable SET Hold=@HoldValue WHERE FlatMaster=@MiscString2";
                else
                    if (colorID == 0)
                    command.CommandText = "UPDATE PageTable SET Hold=@HoldValue WHERE MiscString2=@MiscString2 AND ColorID=" + colorID.ToString();
                else
                    command.CommandText = "UPDATE PageTable SET Hold=@HoldValue WHERE MiscString2=@MiscString2";
            }
            command.CommandType = CommandType.Text;

            SqlParameter param;
            param = command.Parameters.Add("@MiscString2", SqlDbType.VarChar, 12);
            param.Value = miscString2;
            param = command.Parameters.Add("@HoldValue", SqlDbType.Int);
            param.Value = nHoldValue;

            Global.logging.WriteLog("UpdateCopyFlatHoldFlatmaster() : sql=" + command.CommandText);
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("UpdateCopyFlatHoldFlatmaster() : error=" + errmsg);
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        public bool UpdateFlatHold(int nFlatSeparationSet, int nHoldValue, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spUpdateFlatHold";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@FlatSeparationSet", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = nFlatSeparationSet;
            param = command.Parameters.Add("@HoldValue", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = nHoldValue;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool InsertUserHistory(string userName, int action, string message, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            //		if ((bool)HttpContext.Current.Application["FieldExists_WebUserLog_Message"])
            command.CommandText = "INSERT INTO WebUserLog (UserName,[Action],EventTime,Message) VALUES ('" + userName + "'," + action.ToString() + ",GETDATE(),'" + message + "')";
            //		else
            //			command.CommandText = "INSERT INTO WebUserLog (UserName,[Action],EventTime) VALUES ('"+userName+"',"+action.ToString()+",GETDATE())";
            command.CommandType = CommandType.Text;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


        public bool UpdateHardProof(int mastercopySeparationSet, int nHardProofID, bool ignoreApproval, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            int n = nHardProofID * 256 + (ignoreApproval ? 1 : 3);

            command.CommandText = "UPDATE PageTable SET HardProofStatus=0, HardProofConfigurationID=" + n.ToString() + " WHERE MasterCopySeparationSet=" + mastercopySeparationSet;
            command.CommandType = CommandType.Text;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }
        public bool QueueFlatProof(int copyFlatSeparationSet, int nHardProofID, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = string.Format("INSERT INTO ProofGenerationQueue (CopyFlatSeparationSet,Type,ProofID,MiscInt,MiscString) VALUES ({0},1,{1},0,'')", copyFlatSeparationSet, nHardProofID);
            command.CommandType = CommandType.Text;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool InsertPrepollLog(int eventCode, int masterCopySeparationSet, string message, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spAddPrepollLogEntry";
            //"INSERT INTO PrePollPageTable (MasterCopySeparationSet,ProcessID,ProcessType,Event,Message,EventTime) VALUES ("+masterCopySeparationSet.ToString()+",0,170,"+eventCode.ToString()+",'"+userName+"',GETDATE())";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
            param.Value = masterCopySeparationSet;

            param = command.Parameters.Add("@ProcessID", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@ProcessType", SqlDbType.Int);
            param.Value = 170;

            param = command.Parameters.Add("@Event", SqlDbType.Int);
            param.Value = eventCode;

            param = command.Parameters.Add("@Message", SqlDbType.VarChar, 50);
            param.Value = message;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        // If Section="" all sections are returned
        public Int64 GetFirstSeparation(int productionID, out string errmsg)
        {
            Int64 sep = 0;
            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            string sql = "SELECT TOP 1 Separation FROM PageTable WITh (NOLOCK) WHERE ProductionID=" + productionID.ToString();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    sep = reader.GetInt64(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return 0;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return sep;
        }

        public int GetMaxPageStatus(int pdfMaster, out string errmsg)
        {
            errmsg = "";
            int maxStatus = 0;

            string sql = "SELECT MAX(Status) FROM PageTable WITH (NOLOCK) WHERE Dirty=0 AND PageType<2 AND Active>0 AND PdfMaster = " + pdfMaster.ToString();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    maxStatus = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return maxStatus;
        }


        public string GetMiscString2(int copyFlatSeparationSet, out string errmsg)
        {
            string miscString2 = "";
            errmsg = "";
            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            string sql = "SELECT TOP 1 ISNULL(MiscString2,'') FROM PageTable WITH (NOLOCK) WHERE CopyFlatSeparationSet=" + copyFlatSeparationSet.ToString();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    miscString2 = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return "";
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return miscString2;
        }

        public bool InsertLogEntry(int processID, int eventCode, string userName, string productionName,
            string pageDescription, int productionID, out string errmsg)
        {

            Int64 sep = GetFirstSeparation(productionID, out errmsg);
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = string.Format("INSERT INTO Log (EventTime, ProcessID,Event,Separation,FlatSeparation, ErrorMsg,[FileName],Version,MiscInt,MiscString) VALUES (GETDATE(), {0},{1},{2},0,'{3}','{4}',1,{5},'{6}')", processID, eventCode, sep, pageDescription, productionName, productionID, userName);

            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public DataTable GetTemplatePageFormatCollection(out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable();
            //	DataSet dataSet = new DataSet();

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("TemplateID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PageFormatID", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PressID", Type.GetType("System.Int32"));

            //connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand command = new SqlCommand("SELECT DISTINCT P.TemplateID, P.PageFormatID,T.PressID FROM TemplatePageFormats P INNER JOIN TemplateConfigurations T ON T.TemplateID=P.TemplateID", connection2);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.Text;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                    connection2.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["TemplateID"] = reader.GetInt32(0);
                    newRow["PageFormatID"] = reader.GetInt32(1);
                    newRow["PressID"] = reader.GetInt32(2);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection2.State == ConnectionState.Open)
                    connection2.Close();
            }
            //	dataSet.Tables.Add(tblResults);
            return tblResults;

        }

        public DataTable GetUserRights(string userName, string dataSetName, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable(dataSetName);
            //DataSet dataSet = new DataSet(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Password", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("Email", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("UserGroup", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("IsAdmin", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("PagesPerRow", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("FlatsPerRow", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("MayApprove", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("MayKillColor", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("MayReimage", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("MayRunProducts", Type.GetType("System.Int32"));
            newColumn = tblResults.Columns.Add("MayDeleteProducts", Type.GetType("System.Int32"));

            //connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand command = new SqlCommand("spGetUserRights", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@Username", SqlDbType.VarChar, 50);
            param.Direction = ParameterDirection.Input;
            param.Value = userName;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    //newRow["UserName"] = reader.GetString(0);
                    newRow["Password"] = reader.GetString(0);
                    newRow["Email"] = reader.GetString(1);
                    newRow["UserGroup"] = reader.GetString(2);

                    newRow["IsAdmin"] = reader.GetInt32(3);

                    newRow["PagesPerRow"] = reader.GetInt32(4);
                    newRow["FlatsPerRow"] = reader.GetInt32(5);

                    newRow["MayApprove"] = reader.GetInt32(6);
                    newRow["MayKillColor"] = reader.GetInt32(7);
                    newRow["MayReimage"] = reader.GetInt32(8);
                    newRow["MayRunProducts"] = reader.GetInt32(9);
                    newRow["MayDeleteProducts"] = reader.GetInt32(10);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            //	dataSet.Tables.Add(tblResults);
            return tblResults;
        }


        public DataTable GetUserPublications(string userName, string dataSetName, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Publication", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("MayApprove", Type.GetType("System.Int32"));

            SqlCommand command = new SqlCommand("spGetUserPublications", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@Username", SqlDbType.VarChar, 50);
            param.Direction = ParameterDirection.Input;
            param.Value = userName;

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["Publication"] = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(0));
                    newRow["MayApprove"] = 1; //reader.GetInt32(1);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblResults;
        }

        public DataTable GetUserPublicationsAdmGroup(string userName, string dataSetName, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Publication", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("MayApprove", Type.GetType("System.Int32"));

            string sql = "SELECT AP.PublicationID FROM AdmGroupUsers GRP INNER JOIN AdmGroupPublications AP ON AP.AdmGroupID=GRP.AdmGroupID WHERE GRP.UserName='" + userName + "'";
            SqlCommand command = new SqlCommand(sql, connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.Text;


            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["Publication"] = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(0));
                    newRow["MayApprove"] = 1; //reader.GetInt32(1);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblResults;
        }

        public DataTable GetPublicationsInAdmGroup(string admGroupName, string dataSetName, out string errmsg)
        {
            errmsg = "";
            DataTable tblResults = new DataTable(dataSetName);

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Publication", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("MayApprove", Type.GetType("System.Int32"));

            string sql = "SELECT DISTINCT AP.PublicationID FROM AdmGroupPublications AP INNER JOIN AdmGroupNames A ON A.AdmGroupID=AP.AdmGroupID WHERE A.AdmGroupName='" + admGroupName + "'";
            SqlCommand command = new SqlCommand(sql, connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.Text;



            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["Publication"] = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(0));
                    newRow["MayApprove"] = 1; //reader.GetInt32(1);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblResults;
        }

        public ArrayList GetUserLocationsAdmGroup(string userName, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();
            string sql = "SELECT AL.LocationID FROM AdmGroupUsers GRP INNER JOIN AdmGroupLocations AL ON AL.AdmGroupID=GRP.AdmGroupID WHERE GRP.UserName='" + userName + "'";

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string s = Globals.GetNameFromID("LocationNameCache", reader.GetInt32(0));
                    al.Add(s);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return al;
        }


        public ArrayList GetUserSections(string userName, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();

            SqlCommand command = new SqlCommand("SELECT DISTINCT SectionID FROM UserSections WHERE UserName='" + userName + "'", connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string s = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(0));
                    al.Add(s);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return al;
        }

        public ArrayList GetUserEditions(string userName, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();

            SqlCommand command = new SqlCommand("SELECT DISTINCT EditionID FROM UserEditions WHERE UserName='" + userName + "'", connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string s = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(0));
                    al.Add(s);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return al;
        }

        public ArrayList GetUserLocations(string userName, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();

            SqlCommand command = new SqlCommand("SELECT DISTINCT LocationID FROM UserLocations WHERE UserName='" + userName + "'", connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string s = Globals.GetNameFromID("LocationNameCache", reader.GetInt32(0));
                    al.Add(s);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return al;
        }

        public ArrayList GetUserPresses(string userName, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();
            string sql;
            if ((bool)HttpContext.Current.Application["UsePressGroups"])
                sql = "SELECT DISTINCT PressGroupID FROM UserPressGroups WHERE UserName='" + userName + "'";
            else
                sql = "SELECT DISTINCT PressID FROM UserPresses WHERE UserName='" + userName + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string s = (bool)HttpContext.Current.Application["UsePressGroups"] ? Globals.GetNameFromID("PressGroupNameCache", id) : Globals.GetNameFromID("PressNameCache", id);
                    al.Add(s);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return al;
        }

        public ArrayList GetPressesForMasterSet(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();

            SqlCommand command = new SqlCommand("SELECT DISTINCT PressID FROM PageTable WITH (NOLOCK) WHERE Dirty=0 AND MasterCopySeparationSet=" + masterCopySeparationSet.ToString(), connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int n = reader.GetInt32(0);
                    al.Add(n);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return al;
        }

        public int HasAdministrativeGroup(string admGroup, out string errmsg)
        {
            int ret = 0;
            errmsg = "";

            string sql = "SELECT TOP 1 AdmGroupName FROM AdmGroupNames WHERE AdmGroupName='" + admGroup + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                ret = 0;
                if (reader.Read())
                {
                    if (string.Equals(reader.GetString(0), admGroup, StringComparison.OrdinalIgnoreCase))
                        ret = 1;

                }
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog(ex.Message);
                ret = -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;

        }

        // Returns 0: user does not exists, 1: user exists, -1: error
        public int UserExists(string UserName, out string errmsg)
        {
            int ret = 0;
            errmsg = "";

            string sql = "SELECT UserName FROM UserNames WHERE UserName='" + UserName + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                ret = 0;
                if (reader.Read())
                {
                    if (string.Equals(reader.GetString(0), UserName, StringComparison.OrdinalIgnoreCase))
                        ret = 1;
                }
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog(ex.Message);
                ret = -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }

        public bool GetCurrentUserGroupForUser(string userName, ref string userGroupName, out string errmsg)
        {
            userGroupName = "";

            errmsg = "";

            string sql = "SELECT UG.UserGroupName FROM UserNames U INNER JOIN UserGroupNames UG ON UG.UserGroupID = U.UserGroupID WHERE U.Username = '" + userName + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();


                if (reader.Read())
                {
                    userGroupName = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog(ex.Message);
                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;

        }

        public int GetUserGroupID(string userGroupName, out string errmsg)
        {
            int ret = 0;
            errmsg = "";

            string sql = "SELECT UserGroupID FROM UserGroupNames WHERE UserGroupName='" + userGroupName + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                ret = 0;
                if (reader.Read())
                {
                    ret = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog(ex.Message);
                ret = -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;

        }

        public bool ChangeUserGroupForExistingUser(string userName, int userGroupID, out string errmsg)
        {
            errmsg = "";

            string sql = string.Format("UPDATE UserNames SET UserGroupID={0} WHERE Username='{1}'", userGroupID, userName);

            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool ChangeAdminGroupForExistingUser(string userName, List<string> AdministrativeGroups, out string errmsg)
        {
            errmsg = "";

            string sql = string.Format("DELETE AdmGroupUsers WHERE UserName='{0}'", userName);

            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if ((bool)HttpContext.Current.Application["UseAdminGroups"])
            {
                List<int> admIDList = new List<int>();
                foreach (string adgrp in AdministrativeGroups)
                {
                    int i = GetAdministrativeID(adgrp, out errmsg);
                    if (i > 0)
                        admIDList.Add(i);
                }

                foreach (int admID in admIDList)
                {
                    sql = string.Format("INSERT INTO AdmGroupUsers SELECT {0},'{1}'", admID, userName);

                    command = new SqlCommand(sql, connection);
                    try
                    {
                        if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                            connection.Open();
                        command.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        errmsg = ex.Message;
                        return false;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            else
            {
                // copy UserPublications etc...
            }

            return true;
        }


        public bool CreateUser(string userName, int userGroupID, List<string> AdministrativeGroups, out string errmsg)
        {

            errmsg = "";
            string sql = string.Format("INSERT INTO UserNames (Username,Password,UserGroupID,AccountEnabled,Email,PagesPerRow,PageFlatSize,RefreshTime,FullName,ColumnOrder,IPrange,CustomerID,DefaultPressID,DefaultPublicationID,MaxPlanPages) VALUES ('{0}','',{1}, 1, '',8,4,60,'','','',0,0,0,0)", userName, userGroupID);

            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if ((bool)HttpContext.Current.Application["UseAdminGroups"])
            {
                List<int> admIDList = new List<int>();
                foreach (string adgrp in AdministrativeGroups)
                {
                    int i = GetAdministrativeID(adgrp, out errmsg);
                    if (i > 0)
                        admIDList.Add(i);
                }

                foreach (int admID in admIDList)
                {
                    sql = string.Format("INSERT INTO AdmGroupUsers SELECT {0},'{1}'", admID, userName);

                    command = new SqlCommand(sql, connection);
                    try
                    {
                        if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                            connection.Open();
                        command.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        errmsg = ex.Message;
                        return false;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            else
            {
                // copy UserPublications etc...
            }

            return true;
        }

        public int GetAdministrativeID(string admName, out string errmsg)
        {
            int admID = 0;
            errmsg = "";

            string sql = "SELECT AdmGroupID FROM AdmGroupNames WHERE AdmGroupName='" + admName + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    admID = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog(ex.Message);
                admID = -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return admID;
        }

        public int PasswordValidation(string UserName, string Password, out bool bIsAdmin, out string IPrange, out bool bIsSuperUser, bool ignorePassword)
        {
            int ret = -1;
            IPrange = "";
            bIsSuperUser = false;
            bIsAdmin = false;

            string commandString = "SELECT U.UserName, U.AccountEnabled, U.Password, G.IsAdmin from UserNames AS U INNER JOIN UserGroupNames AS G ON U.USERGROUPID=G.USERGROUPID WHERE U.UserName='" + UserName + "'";

            if (Global.databaseVersion > 1)
                commandString = "SELECT U.UserName, U.AccountEnabled, U.Password, G.IsAdmin,U.IPrange,G.IPrange,G.USERGROUPID from UserNames AS U INNER JOIN UserGroupNames AS G ON U.USERGROUPID=G.USERGROUPID WHERE U.UserName='" + UserName + "'";


            SqlCommand command = new SqlCommand(commandString, connection);
            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                ret = 0;
                if (reader.Read())
                {
                    if (reader.GetString(0) == UserName)
                        ret = 1;

                    if (reader.GetInt32(1) == 0) // Account enabled
                        ret = 2;

                    if (reader.GetString(2) == Password && ret != 2)
                        ret = 3;
                    if (ignorePassword && ret != 2)
                        ret = 3;

                    bIsAdmin = reader.GetInt32(3) == 1 && ret == 3 ? true : false;

                    if (Global.databaseVersion > 1)
                    {
                        IPrange = reader.GetString(4);
                        if (IPrange == "")
                            IPrange = reader.GetString(5);

                        if (reader.GetInt32(6) <= 2)
                            bIsSuperUser = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog(ex.Message);
                ret = -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }


        public bool UpdateProductionHold(int nHoldValue, string press, string publication, DateTime pubdate, string issue, string edition, string section, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spUpdateProductionHold";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PressNameCache", press);

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Direction = ParameterDirection.Input;
            param.Value = pubdate;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("PublicationNameCache", publication);

            param = command.Parameters.Add("@IssueID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = 1;//Globals.GetIDFromName("IssueNameCache", issue);

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("EditionNameCache", edition);

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("SectionNameCache", section);

            param = command.Parameters.Add("@HoldValue", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = nHoldValue;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }

        public bool GetProductionDeadLine(DateTime pubDate, string publication, string channel, ref DateTime deadline, out string errmsg)
        {
            errmsg = "";
            deadline = DateTime.MinValue;
            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "SELECT TOP 1 DeadLine FROM PageTable WHERE PublicationID=@PublicationID AND PubDate=@PubDate";
            command.CommandType = CommandType.Text;

            SqlParameter param;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = Globals.GetIDFromName("PublicationNameCache", publication);
            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    deadline = reader.GetDateTime(0);
                }
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog(ex.Message);
                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        } 

        public bool UpdateProductionPriority(int nPriority, string press, string publication, DateTime pubDate, string issue, string edition, string section, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spUpdateProductionPriority";
			command.CommandType = CommandType.StoredProcedure;
				
			SqlParameter param;
			param = command.Parameters.Add("@PressID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			int nPressID = Globals.GetIDFromName("PressNameCache", press);
			param.Value = nPressID > 0 ? nPressID : 1;
			
			param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
			param.Direction = ParameterDirection.Input;
			param.Value = pubDate;

			param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = Globals.GetIDFromName("PublicationNameCache", publication);

			param = command.Parameters.Add("@IssueID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = 1;//Globals.GetIDFromName("IssueNameCache", issue);

			param = command.Parameters.Add("@EditionID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = Globals.GetIDFromName("EditionNameCache", edition);

			param = command.Parameters.Add("@SectionID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = Globals.GetIDFromName("SectionNameCache", section);

			param = command.Parameters.Add("@PriorityValue", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = nPriority;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		public bool SetProductionDirtyFlag(int nProductionID, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PageTable SET Dirty=1 WHERE ProductionID="+nProductionID.ToString();
			command.CommandType = CommandType.Text;
		
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		public bool SetProductionUnDirtyFlag(int nProductionID, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PageTable SET Dirty=0 WHERE ProductionID="+nProductionID.ToString();
			command.CommandType = CommandType.Text;
		
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}
		
		public bool ChangePubDate(int nProductionID, DateTime pubDate, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PageTable SET PubDate=@PubDate WHERE ProductionID=@ProductionID";
			command.CommandType = CommandType.Text;
		
			SqlParameter param;
			param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
			param.Direction = ParameterDirection.Input;
			param.Value = pubDate;
			param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = nProductionID;
		

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}
		

		public bool SetPressRunDirtyFlag(int nPressRunID, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PageTable SET Dirty=1 WHERE PressRunID="+nPressRunID.ToString();
			command.CommandType = CommandType.Text;
		
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		public bool DeleteDirtyFlag(int productionID, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spImportCenterCleanProduction";
			command.CommandType = CommandType.StoredProcedure;

			SqlParameter param;
			param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = productionID;
		
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

        public bool DeleteAllDirtyPages(int productionID, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spImportCenterCleanDirtyPages";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = productionID;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }
        
        //  color="" UPDATES ALL COLORS
		public bool UpdateActive(int nMasterCopySeparationSet, string color, int nActiveValue, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spUpdateActiveEx";
			command.CommandType = CommandType.StoredProcedure;
				
			SqlParameter param;
			param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = nMasterCopySeparationSet;
			param = command.Parameters.Add("@ColorID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = Globals.GetIDFromName("ColorNameCache", color);

			param = command.Parameters.Add("@Active", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = nActiveValue;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		public bool ChangePDFcolors(int nMasterCopySeparationSet, string color, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PageTable SET ColorID=@ColorID WHERE MasterCopySeparationSet=@MasterCopySeparationSet";
			command.CommandType = CommandType.Text;
				
			SqlParameter param;
			param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
			param.Value = nMasterCopySeparationSet;
			param = command.Parameters.Add("@ColorID", SqlDbType.Int);
			param.Value = Globals.GetIDFromName("ColorNameCache", color);

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		// MasterCopySeparationSet = ColorSet
		// FlatSeparation = PairSet
		public bool ReimageFlatSeparation(int nMasterCopySeparationSet, int nFlatSeparation, string color, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spReimage";
			command.CommandType = CommandType.StoredProcedure;
				
			SqlParameter param;
			param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = nMasterCopySeparationSet;

			if (Global.databaseVersion == 2)
			{
				param = command.Parameters.Add("@FlatSeparation", SqlDbType.BigInt);
				param.Value = Convert.ToInt64(nFlatSeparation);
			}
			else
			{
				param = command.Parameters.Add("@FlatSeparation", SqlDbType.Int);
				param.Value = nFlatSeparation;
			}	
			param.Direction = ParameterDirection.Input;

			param = command.Parameters.Add("@ColorID", SqlDbType.VarChar,48);
			param.Direction = ParameterDirection.Input;
			param.Value = Globals.GetIDFromName("ColorNameCache", color);
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		public bool ReimageFlatSeparationSet(int nCopyFlatSeparationSet, int nCopyNumber, string color, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			int nColorID = Globals.GetIDFromName("ColorNameCache", color);

            if (nCopyNumber == 0) // TX-system
            {
                if (color == "")
                    command.CommandText = "UPDATE PageTable SET Status=30,MiscInt4=0,FlatProofStatus=0 WHERE CopyFlatSeparationSet=" + nCopyFlatSeparationSet.ToString();
                else
                    command.CommandText = "UPDATE PageTable SET Status=30,MiscInt4=0,FlatProofStatus=0 WHERE CopyFlatSeparationSet=" + nCopyFlatSeparationSet.ToString() + " AND ColorID=" + nColorID.ToString();
            }
            else
            {
                if (color == "")
                    command.CommandText = "UPDATE PageTable SET Status=30,MiscInt4=0 WHERE CopyNumber=" + nCopyNumber.ToString() + " AND CopyFlatSeparationSet=" + nCopyFlatSeparationSet.ToString();
                else
                    command.CommandText = "UPDATE PageTable SET Status=30,MiscInt4=0 WHERE CopyNumber=" + nCopyNumber.ToString() + " AND CopyFlatSeparationSet=" + nCopyFlatSeparationSet.ToString() + " AND ColorID=" + nColorID.ToString();
            }

			command.CommandType = CommandType.Text;
				
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

        // TX-system!
        public bool RetransmitFlatSeparationSet(int nCopyFlatSeparationSet, string color, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            int nColorID = Globals.GetIDFromName("ColorNameCache", color);

            if (color == "")
                command.CommandText = "UPDATE PageTable SET Status=49 WHERE CopyFlatSeparationSet=" + nCopyFlatSeparationSet.ToString();
            else
                command.CommandText = "UPDATE PageTable SET Status=49 WHERE CopyFlatSeparationSet=" + nCopyFlatSeparationSet.ToString() + " AND ColorID=" + nColorID.ToString();
            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }

        public bool RetransmitCustom(int nCopyFlatSeparationSet, string color, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;


            command.CommandText = "spCustomRetransmit";
            SqlParameter param;
            param = command.Parameters.Add("@CopyFlatSeparationSet", SqlDbType.BigInt);
            param.Direction = ParameterDirection.Input;
            param.Value = nCopyFlatSeparationSet;

            param = command.Parameters.Add("@ColorID", SqlDbType.VarChar, 48);
            param.Direction = ParameterDirection.Input;
            param.Value = Globals.GetIDFromName("ColorNameCache", color);

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }


        // TX-system!
        public bool RetransmitMasterCopySeparationSetTX(int pressID, int nMasterCopySeparationSet, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;


            if (pressID == 0)
                command.CommandText = "UPDATE PageTable SET Status=49,MiscInt4=0 WHERE MasterCopySeparationSet=" + nMasterCopySeparationSet.ToString();
            else
                command.CommandText = "UPDATE PageTable SET Status=49,MiscInt4=0 WHERE MasterCopySeparationSet=" + nMasterCopySeparationSet.ToString() + " AND PressID=" + pressID.ToString();
            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }

        // Release specific plate set on specific press - only unique/forced plates will be released!
        public bool ReleaseMasterCopySeparationSetTX(int pressID, int nMasterCopySeparationSet, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "UPDATE PageTable SET Hold=0 WHERE  CopyFlatSeparationSet IN (SELECT DISTINCT P2.CopyFlatSeparationSet FROM PageTable P2 WHERE P2.MasterCopySeparationSet=" + nMasterCopySeparationSet.ToString() + " AND PressID=" + pressID.ToString() + " AND UniquePage>0 AND Dirty=0 AND Active>0)";
            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }


        // Release specific plate set on specific press - only unique/forced plates will be released!
        public bool ReleaseCopyFlatSeparationSetTX(int copyFlatSeparationSet, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "UPDATE PageTable SET Hold=0 WHERE  CopyFlatSeparationSet=" + copyFlatSeparationSet.ToString() + " AND UniquePage>0 AND Dirty=0 AND Active>0)";
            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }



		public bool Retransmit(int nMasterCopySeparationSet, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PageTable SET Status=20,MiscInt4=0 WHERE Status >= 29 AND MasterCopySeparationSet="+nMasterCopySeparationSet.ToString();

			command.CommandType = CommandType.Text;
				
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}


        public bool PeekFlashQueue(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            bool found = false; 
            SqlCommand command = new SqlCommand(string.Format("SELECT TOP 1 FileName FROM FlashPreviewQueue WHERE (FileName='{0}.jpg') OR (FileName LIKE '%===={1}.jpg')",masterCopySeparationSet,masterCopySeparationSet), connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    found = true;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return found;
        }

        public bool LookupReproofEvent(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            bool found = false;
            SqlCommand command = new SqlCommand(string.Format("SELECT TOP 1 Event FROM PrePollPageTable WHERE MasterCopySeparationSet={0} AND Event=298", masterCopySeparationSet), connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    found = true;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return found;
        }


        public bool Reproof(int nMasterCopySeparationSet, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "UPDATE PageTable SET ProofStatus=0 WHERE MasterCopySeparationSet=" + nMasterCopySeparationSet.ToString();

            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }


        
        public bool UpdatePriority(int nCopySeparationSet, int nPriorityValue, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spUpdatePriority";
			command.CommandType = CommandType.StoredProcedure;
				
			SqlParameter param;
			param = command.Parameters.Add("@CopySeparationSet", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = nCopySeparationSet;
			param = command.Parameters.Add("@Priority", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = nPriorityValue;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		public bool UpdatePriorityEx(ArrayList al, ArrayList alcolor, int nPriority, bool bAllColors, bool bAllPages, bool bAllCopies, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			string setName = bAllCopies ? "MasterCopySeparationSet" : "SeparationSet";

			string sSQL = "UPDATE PageTable SET Priority="+nPriority.ToString()+" WHERE ";
			
			if (bAllColors)
			{
				for (int i=0;i<al.Count; i++) 
				{
					if (i>0)
						sSQL += " OR ";
					sSQL += setName + "=" + ((int)al[i]).ToString(); 
				
				}
			}
			else
			{
				for (int i=0;i<al.Count; i++) 
				{
					int colorID =  Globals.GetIDFromName("ColorNameCache", (string)alcolor[i]);
					if (i>0)
						sSQL += " OR ";
					sSQL += "(" + setName + "=" + ((int)al[i]).ToString()+ " AND ColorID="+colorID.ToString()+")"; 				
				}
			}				
			command.CommandText = sSQL;	
			command.CommandType = CommandType.Text;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		

		public bool ResetProofed(int nMasterCopySeparationSet, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			string sSQL = "UPDATE PageTable SET ProofStatus=0 WHERE MasterCopySeparationSet="+nMasterCopySeparationSet.ToString();
		
			
			command.CommandText = sSQL;	
			command.CommandType = CommandType.Text;
		
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		public bool UpdateTemplate(ArrayList al, int nTemplateID, bool bAllCopies, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			string setName = bAllCopies ? "CopyFlatSeparationSet" : "FlatSeparationSet";

			string sSQL = "UPDATE PageTable SET DeviceID=0, TemplateID="+nTemplateID.ToString()+" WHERE ";
			
			for (int i=0;i<al.Count; i++) 
			{
				if (i>0)
					sSQL += " OR ";
				int n = (int)al[i];
				if (bAllCopies)
					n /= 100;

				sSQL += setName + "=" + n.ToString(); 
			}
	
			command.CommandText = sSQL;	
			command.CommandType = CommandType.Text;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}

		public bool GetNumberOfPageColors(int nMasterCopySeparatioSet, out int colors, out string errmsg)
		{
			errmsg = "";
			colors = 0;

			string sqlCmd = "SELECT COUNT(DISTINCT ColorID) FROM PageTable WITH (NOLOCK) WHERE Active>0 AND PageType<2 AND UniquePage>0 AND MasterCopySeparationSet="+nMasterCopySeparatioSet.ToString();
			
			SqlCommand command = new SqlCommand(sqlCmd, connection);
			command.CommandType = CommandType.Text;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if  ( reader.Read()) 
				{
					colors = reader.GetInt32(0);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}

        public int GetMasterSetFromCopyFlatSeparationSet(int copyFlatSeparatioSet, out string errmsg)
        {
            errmsg = "";

            int masterCopySeparatioSet = 0;

            string sqlCmd = "SELECT TOP 1 MasterCopySeparationSet FROM PageTable WITH (NOLOCK)  WHERE Dirty=0 AND CopyFlatSeparationSet=" + copyFlatSeparatioSet.ToString();

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    masterCopySeparatioSet = reader.GetInt32(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return 0;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return masterCopySeparatioSet;
        }

        public bool GetMasterSetFromCopyFlatSeparationSetMulti(int copyFlatSeparatioSet, ref ArrayList sMasterCopySeparationList, out string errmsg)
        {
            errmsg = "";

            sMasterCopySeparationList.Clear();
            int mst = GetMasterSetFromCopyFlatSeparationSet(copyFlatSeparatioSet, out errmsg);
            if (mst == 0)
                return true;
            sMasterCopySeparationList.Add(mst);

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "spGetEquivalentMasterSets";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
            param.Value = mst;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    sMasterCopySeparationList.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


		public bool GetPageColors(int nMasterCopySeparatioSet, out string pagename, ref ArrayList ColorNames, ref ArrayList ColorActiveFlags, out string errmsg)
		{
			errmsg = "";

			ColorNames.Clear();
			ColorActiveFlags.Clear();
			pagename = "";

			string sqlCmd = "SELECT DISTINCT PageName, ColorNames.ColorName, PageTable.Active FROM PageTable INNER JOIN ColorNames ON PageTable.ColorID=ColorNames.ColorID WHERE MasterCopySeparationSet="+nMasterCopySeparatioSet.ToString();
			
			SqlCommand command = new SqlCommand(sqlCmd, connection);
			command.CommandType = CommandType.Text;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				while ( reader.Read()) 
				{
					pagename = reader.GetString(0);
					ColorNames.Add(reader.GetString(1));
					ColorActiveFlags.Add(reader.GetInt32(2));
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}


		public bool GetPlateColors(int copyFlatSeparationSet, ref ArrayList ColorNames, out string errmsg)
		{
			errmsg = "";

			ColorNames.Clear();

			string sqlCmd = "SELECT DISTINCT ColorNames.ColorName FROM PageTable WITH (NOLOCK) INNER JOIN ColorNames WITH (NOLOCK) ON PageTable.ColorID=ColorNames.ColorID WHERE PageTable.Active>0 AND PageTable.PageType<=2 AND PageTable.CopyFlatSeparationSet="+copyFlatSeparationSet.ToString();
			
			SqlCommand command = new SqlCommand(sqlCmd, connection);
			command.CommandType = CommandType.Text;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				while ( reader.Read()) 
				{
					ColorNames.Add(reader.GetString(0));
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}

		public bool GetPlateCopies(int copyFlatSeparationSet, ref int nCopies, out string errmsg)
		{
			errmsg = "";

			nCopies = 1;

			string sqlCmd = "SELECT DISTINCT CopyNumber FROM PageTable WITH (NOLOCK)  WHERE Active>0 AND PageType<=2 AND CopyFlatSeparationSet="+copyFlatSeparationSet.ToString();
			
			SqlCommand command = new SqlCommand(sqlCmd, connection);
			command.CommandType = CommandType.Text;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{
					nCopies = reader.GetInt32(0);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}

        public bool GetPlateVersion(int copyFlatSeparationSet, ref int version, out string errmsg)
        {
            errmsg = "";
            version = 1;

            string sqlCmd = "SELECT MIN(Version) FROM PageTable WITH (NOLOCK) WHERE Active>0 AND PageType<=2 AND CopyFlatSeparationSet=" + copyFlatSeparationSet.ToString();

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    version = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        
        public bool GetPageVersion(int nMasterCopySeparatioSet, out int nVersion, out string errmsg)
		{
			errmsg = "";

			nVersion = 0;

			string sqlCmd = "SELECT MAX(Version) FROM PageTable WITH (NOLOCK) WHERE Active>0 AND MasterCopySeparationSet="+nMasterCopySeparatioSet.ToString();
			
			SqlCommand command = new SqlCommand(sqlCmd, connection);
			command.CommandType = CommandType.Text;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{
					nVersion = reader.GetInt32(0);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}

		public bool HasMask(int nMasterCopySeparatioSet, out string errmsg)
		{
			errmsg = "";

			bool hasMask = false;
			string sqlCmd = "SELECT TOP 1 WriteEachColor & 0x240,PR.MiscInt1 FROM ProofConfigurations PROOF WITH (NOLOCK) INNER JOIN PageTable P WITH (NOLOCK) ON P.ProofID=PROOF.ProofID INNER JOIN PressRunID PR WITH (NOLOCK) ON PR.PressRunID=P.PressRunID WHERE P.MasterCopySeparationSet="+nMasterCopySeparatioSet.ToString();
			
			SqlCommand command = new SqlCommand(sqlCmd, connection);
			command.CommandType = CommandType.Text;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{

					hasMask = reader.GetInt32(0) > 0;

				//	if ((bool)HttpContext.Current.Application["MustUsePressRunMask"])
				//		if (reader.GetInt32(1) == 0)
				//			hasMask = false;

				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return hasMask;
		}


		public bool HasMaskFlat(int nCopyFlatSeparatioSet, out string errmsg)
		{
			errmsg = "";

			bool hasMask = false;
			string sqlCmd = "SELECT TOP 1 WriteEachColor & 0x40 / 64,PR.MiscInt1 FROM ProofConfigurations PROOF WITH (NOLOCK) INNER JOIN PageTable P WITH (NOLOCK) ON P.ProofID=PROOF.ProofID INNER JOIN PressRunID PR WITH (NOLOCK) ON PR.PressRunID=P.PressRunID WHERE P.CopyFlatSeparatioSet="+nCopyFlatSeparatioSet.ToString();
			
			SqlCommand command = new SqlCommand(sqlCmd, connection);
			command.CommandType = CommandType.Text;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{

					hasMask = reader.GetInt32(0) > 0;

					if ((bool)HttpContext.Current.Application["MustUsePressRunMask"])
						if (reader.GetInt32(1) == 0)
							hasMask = false;

				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return hasMask;
		}

		public bool GetComment(int nMasterCopySeparatioSet, out string comment, out int publicationID, out DateTime pubDate,  out string errmsg)
		{
			errmsg = "";
			comment = "";
			publicationID = 0;
			pubDate = DateTime.MinValue;
            string sqlCmd = "SELECT TOP 1 Comment,PublicationID,PubDate FROM PageTable WITH (NOLOCK) WHERE UniquePage>0 AND Active>0 AND MasterCopySeparationSet="+nMasterCopySeparatioSet.ToString() + " ORDER BY UniquePage";


            SqlCommand command = new SqlCommand(sqlCmd, connection);
			command.CommandType = CommandType.Text;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{
					comment = reader.GetString(0);
					publicationID = reader.GetInt32(1);
					pubDate = reader.GetDateTime(2);
  
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}

		public bool UpdateApproveLog(int mastercopyseparationset, int colorID, bool approved, string comment, string approveUser, out string errmsg)
		{
			bool ret = false;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spAddLogEntryCopySeparationSetVersion";
			command.CommandType = CommandType.StoredProcedure;
				
			SqlParameter param;
			param = command.Parameters.Add("@ProcessID", SqlDbType.Int);	
			param.Value = 0;

			param = command.Parameters.Add("@Event", SqlDbType.Int);
			param.Value = approved ? 70 : 71;

			param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
			param.Value = mastercopyseparationset;
			
			param = command.Parameters.Add("@ColorID", SqlDbType.Int);
			param.Value = colorID;

			param = command.Parameters.Add("@ErrorMsg", SqlDbType.VarChar, 200);
			param.Value = approved ? "" : comment;
			
			param = command.Parameters.Add("@DaysToKeep", SqlDbType.Int);
			param.Value = 0;

			param = command.Parameters.Add("@FileName", SqlDbType.VarChar, 50);
			param.Value = "";
			
			param = command.Parameters.Add("@Version", SqlDbType.Int);
			param.Value = -1;
			
			param = command.Parameters.Add("@MiscInt", SqlDbType.Int);
			param.Value = 0;
			
			param = command.Parameters.Add("@MiscString", SqlDbType.VarChar, 50);
			param.Value = approveUser;

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
				ret = true;
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return ret;
		}


        public bool GetUserProfileEx(string userName, out bool showFlatView, out bool showListView, out bool showReadView,
            out bool showPressView, out bool showPlanView, out bool showLogView, out bool showReportView, out bool showUnknownView, out string errmsg)
        {
            errmsg = "";

            showReadView = false;
            showListView = false;
            showFlatView = false;
            showPressView = false;
            showPlanView = false;
            showLogView = false;
            showReportView = false;
            showUnknownView = false;

            /*-- WebCenterViews bits
-- bit 1: Readview
-- bit 2: Separation
-- bit 3: Flatview
-- bit 4: Press runs
-- bit 5: Planning view
-- bit 6: Flow view
-- bit 7: Report view*/

            string sqlCmd = "SELECT G.WebCenterViews FROM UserNames AS U INNER JOIN UserGroupNames AS G On G.UserGroupID=U.UserGroupID WHERE U.UserName='" + userName + "'";
            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int n = reader.GetInt32(0);
                    showReadView = (n & 0x01) > 0 ? true : false;
                    showListView = (n & 0x02) > 0 ? true : false;
                    showFlatView = (n & 0x04) > 0 ? true : false;
                    showPressView = (n & 0x08) > 0 ? true : false;
                    showPlanView = (n & 0x10) > 0 ? true : false;
                    showLogView = (n & 0x20) > 0 ? true : false;
                    showReportView = (n & 0x40) > 0 ? true : false;
                    showUnknownView = (n & 0x80) > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        public bool GetUserProfile(string userName, out string fullUserName, out string email,
            out int nPagesPerRow, out int nRefreshTime, out bool bMayApprove,
            out bool bMayReImage, out bool bMaýRunProduction,
            out int nFlatsPerRow, out string columnOrder, out int customerID, out int defaultPressID, out int defaultPublicationID,
            out bool bMayHardProof, out bool bMayFlatProof, out bool bMayKillColor, out bool bMayDeleteProduction, out bool bMayConfigure,
            out int nMaxPlanPages, out bool bHideOld, out bool bMayUpload, out bool bMayReprocess, out string userGroup, out string errmsg)
        {
            errmsg = "";
            nPagesPerRow = 8;
            nRefreshTime = 60;
            fullUserName = "";
            userGroup = "";
            nFlatsPerRow = 4;
            email = "";
            bMayApprove = false;
            bMayReImage = false;
            bMayKillColor = false;
            bMaýRunProduction = false;
            bMayDeleteProduction = false;
            bMayConfigure = false;
            bHideOld = false;
            bMayUpload = true;
            bMayReprocess = true;

            customerID = 0;
            defaultPublicationID = 0;
            defaultPressID = 0;
            bMayHardProof = false;
            bMayFlatProof = false;
            nMaxPlanPages = 0;

            columnOrder = "";

            bool hasMaxPlanPages = FieldExists("UserNames", "MaxPlanPages", out errmsg) == 1;
            bool hasMayUpload = FieldExists("UserGroupNames", "MayUpload", out errmsg) == 1;
            bool hasMayReprocess = FieldExists("UserGroupNames", "MayReprocess", out errmsg) == 1;

            errmsg = "";

            string sqlCmd = "SELECT U.PagesPerRow, U.RefreshTime, U.FullName, U.Email, G.MayApprove, G.MayReImage, G.MayRunProducts, U.PageFlatSize,G.MayKillColor,G.MayDeleteProducts,G.MayConfigure,G.UserGroupName FROM UserNames AS U INNER JOIN UserGroupNames AS G On G.UserGroupID=U.UserGroupID WHERE U.UserName='" + userName + "'";

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {

                    nPagesPerRow = reader.GetInt32(0);
                    nRefreshTime = reader.GetInt32(1);
                    fullUserName = reader.GetString(2);
                    email = reader.GetString(3);
                    bMayApprove = reader.GetInt32(4) > 0 ? true : false;
                    bMayReImage = reader.GetInt32(5) > 0 ? true : false;
                    bMaýRunProduction = reader.GetInt32(6) > 0 ? true : false;
                    nFlatsPerRow = reader.GetInt32(7);
                    bMayKillColor = reader.GetInt32(8) > 0 ? true : false;
                    bMayDeleteProduction = reader.GetInt32(9) > 0 ? true : false;
                    bMayConfigure = reader.GetInt32(10) > 0 ? true : false;
                    userGroup = reader.GetString(11);
                    //  bHideOld = bMayConfigure == false;
                    /* ## */
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            /*
            command.CommandText = "SELECT ColumnOrder FROM UserNames WHERE UserName='" + userName + "'";
            SqlDataReader reader2 = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader2 = command.ExecuteReader();
                if (reader2.Read())
                {
                    columnOrder = reader2.GetString(0);
                }
            }
            catch (Exception ex)
            {
                // do not report error..(backward compatible)
            }
            finally
            {
                // always call Close when done reading.
                if (reader2 != null)
                    reader2.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }*/

            if (hasMaxPlanPages)
                command.CommandText = "SELECT U.CustomerID,U.DefaultPressID,U.DefaultPublicationID, G.MayHardProof, G.MayFlatProof,U.MaxPlanPages FROM UserNames U INNER JOIN UserGroupNames AS G On G.UserGroupID=U.UserGroupID WHERE U.UserName='" + userName + "'";
            else
                command.CommandText = "SELECT U.CustomerID,U.DefaultPressID,U.DefaultPublicationID, G.MayHardProof, G.MayFlatProof FROM UserNames U INNER JOIN UserGroupNames AS G On G.UserGroupID=U.UserGroupID WHERE U.UserName='" + userName + "'";
            SqlDataReader reader3 = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader3 = command.ExecuteReader();
                if (reader3.Read())
                {
                    customerID = reader3.GetInt32(0);
                    defaultPressID = reader3.GetInt32(1);
                    defaultPublicationID = reader3.GetInt32(2);
                    bMayHardProof = reader3.GetInt32(3) > 0 ? true : false;
                    bMayFlatProof = reader3.GetInt32(4) > 0 ? true : false;
                    if (hasMaxPlanPages)
                        nMaxPlanPages = reader3.GetInt32(5);
                }
            }
            catch //(Exception ex)
            {
                // do not report error..(backward compatible)
            }
            finally
            {
                // always call Close when done reading.
                if (reader3 != null)
                    reader3.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }


            if (hasMayUpload)
            {
                command.CommandText = "SELECT ISNULL(G.MayUpload,1) FROM UserNames U INNER JOIN UserGroupNames AS G On G.UserGroupID=U.UserGroupID WHERE U.UserName='" + userName + "'";

                SqlDataReader reader4 = null;
                try
                {
                    // Execute the command
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    reader4 = command.ExecuteReader();
                    if (reader4.Read())
                    {
                        bMayUpload = reader4.GetInt32(0) > 0 ? true : false;
                    }
                }
                catch //(Exception ex)
                {
                    // do not report error..(backward compatible)
                }
                finally
                {
                    // always call Close when done reading.
                    if (reader4 != null)
                        reader4.Close();
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            if (hasMayReprocess)
            {
                command.CommandText = "SELECT ISNULL(G.MayReprocess,1) FROM UserNames U INNER JOIN UserGroupNames AS G On G.UserGroupID=U.UserGroupID WHERE U.UserName='" + userName + "'";

                SqlDataReader reader5 = null;
                try
                {
                    // Execute the command
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    reader5 = command.ExecuteReader();
                    if (reader5.Read())
                    {
                        bMayReprocess = reader5.GetInt32(0) > 0 ? true : false;
                    }
                }
                catch // (Exception ex)
                {
                    // do not report error..(backward compatible)
                }
                finally
                {
                    // always call Close when done reading.
                    if (reader5 != null)
                        reader5.Close();
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return true;
        }

        public bool RegisterUserNameLoginTime(string userName, out string errmsg)
        {

            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "UPDATE UserNames SET LastLoginTime=GETDATE() WHERE UserName='" + userName + "'";
            command.CommandType = CommandType.Text;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }




        public bool UpdateUserProfile(string userName, string fullUserName, string email, int nPagesPerRow,  int nRefreshTime, int nFlatViewSize, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE UserNames SET FullName='" + fullUserName + "',Email='"+email+"',PagesPerRow="+nPagesPerRow.ToString()+", RefreshTime="+nRefreshTime.ToString()+", PageFlatSize="+nFlatViewSize.ToString()+" WHERE UserName='" + userName + "'";
			command.CommandType = CommandType.Text;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}


		public bool UpdateUserProfileColumnOrder(string userName, string columnOrder, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE UserNames SET ColumnOrder='"+columnOrder+"' WHERE UserName='" + userName + "'";
			command.CommandType = CommandType.Text;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}



		// ControloCenter only stuff...

		public DateTime GetDate(out string errmsg)
		{
			DateTime now = DateTime.Now;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "SELECT GetDate()";
			command.CommandType = CommandType.Text;	
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{	
					now = reader.GetDateTime(0);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return now;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			return now;
		}


		public bool GetPageCount(int pressRunID, out int npages, out string errmsg)
		{
			npages = 0;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "SELECT count(DISTINCT PageName) FROM PageTable WITH (NOLOCK) WHERE Active>0 AND PageType<2 AND PressRunID="+pressRunID.ToString();
			command.CommandType = CommandType.Text;	
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{	
					npages = reader.GetInt32(0);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			return true;
		}


		public bool GetPublicationEmail(int publicationID, out int customerID, out string uploadFolder, out string emailRecipient, out string emailCC, out string emailSubject, out string emailBody, out string errmsg)
		{
			customerID = 0;
			uploadFolder = "";
			emailRecipient = "";
			emailCC = "";
			emailSubject = "";
			emailBody = "";
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "SELECT ISNULL(CustomerID,0),ISNULL(EmailRecipient,''),ISNULL(EmailCC,''),ISNULL(EmailSubject,''),ISNULL(EmailBody,''),ISNULL(UploadFolder,'') FROM PublicationNames WHERE PublicationID="+publicationID.ToString();
			command.CommandType = CommandType.Text;	
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{	
					int f=0;
					customerID = reader.GetInt32(f++);
					emailRecipient = reader.GetString(f++);
					emailCC = reader.GetString(f++);
					emailSubject = reader.GetString(f++);
					emailBody = reader.GetString(f++);
					uploadFolder = reader.GetString(f++);
                    int n = uploadFolder.IndexOf(';');
                    if (n == 0)
                        uploadFolder = "";
                    if (n > 0)
                        uploadFolder = uploadFolder.Substring(0, n);

				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			return true;
		}
		
		public bool GetCustomerEmail(int customerID, out string customerName, out string uploadFolder, out string emailRecipient, out string emailCC, out string emailSubject, out string emailBody, out string errmsg)
		{
			customerName = "";
			uploadFolder = "";
			emailRecipient = "";
			emailCC = "";
			emailSubject = "";
			emailBody = "";
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "SELECT ISNULL(CustomerName,''),ISNULL(CustomerEmail,''),ISNULL(CustomerEmailCC,''),ISNULL(CustomerEmailSubject,''),ISNULL(CustomerEmailBody,''),ISNULL(UploadPath,'') FROM CustomerNames WHERE CustomerID="+customerID.ToString();
			command.CommandType = CommandType.Text;	
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if ( reader.Read()) 
				{	
					int f=0;
					customerName = reader.GetString(f++);
					emailRecipient = reader.GetString(f++);
					emailCC = reader.GetString(f++);
					emailSubject = reader.GetString(f++);
					emailBody = reader.GetString(f++);
					uploadFolder = reader.GetString(f++);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return false;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			return true;
		}



		public DataTable GetProcessDescriptors(int filterlocationID, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spGetProcessStates";
			command.CommandType = CommandType.StoredProcedure;	
		
			DataTable	tbl = new DataTable();

			DataColumn newColumn;
			
			newColumn = tbl.Columns.Add("ProcessID",Type.GetType("System.Int32"));
			newColumn = tbl.Columns.Add("MachineName",Type.GetType("System.String"));
			newColumn = tbl.Columns.Add("ProcessType",Type.GetType("System.Int32"));

			newColumn = tbl.Columns.Add("LocationID",Type.GetType("System.Int32"));
			newColumn = tbl.Columns.Add("ProcessName",Type.GetType("System.String"));
			newColumn = tbl.Columns.Add("Enabled",Type.GetType("System.Int32"));
			newColumn = tbl.Columns.Add("CurrentState",Type.GetType("System.Int32"));
			newColumn = tbl.Columns.Add("CurrentJob",Type.GetType("System.String"));
			newColumn = tbl.Columns.Add("LastError",Type.GetType("System.String"));
			newColumn = tbl.Columns.Add("LastEvent",Type.GetType("System.DateTime"));
			newColumn = tbl.Columns.Add("DeviceID",Type.GetType("System.Int32"));

			DataRow newRow = null;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				while ( reader.Read()) 
				{	

					int processID = reader.GetInt32(0);
					string machineName = reader.GetString(1);
					int processType = reader.GetInt32(2);
					int locationID = reader.GetInt32(3);

					if (filterlocationID == 0 || locationID == filterlocationID) 
					{

						newRow = tbl.NewRow();
						newRow["ProcessID"] = processID;
						newRow["MachineName"] = machineName;
						newRow["ProcessType"] = processType;
						newRow["LocationID"] = locationID;
						newRow["ProcessName"] = reader.GetString(4);
						newRow["Enabled"] = reader.GetInt32(5);
						newRow["CurrentState"] = reader.GetInt32(6);
						newRow["CurrentJob"] = reader.GetString(7);
						newRow["LastError"] = reader.GetString(8);
						newRow["LastEvent"] = reader.GetDateTime(9);
						newRow["DeviceID"] = reader.GetInt32(10);
						tbl.Rows.Add(newRow);
					}
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return null;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
		
			return tbl;
		}


		public DataTable GetLogEntries(int processTypeFilter, int eventFilter, bool getFlats, ref DateTime tLastDateTime,  out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spGetLog";
			command.CommandType = CommandType.StoredProcedure;	

			SqlParameter param;
			param = command.Parameters.Add("@ProcessTypeFilter", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = processTypeFilter;
			param = command.Parameters.Add("@GetFlats", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = getFlats ? 1 : 0;
			param = command.Parameters.Add("@EventFilter", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = eventFilter;
			param = command.Parameters.Add("@OldestEventTime", SqlDbType.DateTime);
			param.Direction = ParameterDirection.Input;
			param.Value = tLastDateTime;

			DataTable	tbl = new DataTable();

			DataColumn newColumn;
			
			newColumn = tbl.Columns.Add("LastEvent",Type.GetType("System.DateTime"));
			newColumn = tbl.Columns.Add("ProcessID",Type.GetType("System.Int32"));
			newColumn = tbl.Columns.Add("EventID",Type.GetType("System.Int32"));
			newColumn = tbl.Columns.Add("LastError",Type.GetType("System.String"));
			newColumn = tbl.Columns.Add("JobName",Type.GetType("System.String"));
			if (Global.databaseVersion == 2)
				newColumn = tbl.Columns.Add("Separation",Type.GetType("System.Int64"));
			else
				newColumn = tbl.Columns.Add("Separation",Type.GetType("System.Int32"));

			DataRow newRow = null;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				while ( reader.Read()) 
				{	
					newRow = tbl.NewRow();
					newRow["LastEvent"] = reader.GetDateTime(0);
					tLastDateTime = (DateTime)newRow["LastEvent"];
					newRow["ProcessID"] = reader.GetInt32(1);
					newRow["EventID"] = reader.GetInt32(2);
					string lastError =  reader.GetString(3);
					newRow["LastError"] = lastError;
					newRow["JobName"] = reader.GetString(4);

					if (lastError.IndexOf(';') != -1)
					{
						string[] ss = lastError.Split(';');
						newRow["LastError"] = ss[0];
						newRow["JobName"] = ss[1];
					}
					if (Global.databaseVersion == 2)
						newRow["Separation"] = reader.GetInt64(5);
					else
						newRow["Separation"] = reader.GetInt32(5);
					tbl.Rows.Add(newRow);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
				return null;
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
		
			return tbl;
		}

		public DataTable GetErrorStatusCollection(out string errmsg)
		{
			errmsg = "";
			DataTable	tblResults = new DataTable();
			
			DataColumn newColumn;
	
			newColumn = tblResults.Columns.Add("Status",Type.GetType("System.String"));
			newColumn = tblResults.Columns.Add("Page",Type.GetType("System.String"));
			newColumn = tblResults.Columns.Add("Location",Type.GetType("System.String"));
			newColumn = tblResults.Columns.Add("Device",Type.GetType("System.String"));
			newColumn = tblResults.Columns.Add("Comment",Type.GetType("System.String"));

			SqlCommand command = null;
			if (Global.databaseVersion == 2)
				command = new SqlCommand("SELECT P.Status, P.PublicationID, P.PubDate, P.EditionID, P.SectionID, P.PageName, P.ColorID, P.LocationID, P.DeviceID, P.LastError FROM PageTable AS P WITH (NOLOCK) WHERE (P.Status=6 OR P.Status=16 OR P.Status=26 OR P.Status=36 OR P.Status=46 OR P.Status=56 OR P.Status=66)", connection);
			else
				command = new SqlCommand("SELECT P.Status, P.PublicationID, P.PubDate, P.EditionID, P.SectionID, P.PageName, P.ColorID, P.LocationID, P.DeviceID, PE.LastError FROM PageTable AS P WITH (NOLOCK) INNER JOIN PageExtraTable PE ON P.Separation=PE.Separation WHERE (P.Status=6 OR P.Status=16 OR P.Status=26 OR P.Status=36 OR P.Status=46 OR P.Status=56 OR P.Status=66)", connection);
			command.CommandType = CommandType.Text;

			DataRow newRow = null;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				while( reader.Read()) 
				{
					newRow = tblResults.NewRow();
					
					newRow["Status"] = Globals.GetStatusName(reader.GetInt32(0),0);
					string publication = Globals.GetNameFromID("PublicationNameCache",reader.GetInt32(1));
					DateTime pubDate =  reader.GetDateTime(2);
					string edition =   Globals.GetNameFromID("EditionNameCache", reader.GetInt32(3));
					string section =  Globals.GetNameFromID("SectionNameCache", reader.GetInt32(4));
					string pageName = reader.GetString(5);
					string colorName = Globals.GetStatusName(reader.GetInt32(6),0);

					newRow["Page"] = publication +"-"+pubDate.ToString("ddMM") + "-"+edition+"-"+section+"-"+pageName+"."+ colorName;
					newRow["Location"] = Globals.GetNameFromID("LocationNameCache",reader.GetInt32(7));
					newRow["Device"] = Globals.GetNameFromID("DeviceNameCache",reader.GetInt32(8));
					newRow["Comment"] = reader.GetString(9);

					tblResults.Rows.Add(newRow);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 			
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
		
			return tblResults;
		}

		public bool UpdateDeviceEnable(int deviceID, bool enable, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spUpdateDeviceEnable";
			command.CommandType = CommandType.StoredProcedure;
			
			SqlParameter param;
			param = command.Parameters.Add("@DeviceID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = deviceID;
			
			param = command.Parameters.Add("@Enabled", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = enable ? 1 : 0;

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}



		public double GetLatestPossibleUpdate(int publicationID, out string errmsg)
		{
			double f = 0.0;
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "SELECT TOP 1 LatestPlanTime FROM PublicationLatestPlanTimes WHERE PublicationID="+publicationID;
			command.CommandType = CommandType.Text;

			SqlDataReader reader = null;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if( reader.Read()) 
				{
					f = reader.GetDouble(0);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				f = 0.0;
			}
			finally 
			{
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				

			return f;
		}

        public int GetDefaultOutputTemplate(int publicationID, int pressID, out string errmsg)
        {
            errmsg = "";
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "SELECT TOP 1 TemplateID FROM PublicationTemplates WHERE PublicationID=" + publicationID.ToString() + " AND PressID=" + pressID.ToString();
            command.CommandType = CommandType.Text;

            int templateID = 0;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    templateID = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }


            return templateID;

        }

        public bool IsPDFTemplate(int templateID, out string errmsg)
        {
            bool pdfTemplate = false;
            errmsg = "";
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "SELECT (UseTiffCopy & 0x400) FROM TemplateConfigurations WHERE TemplateID=" + templateID.ToString();
             command.CommandType = CommandType.Text;

			SqlDataReader reader = null;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if( reader.Read()) 
				{
                    pdfTemplate = (reader.GetInt32(0) & 0x01) > 0;
                }
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}				

            return pdfTemplate;
        }


        public bool GetAutoApply(int publicationID, int pressID, ref int paginationMode, ref bool onHold, 
                                ref bool insertedSections, ref bool separateRuns, ref int flatProofID, ref int priority, 
                                ref int templateID, ref int copies, ref bool pdfFileFormat, ref string ripSetup, out string errmsg)
		{
            bool autoApply = false;
            paginationMode = 0;
            separateRuns = false;
            insertedSections = false;
            onHold = true;
            flatProofID = 0;
            templateID = 0;
            priority = 50;
            copies = 1;
            pdfFileFormat = false;
            ripSetup = "";

            errmsg = "";

            int hasHold = FieldExists("PublicationTemplates", "Hold", out errmsg);
            int hasPaginationMode = FieldExists("PublicationTemplates", "PaginationMode", out errmsg);
           
			SqlCommand command = new SqlCommand();
			command.Connection = connection;

            if (hasPaginationMode > 0)
                command.CommandText = "SELECT TOP 1 MiscInt,ISNULL(Hold,0),ISNULL(PaginationMode,0),ISNULL(InsertedSections,0),ISNULL(SeparateRuns,0),ISNULL(MiscString,'0'),ISNULL(Priority,50),ISNULL(TemplateID,0),ISNULL(Copies,1),ISNULL(RipSetup,'') FROM PublicationTemplates WHERE PublicationID=" + publicationID.ToString() + " AND PressID=" + pressID.ToString();
            else if (hasHold > 0)
                command.CommandText = "SELECT TOP 1 MiscInt,ISNULL(Hold,0),ISNULL(TemplateID,0) FROM PublicationTemplates WHERE PublicationID=" + publicationID.ToString() + " AND PressID=" + pressID.ToString();
            else
                command.CommandText = "SELECT TOP 1 MiscInt,ISNULL(TemplateID,0) FROM PublicationTemplates WHERE PublicationID=" + publicationID.ToString() + " AND PressID=" + pressID.ToString();
            command.CommandType = CommandType.Text;

 
			SqlDataReader reader = null;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if( reader.Read()) 
				{
                    if (hasPaginationMode > 0)
                    {
                        autoApply = (reader.GetInt32(0) & 0x01) > 0;
                        onHold = reader.GetInt32(1) > 0;
                        paginationMode = reader.GetInt32(2);
                        insertedSections = reader.GetInt32(3) > 0;
                        int n = reader.GetInt32(4);
                        pdfFileFormat = (n & 0x02) > 0 ? true : false;
                        separateRuns = (n & 0x01) > 0 ? true : false;
                        string s = reader.GetString(5);
                        if (s == "")
                            s = "0";
                        flatProofID = Globals.TryParse(s, 0);

                        priority = reader.GetInt32(6);
                        templateID = reader.GetInt32(7);
                        copies = reader.GetInt32(8);

                        ripSetup = reader.GetString(9); 
                    }
                    else if (hasHold > 0)
                    {
                        autoApply = reader.GetInt32(0) > 0;
                        onHold = reader.GetInt32(1) > 0;
                        templateID = reader.GetInt32(2);
                    }
                    else
                    {
                        int n = reader.GetInt32(0);
                        byte b = (byte)(n & 0x01);
                        autoApply = b > 0 ? true : false;
                        b = (byte)(n & 0x02);
                        paginationMode = (int)b;
                        templateID = reader.GetInt32(1);
                    }
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				autoApply = false;
			}
			finally 
			{
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				

			return autoApply;
		}

        public bool CreatePagePlan(int publicationID, DateTime pubDate, int numberOfEditions,
            int pressID, int templateID, int proofID, bool bApproval, int nPriority,
            int numberOfSections, int numberOfPages, /*ArrayList aColors,*/
            out bool bExistingProduction, out bool bIsLockedProduction, out int productionID, bool unplannedmode, out string errmsg)
        {
            errmsg = "";
            productionID = 0;
            bExistingProduction = false;
            bIsLockedProduction = false;

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = CreatePagePlan2(publicationID, pubDate, numberOfEditions, pressID, templateID, proofID, bApproval, nPriority, numberOfSections, numberOfPages,/* aColors,*/
                                            out  bExistingProduction, out  bIsLockedProduction, out  productionID, unplannedmode, out  errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }

        public bool CreatePagePlan2(int publicationID, DateTime pubDate, int numberOfEditions,
            int pressID, int templateID, int proofID, bool bApproval, int nPriority,
            int numberOfSections, int nNumberOfPagesTotal, /*ArrayList aColors,*/
            out bool bExistingProduction, out bool bIsLockedProduction, out int nProductionID, bool unplannedmode, out string errmsg)
        {
            bool ret = false;
            errmsg = "";
            nProductionID = 0;
            bExistingProduction = false;
            bIsLockedProduction = false;


            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spImportCenterAddProduction";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;

            param = command.Parameters.Add("@NumberOfPages", SqlDbType.Int);
            param.Value = nNumberOfPagesTotal;

            param = command.Parameters.Add("@NumberOfSections", SqlDbType.Int);
            param.Value = numberOfSections;

            param = command.Parameters.Add("@NumberOfEditions", SqlDbType.Int);
            param.Value = 1;

            param = command.Parameters.Add("@PlannedApproval", SqlDbType.Int);
            param.Value = bApproval ? 0 : -1;

            param = command.Parameters.Add("@PlannedHold", SqlDbType.Int);
            param.Value = unplannedmode ? 2 : 1;

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Value = pressID;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    nProductionID = reader.GetInt32(0);
                    int nPlanMode = reader.GetInt32(1);
                    bExistingProduction = nPlanMode > 0 ? true : false;
                    bIsLockedProduction = nPlanMode > 1;
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }

        public bool GetPressRunID(int publicationID, DateTime pubDate, ref int editionID, int sectionID, int pressID, out int pressRunID, out int productionID, out string errmsg)
        {
            errmsg = "";
            pressRunID = 0;
            productionID = 0;

            string sql = "SELECT TOP 1 PressRunID, ProductionID,EditionID FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID <=0) AND (SectionID=@SectionID OR @SectionID<=0) AND (PressID=@PressID OR @PressID<=0) AND Dirty=0";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            SqlParameter param;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = editionID;

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Value = sectionID;

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Value = pressID;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    pressRunID = reader.GetInt32(0);
                    productionID = reader.GetInt32(1);
                    editionID = reader.GetInt32(2); 
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool GetPressRunID(int publicationID, DateTime pubDate, int editionID, int sectionID, int pressID, bool bCombineSections, string sComment, string sOrderNumber,
            int circulation, int circulation2, out int nPressRunID, int pressSectionNumber, int weekNumber,
            int timedFrom, int timedTo, int timedSequence, int pageFormatID, int cropMode, out string errmsg)
        {
            errmsg = "";
            nPressRunID = 0;

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = GetPressRunID2(publicationID, pubDate, editionID, sectionID, pressID, bCombineSections, sComment, sOrderNumber,
                                         circulation, circulation2, out  nPressRunID, pressSectionNumber, weekNumber,
                                         timedFrom, timedTo, timedSequence, pageFormatID, cropMode, out  errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }


        public bool GetPressRunID2(int publicationID, DateTime pubDate, int editionID, int sectionID, int pressID, bool bCombineSections, string sComment, string sOrderNumber,
            int circulation, int circulation2, out int nPressRunID, int pressSectionNumber, int weekNumber,
            int timedFrom, int timedTo, int timedSequence, int pageFormatID, int cropMode, out string errmsg)
        {
            bool ret = false;
            errmsg = "";
            nPressRunID = 0;


            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spImportCenterGetPressRun2";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;

            param = command.Parameters.Add("@IssueID", SqlDbType.Int);
            param.Value = 1;

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = editionID;

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Value = sectionID;

            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Value = pressID > 0 ? pressID : -1;

            param = command.Parameters.Add("@CombineSections", SqlDbType.Int);
            param.Value = bCombineSections ? 1 : 0;

            param = command.Parameters.Add("@KeepPress", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@SequenceNumber", SqlDbType.Int);
            param.Value = pressSectionNumber;

            param = command.Parameters.Add("@WeekNumber", SqlDbType.Int);
            param.Value = weekNumber;



            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    nPressRunID = reader.GetInt32(0);
                    ret = nPressRunID > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (ret && nPressRunID > 0)
            {
                ret = false;
                if (sOrderNumber != "")
                    command.CommandText = "UPDATE PressRunID SET Comment='" + sComment + "', OrderNumber='" + sOrderNumber + "' WHERE PressRunID=" + nPressRunID.ToString();
                else
                    command.CommandText = "UPDATE PressRunID SET Comment='" + sComment + "' WHERE PressRunID=" + nPressRunID.ToString();

                command.CommandType = CommandType.Text;
                try
                {
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    command.ExecuteNonQuery();
                    ret = true;
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;

                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }


            }


            if (ret && nPressRunID > 0)
            {
                command.CommandText = "UPDATE PressRunID SET PlanType=0, Circulation=" + circulation.ToString() +
                    ",Circulation2=" + circulation2.ToString() +
                    ",TimedEditionFrom=" + timedFrom.ToString() +
                    ",TimedEditionTo=" + timedTo.ToString() +
                    ",FromZone=" + timedSequence.ToString() +
                    ",ToZone=0,TimedEditionState=0,MiscInt1=" + pageFormatID.ToString() +
                    ",MiscInt2=" + cropMode.ToString() + " WHERE PressRunID=" + nPressRunID.ToString();

                command.CommandType = CommandType.Text;
                try
                {
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    command.ExecuteNonQuery();
                    ret = true;
                }
                catch //(Exception ex)
                {
                    // ignore error (backward comp)
                    // errmsg = ex.Message; 
                    // ret = false;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return ret;
        }




		public bool UpdatePressTime(int pressID, DateTime pressTime, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PageTable SET PressTime=@PressTime WHERE PressRunID=@PressRunID";
			command.CommandType = CommandType.Text;
			
			SqlParameter param;
			param = command.Parameters.Add("@PressTime", SqlDbType.DateTime);
			param.Direction = ParameterDirection.Input;
			param.Value = pressTime;

			param = command.Parameters.Add("@PressRunID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = pressID;

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			return true;
		}

		public bool UpdatePressComment(int pressID, string comment, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PressRunID SET Comment=@Comment WHERE PressRunID=@PressRunID";
			command.CommandType = CommandType.Text;
			
			SqlParameter param;
			param = command.Parameters.Add("@Comment", SqlDbType.VarChar,50);
			param.Direction = ParameterDirection.Input;
			param.Value = comment;

			param = command.Parameters.Add("@PressRunID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = pressID;

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			return true;
		}

		public bool UpdatePressRunOrderNumber(int pressID, string orderNumber, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE PressRunID SET OrderNumber=@OrderNumber WHERE PressRunID=@PressRunID";
			command.CommandType = CommandType.Text;
			
			SqlParameter param;
			param = command.Parameters.Add("@OrderNumber", SqlDbType.VarChar,50);
			param.Direction = ParameterDirection.Input;
			param.Value = orderNumber;

			param = command.Parameters.Add("@PressRunID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = pressID;

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			return true;
		}


		public bool UpdateProductionOrderNumber(int productionID, string orderNumber, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "UPDATE ProductionNames SET OrderNumber=@OrderNumber WHERE ProductionID=@ProductionID";
			command.CommandType = CommandType.Text;
			
			SqlParameter param;
			param = command.Parameters.Add("@OrderNumber", SqlDbType.VarChar, 50);
			param.Value = orderNumber;
			
			param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
			param.Value = productionID;

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}

        public bool UpdateMiscString3(int productionID, string miscString3, out string errmsg)
        {
            return UpdateMiscString3(productionID, 0, miscString3, out errmsg);
        }

        public bool UpdateMiscString3(int productionID, int editionID, string miscString3, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            if (editionID == 0)
                command.CommandText = "UPDATE PageTable SET MiscString3=@MiscString3 WHERE ProductionID=@ProductionID";
            else
                command.CommandText = "UPDATE PageTable SET MiscString3=@MiscString3 WHERE ProductionID=@ProductionID AND EditionID=@EditionID";
            command.CommandType = CommandType.Text;

            SqlParameter param;
            param = command.Parameters.Add("@MiscString3", SqlDbType.VarChar, 50);
            param.Value = miscString3;

            param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
            param.Value = productionID;

            if (editionID > 0)
            {
                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Value = editionID;
            }


                try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


		public bool AddAutoRetryRequest(int productionID, int type, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

            if (type == 2)
                command.CommandText = "INSERT INTO AutoRetryQueueFileCenter SELECT " + productionID.ToString() + ",0,'',GETDATE()";
            else
                command.CommandText = "INSERT INTO AutoRetryQueue SELECT " + productionID.ToString() + ",0,'',GETDATE()";
			command.CommandType = CommandType.Text;
			

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}


		public bool AddPlanExportJob(int productionID, int pressrunID, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "INSERT INTO PlanExportJobs (ProductionID, PressRunID,EventTime,MiscInt,MiscString) VALUES (@ProductionID,@PressRunID, GETDATE(),0,'')";
			command.CommandType = CommandType.Text;
			
			SqlParameter param;
			param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = productionID;
			
			param = command.Parameters.Add("@PressRunID", SqlDbType.Int);
			param.Direction = ParameterDirection.Input;
			param.Value = pressrunID;

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;

		}

         public bool PreImport(int publicationID, DateTime pubDate, int pressID, int planType, out string errmsg)
        {
            errmsg = "";

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = PreImport2(publicationID, pubDate, pressID, planType, out errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }

         public bool PreImport2(int publicationID, DateTime pubDate, int pressID, int planType, out string errmsg)
         {
             errmsg = "";

             SqlCommand command = new SqlCommand();
             command.Connection = connection;

             command.CommandText = "spImportCenterPreImportCustom";
             command.CommandType = CommandType.StoredProcedure;

             SqlParameter param;
             param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
             param.Direction = ParameterDirection.Input;
             param.Value = publicationID;

             param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
             param.Direction = ParameterDirection.Input;
             param.Value = pubDate;

             param = command.Parameters.Add("@PressID", SqlDbType.Int);
             param.Direction = ParameterDirection.Input;
             param.Value = pressID;

             param = command.Parameters.Add("@PlanType", SqlDbType.Int);
             param.Direction = ParameterDirection.Input;
             param.Value = planType;

             try
             {
                 if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                     connection.Open();
                 command.ExecuteNonQuery();
             }
             catch (Exception ex)
             {
                 errmsg = ex.Message;
                 Global.logging.WriteLog(errmsg);
                 return false;
             }
             finally
             {
                 if (connection.State == ConnectionState.Open)
                     connection.Close();
             }
             return true;

         }


        public bool PostImport(int pressrunID, out string errmsg)
        {
            errmsg = "";

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = PostImport2(pressrunID, out errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }

       


        public bool PostImport2(int pressrunID, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spImportCenterPressRunCustom";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@PressRunID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = pressrunID;

            param = command.Parameters.Add("@PressSpecificPages", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = (bool)HttpContext.Current.Application["PlanPressSpecificPages"] ? 1 : 0;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog(errmsg);
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;

        }


        public bool PostImportProduction(int productionID, out string errmsg)
        {
            errmsg = "";

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = PostImportProduction2(productionID, out errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }

        public bool PostImportProduction2(int productionID, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spImportCenterProductionCustom";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = productionID;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog(errmsg);
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;

        }

        public bool AutoApplyProduction(int productionID, int paginationMode, bool insertedSections, bool separateRuns, int flatProofID, int templateID, int internalmode, out string errmsg)
        {
            errmsg = "";

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = AutoApplyProduction2(productionID, paginationMode, insertedSections, separateRuns, flatProofID, templateID, internalmode, out errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }

        public bool AutoApplyProduction2(int productionID, int paginationMode, bool insertedSections, bool separateRuns, int flatProofID, int templateID, int internalmode, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spApplyProduction";
            if (internalmode == 2)
                command.CommandText = "spApplyProduction2";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = productionID;

            param = command.Parameters.Add("@InsertedSections", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = insertedSections ? 1 : 0;

            param = command.Parameters.Add("@TemplateID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = templateID;

            param = command.Parameters.Add("@DeviceID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = 0;

            param = command.Parameters.Add("@MarkGroupIDList", SqlDbType.VarChar, 50);
            param.Direction = ParameterDirection.Input;
            param.Value = "";

            param = command.Parameters.Add("@Creep", SqlDbType.Float);
            param.Direction = ParameterDirection.Input;
            param.Value = 0.0;

            param = command.Parameters.Add("@FlatProofTemplateID", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = flatProofID;

            param = command.Parameters.Add("@PaginationMode", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = paginationMode;

            param = command.Parameters.Add("@SeparateRuns", SqlDbType.Int);
            param.Direction = ParameterDirection.Input;
            param.Value = separateRuns ? 1 : 0;


            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog(errmsg);
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;

        }



        public bool InsertSeparation(PageTableEntry item, int nInCopySeparationSet, out int nCopySeparationSet, int nInCopyFlatSeparationSet,
            out int nCopyFlatSeparationSet, int nNumberOfColors, bool bFirstPagePosition, int nCopies,
            bool keepExistingColors, bool keepApproval, bool keepUnique, out string errmsg)
        {
            errmsg = "";
            nCopySeparationSet = 0;
            nCopyFlatSeparationSet = 0;

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = InsertSeparation2(item, nInCopySeparationSet, out   nCopySeparationSet, nInCopyFlatSeparationSet,
                                            out   nCopyFlatSeparationSet, nNumberOfColors, bFirstPagePosition, nCopies,
                                            keepExistingColors, keepApproval, keepUnique, out  errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }

        public bool InsertSeparation2(PageTableEntry item, int nInCopySeparationSet, out int nCopySeparationSet, int nInCopyFlatSeparationSet,
            out int nCopyFlatSeparationSet, int nNumberOfColors, bool bFirstPagePosition, int nCopies,
            bool keepExistingColors, bool keepApproval, bool keepUnique, out string errmsg)
        {
            bool ret = false;
            bool bUpdated = false;
            errmsg = "";
            nCopySeparationSet = 0;
            nCopyFlatSeparationSet = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spImportCenterAddSeparation2";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@PageCountChanged", SqlDbType.Int);
            param.Value = item.m_pagecountchange ? 1 : 0;

            param = command.Parameters.Add("@KeepColors", SqlDbType.Int);
            param.Value = keepExistingColors == true ? 1 : 0;

            param = command.Parameters.Add("@KeepApproval", SqlDbType.Int);
            param.Value = keepApproval == true ? 1 : 0;

            param = command.Parameters.Add("@KeepUnique", SqlDbType.Int);
            param.Value = keepUnique == true ? 1 : 0;

            param = command.Parameters.Add("@UseMainLocation", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@FirstPagePosition", SqlDbType.Int);
            param.Value = bFirstPagePosition ? 1 : 0;

            param = command.Parameters.Add("@CopyFlatSeparationSet", SqlDbType.Int);
            param.Value = nInCopyFlatSeparationSet;

            param = command.Parameters.Add("@NumberOfColors", SqlDbType.Int);
            param.Value = nNumberOfColors;

            param = command.Parameters.Add("@Copies", SqlDbType.Int);
            param.Value = nCopies;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = item.m_publicationID;

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Value = item.m_sectionID;

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = item.m_editionID;

            param = command.Parameters.Add("@IssueID", SqlDbType.Int);
            param.Value = item.m_issueID;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = item.m_pubdate;

            param = command.Parameters.Add("@PageName", SqlDbType.VarChar, 100);
            param.Value = item.m_pagename;

            param = command.Parameters.Add("@ColorID", SqlDbType.Int);
            param.Value = item.m_colorID;

            param = command.Parameters.Add("@TemplateID", SqlDbType.Int);
            param.Value = item.m_templateID;

            param = command.Parameters.Add("@ProofID", SqlDbType.Int);
            param.Value = item.m_proofID;

            param = command.Parameters.Add("@DeviceID", SqlDbType.Int);
            param.Value = item.m_deviceID;

            param = command.Parameters.Add("@Version", SqlDbType.Int);
            param.Value = item.m_version;

            param = command.Parameters.Add("@Pagination", SqlDbType.Int);
            param.Value = item.m_pagination;

            param = command.Parameters.Add("@Approval", SqlDbType.Int);
            param.Value = item.m_approved;

            param = command.Parameters.Add("@Hold", SqlDbType.Int);
            param.Value = item.m_hold;

            param = command.Parameters.Add("@Active", SqlDbType.Int);
            param.Value = item.m_active;

            param = command.Parameters.Add("@Priority", SqlDbType.Int);
            param.Value = item.m_priority;

            param = command.Parameters.Add("@PagePositions", SqlDbType.VarChar, 50);
            param.Value = item.m_pagepositions;

            param = command.Parameters.Add("@PageType", SqlDbType.Int);
            param.Value = item.m_pagetype;

            param = command.Parameters.Add("@PagesOnPlate", SqlDbType.Int);
            param.Value = item.m_pagesonplate;

            param = command.Parameters.Add("@SheetNumber", SqlDbType.Int);
            param.Value = item.m_sheetnumber;

            param = command.Parameters.Add("@SheetSide", SqlDbType.Int);
            param.Value = item.m_sheetside;

            param = command.Parameters.Add("@PressId", SqlDbType.Int);
            param.Value = item.m_pressID;

            param = command.Parameters.Add("@PressSectionNumber", SqlDbType.Int);
            param.Value = item.m_presssectionnumber;

            param = command.Parameters.Add("@SortingPosition", SqlDbType.VarChar, 4);
            param.Value = item.m_sortingposition;

            param = command.Parameters.Add("@PressTower", SqlDbType.VarChar, 8);
            param.Value = item.m_presstower;

            param = command.Parameters.Add("@PressZone", SqlDbType.VarChar, 8);
            param.Value = item.m_presszone;

            param = command.Parameters.Add("@PressCylinder", SqlDbType.VarChar, 8);
            param.Value = item.m_presscylinder;

            param = command.Parameters.Add("@PressHighlow", SqlDbType.VarChar, 8);
            param.Value = item.m_presshighlow;

            param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
            param.Value = item.m_productionID;

            param = command.Parameters.Add("@PressrunID", SqlDbType.Int);
            param.Value = item.m_pressrunID;

            param = command.Parameters.Add("@PlanPageName", SqlDbType.VarChar, 50);
            param.Value = item.m_planpagename;

            param = command.Parameters.Add("@IssueSequenceNumber", SqlDbType.Int);
            param.Value = item.m_issuesequencenumber;

            param = command.Parameters.Add("@UniquePage", SqlDbType.Int);
            param.Value = item.m_uniquepage;

            param = command.Parameters.Add("@LocationID", SqlDbType.Int);
            param.Value = item.m_locationID;

            param = command.Parameters.Add("@FlatProofID", SqlDbType.Int);
            param.Value = item.m_flatproofID;

            param = command.Parameters.Add("@Creep", SqlDbType.Float);
            param.Value = (float)item.m_creep;

            param = command.Parameters.Add("@MarkGroups", SqlDbType.VarChar, 50);
            param.Value = item.m_markgroups;

            param = command.Parameters.Add("@PageIndex", SqlDbType.Int);
            param.Value = item.m_pageindex;

            param = command.Parameters.Add("@HardProofID", SqlDbType.Int);
            param.Value = item.m_hardproofID;

            param = command.Parameters.Add("@DeadLine", SqlDbType.DateTime);
            param.Value = item.m_deadline;

            param = command.Parameters.Add("@Comment", SqlDbType.VarChar, 50);
            param.Value = item.m_comment;

            param = command.Parameters.Add("@MasterEditionID", SqlDbType.Int);
            param.Value = item.m_mastereditionID;

            param = command.Parameters.Add("@MasterLocationID", SqlDbType.Int);
            param.Value = item.m_locationID;

            param = command.Parameters.Add("@ColorIndex", SqlDbType.Int);
            param.Value = item.m_colorIndex;

            param = command.Parameters.Add("@CopiesToProduce", SqlDbType.Int);
            param.Value = 1;

            param = command.Parameters.Add("@OutputPriority", SqlDbType.Int);
            param.Value = item.m_priority;

            param = command.Parameters.Add("@PressChange", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@PressTime", SqlDbType.DateTime);
            param.Value = item.m_pubdate;										// Is set later..

            DataRow pubRow = Globals.GetPublicationRow(item.m_publicationID);
            int customerID = 0;
            try
            {
                customerID = (int)pubRow["CustomerID"];
            }
            catch
            { }

            param = command.Parameters.Add("@CustomerID", SqlDbType.Int);
            param.Value = customerID;

            param = command.Parameters.Add("@Miscint1", SqlDbType.Int);
            param.Value = 0;
            param = command.Parameters.Add("@Miscint2", SqlDbType.Int);
            param.Value = 0;
            param = command.Parameters.Add("@Miscint3", SqlDbType.Int);
            param.Value = 0;
            param = command.Parameters.Add("@Miscint4", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@Miscstring1", SqlDbType.VarChar, 50);
            param.Value = "";
            param = command.Parameters.Add("@Miscstring2", SqlDbType.VarChar, 50);
            param.Value = ""; 
            param = command.Parameters.Add("@Miscstring3", SqlDbType.VarChar, 50);
            param.Value = "";

            param = command.Parameters.Add("@Miscdate", SqlDbType.DateTime);
            param.Value = new DateTime(1975, 1, 1);



            param = command.Parameters.Add("@PageFormatID", SqlDbType.Int);
            param.Value = item.m_pageFormatID;

            param = command.Parameters.Add("@RipSetupID", SqlDbType.Int);
            param.Value = item.m_ripSetupID;

            param = command.Parameters.Add("@FanoutID", SqlDbType.Int);
            param.Value = 0;
            param = command.Parameters.Add("@PageCategoryID", SqlDbType.Int);
            param.Value = 0;
            param = command.Parameters.Add("@DeviceGroupID", SqlDbType.Int);
            param.Value = 0;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    nCopySeparationSet = reader.GetInt32(0);

                    nCopyFlatSeparationSet = reader.GetInt32(1);

                    bUpdated = reader.GetInt32(2) > 0 ? true : false;
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                nCopySeparationSet = 0;
                nCopyFlatSeparationSet = 0;

                errmsg = ex.Message;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }






        public bool InsertSeparationExtended(PageTableEntry item, int nInCopySeparationSet, out int nCopySeparationSet, int nInCopyFlatSeparationSet,
            out int nCopyFlatSeparationSet, int nNumberOfColors, bool bFirstPagePosition, int nCopies,
            bool keepExistingColors, bool keepApproval, bool keepUnique, out string errmsg)
        {
            errmsg = "";
            nCopySeparationSet = 0;
            nCopyFlatSeparationSet = 0;

            int retries = Global.queryRetries;
            bool success = false;
            while (retries-- > 0 && success == false)
            {
                success = InsertSeparationExtended2(item, nInCopySeparationSet, out   nCopySeparationSet, nInCopyFlatSeparationSet,
                                            out   nCopyFlatSeparationSet, nNumberOfColors, bFirstPagePosition, nCopies,
                                            keepExistingColors, keepApproval, keepUnique, out  errmsg);
                if (success)
                    break;
                System.Threading.Thread.Sleep(Global.queryBackoffTime);
            }

            return success;
        }

        public bool InsertSeparationExtended2(PageTableEntry item, int nInCopySeparationSet, out int nCopySeparationSet, int nInCopyFlatSeparationSet,
            out int nCopyFlatSeparationSet, int nNumberOfColors, bool bFirstPagePosition, int nCopies,
            bool keepExistingColors, bool keepApproval, bool keepUnique, out string errmsg)
        {
            bool ret = false;
            bool bUpdated = false;
            errmsg = "";
            nCopySeparationSet = 0;
            nCopyFlatSeparationSet = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spImportCenterAddSeparation2";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@PageCountChanged", SqlDbType.Int);
            param.Value = item.m_pagecountchange ? 1 : 0;

            param = command.Parameters.Add("@KeepColors", SqlDbType.Int);
            param.Value = keepExistingColors == true ? 1 : 0;

            param = command.Parameters.Add("@KeepApproval", SqlDbType.Int);
            param.Value = keepApproval == true ? 1 : 0;

            param = command.Parameters.Add("@KeepUnique", SqlDbType.Int);
            param.Value = keepUnique == true ? 1 : 0;

            param = command.Parameters.Add("@UseMainLocation", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@FirstPagePosition", SqlDbType.Int);
            param.Value = bFirstPagePosition ? 1 : 0;

            param = command.Parameters.Add("@CopyFlatSeparationSet", SqlDbType.Int);
            param.Value = nInCopyFlatSeparationSet;

            param = command.Parameters.Add("@NumberOfColors", SqlDbType.Int);
            param.Value = nNumberOfColors;

            param = command.Parameters.Add("@Copies", SqlDbType.Int);
            param.Value = nCopies;

            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = item.m_publicationID;

            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Value = item.m_sectionID;

            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = item.m_editionID;

            param = command.Parameters.Add("@IssueID", SqlDbType.Int);
            param.Value = item.m_issueID;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = item.m_pubdate;

            param = command.Parameters.Add("@PageName", SqlDbType.VarChar, 100);
            param.Value = item.m_pagename;

            param = command.Parameters.Add("@ColorID", SqlDbType.Int);
            param.Value = item.m_colorID;

            param = command.Parameters.Add("@TemplateID", SqlDbType.Int);
            param.Value = item.m_templateID;

            param = command.Parameters.Add("@ProofID", SqlDbType.Int);
            param.Value = item.m_proofID;

            param = command.Parameters.Add("@DeviceID", SqlDbType.Int);
            param.Value = item.m_deviceID;

            param = command.Parameters.Add("@Version", SqlDbType.Int);
            param.Value = item.m_version;

            param = command.Parameters.Add("@Pagination", SqlDbType.Int);
            param.Value = item.m_pagination;

            param = command.Parameters.Add("@Approval", SqlDbType.Int);
            param.Value = item.m_approved;

            param = command.Parameters.Add("@Hold", SqlDbType.Int);
            param.Value = item.m_hold;

            param = command.Parameters.Add("@Active", SqlDbType.Int);
            param.Value = item.m_active;

            param = command.Parameters.Add("@Priority", SqlDbType.Int);
            param.Value = item.m_priority;

            param = command.Parameters.Add("@PagePositions", SqlDbType.VarChar, 50);
            param.Value = item.m_pagepositions;

            param = command.Parameters.Add("@PageType", SqlDbType.Int);
            param.Value = item.m_pagetype;

            param = command.Parameters.Add("@PagesOnPlate", SqlDbType.Int);
            param.Value = item.m_pagesonplate;

            param = command.Parameters.Add("@SheetNumber", SqlDbType.Int);
            param.Value = item.m_sheetnumber;

            param = command.Parameters.Add("@SheetSide", SqlDbType.Int);
            param.Value = item.m_sheetside;

            param = command.Parameters.Add("@PressId", SqlDbType.Int);
            param.Value = item.m_pressID;

            param = command.Parameters.Add("@PressSectionNumber", SqlDbType.Int);
            param.Value = item.m_presssectionnumber;

            param = command.Parameters.Add("@SortingPosition", SqlDbType.VarChar, 16);
            param.Value = item.m_sortingposition;

            param = command.Parameters.Add("@PressTower", SqlDbType.VarChar, 8);
            param.Value = item.m_presstower;

            param = command.Parameters.Add("@PressZone", SqlDbType.VarChar, 8);
            param.Value = item.m_presszone;

            param = command.Parameters.Add("@PressCylinder", SqlDbType.VarChar, 8);
            param.Value = item.m_presscylinder;

            param = command.Parameters.Add("@PressHighlow", SqlDbType.VarChar, 8);
            param.Value = item.m_presshighlow;

            param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
            param.Value = item.m_productionID;

            param = command.Parameters.Add("@PressrunID", SqlDbType.Int);
            param.Value = item.m_pressrunID;

            param = command.Parameters.Add("@PlanPageName", SqlDbType.VarChar, 50);
            param.Value = item.m_planpagename;

            param = command.Parameters.Add("@IssueSequenceNumber", SqlDbType.Int);
            param.Value = item.m_issuesequencenumber;

            param = command.Parameters.Add("@UniquePage", SqlDbType.Int);
            param.Value = item.m_uniquepage;

            param = command.Parameters.Add("@LocationID", SqlDbType.Int);
            param.Value = item.m_locationID;

            param = command.Parameters.Add("@FlatProofID", SqlDbType.Int);
            param.Value = item.m_flatproofID;

            param = command.Parameters.Add("@Creep", SqlDbType.Float);
            param.Value = (float)item.m_creep;

            param = command.Parameters.Add("@MarkGroups", SqlDbType.VarChar, 50);
            param.Value = item.m_markgroups;

            param = command.Parameters.Add("@PageIndex", SqlDbType.Int);
            param.Value = item.m_pageindex;

            param = command.Parameters.Add("@HardProofID", SqlDbType.Int);
            param.Value = item.m_hardproofID;

            param = command.Parameters.Add("@DeadLine", SqlDbType.DateTime);
            param.Value = item.m_deadline;

            param = command.Parameters.Add("@Comment", SqlDbType.VarChar, 50);
            param.Value = item.m_comment;

            param = command.Parameters.Add("@MasterEditionID", SqlDbType.Int);
            param.Value = item.m_mastereditionID;

            param = command.Parameters.Add("@MasterLocationID", SqlDbType.Int);
            param.Value = item.m_locationID;

            param = command.Parameters.Add("@ColorIndex", SqlDbType.Int);
  //          param.Value = item.m_colorIndex;
            param.Value = item.m_colorID;

            param = command.Parameters.Add("@CopiesToProduce", SqlDbType.Int);
            param.Value = nCopies;

            param = command.Parameters.Add("@OutputPriority", SqlDbType.Int);
            param.Value = item.m_priority;

            param = command.Parameters.Add("@PressChange", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@PressTime", SqlDbType.DateTime);
            param.Value = item.m_pubdate;										// Is set later..

            param = command.Parameters.Add("@CustomerID", SqlDbType.Int);
            param.Value = item.m_customerID;

            param = command.Parameters.Add("@Miscint1", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@Miscint2", SqlDbType.Int);
            param.Value = item.m_weeknumber;

            param = command.Parameters.Add("@Miscint3", SqlDbType.Int);
            param.Value = 0;
            param = command.Parameters.Add("@Miscint4", SqlDbType.Int);
            param.Value = 0;

            param = command.Parameters.Add("@Miscstring1", SqlDbType.VarChar, 50);
            param.Value = string.Format("{0:00}",2*(item.m_sheetnumber-1)+item.m_sheetside+1);
            
            param = command.Parameters.Add("@Miscstring2", SqlDbType.VarChar, 50);
            param.Value = "";

            param = command.Parameters.Add("@Miscstring3", SqlDbType.VarChar, 50);
            param.Value = "";

            param = command.Parameters.Add("@Miscdate", SqlDbType.DateTime);
            param.Value = DateTime.Now;									// Is set later..


            param = command.Parameters.Add("@PageFormatID", SqlDbType.Int);
            param.Value = item.m_pageFormatID;

            param = command.Parameters.Add("@RipSetupID", SqlDbType.Int);
            param.Value = item.m_ripSetupID;

            param = command.Parameters.Add("@FanoutID", SqlDbType.Int);
            param.Value = 0;
            param = command.Parameters.Add("@PageCategoryID", SqlDbType.Int);
            param.Value = 0;
            param = command.Parameters.Add("@DeviceGroupID", SqlDbType.Int);
            param.Value = 0;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    nCopySeparationSet = reader.GetInt32(0);
                    nCopyFlatSeparationSet = reader.GetInt32(1);

                    bUpdated = reader.GetInt32(2) > 0 ? true : false;
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                nCopySeparationSet = 0;
                nCopyFlatSeparationSet = 0;

                errmsg = ex.Message;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }

















		private int GetNextID(string tableName, out string errmsg)
        {
			int nID = 0;
			errmsg = "";

			string idColumnName = tableName.Substring(0, tableName.Length-5) + "ID";
			SqlCommand command = new SqlCommand();
			command.Connection = connection;


			command.CommandText = "SELECT ISNULL(MAX("+idColumnName+"),0) FROM "+tableName;
			command.CommandType = CommandType.Text;

			SqlDataReader reader = null;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if( reader.Read()) 
					nID = reader.GetInt32(0);
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
			}
			finally 
			{
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}			

			return nID + 1;
		}


		public bool InsertPublicationName(string pubName, int nPageFormatID, bool bTrimToFormat, double latestHour, int nProoferID, int nHardProoferID, bool bMustApprove, out string errmsg)
		{
			errmsg = "";

			int nPubID = GetNextID("PublicationNames", out errmsg);
			if (errmsg != "") 
				return false;

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			int nApprove = bMustApprove ? 0 : -1;
			int nTrim = bTrimToFormat == true ? 1 : 0;
			command.CommandText = "INSERT INTO PublicationNames (PublicationID, Name,PageFormatID,TrimToFormat,LatestHour,DefaultProofID,DefaultHardProofID,DefaultApprove) VALUES ("
				+nPubID.ToString()+
				",'"+pubName+"',"+
				nPageFormatID.ToString()+","+
				nTrim.ToString()+","+
				latestHour.ToString()+","+
				nProoferID.ToString()+","+
				nHardProoferID.ToString()+","+
				nApprove.ToString()+")";

			command.CommandType = CommandType.Text;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			AdjustChangeNotification("PublicationNames", out errmsg);
			return true;
		}

		public bool InsertPageformatName(string pageformatName, double pageformatWidth, double pageformatHeight, double pageformatBleed, out string errmsg)
		{
			errmsg = "";

			int nPageformatID = GetNextID("PageFormatNames", out errmsg);
			if (errmsg != "") 
				return false;

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "INSERT INTO PageFormatNames (PageFormatID,PageFormatName, Width, Height,Bleed) VALUES ("
				+nPageformatID.ToString()+
				",'"+pageformatName+"',"+
				pageformatWidth.ToString()+","+
				pageformatHeight.ToString()+","+
				pageformatBleed.ToString()+")";

			command.CommandType = CommandType.Text;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}

			AdjustChangeNotification("PageFormatNames", out errmsg);
			return true;
		}

		private bool AdjustChangeNotification(string tableName, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "UPDATE ChangeNotification SET ChangeID = ChangeID+1 WHERE TableName='"+tableName+"'";
			command.CommandType = CommandType.Text;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
			return true;
		}

		public bool PlanLock (string userName, int bRequestedPlanLock, ref int bCurrentPlanLock, ref string sClientName, ref DateTime tClientTime, out string errmsg)
		{
			errmsg = "";
			sClientName = "";
			tClientTime = DateTime.MinValue;
			bCurrentPlanLock = -1;

			string myClientName = "WEBCENTER-"+userName; 			

			SqlCommand command = new SqlCommand();
			command.Connection = connection;

			command.CommandText = "spPlanningLock";
			command.CommandType = CommandType.StoredProcedure;

			SqlParameter param;
			param = command.Parameters.Add("@PlanLock", SqlDbType.Int);	
			param.Value = bRequestedPlanLock;

			param = command.Parameters.Add("@ClientName", SqlDbType.VarChar, 50);
			param.Value = myClientName;

			SqlDataReader reader = null;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if( reader.Read()) 
				{
					bCurrentPlanLock = reader.GetInt32(0);
					sClientName = reader.GetString(1);
					tClientTime = reader.GetDateTime(2);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}			

			return true;

		}

        public DataTable GetProductionList(bool allplans, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();

            string sql = "SELECT DISTINCT P.PublicationID, PN.Name AS ProdName, PN.ProductionID,P.PubDate FROM PageTable AS P WITH (NOLOCK) INNER JOIN ProductionNames AS PN ON PN.ProductionID=P.ProductionID WHERE P.Dirty=0 AND PN.PlanType=0 ORDER BY P.PubDate, ProdName";
            if (allplans)
                sql = "SELECT DISTINCT P.PublicationID, PN.Name AS ProdName, PN.ProductionID,P.PubDate FROM PageTable AS P WITH (NOLOCK) INNER JOIN ProductionNames AS PN ON PN.ProductionID=P.ProductionID WHERE P.Dirty=0 ORDER BY P.PubDate, ProdName";
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            DataTable tblResults = new DataTable();

            DataColumn newColumn;

            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));

            DataRow newRow = null;
            SqlDataReader reader = null;

            string pubsallowed = (string)HttpContext.Current.Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string thisPublication = Globals.GetNameFromID("PublicationNameCache", reader.GetInt32(0));
                    if (pubsallowed != "*")
                    {
                        bool found = false;
                        foreach (string sp in publist)
                        {
                            if (sp == thisPublication)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                            continue;
                    }

                    newRow = tblResults.NewRow();
                    newRow["Name"] = reader.GetString(1);
                    newRow["ID"] = reader.GetInt32(2);
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblResults;
        }

        public bool DeleteProduction(int productionID, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "spImportCenterCleanProduction";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
            param.Value = productionID;
            param = command.Parameters.Add("@IgnoreDirtyFlag", SqlDbType.Int);
            param.Value = 1;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool DeleteProductionPages(int productionID, int publicationID, DateTime pubDate, out string errmsg)
        {
            errmsg = "";
            DateTime orgPubDate = pubDate;
            pubDate = pubDate.AddYears(100);
            if (ChangePubDate(productionID, pubDate, out errmsg) == false)
                return false;

            if (SetProductionDirtyFlag(productionID, out errmsg) == false)
            {
                string errmsg2 = "";
                ChangePubDate(productionID, orgPubDate, out errmsg2);
                return false;
            }

            string sql = "INSERT INTO ProductDeleteQueue SELECT @ProductionID,@PublicationID,@PubDate,0,0,0,0,GETDATE(),'',''";

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;
            SqlParameter param;
            param = command.Parameters.Add("@ProductionID", SqlDbType.Int);
            param.Value = productionID;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;
            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }
		 

		private bool GetStorageFolder(ref string storageFolder, ref string previewFolder, ref string thumbnailFolder, ref string storageFolderUsername, ref string storageFolderPassword, out string errmsg)
		{
			errmsg = "";

			storageFolder = "";
			previewFolder = "";
			thumbnailFolder = "";
			storageFolderUsername = "";
			storageFolderPassword = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;


			command.CommandText = "SELECT ServerFilePath, ServerPreviewPath, ServerThumbnailPath, ServerUseCurrentUser, ServerUserName, ServerPassword FROM GeneralPreferences"; 
			command.CommandType = CommandType.Text;

			SqlDataReader reader = null;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				if( reader.Read()) 
				{
					storageFolder = reader.GetString(0);
					previewFolder = reader.GetString(1);
					thumbnailFolder = reader.GetString(2);
					storageFolderUsername = reader.GetString(3);
					storageFolderPassword = reader.GetString(4);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}			

			return true;
		}

		public bool MarkMessageRead(int messageID, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "UPDATE Messages SET IsRead=1,Severity=0 WHERE MessageID="+messageID.ToString() ;
			command.CommandType = CommandType.Text;
		
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
				finally 
			{
				if (connection.State == ConnectionState.Open)
				connection.Close();
			}
				
			return true;
		}

		public int GetNextMessageID(out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "SELECT ISNULL(MAX(MessageID)+1,1) FROM  Messages WITH (NOLOCK)" ;
			command.CommandType = CommandType.Text;

			int id = 1;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();

				id = (int)command.ExecuteScalar();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return -1;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				
			return id;
		}


		public int HasUnreadMessages(string userName, int publicationID, DateTime pubDate, out string errmsg)
		{
			errmsg = "";

			//int numberOfMessages = 0;
			bool hasUnread = false;
			bool hasSevere = false;

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "spGetMessages";
			command.CommandType = CommandType.StoredProcedure;

		
			SqlParameter param;
			param = command.Parameters.Add("@UserName", SqlDbType.VarChar, 50);	
			param.Value = userName;
			param = command.Parameters.Add("@PublicationID", SqlDbType.Int);	
			param.Value = publicationID;			
			param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);	
			param.Value = pubDate;	
			param = command.Parameters.Add("@UnreadOnly", SqlDbType.Int);	
			param.Value = 1;	

			SqlDataReader reader = null;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				while( reader.Read()) 
				{

					int n = reader.GetInt32(2);
					if ((n & 0x01) > 0)
						hasUnread = true;
					if ((n & 0x02) > 0)
						hasSevere = true;
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return -1;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				
			return hasUnread && hasSevere ? 3 : (hasSevere ? 2 : (hasUnread ? 1 : 0));
		}

		public bool UpdateMessage(int messageID, string sender, string receiver, string title, 
									string message, bool severe, DateTime pubDate, int publicationID, out string errmsg)
		{
			errmsg = "";
			bool isNew = false;

			if (messageID <= 0)
			{
				isNew = true;
				messageID = GetNextMessageID(out errmsg);
				if (messageID <= 0)
					return false;
			}

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			int messageType = 0;

			if (isNew)
			{
				command.CommandText = "spAddMessage";	
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.Add("@Sender", SqlDbType.VarChar,50).Value = sender;
				command.Parameters.Add("@Receiver", SqlDbType.VarChar,50).Value = receiver;
				command.Parameters.Add("@Severity", SqlDbType.Int).Value = severe;
				command.Parameters.Add("@MessageType", SqlDbType.Int).Value = messageType;
				command.Parameters.Add("@Message", SqlDbType.VarChar,8000).Value = message;
				command.Parameters.Add("@Title", SqlDbType.VarChar,256).Value = title;				
				command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
				command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
			}
			else
			{
				command.CommandText = string.Format("UPDATE Messages SET IsRead=0, Severity={0}, Sender='{1}', Receiver='{2}', Message='{3}', EventTime=GETDATE(),Title='{4}' WHERE MessageID={5}",(severe ? 1 : 0), sender, receiver, message, title, messageID);
				command.CommandType = CommandType.Text;
			}

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				
			return true;
		}

		public bool DeleteMessage(int messageID, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandType = CommandType.Text;
			command.CommandText = string.Format("DELETE Messages WHERE MessageID={0}", messageID);
		
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return false;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				
			return true;
		}


		public int TableExists(string tableName, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "sp_tables";
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Add("@table_name", SqlDbType.VarChar,50).Value = tableName;

			SqlDataReader reader = null;

			bool found = false;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();

				reader = command.ExecuteReader();
				if( reader.Read()) 
				{
					found = true;
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return -1;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				
			return found ? 1 : 0;
		}

		public int FieldExists(string tableName, string fieldName, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "sp_columns";
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Add("@table_name", SqlDbType.VarChar,50).Value = tableName;

			SqlDataReader reader = null;
			bool found = false;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();

				reader = command.ExecuteReader();
				while( reader.Read()) 
				{
					string columnName = reader.GetString(3).ToUpper();

					if (columnName == fieldName.ToUpper())
						found = true;

				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return -1;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				
			return found ? 1 : 0;
		}

		public int StoredProcedureExists(string sSPname, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "sp_stored_procedures";
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Add("@sp_name", SqlDbType.VarChar,50).Value = sSPname;

			SqlDataReader reader = null;

			bool found = false;
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State ==ConnectionState.Broken)
					connection.Open();

				reader = command.ExecuteReader();
				if( reader.Read()) 
				{
					found = true;
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return -1;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				
			return found ? 1 : 0;
		}

		public int StoredProcParameterExists(string sSPname, string sParameterName, out string errmsg)
		{
			errmsg = "";

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "sp_sproc_columns";
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Add("@procedure_name", SqlDbType.VarChar,50).Value = sSPname;

			SqlDataReader reader = null;
			bool found = false;
			
			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();

				reader = command.ExecuteReader();
				while( reader.Read()) 
				{
					string columnName = reader.GetString(3).ToUpper();

					if (columnName == sParameterName.ToUpper())
						found = true;

				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				return -1;
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
				
			return found ? 1 : 0;
		}



		public DataTable GetPublicationNamingConvensionCollection(out string errmsg)
		{
			errmsg = "";
			DataTable	tblResults = new DataTable();
			//DataSet ds = new DataSet();

			
			if (TableExists("WebCenterNamingConvensions", out errmsg) != 1)
				return tblResults;

			DataColumn newColumn;
			newColumn = tblResults.Columns.Add("ID",Type.GetType("System.Int32"));
			newColumn = tblResults.Columns.Add("Name",Type.GetType("System.String"));

			SqlCommand command = new SqlCommand("SELECT PublicationID,NamingConvension FROM WebCenterNamingConvensions", connection);
			command.CommandType = CommandType.Text;

			DataRow newRow = null;
			SqlDataReader reader = null;
			try 
			{
				// Execute the command
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				reader = command.ExecuteReader();
				while( reader.Read()) 
				{
					newRow = tblResults.NewRow();
					newRow["ID"] = reader.GetInt32(0);
					newRow["Name"] = reader.GetString(1);
					tblResults.Rows.Add(newRow);
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 		
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
		//	ds.Tables.Add(tblResults);
			return tblResults;
		}


		public int GetMasterCopySeparationSet(int publicationID, DateTime pubDate, int editionID, int sectionID, string pageName, out string errmsg)
		{
			int masterCopySeparationSet = 0;

			errmsg = "";


			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandType = CommandType.Text;

			command.CommandText = "SELECT TOP 1 mastercopyseparationset FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND EditionID=@EditionID AND SectionID=@SectionID AND PageName=@PageName";
			command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
			command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;	
			command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
			command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;
			command.Parameters.Add("@PageName", SqlDbType.VarChar, 50).Value = pageName;

			try
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();
				masterCopySeparationSet = (int)command.ExecuteScalar();
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				Global.logging.WriteLog("ERROR: GetMasterCopySeparationSet() - " + ex.Message);
			}
			finally 
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
					
			return masterCopySeparationSet;
		}

        public int GetPageHistoryExcel(ref PageHistory[] versions, int publicationID, DateTime pubDate, int editionID, int sectionID, string pageName, DateTime deadline, out string errmsg)
        {
            int versionindex = 0;
            int maxversions = 0;
            errmsg = "";

            int masterCopySeparationSet = GetMasterCopySeparationSet(publicationID, pubDate, editionID, sectionID, pageName, out errmsg);
            if (masterCopySeparationSet == 0)
            {
                Global.logging.WriteLog("ERROR: GetPageHistory() - Master number not found");
                return 0;
            }

            SqlCommand command = new SqlCommand("spGetPageHistory", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = null;



            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int);
                param.Value = masterCopySeparationSet;

                param = command.Parameters.Add("@HistoryMode", SqlDbType.Int);
                param.Value = 1;

                param = command.Parameters.Add("@AllColors", SqlDbType.Int);
                param.Value = 0;

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();


                while (reader.Read())
                {
                    int idx = 0;
                    DateTime eventTime = reader.GetDateTime(idx++);
                    string processTitle = reader.GetString(idx++);
                    string eventName = reader.GetString(idx++);

                    int version = reader.GetInt32(idx++);

                    string color = reader.GetString(idx++);
                    string fileName = reader.GetString(idx++);
                    reader.GetString(idx++);
                    int eventCode = reader.GetInt32(idx++);
                    int miscInt = reader.GetInt32(idx++);
                    string miscString = reader.GetString(idx++);

                    if (eventCode == 10 || eventCode == 70 || eventCode == 71)
                    {
                        versionindex = 0;

                        for (int i = 0; i < 100; i++)
                        {
                            if (versions[i].version == version)
                            {
                                versionindex = i;
                                break;
                            }
                            else if (versions[i].version == 0)
                            {
                                versionindex = i;
                                versions[versionindex].version = version;
                                break;
                            }
                        }

                        if (versions[versionindex].version > maxversions)
                            maxversions = versions[versionindex].version;

                        switch (eventCode)
                        {
                            case 10: // Polled
                                versions[versionindex].inputTime = eventTime;
                                versions[versionindex].message = (eventTime > deadline && deadline.Year > 2000) ? Global.rm.GetString("txtOverDeadline") : "";
                                break;
                            case 70:
                            case 71:
                                if (versions[versionindex].approveState == -1 || eventTime > versions[versionindex].approveTime)
                                {
                                    versions[versionindex].approveState = eventCode == 70 ? 1 : 0;
                                    versions[versionindex].approveUser = miscString;
                                    versions[versionindex].approveTime = eventTime;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetPageHistoryExcel() - " + ex.Message);
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return maxversions;
        }

		public ArrayList GetEditionsInProduction( int publicationID, DateTime pubDate, out string errmsg)
		{
			ArrayList list = new ArrayList();
			errmsg = "";
			
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandType = CommandType.Text;

			command.CommandText = "SELECT DISTINCT EditionID FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate ORDER BY EditionID";
			command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
			command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;	
		
			SqlDataReader reader = null;
			try 
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();

				reader = command.ExecuteReader();

				while( reader.Read()) 
				{
					list.Add(reader.GetInt32(0));
				
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				Global.logging.WriteLog("ERROR: GetEditionsInProduction() - " + ex.Message);
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
					
			return list;
		}

		public List<int> GetSectionsInEdition( int publicationID, DateTime pubDate, int editionID, out string errmsg)
		{
            List<int> list = new List<int>();
			errmsg = "";
			
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandType = CommandType.Text;

			command.CommandText = "SELECT DISTINCT SectionID FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID=0) ORDER BY SectionID";
			command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
			command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;	
			command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
		
			SqlDataReader reader = null;
			try 
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();

				reader = command.ExecuteReader();

				while( reader.Read()) 
				{
					list.Add(reader.GetInt32(0));				
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				Global.logging.WriteLog("ERROR: GetSectionsInEdition() - " + ex.Message);
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
					
			return list;
		}

        public int GetMainEditionInProduct(int publicationID, DateTime pubDate, out string errmsg)
        {
            errmsg = "";
            int editionID = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT TOP 1 EditionID FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND UniquePage=1 AND PageType < 3 AND Active>0 ORDER BY EditionID";
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if  (reader.Read())
                {
                    editionID = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetPagesInSection() - " + ex.Message);
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return editionID;
        }

        public ArrayList GetPagesInSection(int publicationID, DateTime pubDate, int editionID,int sectionID, out string errmsg)
		{
			ArrayList list = new ArrayList();
			errmsg = "";
		
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandType = CommandType.Text;

            if (editionID > 0 && sectionID > 0)
            {
                command.CommandText = "SELECT DISTINCT PageName,PageIndex FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND EditionID=@EditionID AND SectionID=@SectionID AND UniquePage=1 AND PageType < 3 AND Active>0 ORDER BY PageIndex";
                command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
                command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;

            }                   
            else if (editionID > 0 && sectionID == 0)
            {
                command.CommandText = "SELECT DISTINCT PageName,PageIndex,SectionID FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND EditionID=@EditionID AND UniquePage=1 AND PageType < 3 AND Active>0 ORDER BY SectionID,PageIndex";
                command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            }
            else if (editionID == 0 && sectionID > 0)
            {
                command.CommandText = "SELECT DISTINCT PageName,PageIndex,EditionID FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND SectionID=@SectionID AND UniquePage=1 AND PageType < 3 AND Active>0 ORDER BY EditionID, PageIndex";
                command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;
            }
            else
            {
                command.CommandText = "SELECT DISTINCT PageName,PageIndex,EditionID,SectionID FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND UniquePage=1 AND PageType < 3 AND Active>0 ORDER BY EditionID, SectionID, PageIndex"; 
            }

            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
			command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;	
																																																														  
			SqlDataReader reader = null;

			try 
			{
				if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
					connection.Open();

				reader = command.ExecuteReader();

				while( reader.Read()) 
				{
					list.Add(reader.GetString(0));				
				}
			}
			catch (Exception ex)
			{			
				errmsg = ex.Message; 
				Global.logging.WriteLog("ERROR: GetPagesInSection() - " + ex.Message);
			}
			finally 
			{
				// always call Close when done reading.
				if (reader != null)
					reader.Close();
				if (connection.State == ConnectionState.Open)
					connection.Close();
			}
					
			return list;
		}

        public bool GetFlatProofStatus(int copyFlatSeparationSet, ref int status, ref int flatProofStatus, ref int maxVersion, ref int publicationID, ref DateTime pubDate, out string errmsg)
        {
            errmsg = "";
            status = 0;
            flatProofStatus = 0;
            maxVersion = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT MIN(Status), MIN(FlatProofStatus),MAX(Version),PublicationID,PubDate FROM PageTable WITH (NOLOCK) WHERE CopyFlatSeparationSet=@CopyFlatSeparationSet AND PageType < 3 AND Active>0 AND Dirty=0 GROUP BY PublicationID,PubDate";
            command.Parameters.Add("@CopyFlatSeparationSet", SqlDbType.Int).Value = copyFlatSeparationSet;


            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    status = reader.GetInt32(0);
                    flatProofStatus = reader.GetInt32(1);
                    maxVersion = reader.GetInt32(2);
                    publicationID = reader.GetInt32(3);
                    pubDate = reader.GetDateTime(4);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetFlatProofStatus() - " + ex.Message);
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }

        public bool GetFlatDetails(int copyFlatSeparationSet, ref int nVersion, ref int publicationID, ref DateTime pubDate, out string errmsg)
        {
            errmsg = "";
            nVersion = 0;
            publicationID = 0;
            pubDate = DateTime.MinValue;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 MAX(VERSION),PublicationID,PubDate FROM PageTable WITH (NOLOCK) WHERE CopyFlatSeparationSet=@CopyFlatSeparationSet AND PageType < 3 AND Active>0 AND Dirty=0 GROUP BY PublicationID,PubDate";
            command.Parameters.Add("@CopyFlatSeparationSet", SqlDbType.Int).Value = copyFlatSeparationSet;


            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    nVersion = reader.GetInt32(0);
                    publicationID = reader.GetInt32(1);
                    pubDate = reader.GetDateTime(2);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetFlatProofStatus() - " + ex.Message);
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }

        public int GetProductionID(ref int pressID, int publicationID, DateTime pubDate, int editionID, out string errmsg)
        {
            errmsg = "";
            int productionID = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 ProductionID,PressID FROM PageTable WITH (NOLOCK) WHERE (PressID=@PressID OR @PressID=0) AND PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID=0)";
            command.Parameters.Add("@PressID", SqlDbType.Int).Value = pressID;
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;


            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    productionID = reader.GetInt32(0);
                    pressID = reader.GetInt32(1);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetProductionID() - " + ex.Message);
                return 0;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return productionID;

        }

        public int GetProductionIDAnyPress(int publicationID, DateTime pubDate, ref int pressID, out string errmsg)
        {

            errmsg = "";
            int productionID = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 ProductionID,PressID FROM PageTable WITH (NOLOCK) WHERE (PressID=@PressID OR @PressID=0) AND PublicationID=@PublicationID AND PubDate=@PubDate";
            command.Parameters.Add("@PressID", SqlDbType.Int).Value = pressID;
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;


            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    productionID = reader.GetInt32(0);
                    pressID = reader.GetInt32(1);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetFlatProofStatus() - " + ex.Message);
                return 0;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return productionID;

        }


        public bool GetSizeInfoForPage(int masterCopySeparationSet, ref string info, out string errmsg)
        {
            errmsg = "";
            info = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            command.CommandText = "SELECT TOP 1 Message FROM PrepollPageTable WITH (NOLOCK) WHERE Event=711 AND MasterCopySeparationSet=" + masterCopySeparationSet.ToString();
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    info = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool GetPageFormatForPage(int masterCopySeparationSet, ref double xdim, ref double ydim, ref double bleed, out string errmsg)
        {
            xdim = 0;
            ydim = 0;
            bleed = 0;
            errmsg = "";
            int pageFormatID = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            if ((bool)HttpContext.Current.Application["FieldExists_PageTable_PageFormatID"])
            {
                command.CommandText = "SELECT TOP 1 PageFormatID FROM PageTable WITH (NOLOCK) WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString();

                try
                {
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        pageFormatID = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                    return false;
                }
                finally
                {
                    // always call Close when done reading.
                    if (reader != null)
                        reader.Close();
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }

            if (pageFormatID == 0)
            {
                command.CommandText = "SELECT TOP 1 MiscInt1 FROM PressRunID WITH (NOLOCK) WHERE PressRunID =(SELECT TOP 1 PressRunID FROM PageTable with (NOLOCK) WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString() + ")";
                try
                {
                    // Execute the command
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        pageFormatID = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                    return false;
                }
                finally
                {
                    // always call Close when done reading.
                    if (reader != null)
                        reader.Close();
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            if (pageFormatID == 0)
            {
                command.CommandText = "SELECT TOP 1 PageFormatID FROM PublicationNames WITH (NOLOCK) WHERE PublicationID = (SELECT TOP 1 PublicationID FROM PageTable with (NOLOCK) WHERE MasterCopySeparationSet=" + masterCopySeparationSet + ")";

                command.CommandType = CommandType.Text;
                reader = null;
                try
                {
                    // Execute the command
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        pageFormatID = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                    return false;
                }
                finally
                {
                    // always call Close when done reading.
                    if (reader != null)
                        reader.Close();
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            if (pageFormatID > 0)
            {

                command.CommandText = "SELECT Width,Height,Bleed FROM PageFormatNames WHERE PageFormatID =" + pageFormatID;

                command.CommandType = CommandType.Text;
                reader = null;
                try
                {
                    // Execute the command
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        xdim = reader.GetDouble(0);
                        ydim = reader.GetDouble(1);
                        bleed = reader.GetDouble(2);
                    }
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                    return false;
                }
                finally
                {
                    // always call Close when done reading.
                    if (reader != null)
                        reader.Close();
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }


            }


            return true;
        }


        public bool ReleaseAll(int publicationID, DateTime pubDate, int editionID, int sectionID, string approveUser, string comment, bool logApproval, out string errmsg)
        {
            ArrayList aCopyFlatNumbers = new ArrayList();

            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT DISTINCT CopyFlatSeparationSet FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID=0) AND (SectionID=@SectionID OR @SectionID=0)";
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID; ;


            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    aCopyFlatNumbers.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: ReleaseAll() - " + ex.Message);
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            foreach (int copyFlatSeparationSet in aCopyFlatNumbers)
            {
                UpdateCopyFlatHold(copyFlatSeparationSet, 0, 0, out errmsg);

            }

            return true;

        }

        public bool RetransmitAndReleaseAllTX(bool release, int publicationID, DateTime pubDate, int editionID, int sectionID, string approveUser, out string errmsg)
        {
            ArrayList aCopyFlatNumbers = new ArrayList();

            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT DISTINCT CopyFlatSeparationSet FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID=0) AND (SectionID=@SectionID OR @SectionID=0)";
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID; ;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    aCopyFlatNumbers.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: ReleaseAll() - " + ex.Message);
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            foreach (int copyFlatSeparationSet in aCopyFlatNumbers)
            {
                command.CommandText = "UPDATE PageTable SET Status=49 WHERE Status>=47 AND CopyFlatSeparationSet=" + copyFlatSeparationSet.ToString();

                try
                {
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                    return false;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }


                if (release)
                {
                    command.CommandText = "UPDATE PageTable SET Hold=0,ApproveTime=GETDATE(),ApproveUser='" + approveUser + "' WHERE CopyFlatSeparationSet=" + copyFlatSeparationSet.ToString();

                    try
                    {
                        if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                            connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errmsg = ex.Message;
                        return false;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }

            return true;
        }

        public bool HoldAll(int publicationID, DateTime pubDate, int editionID, int sectionID, string approveUser, string comment, bool logApproval, out string errmsg)
        {
            ArrayList aCopyFlatNumbers = new ArrayList();

            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT DISTINCT CopyFlatSeparationSet FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID=0) AND (SectionID=@SectionID OR @SectionID=0)";
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID; ;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    aCopyFlatNumbers.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: ReleaseAll() - " + ex.Message);
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            foreach (int copyFlatSeparationSet in aCopyFlatNumbers)
            {
                UpdateCopyFlatHold(copyFlatSeparationSet, 1, 0, out errmsg);

            }

            return true;
        }


        public bool ApproveAll(int publicationID, DateTime pubDate, int editionID, int sectionID, string approveUser, string comment, bool logApproval, out string errmsg)
        {
            return ApproveAllEx(1, publicationID, pubDate, editionID, sectionID, approveUser, comment, logApproval, out errmsg);
        }

        public bool UnApproveAll(int publicationID, DateTime pubDate, int editionID, int sectionID, string approveUser, string comment, bool logApproval, out string errmsg)
        {
            return ApproveAllEx(0, publicationID, pubDate, editionID, sectionID, approveUser, comment, logApproval, out errmsg);
        }

        public List<int> GetMasterListFromProduction(int publicationID, DateTime pubDate, int editionID, int sectionID, out string errmsg)
        {
            return GetMasterListFromProduction(0, publicationID, pubDate, editionID, sectionID, out errmsg);
        }

        public List<int> GetMasterListFromProduction(int pressID, int publicationID, DateTime pubDate, int editionID, int sectionID, out string errmsg)
        {
            List<int> al = new List<int>();

            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            if (pressID > 0)
            {
                command.CommandText = "SELECT DISTINCT MasterCopySeparationSet FROM PageTable WITH (NOLOCK) WHERE PressID=@PressID AND PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID=0) AND (SectionID=@SectionID OR @SectionID=0) AND Dirty=0 AND PageType<3";
                command.Parameters.Add("@PressID", SqlDbType.Int).Value = pressID;
            } else
                command.CommandText = "SELECT DISTINCT MasterCopySeparationSet FROM PageTable WITH (NOLOCK) WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID=0) AND (SectionID=@SectionID OR @SectionID=0) AND Dirty=0 AND PageType<3";

            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID; 

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    al.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: ReleaseAll() - " + ex.Message);
                return al;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return al;
        }

        
        public bool ApproveAllEx(int Approval, int publicationID, DateTime pubDate, int editionID, int sectionID, string approveUser, string comment, bool logApproval, out string errmsg)
        {
            errmsg = "";

            List<int> al = GetMasterListFromProduction(publicationID,pubDate, editionID,sectionID, out errmsg);

            if (errmsg != "")
                return false;

            foreach (int masterCopySeparationSet in al)
                if (UpdateApproval(approveUser, masterCopySeparationSet, Approval, comment, out errmsg) == false)
                    return false;


            if (logApproval)
            {
                foreach (int masterCopySeparationSet in al)
                    UpdateApproveLog(masterCopySeparationSet, 1, Approval == 1 || Approval == -1 ? true : false, "Bulk approved", approveUser, out  errmsg);
            }

            return true;

        }

        public bool CustomAction(int publicationID, DateTime pubDate, int editionID, int sectionID, string miscstring, int version, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "INSERT INTO CustomMessageQueue (PublicationID,PubDate,EditionID,SectionID,LocationID,MessageType,EventTime,MiscInt,MiscString) VALUES (@PublicationID,@PubDate,@EditionID,@SectionID,0,1,GETDATE(),@Version,@MiscString)";

            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;
            command.Parameters.Add("@Version", SqlDbType.Int).Value = version;

            command.Parameters.Add("@MiscString", SqlDbType.VarChar, 50).Value = miscstring;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog("Error CustomAction() : " + ex.Message);
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }

        public bool ReadyAction(int publicationID, DateTime pubDate, int editionID, int sectionID, string message, int planVersion, bool setComment, out string errmsg)
        {
            errmsg = "";
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            List<int> pressRunIDList = new List<int>();

            command.Parameters.Clear();
            command.CommandText = "SELECT DISTINCT PressRunID FROM PageTable WITH (NOLOCK) WHERE PubDate=@PubDate AND PublicationID=@PublicationID AND (EditionID=@EditionID OR @EditionID=0) AND (SectionID=@SectionID OR @SectionID=0)";
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pressRunIDList.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: ReadyAction() - " + ex.Message);
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            foreach (int pressRunID in pressRunIDList)
            {
                command.Parameters.Clear();
                if (setComment)
                {
                    command.CommandText = "UPDATE PressRunID SET OrderNumber=@OrderNumber, PlanVersion=@PlanVersion,MiscDate=GETDATE() WHERE PressRunID=@PressRunID";
                    command.Parameters.Add("@OrderNumber", SqlDbType.VarChar, 50).Value = message;
                } else
                    command.CommandText = "UPDATE PressRunID SET PlanVersion=@PlanVersion,MiscDate=GETDATE() WHERE PressRunID=@PressRunID";
                command.Parameters.Add("@PlanVersion", SqlDbType.Int).Value = planVersion;
                command.Parameters.Add("@PressRunID", SqlDbType.Int).Value = pressRunID;

                try
                {
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                        connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Global.logging.WriteLog("Error Readyction() : " + ex.Message);
                    errmsg = ex.Message;
                    return false;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            // Stage 2 - set for full product also...
            int dummypressID = 0;
            int productionID = GetProductionID(ref dummypressID, publicationID, pubDate, 0, out errmsg);
            
            command.CommandText = "UPDATE ProductionNames SET OrderNumber=@OrderNumber WHERE P.ProductionID=@ProductionID";
            command.Parameters.Clear();
            command.Parameters.Add("@OrderNumber", SqlDbType.VarChar, 50).Value = message;
            command.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog("Error Readyction() : " + ex.Message);
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }



        public bool SetPlanVersion(int productionID, int planVersion, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "UPDATE PressRunID SET PlanVersion=@PlanVersion WHERE PressRunID IN (SELECT DISTINCT P.PressRunID FROM PageTable AS P WITH (NOLOCK) WHERE P.ProductionID=@ProductionID)";
            command.Parameters.Add("@PlanVersion", SqlDbType.Int).Value = planVersion;
            command.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog("Error CustomAction() : " + ex.Message);
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool SetPlanVisiolinkStatus(int productionID, int state, string message, int pushVersion, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "UPDATE ProductionNames SET Combined=@State,Sections=@Message,bindingstyle=@PushVersion WHERE ProductionID=@ProductionID";
            command.Parameters.Add("@State", SqlDbType.Int).Value = state;
            command.Parameters.Add("@Message", SqlDbType.VarChar, 50).Value = message;
            command.Parameters.Add("@PushVersion", SqlDbType.Int).Value = pushVersion;
            command.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog("Error CustomAction() : " + ex.Message);
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        public bool RetransmitAll(int publicationID, DateTime pubDate, int editionID, int sectionID, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "UPDATE PageTable SET Status=10 WHERE PublicationID=@PublicationID AND PubDate=@PubDate AND (EditionID=@EditionID OR @EditionID=0) AND (SectionID=@SectionID OR @SectionID=0)";

            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }

        public bool UpdateUnknownFiles(string filename, string inputfolder, int retry, string newname, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            if (newname == "")
                newname = filename;
            else
                newname = filename + ";" + newname;

            command.CommandText = "UPDATE UnknownFiles SET Retry=@Retry,FileName=@NewFileName WHERE FileName=@Filename AND InputFolder=@InputFolder";

            command.Parameters.Add("@Retry", SqlDbType.Int).Value = retry;
            command.Parameters.Add("@Filename", SqlDbType.VarChar, 255).Value = filename;
            command.Parameters.Add("@NewFileName", SqlDbType.VarChar, 255).Value = newname;
            command.Parameters.Add("@InputFolder", SqlDbType.VarChar, 25).Value = inputfolder;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public int GetCopySeparationSet(int masterCopySeparationSet, int editionID, out string errmsg)
        {
            int copySeparationSet = masterCopySeparationSet;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 CopySeparationSet FROM PageTable WITH (NOLOCK) WHERE MasterCopySeparationSet=@MasterCopySeparationSet AND EditionID=@EditionID AND Dirty=0 AND Active>0 AND PageType<2 AND UniquePage>0";
            command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int).Value = masterCopySeparationSet;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    copySeparationSet = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetCopySeparationSet() - " + ex.Message);
                return copySeparationSet;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return copySeparationSet;
        }

        public ArrayList GetValidDefaultPressesForPublication(int publicationID, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT DISTINCT PressID FROM  PublicationTemplates WITH (NOLOCK) WHERE PublicationID=@PublicationID AND TemplateID > 0";
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    al.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetValidDefaultPressesForPublication() - " + ex.Message);
                return al;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return al;
        }

        public List<int> GetMastersInProduction(int productionID, out string errmsg)
        {
            errmsg = "";
            List<int> al = new List<int>();

            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = "SELECT DISTINCT MasterCopySeparationSet FROM  PageTable WITH (NOLOCK) WHERE ProductionID=@ProductionID AND Dirty=0 AND Active>0 AND PageType<2 AND UniquePage>0"
            };
            command.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    al.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetMastersInProduction() - " + ex.Message);
                return al;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return al;

        }

        public int GetMinStatus(int masterCopySeparationSet, out string errmsg)
        {
            int status = -1;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT MIN(Status) FROM PageTable WITH (NOLOCK) WHERE MasterCopySeparationSet=@MasterCopySeparationSet AND Dirty=0 AND Active>0 AND PageType<2 AND UniquePage>0";
            command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int).Value = masterCopySeparationSet;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    status = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetMinStatus() - " + ex.Message);
                return status;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return status;

        }


        public int InProgressStatus(int masterCopySeparationSet, out string errmsg)
        {
            int status = 0;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 Status FROM PageTable WITH (NOLOCK) WHERE MasterCopySeparationSet=@MasterCopySeparationSet AND Dirty=0 AND Active>0 AND PageType<2 AND UniquePage>0 AND Status IN (5,15,25,35,45,47,48)";
            command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int).Value = masterCopySeparationSet;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    status = 1;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: InProgressStatus() - " + ex.Message);
                return -1;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return status;
        }

        public bool DeleteMasterCopySeparationSet(int masterCopySeparationSet,out string errmsg)
        {
            errmsg = "";
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM PrePollPageTable WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString();
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            command.CommandText = "DELETE FROM PageTable WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString();
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool UpdateMasterStatus(int masterCopySeparationSet, int status, string comment, out string errmsg)
        {
            errmsg = "";
            
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            if (status == 0)
                command.CommandText = "UPDATE PageTable SET Status=0,Version=0,FlatProofStatus=0,HardProofStatus=0,ProofStatus=0,Comment='" + comment + "' WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString();
            else
                command.CommandText = "UPDATE PageTable SET Status=" + status.ToString() + ",Comment='" + comment + "' WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString();

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (status == 0)
                DeletePrepollMasterSet(masterCopySeparationSet, out errmsg);

            return true;
        }

        public bool DeletePrepollMasterSet(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "DELETE FROM PrepollPageTable WHERE Event NOT IN (130,136,137) AND MasterCopySeparationSet=" + masterCopySeparationSet.ToString();
 
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


        public string GetMiscString3(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            string miscString3 = "", s;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT DISTINCT MiscString3 FROM PageTable WITH (NOLOCK) WHERE MasterCopySeparationSet=@MasterCopySeparationSet AND Dirty=0 AND Active>0 AND PageType<2 AND UniquePage>0";
            command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int).Value = masterCopySeparationSet;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    
					s = reader.GetString(0);
					if (miscString3 != "")
						miscString3 += ",";
					miscString3 += s;

                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetMiscString3() - " + ex.Message);
                return "";
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return miscString3;
        }

        public int GetPlanVersion(int productionID, out string errmsg)
        {
            errmsg = "";

            int planVersion = 1;


            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 PR.PlanVersion FROM PageTable P WITH (NOLOCK) INNER JOIN PressRunID AS PR ON PR.PressRunID=P.PressRunID WHERE P.PressRunID IN (SELECT TOP 1 P2.PressRunID FROM PageTable AS P2 WITh (NOLOCK) WHERE P2.ProductionID=@ProductionID)";
            command.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    planVersion = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetPlanVersion() - " + ex.Message);
                return 1;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return planVersion;
        }


        public string GetUploadFolder(int publicationID, out string errmsg)
        {
            errmsg = "";
            string miscString3 = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 UploadFolder FROM PublictionNames WITH (NOLOCK) WHERE PublicationID=@PublicationID";
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    miscString3 = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetUploadFolder() - " + ex.Message);
                return "";
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return miscString3;
        }


        public string GetChannelsForProduction(int publicationID, DateTime pubDate, out string errmsg)
        {
            errmsg = "";
            string channelList = "", s;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = 
                "SELECT DISTINCT CS.ChannelID FROM ChannelStatus CS WITH (NOLOCK) INNER JOIN PageTable P WITH (NOLOCK) ON P.MasterCopySeparationSet = CS.MasterCopySeparationSet WHERE P.PublicationID=@PublicationID AND P.PubDate=@PubDate AND Dirty = 0 AND Active> 0 AND PageType<2 AND UniquePage> 0";
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    s = reader.GetInt32(0).ToString(); ;
					if (channelList != "")
                        channelList += ",";
                    channelList += s;
					
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetChannelsForProduction() - " + ex.Message);
                return "";
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return channelList;
        }

        public string GetFileName(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            string fileName = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 FileName FROM PageTable WITH (NOLOCK) WHERE MasterCopySeparationSet=@MasterCopySeparationSet AND Dirty=0 AND Active>0 AND PageType<2 AND UniquePage>0";
            command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int).Value = masterCopySeparationSet;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    fileName = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetFileName() - " + ex.Message);
                return "";
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return fileName;
        }

        public int GetInkLimitForPage(int masterCopySeparationSet, out string errmsg)
        {
            int inkLimit = 240;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT TOP 1 PR.JpegThumbnail FROM PageTable WITH (NOLOCK) INNER JOIN ProofConfigurations PR ON PR.ProofID=PageTable.ProofID WHERE PageTable.MasterCopySeparationSet=@MasterCopySeparationSet AND Dirty=0 AND Active>0 AND PageType<2 AND UniquePage>0";
            command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int).Value = masterCopySeparationSet;

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int n = reader.GetInt32(0);
                    inkLimit = (n >> 3) & 0xFFF;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetInkLimitForPage() - " + ex.Message);
                return 240;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return inkLimit;
        }

        public DataTable GetLogTXEntries(string location, ref DateTime tLastDateTime, out string errmsg)
        {
            errmsg = "";
            int nEntries = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            int n = (int)HttpContext.Current.Application["MaxLogEntries"];
            if (n == 0)
                n = 250;

            command.CommandText = "SELECT TOP " + n.ToString() + " EventTime, E.EventName, FileName, C.ColorName,Version,ErrorMsg FROM Log WITH (NOLOCK) INNER JOIN EventCodes E ON E.EventNumber=Log.Event INNER JOIN ColorNames C ON C.ColorID=FlatSeparation%10 WHERE FlatSeparation>0 AND MiscString='" + location + "' ORDER BY EventTime DESC";
            command.CommandType = CommandType.Text;


            DataTable tbl = new DataTable();

            DataColumn newColumn;

            newColumn = tbl.Columns.Add("EventTime", Type.GetType("System.DateTime"));
            newColumn = tbl.Columns.Add("EventName", Type.GetType("System.String"));
            newColumn = tbl.Columns.Add("FileName", Type.GetType("System.String"));
            newColumn = tbl.Columns.Add("ColorName", Type.GetType("System.String"));
            newColumn = tbl.Columns.Add("Version", Type.GetType("System.Int32"));
            newColumn = tbl.Columns.Add("ErrorMsg", Type.GetType("System.String"));

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    nEntries++;
                    newRow = tbl.NewRow();
                    newRow["EventTime"] = reader.GetDateTime(0);
                    tLastDateTime = (DateTime)newRow["EventTime"];
                    newRow["EventName"] = reader.GetString(1);
                    newRow["FileName"] = reader.GetString(2);
                    newRow["ColorName"] = reader.GetString(3);
                    newRow["Version"] = reader.GetInt32(4);
                    string msg = reader.GetString(5);
                    if (msg.Length > (int)HttpContext.Current.Application["MaxLogEntries"])
                        msg = msg.Substring(0, (int)HttpContext.Current.Application["MaxLogEntries"]);
                    newRow["ErrorMsg"] = msg;

                    tbl.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            if (nEntries == 0)
            {
                newRow = tbl.NewRow();
                newRow["EventTime"] = DateTime.MinValue;
                newRow["EventName"] = "n/a";
                newRow["FileName"] = "n/a";
                newRow["ColorName"] = "";
                newRow["Version"] = 0;
                newRow["ErrorMsg"] = "No log entries";
                tbl.Rows.Add(newRow);
            }
            return tbl;
        }

        public bool GetLogTXEntriesForReport(string press, ref string log, out string errmsg)
        {
            errmsg = "";
            log = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            int hoursToReport = (int)HttpContext.Current.Application["LogReportPeriodHours"];
            if (hoursToReport == 0)
                hoursToReport = 48;

            string s;

            string sql = "SELECT MiscString,CAST(DATEPART(YEAR,EventTime) as varchar(4))+'-'+RIGHT('0'+CAST(DATEPART(MONTH,EventTime) as varchar(4)),2)+'-'+RIGHT('0'+CAST(DATEPART(DAY,EventTime) as varchar(4)),2)+' '+RIGHT('0'+CAST(DATEPART(HOUR,EventTime) as varchar(4)),2)+':'+RIGHT('0'+CAST(DATEPART(MINUTE,EventTime) as varchar(4)),2)+':'+RIGHT('0'+CAST(DATEPART(SECOND,EventTime) as varchar(4)),2),EV.EventName,FileName,ErrorMsg FROM Log INNER JOIN EventCodes EV on Log.Event=EV.EventNumber WHERE Event IN (26,27,30) AND DATEDIFF(hour,EventTime,GETDATE())<" + hoursToReport.ToString() + " AND MiscString='" + press + "' ORDER BY EventTime";
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            log = "Location;EventTime;Status;Filename;Message\r\n";


            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    s = reader.GetString(0) + ";";
                    s += reader.GetString(1) + ";";
                    s += reader.GetString(2) + ";";
                    s += reader.GetString(3) + ";";
                    s += reader.GetString(4);
                    log += s + "\r\n";
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader == null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


        public int GetTransmitterStatus(out string errmsg)
        {
            errmsg = "";
            int status = -1;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT CurrentState FROM ProcessConfigurations WHERE ProcessType=40";

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    status = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetFileName() - " + ex.Message);
                return -1;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return status;

        }


/*
        public DataTable GetDeviceStates(out string errmsg)
        {
            errmsg = "";

            int nTXEnabled = GetTransmitterStatus(out errmsg);

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            string devicesToIgnore = (string)HttpContext.Current.Application["DevicesToIgnore"];
            if (devicesToIgnore == "")
                devicesToIgnore = "'Internal','Tyrkiet','Thailand','All'";
            command.CommandText = "SELECT DeviceName,CurrentState,CurrentJob,LastError,Enabled FROM DeviceConfigurations WHERE DeviceName NOT IN (" + devicesToIgnore + ")";
            command.CommandType = CommandType.Text;


            DataTable tbl = new DataTable();

            DataColumn newColumn;
            newColumn = tbl.Columns.Add("DeviceName", Type.GetType("System.String"));
            newColumn = tbl.Columns.Add("CurrentState", Type.GetType("System.Int32"));
            newColumn = tbl.Columns.Add("CurrentJob", Type.GetType("System.String"));
            newColumn = tbl.Columns.Add("LastError", Type.GetType("System.String"));
            newColumn = tbl.Columns.Add("Enabled", Type.GetType("System.Int32"));

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newRow = tbl.NewRow();
                    newRow["DeviceName"] = reader.GetString(0);
                    newRow["CurrentState"] = reader.GetInt32(1);
                    int nCurrentStateDB = (int)newRow["CurrentState"];
                    if (nTXEnabled == 0)
                        newRow["CurrentState"] = 0;
                    newRow["CurrentJob"] = reader.GetString(2);
                    newRow["LastError"] = reader.GetString(3);
                    newRow["Enabled"] = reader.GetInt32(4);
                    if (nTXEnabled == 0)
                        newRow["Enabled"] = 0;

                    if ((string)newRow["LastError"] == "")
                        newRow["LastError"] = "Idle";

                    // Mark Error State
                    if (nCurrentStateDB == 1 && (string)newRow["LastError"] != "Idle" && (string)newRow["LastError"] != "Line OK" && (string)newRow["LastError"] != "Off")
                        newRow["CurrentState"] = 2;

                    tbl.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tbl;
        }
*/
        public bool PublicationPlanLock(int publicationID, DateTime pubDate, string userName, int bRequestedPlanLock, ref int bCurrentPlanLock, ref string sClientName, ref DateTime tClientTime, out string errmsg)
        {
            errmsg = "";
            sClientName = "";
            tClientTime = DateTime.MinValue;
            bCurrentPlanLock = -1;

            string myClientName = "WEBCENTER-" + userName;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spPublicationLock";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;

            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;
            
            param = command.Parameters.Add("@Lock", SqlDbType.Int);
            param.Value = bRequestedPlanLock;

            param = command.Parameters.Add("@ClientName", SqlDbType.VarChar, 50);
            param.Value = myClientName;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    bCurrentPlanLock = reader.GetInt32(0);
                    sClientName = reader.GetString(1);
                    tClientTime = reader.GetDateTime(2);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }





        public bool GetPageProcessingSettings(int nMasterCopySeparatioSet, out int RipSetupID, out string errmsg)
        {
            errmsg = "";
            RipSetupID = 0;


            if (FieldExists("PageTable","RipSetupID", out errmsg) != 1)
                return true; // on purpose..

            string sqlCmd = "SELECT TOP 1 RipSetupID FROM PageTable WITH (NOLOCK) WHERE MasterCopySeparationSet=" + nMasterCopySeparatioSet.ToString();

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    RipSetupID = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }


            return true;
        }



        public bool GetDefaultPageProcessingSettings(int nMasterCopySeparatioSet, out string RipSetupString, out string errmsg)
        {
            errmsg = "";
            RipSetupString = "";

            string sqlCmd = "SELECT TOP 1 PT.MiscString FROM PageTable P WITH (NOLOCK) INNER JOIN PublicationTemplates PT ON P.PublicationID=PT.PublicationID AND P.PressID=PT.PressID WHERE P.MasterCopySeparationSet=" + nMasterCopySeparatioSet.ToString();

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    RipSetupString = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }


            return true;
        }

        public bool GetDefaultPageProcessingSettingsPublication(int pressID, int publicationID, out string RipSetupString, out string errmsg)
        {
            errmsg = "";
            RipSetupString = "";

            string sqlCmd = "SELECT TOP 1 MiscString FROM PublicationTemplates WHERE PublicationID=" + publicationID.ToString() + " AND PressID=" + pressID.ToString();

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    RipSetupString = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }


            return true;
        }

        public bool GetPageProcessingSettingsPressRun(int pressRunID, out int RipSetupID, out string errmsg)
        {
            errmsg = "";
            RipSetupID = 0;


            if (FieldExists("PageTable", "RipSetupID", out errmsg) != 1)
                return true; // on purpose..

            string sqlCmd = "SELECT TOP 1 RipSetupID FROM PageTable WITH (NOLOCK) WHERE PressRunID=" + pressRunID.ToString();

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    RipSetupID = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool GetPageProcessingSettingsPressRun(int pressID,DateTime pubDate, int publicationID, int editionID, int sectionID, 
                                                out int RipSetupID, out string errmsg)
        {
            errmsg = "";
            RipSetupID = 0;

            if (FieldExists("PageTable", "RipSetupID", out errmsg) != 1)
                return true; // on purpose..

            string sqlCmd = "SELECT TOP 1 RipSetupID FROM PageTable WITH (NOLOCK) WHERE PressID=@PressID AND PubDate=@PubDate AND PublicationID=@PublicationID AND EditionID=@EditionID AND SectionID=@SectionID AND Dirty=0";


            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("@PressID", SqlDbType.Int).Value = pressID;
            command.Parameters.Add("@PubDate", SqlDbType.DateTime).Value = pubDate;
            command.Parameters.Add("@PublicationID", SqlDbType.Int).Value = publicationID;
            command.Parameters.Add("@EditionID", SqlDbType.Int).Value = editionID;
            command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;
              
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    RipSetupID = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


        public bool UpdateProcessingParameter(int masterCopySeparationSet, int ripSetupID, out string errmsg)
        {
            errmsg = "";
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "UPDATE PageTable SET RipSetupID="+ripSetupID.ToString()+" WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString();

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public int GetPDFMaster(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            int PdfMaster = 0;
            string sqlCmd = "SELECT TOP 1 PDFMaster FROM PageTable P WITH (NOLOCK) WHERE P.MasterCopySeparationSet=" + masterCopySeparationSet.ToString();

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    PdfMaster = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return 0;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return PdfMaster;
        }

        public bool GetFileCenterPDFname(int masterCopySeparationSet, bool isArchiveEvent, out string PDFName, out string errmsg)
        {
            errmsg = "";
            PDFName = "";
            int PDFmaster = GetPDFMaster( masterCopySeparationSet, out errmsg);

            if (PDFmaster == 0)
                return false;

            string sqlCmd;
            if (isArchiveEvent)
                sqlCmd = "SELECT DISTINCT ISNULL(Message,'') FROM PrepollPageTable WHERE Event=260 AND MasterCopySeparationSet=" + PDFmaster.ToString();
            else
                sqlCmd = "SELECT DISTINCT ISNULL(Message,'') FROM PrepollPageTable WHERE Event=130 AND MasterCopySeparationSet=" + PDFmaster.ToString();
            
            //sqlCmd = "SELECT DISTINCT ISNULL(PRE.Message,'') FROM PageTable P WITH (NOLOCK) LEFT OUTER JOIN PrepollPageTable PRE ON P.PdfMaster=PRE.MasterCopySeparationSet AND PRE.Event=130 WHERE P.MasterCopySeparationSet=" + masterCopySeparationSet.ToString();

            SqlCommand command = new SqlCommand(sqlCmd, connection);
            command.CommandType = CommandType.Text;
    
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    PDFName = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;    
        }

        public bool QueueFileRetryRequest(int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "INSERT INTO FileCenterRetryQueue (MasterCopySeparationSet,EventTime) VALUES ("+masterCopySeparationSet.ToString()+",GETDATE())" ;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

    

        public bool GetCustomLog(string logName, ref string result, out string errmsg)
        {
            errmsg = "";
            result = "Time           Filename\r\n";

            SqlCommand command = new SqlCommand("spGetFileDistributorLog", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@Press", SqlDbType.VarChar, 50);
            param.Direction = ParameterDirection.Input;
            param.Value = logName;
    
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DateTime eventTime = reader.GetDateTime(0);
                    string fileName = reader.GetString(1);
                    result += "\r\n";
                    result += string.Format("{0:00}-{1:00} {2:00}:{3:00}:{4:00} {5}\r\n", eventTime.Month, eventTime.Day, eventTime.Hour, eventTime.Minute, eventTime.Second, fileName);
                    
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            result += "\r\n";
            return true;    
        }

        public bool LockPage(int nMasterCopySeparationSet, bool locked, out string errmsg)
        {
            bool ret = false;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            if (locked)
                command.CommandText = "UPDATE PageTable SET OutputPriority=-1 WHERE  MasterCopySeparationSet=" + nMasterCopySeparationSet.ToString();
            else
                command.CommandText = "UPDATE PageTable SET OutputPriority=50 WHERE  MasterCopySeparationSet=" + nMasterCopySeparationSet.ToString();

            command.CommandType = CommandType.Text;


            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }


        public List<string> GetPressTemplateList(int pressID, out string errmsg)
        {
            List<string> arrList = new List<string>();
            errmsg = "";

            string sql;
            if (pressID != 0)
                sql = string.Format("SELECT DISTINCT name FROM Presstemplatenames PT INNER JOIN Presstemplate p on pt.PresstemplateID=p.PresstemplateID WHERE p.PressID={0} order by [name]", pressID);
            else
                sql = string.Format("SELECT DISTINCT name FROM Presstemplatenames order by [name]");

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    arrList.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return arrList;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return arrList;
        }

        public int GetPressTemplatePressID(string pressTemplateName,out string errmsg)
        {
            errmsg = "";
            int pressID = 0;
            string sql = string.Format("SELECT TOP 1 PressID FROM Presstemplate P INNER JOIN PresstemplateNames PN ON P.PressTemplateID=PN.PressTemplateID WHERE PN.Name='{0}' ", pressTemplateName);

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    pressID = reader.GetInt32(0);


                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return 0;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return 0;
        }


        public bool LoadPressTemplate(string pressTemplateName, ref List<PageTableEntry> plateList, ref int numberOfRuns, out string errmsg)
        {
            errmsg = "";
            numberOfRuns = 0;

            plateList.Clear();

            //                                                0         1         2      3        4         5       6        7           8     9      10      11       12   13      14        15        16       17        18          19
            string sql = string.Format("SELECT DISTINCT NumberOfRuns,SectionID,Pagina,PageName,PageIndex,RunNumber,Color,TemplateID,Halfweb,TowerID,ZoneID,CylinderID,High,Pairpos,Front,FlatNumber,CopyNumber,PageType,Markgroups,PagePositions FROM Presstemplate P INNER JOIN PresstemplateNames PN ON P.PressTemplateID=PN.PressTemplateID WHERE PN.Name='{0}' ORDER BY RunNumber,FlatNumber,Front,Pairpos,Color,CopyNumber", pressTemplateName);

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    numberOfRuns = reader.GetInt32(0);
                    
                    PageTableEntry p = new PageTableEntry();
                    p.m_sectionID = reader.GetInt32(1);
                    p.m_pagination = reader.GetInt32(2);
                    p.m_pagename = reader.GetString(3);
                    p.m_pageindex  = reader.GetInt32(4);
                    p.m_presssectionnumber = reader.GetInt32(5);
                    p.m_colorID = reader.GetInt32(6);
                    p.m_active = p.m_colorID != 6;
                    p.m_templateID = reader.GetInt32(7);
                    int halfweb = reader.GetInt32(8);
                    p.m_presstower = reader.GetString(9);
                    p.m_presszone = reader.GetString(10);
                    p.m_presscylinder = reader.GetString(11);
                    p.m_presshighlow = reader.GetString(12);
                    p.m_pageposition = reader.GetInt32(13);
                    p.m_sheetside = reader.GetInt32(14);
                    p.m_sheetnumber = reader.GetInt32(15);
                    p.m_copynumber = reader.GetInt32(16);
                    p.m_pagetype = reader.GetInt32(17);
                    p.m_markgroups = reader.GetString(18);
                    p.m_pagepositions = reader.GetString(19);

                    plateList.Add(p);

                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        ///////////////////////////////////////////

        public DataTable GetChannelCollection(out string errmsg)
        {
            errmsg = "";
            DataTable dataTable = new DataTable();

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter("SELECT DISTINCT ChannelID AS ID,Name,PDFType,MergedPDF,ChannelNameAlias FROM ChannelNames ORDER BY Name", connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }
        

        

        public DataTable GetPublicationChannelsCollection(out string errmsg)
        {
            errmsg = "";
            DataTable dataTable = new DataTable();

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter("SELECT DISTINCT PublicationID, ChannelID FROM PublicationChannels WHERE PublicationID>0 AND ChannelID>0 ORDER BY PublicationID, ChannelID", connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }

        public bool GetPublicationChannelsForPublication(int publicationID, ref ArrayList al, out string errmsg)
        {
            errmsg = "";
            al.Clear();

            string sql = "SELECT DISTINCT ChannelID FROM PublicationChannels WHERE PublicationID=" + publicationID.ToString() + " AND ChannelID>0 ORDER BY ChannelID";

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    al.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }



        public DataTable GetChannelStatusCollection(out string errmsg)
        {
            errmsg = "";

            int channelIDFilter = Globals.GetIDFromName("ChannelNameCache", (string)HttpContext.Current.Session["SelectedChannel"]);

            DateTime tDefaultPubDate = new DateTime(1975, 1, 1, 0, 0, 0, 0);

            SqlCommand command = new SqlCommand("spThumbnailPageListChannels", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            DataTable tblMain = new DataTable("PageTable");
            SqlDataReader reader = null;

            DataColumn newColumn;

            newColumn = tblMain.Columns.Add("MasterCopySeparationSet", Type.GetType("System.Int32"));
            newColumn = tblMain.Columns.Add("Status", Type.GetType("System.Int32")); // Minstatus!

            DataRow newRow = null;

            try
            {
                command.Parameters.Clear();
                SqlParameter param;

                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]);
                Global.logging.WriteLog("spThumbnailPageListChannels() " + (Globals.GetIDFromName("PublicationNameCache", (string)HttpContext.Current.Session["SelectedPublication"]).ToString()));

                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Input;
                param.Value = (DateTime)HttpContext.Current.Session["SelectedPubDate"];
                Global.logging.WriteLog("spThumbnailPageListChannels() " + ((DateTime)HttpContext.Current.Session["SelectedPubDate"]).ToString());

                param = command.Parameters.Add("@EditionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]);
                Global.logging.WriteLog("spThumbnailPageListChannels() " + (Globals.GetIDFromName("EditionNameCache", (string)HttpContext.Current.Session["SelectedEdition"]).ToString()));

                param = command.Parameters.Add("@SectionID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]);
                Global.logging.WriteLog("spThumbnailPageListChannels() " + Globals.GetIDFromName("SectionNameCache", (string)HttpContext.Current.Session["SelectedSection"]).ToString());

                param = command.Parameters.Add("@ChannelID", SqlDbType.Int);
                param.Direction = ParameterDirection.Input;
                param.Value = channelIDFilter;
                Global.logging.WriteLog("spThumbnailPageListChannels() " + channelIDFilter.ToString());

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                int thisMasterCopySeparationSet = 0;
                int prevMasterCopySeparationSet = -1;
                int thisChannelID;
                int thisInputStatus;
                int thisExportStatus;
                int thisProcessStatus;
                bool bHasError = false;
                bool bIsFirst = true;
                int minStatus = 11;
                bool bHasSpecialChannelDone = false;

                while (reader.Read())
                {
                    int idx = 0;
                    int fieldCount = reader.FieldCount;

                    thisMasterCopySeparationSet = reader.GetInt32(idx++);	// CopyColorSet
                   // Global.logging.WriteLog("spThumbnailPageListChannels() thisMasterCopySeparationSet " + thisMasterCopySeparationSet.ToString());
                    thisChannelID = reader.GetInt32(idx++);          
                    thisInputStatus = reader.GetInt32(idx++);
                    thisProcessStatus = reader.GetInt32(idx++);
                    thisExportStatus = reader.GetInt32(idx++);

                    if (thisMasterCopySeparationSet != prevMasterCopySeparationSet)
                    {
                        if (bIsFirst == false)
                        {
                            tblMain.Rows.Add(newRow);
                        }

                        bIsFirst = false;
                        bHasError = false;
                        minStatus = 11;
                        bHasSpecialChannelDone = false;
                        newRow = tblMain.NewRow();
                    }

                    newRow["MasterCopySeparationSet"] = thisMasterCopySeparationSet;

                    if (thisInputStatus == 6 || thisProcessStatus == 6 || thisExportStatus == 6)
                        bHasError = true;

                    if (thisExportStatus == 9)
                    {
                        bHasSpecialChannelDone = true;
                        thisExportStatus = 10;
                    }

                    // get lowest..
                    if (minStatus > thisExportStatus)
                        minStatus = thisExportStatus;

                    newRow["Status"] = bHasError ? 6 : (bHasSpecialChannelDone && minStatus != 10 ? 9 : minStatus);

                    prevMasterCopySeparationSet = thisMasterCopySeparationSet;

                }
                if (bIsFirst == false)
                    tblMain.Rows.Add(newRow);

            }
            catch (Exception ex)
            {
                errmsg = "!!!  " + ex.Message;
                Global.logging.WriteLog(ex.Message);
                tblMain = null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblMain;

        }

        public bool ResendToChannel(int masterCopySeparationSet, int editionID, List<int> channelIDList, out string errmsg)
        {
            errmsg = "";

            string inList = "";
            foreach (int n in channelIDList)
            {
                if (inList != "")
                    inList += ",";
                inList += n.ToString();
            }

            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = string.Format("UPDATE ChannelStatus SET ExportStatus=0 WHERE MasterCopySeparationSet={0} AND ChannelID IN ({1})", masterCopySeparationSet, inList)
            };

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        public bool ResendInPageTable(int masterCopySeparationSet, int editionID, out string errmsg)
        {
            errmsg = "";


            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = string.Format("UPDATE PageTable SET Status=30 WHERE MasterCopySeparationSet={0} AND Status=50", masterCopySeparationSet)
            };

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public DataTable GetChannelList(out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "SELECT DISTINCT ChannelID,[Name],[Enabled] FROM ChannelNames WITH (NOLOCK)";

            DataTable tblResults = new DataTable();

            DataColumn newColumn;
            newColumn = tblResults.Columns.Add("Enabled", Type.GetType("System.Boolean"));
            newColumn = tblResults.Columns.Add("Name", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("LastError", Type.GetType("System.String"));
            newColumn = tblResults.Columns.Add("ID", Type.GetType("System.Int32"));

            SqlDataReader reader = null;
            DataRow newRow = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    newRow = tblResults.NewRow();
                    newRow["ID"] = reader.GetInt32(0);
                    newRow["Name"] = reader.GetString(1);
                    newRow["Enabled"] = reader.GetInt32(2) == 0 ? true : false;

                    newRow["LastError"] = "";
                    tblResults.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetChannelList() - " + ex.Message);
                return tblResults;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return tblResults;

        }

        public bool UpdateChannelEnable(int channelID, bool enable, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "UPDATE ChannelNames SET [Enabled]=@Enabled WHERE ChannelID=@ChannelID";

            command.Parameters.Add("@Enabled", SqlDbType.Int).Value = enable == true ? 0 : 1;
            command.Parameters.Add("@ChannelID", SqlDbType.Int).Value = channelID;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


  


        public ArrayList GetUserPublishers(string userName, out string errmsg)
        {
            errmsg = "";
            ArrayList al = new ArrayList();
            string sql = "SELECT DISTINCT PublisherID FROM UserPublishers WHERE UserName='" + userName + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string s = Globals.GetNameFromID("PublisherNameCache", id);
                    al.Add(s);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            };
            return al;
        }

        public DataTable GetPublisherCollection(out string errmsg)
        {
            errmsg = "";
            DataTable dataTable = new DataTable();

            SqlDataAdapter sqlDataAdaptor = new SqlDataAdapter("SELECT DISTINCT PublisherID AS ID,PublisherName AS Name FROM PublisherNames ORDER BY PublisherName", connection2);
            try
            {
                // Execute the command
                //				if (connection2.State == ConnectionState.Closed || connection2.State == ConnectionState.Broken)
                //					connection2.Open();
                sqlDataAdaptor.Fill(dataTable);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return dataTable;
        }



        public int GetChannelExportStatus(int productionID, int masterCopySeparationSet, int channelID, out string errmsg)
        {
            errmsg = "";
            int status = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            if (masterCopySeparationSet > 0)
            {
                command.CommandText =
                        "SELECT TOP 1 ExportStatus,ExportStatus FROM ChannelStatus WHERE " +
                        "MasterCopySeparationSet=@MasterCopySeparationSet AND ChannelID=@ChannelID";
                command.Parameters.Add("@ChannelID", SqlDbType.Int).Value = channelID;
                command.Parameters.Add("@MasterCopySeparationSet", SqlDbType.Int).Value = masterCopySeparationSet;
            }
            else
            {
                command.CommandText =
                        "SELECT  ISNULL(MIN( ExportStatus),0),ISNULL(MAX( ExportStatus),0) FROM ChannelStatus CS WITH (NOLOCK) " +
                        "INNER JOIN PageTable P WITH (NOLOCK) ON P.MasterCopySeparationSet = CS.MasterCopySeparationSet " +
                        "WHERE P.ProductionID = @ProductionID AND CS.ChannelID = @ChannelID";
                command.Parameters.Add("@ChannelID", SqlDbType.Int).Value = channelID;
                command.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID;
            }

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    status = reader.GetInt32(0);
                    int maxstatus = reader.GetInt32(1);

                    if (status == 0 && maxstatus == 10) // falg in progress for production
                        status = 5;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR: GetFileName() - " + ex.Message);
                return 0;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return status;

        }

        public bool GetProductionChannels(int productionID, ref List<ChannelInfo> channelInfoList, out string errmsg)
        {
            errmsg = "";
            channelInfoList.Clear();

            string sqlStmt = "SELECT DISTINCT CS.ChannelID,CN.PDFtype FROM ChannelStatus CS WITH(NOLOCK) " +
                            "INNER JOIN PageTable P WITH(NOLOCK) ON P.MasterCopySeparationSet = CS.MasterCopySeparationSet " +
                            "INNER JOIN ChannelNames CN WITH(NOLOCK) ON CN.ChannelID = CS.ChannelID " +
                            "WHERE P.ProductionID = " + productionID.ToString();
            Global.logging.WriteLog("GetProductionChannels() - " + sqlStmt);
            SqlCommand command = new SqlCommand(sqlStmt, connection)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    channelInfoList.Add(new ChannelInfo()
                    {
                        ChannelID = reader.GetInt32(0),
                        PDFType = reader.GetInt32(1)
                    });                    
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }

        public bool GetUserDefaultViewer(string userName, out bool useHTML5, out string errmsg)
        {
            errmsg = "";
            useHTML5 = false; 

            string sqlStmt = "SELECT ISNULL(UseHTML5,0) FROM UserNames WHERE Username='" + userName + "'";

            SqlCommand command = new SqlCommand(sqlStmt, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    useHTML5 = reader.GetInt32(0) > 0;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }


        public bool GetPageNameFromMaster(ref string pageName, ref string sectionName, ref string editionName, int masterCopySeparationSet, out string errmsg)
        {
            errmsg = "";
            pageName = "";
            editionName = "";
            sectionName = "";

            string sqlStmt = "SELECT TOP 1 PageName,SectionID,EditionID FROM PageTable WITH (NOLOCK) WHERE Dirty=0 AND UniquePage=1 AND MasterCopySeparationSet="+masterCopySeparationSet.ToString();

            SqlCommand command = new SqlCommand(sqlStmt, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    pageName = reader.GetString(0);
                    sectionName = Globals.GetNameFromID("SectionNameCache", reader.GetInt32(1));
                    editionName = Globals.GetNameFromID("EditionNameCache", reader.GetInt32(2));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;

        }


        public bool UpdateDefaultUserViewer(string userName, bool HTML5viewer, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            int viewer = HTML5viewer ? 1 : 0;

            command.CommandText = "UPDATE UserNames SET UseHTML5=" + viewer.ToString() + " WHERE UserName='" + userName + "'";

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


        public bool UpdateExternalStatus(int masterCopySeparationSet, int externalStatusCode, out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "UPDATE PageTable SET ExternalStatus=" + externalStatusCode.ToString() + " WHERE MasterCopySeparationSet=" + masterCopySeparationSet.ToString();

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }


        public bool GetChannelType(int channelID, ref int channelType, out string errmsg)
        {
            errmsg = "";
            channelType = -1;

            string sqlStmt = "SELECT TOP 1 PDFtype FROM ChannelNames WHERE ChannelID=" + channelID.ToString();

            SqlCommand command = new SqlCommand(sqlStmt, connection);
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    channelType = reader.GetInt32(0) ;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        // Returns new PressRunID
        public int InsertNewSubEdition(int pressID, DateTime pubDate, int publicationID,  int editionID, int sectionID, int newEditionID, out string errmsg)
        {
            int pressRunID = 0;
            errmsg = "";
          
            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spInsertNewSubedition";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Value = pressID;
            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;
            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = editionID;
            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Value = sectionID;
            param = command.Parameters.Add("@NewEditionID", SqlDbType.Int);
            param.Value = newEditionID;

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    pressRunID= reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR in spInsertNewSubedition - " + errmsg);
                return -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return pressRunID;
        }



 
        public int spInsertPagesInSection(int pressID, DateTime pubDate, int publicationID, int editionID, int sectionID, int numberOfPages, int pageOffset, string miscstring3, out string errmsg)
        {
            int pressRunID = 0;
            errmsg = "";

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "spInsertPagesInSection";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Value = pressID;
            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;
            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = editionID;
            param = command.Parameters.Add("@SectionID", SqlDbType.Int);
            param.Value = sectionID;
            param = command.Parameters.Add("@NumberOfPages", SqlDbType.Int);
            param.Value = numberOfPages;
            param = command.Parameters.Add("@PageOffset", SqlDbType.Int);
            param.Value = pageOffset;

            param = command.Parameters.Add("@MiscString3", SqlDbType.VarChar,50);
            param.Value = miscstring3;

    

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    pressRunID = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                Global.logging.WriteLog("ERROR in spInsertPagesInSection - " + errmsg);
                return -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return pressRunID;
        }


        public bool DeleteEdition(int pressID, DateTime pubDate, int publicationID, int editionID,  out string errmsg)
        {
            int pressRunID = 0;
            int productionID = 0;
            GetPressRunID(publicationID, pubDate, ref editionID, 0, pressID, out pressRunID, out productionID, out errmsg);
            if (pressRunID >= 0)
                return false;

            errmsg = "";

            string sql = "DELETE FROM PageTable WHERE PressID=@PressID AND PublicationID=@PublicationID AND PubDate=@PubDate AND EditionID=@EditionID";
            SqlCommand command = new SqlCommand(sql, connection);

            SqlParameter param;
            param = command.Parameters.Add("@PressID", SqlDbType.Int);
            param.Value = pressID;
            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;
            param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
            param.Value = publicationID;
            param = command.Parameters.Add("@EditionID", SqlDbType.Int);
            param.Value = editionID;
            
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
      
            command.Parameters.Clear();
            command.CommandText = "DELETE FROM PressRunID WHERE PressRunID=" + pressRunID.ToString();
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {

                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool GetAdminGroupUserGroupRelations(string adminGroupName, ref string userGroupName, out string errmsg)
        {
            errmsg = "";
            userGroupName = "";

            string sqlStmt = string.Format("SELECT UserGroupName FROM AdmGroupUserGroupRelations WHERE AdmGroupName = '{0}'", adminGroupName);

            SqlCommand command = new SqlCommand(sqlStmt, connection);

            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    userGroupName = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }


        public DataTable GetLogCollection(string filter, ref DateTime tLastDateTime, out string errmsg)
        {
            errmsg = "";
            int nEntries = 0;

            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            int top = (int)HttpContext.Current.Application["MaxLogEntries"];
            if (top == 0)
                top = 250;
            int maxLength = (int)HttpContext.Current.Application["MaxLogMessageLength"];

            command.CommandText = "SELECT TOP " + top.ToString() + " EventTime,E.EventName,FileName,ErrorMsg,MiscString FROM Log WITH (NOLOCK) INNER JOIN EventCodes E ON E.EventNumber=Log.Event WHERE MiscString='" + filter + "' AND Event in (26,27,30) ORDER BY EventTime DESC";
            command.CommandType = CommandType.Text;

            DataTable tbl = new DataTable();

            DataColumn newColumn;
            newColumn = tbl.Columns.Add("Location", Type.GetType("System.String"));
            newColumn = tbl.Columns.Add("Time", Type.GetType("System.DateTime"));
            newColumn = tbl.Columns.Add("Status", Type.GetType("System.String"));
            newColumn = tbl.Columns.Add("FileName", Type.GetType("System.String"));        
            newColumn = tbl.Columns.Add("Message", Type.GetType("System.String"));

            DataRow newRow = null;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    nEntries++;
                    newRow = tbl.NewRow();
                    newRow["Time"] = reader.GetDateTime(0);
                    tLastDateTime = (DateTime)newRow["Time"];
                    newRow["Status"] = reader.GetString(1);
                    newRow["FileName"] = reader.GetString(2);
                   
                    string msg = reader.GetString(3);
                    if (msg.Length > maxLength)
                        msg = msg.Substring(0, maxLength);
                    newRow["Message"] = msg;
                    newRow["Location"] = reader.GetString(4);

                    tbl.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            if (nEntries == 0)
            {
                newRow = tbl.NewRow();
                newRow["Location"] = "n/a";
                newRow["Time"] = DateTime.MinValue;
                newRow["Status"] = "n/a";
                newRow["FileName"] = "n/a";                
                newRow["Message"] = "No log entries";
                tbl.Rows.Add(newRow);
            }
            return tbl;
        }

        public bool GetCustomPlanningPresses(int publicationID, DateTime pubDate, ref string pressName, out string errmsg)
        {
            pressName = "";
            errmsg = "";
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {

                command = new SqlCommand("spWebCenterSelectPress", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param;
                param = command.Parameters.Add("@PublicationID", SqlDbType.Int);
                param.Value = publicationID;
                param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
                param.Value = pubDate;

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    pressName = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection != null)
                    connection.Close();
            }

            return true;
        }

        public bool LoadNicePageFormatList(ref List<string> pageFormatList, ref List<int> pageFormatNupList, out string errmsg)
        {
            errmsg = "";
            pageFormatList.Clear();
            pageFormatNupList.Clear();

            SqlCommand command = new SqlCommand("SELECT DISTINCT N.PageFormatName,T.PagesDown*T.PagesAcross FROM NicePageFormatTemplateMapping N INNER JOIN TemplateConfigurations T ON N.TemplateID = T.TemplateID", connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    pageFormatList.Add(reader.GetString(0));
                    pageFormatNupList.Add(reader.GetInt32(1));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public bool LoadNiceTemplatePageFormatList(string pageFormatName, ref List<TemplateInfo> templateList, out string errmsg)
        {
            templateList.Clear();
            errmsg = "";

            string sql = "SELECT DISTINCT N.TemplateID,N.PressID,ISNULL(N.MarkGroups,''),T.TemplateName,T.PagesDown*T.PagesAcross,T.PageRotationList,T.PageNumberingFront,T.PageNumberingBack,T.PageNumberingFrontHalfWeb,T.PageNumberingBackHalfWeb FROM TemplateConfigurations T " + 
                         "INNER JOIN NicePageFormatTemplateMapping N ON N.TemplateID = T.TemplateID " +
                         "WHERE N.PageFormatName = '" + pageFormatName + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    templateList.Add(new TemplateInfo
                    {
                        templateID = reader.GetInt32(0),
                        pressID = reader.GetInt32(1),
                        markGroups = reader.GetString(2),
                        templateName = reader.GetString(3),
                        
                        nUP = reader.GetInt32(4),
                        assembleVertical = reader.GetString(5).Contains("1"),
                        frontPageList = reader.GetString(6),
                        backPageList = reader.GetString(7),
                        frontPageListHalfWeb = reader.GetString(8),
                        backPageListHalfWeb = reader.GetString(9),
                        plateCopies = 1

                    });
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading.
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

    }
}

