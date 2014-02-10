using System.Data.SqlClient;
using System.Data;
using System;
using System.Windows.Controls;
using System.Threading.Tasks;
namespace DC
{
    class SQLUpdater : IUpdate
    {
        BaseDataTable Source;

        public void Update(BaseDataTable sourceTable, ItemCollection connectionArray)
        {
            Source = new GetMSSQLData(sourceTable.TableName);
            DataTable changesTable = sourceTable.GetChanges();

            string command;
            bool exist = false;
            foreach (DataRow dr in changesTable.Rows)
            {
                for (int i = 0; i < Source.Rows.Count; i++)
                {
                    if (dr[Source.PrimaryKey].ToString() == Source.Rows[i][Source.PrimaryKey].ToString())
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist)
                    command = BuildUpdateCommand(dr);
                else
                    command = BuildInsertCommand(dr);

                foreach (string s in connectionArray)
                {
                    Task.Factory.StartNew(() =>
                    {
                        using (SqlConnection updateConnection = new SqlConnection())
                        {
                            if (SessionParameters.IntegratedSecurity)
                                updateConnection.ConnectionString = string.Format("Server={0};DataBase={1};Integrated security=true", s.Split('/')[0], s.Split('/')[1]);
                            else
                                updateConnection.ConnectionString = string.Format("Server={0};DataBase={1}; UID={2};PWD={3}", s.Split('/')[0], s.Split('/')[1], SessionParameters.Login, SessionParameters.Password);

                            SqlCommand updateCommand = updateConnection.CreateCommand();
                            updateCommand.CommandText = command;
                            updateConnection.Open();
                            updateCommand.ExecuteNonQuery();
                            updateConnection.Close();
                        }

                    });
                    Task.WaitAll();
                }
            }
        }

        public string BuildUpdateCommand(DataRow dr)
        {
            string returnString = string.Format("UPDATE {0} SET ", Source.TableName);
            for (int i = 0; i < dr.ItemArray.Length; i++)
            {
                if (Source.Columns[i].ColumnName != Source.PrimaryKey)
                {
                    if (Source.Columns[i].ColumnName == "vcChangeDate")
                        returnString += string.Format("{0}='{1}',", Source.Columns[i].ColumnName, DateTime.Now);
                    else
                        returnString += string.Format("{0}='{1}',", Source.Columns[i].ColumnName, dr.ItemArray[i].ToString());
                }
            }
            returnString = returnString.Substring(0, returnString.Length - 1);
            returnString += string.Format(" WHERE {0}='{1}'", Source.Columns[Source.PrimaryKey].ColumnName, dr[Source.PrimaryKey].ToString());
            return returnString;
        }

        public string BuildInsertCommand(DataRow dr)
        {
            string where = "(";
            string what = "VALUES (";
            string returnString = string.Format("INSERT INTO {0} ", Source.TableName);
            for (int i = 0; i < dr.ItemArray.Length; i++)
            {
                if (Source.Columns[i].ColumnName != Source.PrimaryKey)
                {
                    if (Source.Columns[i].ColumnName == "vcChangeDate")
                    {
                        where += string.Format("{0},", Source.Columns[i].ColumnName);
                        what += string.Format("'{0}',", DateTime.Now);
                    }
                    else
                    {
                        where += string.Format("{0},", Source.Columns[i].ColumnName);
                        what += string.Format("'{0}',", dr[i].ToString());
                    }
                }
            }

            returnString += string.Format("{1}) {2}) ", where.Substring(0, where.Length - 1), what.Substring(0, what.Length - 1));
            return returnString;
        }
    }
}
