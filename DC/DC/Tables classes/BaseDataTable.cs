using System.Data;
using System.Data.SqlClient;
using System;
using System.Linq;

namespace DC
{
    /// <summary>
    /// Тип, який отримує дані з бази на основі строки підключення SystemSettings.Connection та команди в сигнатурі
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract class BaseDataTable
    {
        protected DataTable resultTable;

        public string PrimaryKey { get; protected set; }

        public static implicit operator BaseDataTable(DataTable resulttable)
        {
            return resulttable;
        }

        public void RejectChanges()
        {
            resultTable.RejectChanges();
        }

        public void AcceptChanges()
        {
            resultTable.AcceptChanges();
        }

        public static implicit operator DataTable(BaseDataTable gd)
        {
            return gd.resultTable;
        }

        public DataColumnCollection Columns
        {
            get { return resultTable.Columns; }
        }

        public DataRow NewRow()
        {
            return resultTable.NewRow();
        }

        public DataRowCollection Rows
        {
            get { return resultTable.Rows; }
        }
        public string TableName
        {
            get { return resultTable.TableName; }
            set { resultTable.TableName = value; }
        }

        public DataTable GetChanges()
        {
            return resultTable.GetChanges();
        }

        public DataView DefaultView
        {
            get { return resultTable.DefaultView; }
        }

        /// <summary>
        /// Повертає массив типу string, у якому знаходяться значення стовпця за його індексом
        /// </summary>
        /// <param name="k">Індекс стовпця, починаючи з 0</param>
        /// <returns></returns>
        public string[] GetColumnData(int k)
        {
            string[] array = new string[resultTable.Rows.Count];
            for (int i = 0; i < resultTable.Rows.Count; i++)
            {
                array.SetValue(resultTable.Rows[i][k].ToString(), i);
            }
            return array;
        }
    }
}
