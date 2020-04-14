using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class Wfs04Service
    {
        private static byte ReadByte(ref FileStream reader, ref bool flag)
        {
            var result = reader.ReadByte();
            if (result == -1)
            {
                flag = true;
            }

            return (byte) result;
        }
        private static void FindNextSignature(ref FileStream reader)
        {
            // signature is [00 00 01 FC 02 12 74 48]
            var isSignatureFound = false;
            
            while (!isSignatureFound)
            {
                var byteFromStream = ReadByte(ref reader, ref isSignatureFound);
                // fileSize--;
                if (byteFromStream != 0x00) continue;
                
                byteFromStream = ReadByte(ref reader, ref isSignatureFound);
                // fileSize--;
                if (byteFromStream != 0x00) continue;
                
                byteFromStream = ReadByte(ref reader, ref isSignatureFound);
                // fileSize--;
                if (byteFromStream != 0x01) continue;
                
                byteFromStream = ReadByte(ref reader, ref isSignatureFound);
                // fileSize--;
                if (byteFromStream != 0xFC) continue;
                
                byteFromStream = ReadByte(ref reader, ref isSignatureFound);
                // fileSize--;
                if (byteFromStream != 0x02) continue;
                
                byteFromStream = ReadByte(ref reader, ref isSignatureFound);
                // fileSize--;
                if (byteFromStream != 0x19) continue;
                
                byteFromStream = ReadByte(ref reader, ref isSignatureFound);
                // fileSize--;
                if (byteFromStream != 0x74) continue;
                
                byteFromStream = ReadByte(ref reader, ref isSignatureFound);
                // fileSize--;
                if (byteFromStream == 0x48)
                {
                    isSignatureFound = true;
                }
            }
        }
        
        private static void WriteAndFindNextSignature(ref FileStream reader, string fileName, byte[] timestamp)
        {
            var signature = new byte[] {0x00, 0x00, 0x01, 0xFC, 0x02, 0x19, 0x74, 0x48};
            var isSignatureFound = false;
            var newFs = File.Create(fileName);
            newFs.Write(signature);
            newFs.Write(timestamp);

            while (!isSignatureFound)
            {
                var byteBuffer = new List<byte>();
                var byteFromStream = (byte) reader.ReadByte();

                if (byteFromStream != 0x00)
                {
                    newFs.WriteByte(byteFromStream);
                    continue;
                }

                byteBuffer.Add(byteFromStream);
                byteFromStream = (byte) reader.ReadByte();
                if (byteFromStream != 0x00)
                {
                    newFs.Write(byteBuffer.ToArray());
                    newFs.WriteByte(byteFromStream);
                    byteBuffer.Clear();
                    continue;
                }
                
                byteBuffer.Add(byteFromStream);
                byteFromStream = (byte) reader.ReadByte();
                if (byteFromStream != 0x01) 
                {
                    newFs.Write(byteBuffer.ToArray());
                    newFs.WriteByte(byteFromStream);
                    byteBuffer.Clear();
                    continue;
                }
                
                byteBuffer.Add(byteFromStream);
                byteFromStream = (byte) reader.ReadByte();
                if (byteFromStream != 0xFC) 
                {
                    newFs.Write(byteBuffer.ToArray());
                    newFs.WriteByte(byteFromStream);
                    byteBuffer.Clear();
                    continue;
                }
                
                byteBuffer.Add(byteFromStream);
                byteFromStream = (byte) reader.ReadByte();
                if (byteFromStream != 0x02) 
                {
                    newFs.Write(byteBuffer.ToArray());
                    newFs.WriteByte(byteFromStream);
                    byteBuffer.Clear();
                    continue;
                }
                
                byteBuffer.Add(byteFromStream);
                byteFromStream = (byte) reader.ReadByte();
                if (byteFromStream != 0x19) 
                {
                    newFs.Write(byteBuffer.ToArray());
                    newFs.WriteByte(byteFromStream);
                    byteBuffer.Clear();
                    continue;
                }
                
                byteBuffer.Add(byteFromStream);
                byteFromStream = (byte) reader.ReadByte();
                if (byteFromStream != 0x74) {
                    newFs.Write(byteBuffer.ToArray());
                    newFs.WriteByte(byteFromStream);
                    byteBuffer.Clear();
                    continue;
                }
                
                byteBuffer.Add(byteFromStream);
                byteFromStream = (byte) reader.ReadByte();
                if (byteFromStream != 0x48)
                {
                    newFs.Write(byteBuffer.ToArray());
                    newFs.WriteByte(byteFromStream);
                    byteBuffer.Clear();
                    continue;
                }
                
                newFs.Close();
                isSignatureFound = true;
            }
        }
        
        //расшифровка временного блока
        public static DateTime GetTimeFromSignatureTimestamp(byte[] timeBytes)
        {

            var timeString = string.Join("", timeBytes.Reverse().Select(item => Convert.ToString(item, 2).PadLeft(8, '0')));
            
            var year = Convert.ToInt32(timeString.Substring(0, 6), 2) + 2000;
            var month = Convert.ToInt32(timeString.Substring(6, 4), 2);
            var day = Convert.ToInt32(timeString.Substring(10, 5), 2);
            var hours= Convert.ToInt32(timeString.Substring(15, 5), 2);
            var minutes = Convert.ToInt32(timeString.Substring(20, 6), 2);
            var seconds = Convert.ToInt32(timeString.Substring(26, 6), 2);

            return new DateTime(year, month, day, hours, minutes, seconds);

        }
        
        public static void ExtractVideos(string tableName, string fileNameOrDeviceId, string pathToVideos, DateTime start, DateTime end)
        {
            using var connection = new SqliteConnection(DatabaseService.ConnectionString);
            var startTimeString = start.ToString("yyyy-MM-dd hh:mm:ss");
            var endTimeString = end.ToString("yyyy-MM-dd hh:mm:ss");
            var sql = $"select * from {tableName} where start_datetime >= datetime('{startTimeString}') and start_datetime <= datetime('{endTimeString}')";
            connection.Open();
            var reader = new SqliteCommand(sql, connection).ExecuteReader();
            while (reader.Read())
            {
                var item = new VideoItem()
                {
                    Name = reader.GetString(1),
                    Offset = reader.GetInt64(2),
                    Size = reader.GetInt32(3),
                    StartDateTime = reader.GetString(4)
                };
                using var deviceReader = new FileStream(fileNameOrDeviceId, FileMode.Open, FileAccess.Read, FileShare.None, item.Size) {Position = item.Offset};
                using var newFileReader = File.Create(Path.Combine(pathToVideos, item.Name));
                var buffer = new byte[item.Size];
                deviceReader.ReadAsync(buffer, 0, item.Size);
                newFileReader.Write(buffer);
            }
            connection.Close();
        }
        
        public static void ExtractVideos(string tableName, string fileNameOrDeviceId, string pathToVideos)
        {
            using var connection = new SqliteConnection(DatabaseService.ConnectionString);
            var sql = $"select * from {tableName}";
            connection.Open();
            var reader = new SqliteCommand(sql, connection).ExecuteReader();
            while (reader.Read())
            {
                var item = new VideoItem()
                {
                    Name = reader.GetString(1),
                    Offset = reader.GetInt32(2),
                    Size = reader.GetInt32(3),
                    StartDateTime = reader.GetString(4)
                };
                using var deviceReader = new FileStream(fileNameOrDeviceId, FileMode.Open, FileAccess.Read, FileShare.None, item.Size) {Position = item.Offset};
                using var fileReader = File.Create(Path.Combine(pathToVideos, item.Name));
                var buffer = new byte[item.Size];
                deviceReader.ReadAsync(buffer, 0, item.Size);
                fileReader.Write(buffer);
            }
            connection.Close();
        }

        public static void StartFindingFromFile(string filePath, string pathToVideos, DateTime start, DateTime end)
        {
            var totalIterations = 1;
            var number = 0;
            // var deviceReader = new FileStream(device.DeviceId, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.Asynchronous);
            var deviceReader = new FileStream(filePath, FileMode.Open);

            var isContinue = true;

            FindNextSignature(ref deviceReader);
            while (isContinue) 
            {
                if (deviceReader.Position < deviceReader.Length)
                {
                    var timestamp = new []
                    {
                        (byte) deviceReader.ReadByte(), 
                        (byte) deviceReader.ReadByte(), 
                        (byte) deviceReader.ReadByte(),
                        (byte) deviceReader.ReadByte()
                    };
                    // await deviceReader.ReadAsync(timestamp, 0, 4);
                    var signatureDateTime = GetTimeFromSignatureTimestamp(timestamp);
                    if (signatureDateTime >= start && signatureDateTime <= end)
                    {
                        WriteAndFindNextSignature(ref deviceReader, $"{pathToVideos}\\signature{Convert.ToString(number).PadLeft(8, '0')}.h264", timestamp);
                        number++;
                    }
                    else
                    {
                        FindNextSignature(ref deviceReader);
                        Console.WriteLine($"totalIterations: {totalIterations}");
                        totalIterations++;
                    }
                }
                else
                {
                    isContinue = false;
                }
            }
        }
        
        public static void StartScanFile(string filePath, string tableName)
        {
            using var connection = new SqliteConnection(DatabaseService.ConnectionString);
            connection.Open();
            
            var deviceReader = new FileStream(filePath, FileMode.Open);
            var isContinue = true;
            var totalIterations = 1;
            
            FindNextSignature(ref deviceReader);
            var startOfFile = deviceReader.Position - 8;
            long startOfNext;
            
            while (isContinue) 
            {
                if (deviceReader.Position < deviceReader.Length)
                {
                    var timestamp = new []
                    {
                        (byte) deviceReader.ReadByte(), 
                        (byte) deviceReader.ReadByte(), 
                        (byte) deviceReader.ReadByte(),
                        (byte) deviceReader.ReadByte()
                    };
                    
                    var signatureDateTime = GetTimeFromSignatureTimestamp(timestamp).ToString("yyyy-MM-dd hh:mm:ss");
                    FindNextSignature(ref deviceReader);
                    startOfNext = deviceReader.Position - 8;
                    new SqliteCommand(
                        $"insert into {tableName} (name, offset, size, start_datetime) values ('signature{totalIterations.ToString().PadLeft(8, '0')}.h264', {startOfFile}, {startOfNext-startOfFile}, '{signatureDateTime}')",
                        connection).ExecuteNonQuery();
                    startOfFile = startOfNext;

                    Console.WriteLine($"totalIterations: {totalIterations}");
                    totalIterations++;
                }
                else
                {
                    isContinue = false;
                }
            }
            connection.Close();
        }

        public static void StartScanDevice(PhysicalDiskItem device, string tableName)
        {
            using var connection = new SqliteConnection(DatabaseService.ConnectionString);
            connection.Open();
            
            var deviceReader = new FileStream(device.DeviceId, FileMode.Open);

            var isContinue = true;
            var totalIterations = 1;
            
            FindNextSignature(ref deviceReader);
            var startOfFile = deviceReader.Position - 8;
            long startOfNext;
            
            while (isContinue) 
            {
                if ((ulong) deviceReader.Position < device.Size)
                {
                    var timestamp = new []
                    {
                        (byte) deviceReader.ReadByte(), 
                        (byte) deviceReader.ReadByte(), 
                        (byte) deviceReader.ReadByte(),
                        (byte) deviceReader.ReadByte()
                    };
                    
                    var signatureDateTime = GetTimeFromSignatureTimestamp(timestamp).ToString("yyyy-MM-dd hh:mm:ss");
                    FindNextSignature(ref deviceReader);
                    startOfNext = deviceReader.Position - 8;
                    new SqliteCommand(
                        $"insert into {tableName} (name, offset, size, start_datetime) values ('signature{totalIterations.ToString().PadLeft(8, '0')}.h264', {startOfFile}, {startOfNext-startOfFile}, '{signatureDateTime}')",
                        connection).ExecuteNonQuery();
                    startOfFile = startOfNext;

                    Console.WriteLine($"totalIterations: {totalIterations}");
                    totalIterations++;
                }
                else
                {
                    isContinue = false;
                }
            }
            connection.Close();
        }
    }
}