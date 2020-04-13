using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public static class DatabaseService
    {
        public static readonly string ConnectionString = $"Data Source={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Wfs04Test", "projects.sqlite")}";
        
        public static bool IsTableExist(string tableName)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var sql = $"select count(*) from sqlite_master where type like 'table' and name like '{tableName}'";
            var result = (long) new SqliteCommand(sql, connection).ExecuteScalar();
            connection.Close();
            return result > 0;
        }

        public static List<string> GetExistingProjects()
        {
            var result = new List<string>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            const string sql = "select name from sqlite_master where type like 'table' and name not like 'sqlite%'";
            var reader = new SqliteCommand(sql, connection).ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }
            connection.Close();
            return result;
        }

        public static void CreateNewProjectTable(string tableName)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var sql = $"create table {tableName}(id INTEGER primary key autoincrement not null, name TEXT, offset INTEGER, size INTEGER, start_datetime TEXT)";
            new SqliteCommand(sql, connection).ExecuteNonQuery();
            connection.Close();
        }

        public static void DeleteProjectTable(string tableName)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var sql = $"drop table {tableName}";
            new SqliteCommand(sql, connection).ExecuteNonQuery();
            connection.Close();
        }

        public static List<VideoItem> GetDataFromProjectTable(string tableName)
        {
            var result = new List<VideoItem>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var sql = $"select * from {tableName} limit 20";
            var reader = new SqliteCommand(sql, connection).ExecuteReader();
            while (reader.Read())
            {
                var item = new VideoItem()
                {
                    Name = reader.GetString(1),
                    Offset = reader.GetInt32(2),
                    Size = reader.GetInt32(3),
                    // StartDateTime = DateTime.ParseExact(reader.GetString(4), "yyyy-MM-dd hh:mm:ss", CultureInfo.CurrentCulture)
                    StartDateTime = reader.GetString(4)
                };
                result.Add(item);
            }
            connection.Close();
            return result;
        }

        public static ScanInfo GetScanInfo(string tableName)
        {
            var result = new ScanInfo();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var reader = new SqliteCommand($"select max(start_datetime), min(start_datetime) from {tableName}", connection).ExecuteReader();
            while (reader.Read())
            {
                result.MaxDateTime = reader.GetString(0);
                result.MinDateTime = reader.GetString(1);
            }
            reader = new SqliteCommand($"select count(1) from {tableName}", connection).ExecuteReader();
            while (reader.Read())
            {
                result.TotalCount = reader.GetInt32(0);
            }
            connection.Close();
            return result;
        }
    }
}