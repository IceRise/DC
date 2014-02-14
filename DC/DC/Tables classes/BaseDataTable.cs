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
    internal abstract class BaseDataTable
    {
        public DataTable Table;

        public string PrimaryKey { get; protected set; }

        
        /// <summary>
        /// Повертає массив типу string, у якому знаходяться значення стовпця за його індексом
        /// </summary>
        /// <param name="k">Індекс стовпця, починаючи з 0</param>
        /// <returns></returns>
        public string[] GetColumnData(int k)
        {
            string[] array = new string[Table.Rows.Count];
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                array.SetValue(Table.Rows[i][k].ToString(), i);
            }
            return array;
        }
    }
}
