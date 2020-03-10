using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebCenter4.Classes
{
    public class PPMDBaccess
    {
        SqlConnection connection;

        public PPMDBaccess()
        {
            connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionStringPPM"]);
        }

        public void CloseAll()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public bool LoadPublications(ref List<PPMPublication> publications, out string errmsg)
        {
            publications.Clear();
            errmsg = "";

            string sql = "SELECT LONG_NAME,SHORT_NAME,ISNULL(PRESS.NAME,'ODE'),ISNULL(SATS.NAME,'Tabloid'),ISNULL(ZONESNAME.NAME,'1'),PRO_NAME.DEF_OPLAG,ISNULL(PAPIR.NAME,'40g') FROM PRO_NAME " +
                        "LEFT OUTER JOIN PRESS ON PRESS.ID=PRO_NAME.PRESS_ID " +
                        "LEFT OUTER JOIN SATS ON SATS.ID=PRO_NAME.SATS_FORMAT " +
                        "LEFT OUTER JOIN ZONESNAME ON ZONESNAME.ID=CAST(PRO_NAME.DEF_ZONES as int) " +
                        "LEFT OUTER JOIN PAPIR ON PAPIR.ID = PRO_NAME.PAPIR_ID "+
                        "WHERE PRO_NAME.ProType<>10 " +
                        "ORDER BY LONG_NAME ";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 60
                
            };

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    publications.Add(new PPMPublication()
                    {
                        Long_name = reader.GetString(0).Trim(),
                        Short_name = reader.GetString(1).Trim(),
                        Default_PressGroup = reader.GetString(2).Trim(),
                        Default_PageFormat = reader.GetString(3).Trim(),
                        Default_Zone = reader.GetString(4).Trim(),
                        Default_copies = reader.GetInt32(5),
                        Default_Paper = reader.GetString(6).Trim()
                    });
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

        public bool LoadEditions(ref List<string> editions, out string errmsg)
        {
            editions.Clear();
            errmsg = "";

            string sql = "SELECT DISTINCT NAME FROM ZonesName WHERE NAME <> '0' ORDER BY NAME";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 60

            };

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    editions.Add(reader.GetString(0).Trim());
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

        public bool LoadPageFormats(ref List<PPMPageFormat> pageFormats, out string errmsg)
        {
            pageFormats.Clear();
            errmsg = "";

            string sql = "SELECT DISTINCT NAME,HOJDE,BREDE FROM SATS ORDER BY NAME";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 60

            };

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    pageFormats.Add(new PPMPageFormat()
                    {
                        Name = reader.GetString(0).Trim(),
                        Height = reader.GetInt32(1),
                        Width = reader.GetInt32(2)
                    });
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


        public bool LoadPapers(ref List<string> papers, out string errmsg)
        {
            papers.Clear();
            errmsg = "";

            string sql = "SELECT DISTINCT NAME FROM PAPIR ORDER BY NAME";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 60

            };

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    papers.Add(reader.GetString(0).Trim());
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


        public bool LoadPresses(ref List<string> presses, out string errmsg)
        {
            presses.Clear();
            errmsg = "";

            string sql = "SELECT DISTINCT NAME FROM PRESS ORDER BY NAME";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 60

            };

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    presses.Add(reader.GetString(0).Trim());
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

        public int LookupPlan(string pressGroup, string publication, DateTime pubDate, out string errmsg)
        {
            errmsg = "";
            int id = 0;

            string sql = "SELECT DATA.ID FROM DATA INNER JOIN PRESS ON PRESS.ID=DATA.PRESS WHERE (DATA.LONG_NAME=@ProductName OR DATA.SHORT_NAME=@ProductName) AND DATA.UDG_DATO=@PubDate AND PRESS.NAME=@PressGroup AND DATA.SEND_TO_CC = 100";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 60
            };
            command.Parameters.Clear();
            SqlParameter param = command.Parameters.Add("@PressGroup", SqlDbType.VarChar, 50);
            param.Value = pressGroup;
            param = command.Parameters.Add("@ProductName", SqlDbType.VarChar, 50);
            param.Value = publication;
            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = pubDate;

            SqlDataReader reader = null;
            try
            {
                // Execute the command
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    id = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                id = -1;
            }
            finally
            {

                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return id;

        }

        public bool InsertPlan(PPMPlan plan, out string errmsg)
        {
            errmsg = "";

            string sql = "INSERT INTO PlanExportRequests (PressGroup,ProductName,PubDate,Editions,Sections,PageFormat,Copies,Comment,AllCommonSubeditions,UpdateTime,Status,UploadedFiles,TrimWidth,TrimHeight) " +
                         " VALUES (@PressGroup,@ProductName,@PubDate,@Editions,@Sections,@PageFormat,@Copies,@Comment,@AllCommonSubeditions,GETDATE(),@Status,@UploadedFiles,@TrimWidth,@TrimHeight)";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 60
            };
            command.Parameters.Clear();
            SqlParameter param = command.Parameters.Add("@PressGroup", SqlDbType.VarChar, 50);
            param.Value = plan.PressGroup;
            param = command.Parameters.Add("@ProductName", SqlDbType.VarChar, 50);
            param.Value = plan.Publication;
            param = command.Parameters.Add("@PubDate", SqlDbType.DateTime);
            param.Value = plan.PubDate;
            param = command.Parameters.Add("@Editions", SqlDbType.VarChar, 50);
            param.Value = plan.Editions;
            param = command.Parameters.Add("@Sections", SqlDbType.VarChar, 50);
            param.Value = plan.Sections;
            param = command.Parameters.Add("@PageFormat", SqlDbType.VarChar, 50);
            param.Value = plan.PageFormat;
            param = command.Parameters.Add("@Copies", SqlDbType.Int);
            param.Value = plan.Circulation;
            param = command.Parameters.Add("@Comment", SqlDbType.VarChar, 200);
            param.Value = plan.Comment;
            param = command.Parameters.Add("@AllCommonSubeditions", SqlDbType.Int);
            param.Value = plan.AllCommonSubeditions ? 1 : 0;
            param = command.Parameters.Add("@Status", SqlDbType.Int);
            param.Value = 1;
            param = command.Parameters.Add("@UploadedFiles", SqlDbType.VarChar, 2000);
            param.Value = plan.UploadedFiles;

            param = command.Parameters.Add("@TrimWidth", SqlDbType.Real);
            param.Value = plan.TrimWidth;

            param = command.Parameters.Add("@TrimHeight", SqlDbType.Real);
            param.Value = plan.TrimHeight;

            try
            {
                // Execute the command
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

    }

}