﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace DC
{
    /// <summary>
    /// Клас, який представляє собою таблицю DataTable
    /// </summary>
    internal sealed class GetMSSQLData : BaseDataTable
    {
        /// <summary>
        /// Тип, який працює з таблицею бази даних
        /// </summary>
        /// <param name="commandText">SQLCommand,яка повинна повертати таблицю бази даних</param>
        public GetMSSQLData(SqlCommand getCommand)
        {
            SqlConnection sc;
            Table = new DataTable();
            sc = new SqlConnection(SessionParameters.Connection);
            getCommand.Connection = sc;
            using (sc)
            {
                sc.Open();
                using (SqlDataReader r = getCommand.ExecuteReader())
                    Table.Load(r);
                sc.Close();
            }
        }


        public GetMSSQLData()
        { }



        /// <summary>
        /// Тип, який працює з таблицею бази даних
        /// </summary>
        /// <param name="table">Ім'я таблиці, яка копіюється з бази даних та присвоюється екземпляру типу</param>
        public GetMSSQLData(string table)
        {
            SqlConnection sc;
            Table = new DataTable();
            sc = new SqlConnection(SessionParameters.Connection);
            SqlCommand getPrimaryKey = sc.CreateCommand();
            getPrimaryKey.CommandText = string.Format(
                "SELECT column_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1 AND table_name = '{0}'",
                table);
            SqlCommand getCommand = sc.CreateCommand();
            getCommand.CommandText = "SELECT * FROM " + table;
            using (sc)
            {
                sc.Open();
                using (SqlDataReader reader = getCommand.ExecuteReader())
                    Table.Load(reader);
                using (SqlDataReader pkreader = getPrimaryKey.ExecuteReader())
                {
                    DataTable primaryKeyTable = new DataTable();
                    primaryKeyTable.Load(pkreader);
                    try
                    {
                        PrimaryKey = primaryKeyTable.Rows[0][0].ToString();
                    }
                    catch (Exception)
                    {

                    }
                }
                sc.Close();
            }
            Table.TableName = table;
        }


        /// <summary>
        /// Отримання таблиці за допомогою переданої строки підключення та SQL-команди 
        /// </summary>
        /// <param name="connectionString">Строка підключення до серверу</param>
        /// <param name="command">Команда вибірки даних</param>
        public GetMSSQLData(string connectionString, string command)
        {
            SqlConnection sc = new SqlConnection();
            sc.ConnectionString = connectionString;
            Table = new DataTable();
            SqlCommand getCommand = sc.CreateCommand();
            getCommand.CommandText = command;
            using (sc)
            {
                sc.Open();
                using (SqlDataReader reader = getCommand.ExecuteReader())
                    Table.Load(reader);
                sc.Close();
            }
        }
    }
}
