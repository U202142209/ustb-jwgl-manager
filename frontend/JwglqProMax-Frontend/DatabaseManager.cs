using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace JwglqProMax_Frontend
{
    class DatabaseManager
    {
        // 数据库文件路径
        private string tempFolderPath = "temp";
        private string dbPath = "temp/data.db";
        private SQLiteConnection connection;
        public DatabaseManager()
        {
            // 检查 temp 文件夹是否存在，不存在则创建
            if (!System.IO.Directory.Exists(tempFolderPath))
            {
                System.IO.Directory.CreateDirectory(tempFolderPath);
            }
            // 检查数据库文件是否存在，不存在则创建
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            // 链接数据库
            // 创建连接对象
            string connectionString = $"Data Source={dbPath};Version=3;";
            this.connection = new SQLiteConnection(connectionString);
            // 打开链接
            this.connection.Open();
            // 创建登录日志表
            this.CreateTables(connection);
        }

        // 创建日志表
        public void CreateTables(SQLiteConnection connection)
        {
            string createLoginLogTableSql = @"
            CREATE TABLE IF NOT EXISTS login_log (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name VARCHAR(100) NOT NULL,
                cookie TEXT NOT NULL,
                token TEXT NOT NULL,
                created_time DATETIME DEFAULT CURRENT_TIMESTAMP
            )";

            using (SQLiteCommand command = new SQLiteCommand(createLoginLogTableSql, connection))
            {
                command.ExecuteNonQuery();
            }

            // 创建选课日志表
            string createCourseLogTableSql = @"
            CREATE TABLE IF NOT EXISTS addCourseLog (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                courseNumber VARCHAR(200) NOT NULL,
                notificationNumber VARCHAR(200) NOT NULL,
                courseName VARCHAR(200) NOT NULL,
                mainTeacher VARCHAR(200) NOT NULL,
                enrollmentLimit VARCHAR(200) NOT NULL,
                courseAvailability VARCHAR(200) NOT NULL,
                courseType VARCHAR(200) NOT NULL,
                credits VARCHAR(200) NOT NULL,
                createTime DATETIME DEFAULT CURRENT_TIMESTAMP
            )";

            using (SQLiteCommand command = new SQLiteCommand(createCourseLogTableSql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        // 析构函数 // 关闭数据库链接
        ~DatabaseManager()
        {
            // this.connection.Close();
        }

        // 插入一条登录日志数据
        public int InsertLoginLog(string name, string cookie, string token)
        {
            string insertSql = @"
            INSERT INTO login_log (name, cookie, token)
            VALUES (@name, @cookie, @token);
            SELECT last_insert_rowid();";
            using (SQLiteCommand command = new SQLiteCommand(insertSql, this.connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@cookie", cookie);
                command.Parameters.AddWithValue("@token", token);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // 修改登录日志数据
        public void UpdateLoginLog(SQLiteConnection connection, int id, string name, string cookie, string token)
        {
            string updateSql = @"
            UPDATE login_log
            SET name = @name, cookie = @cookie, token = @token
            WHERE id = @id";

            using (SQLiteCommand command = new SQLiteCommand(updateSql, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@cookie", cookie);
                command.Parameters.AddWithValue("@token", token);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        // 查询登录日志数据
        public LoginLog GetLoginLog(SQLiteConnection connection, int id)
        {
            string selectSql = @"
            SELECT id, name, cookie, token, created_time
            FROM login_log
            WHERE id = @id";

            using (SQLiteCommand command = new SQLiteCommand(selectSql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new LoginLog
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Cookie = reader.GetString(2),
                            Token = reader.GetString(3),
                            CreatedTime = reader.GetDateTime(4)
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        // 删除登录日志数据
        public void DeleteLoginLog(SQLiteConnection connection, int id)
        {
            string deleteSql = @"
            DELETE FROM login_log
            WHERE id = @id";

            using (SQLiteCommand command = new SQLiteCommand(deleteSql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        // 删除所有的登录日志数据
        public void DeleteAllLoginLog()
        {
            string deleteSql = @"DELETE FROM login_log";
            using (SQLiteCommand command = new SQLiteCommand(deleteSql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
        // 查询最近登录日志数据
        public LoginLog GetLatestLoginLog()
        {
            string selectSql = @"
        SELECT id, name, cookie, token, created_time
        FROM login_log  ORDER BY id DESC LIMIT 1";

            using (SQLiteCommand command = new SQLiteCommand(selectSql, this.connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new LoginLog
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Cookie = reader.GetString(2),
                            Token = reader.GetString(3),
                            CreatedTime = reader.GetDateTime(4)
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 增加课程信息
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int AddCourseLog(Course c)
        {
            string selectSql = "SELECT COUNT(*) FROM addCourseLog WHERE notificationNumber = @courseNumber";
            using (SQLiteCommand selectCommand = new SQLiteCommand(selectSql, this.connection))
            {
                selectCommand.Parameters.AddWithValue("@courseNumber", c.NotificationNumber);
                if (Convert.ToInt32(selectCommand.ExecuteScalar()) > 0)
                {                    
                    return -1;
                }
            }

            string insertSql = @"
    INSERT INTO addCourseLog (courseNumber, notificationNumber, courseName, mainTeacher,enrollmentLimit,courseAvailability,courseType,credits)
    VALUES (@courseNumber, @notificationNumber, @courseName, @mainTeacher,@enrollmentLimit,@courseAvailability,@courseType,@credits);
    SELECT last_insert_rowid();";
            using (SQLiteCommand insertCommand = new SQLiteCommand(insertSql, this.connection))
            {
                insertCommand.Parameters.AddWithValue("@courseNumber", c.courseNumber);
                insertCommand.Parameters.AddWithValue("@notificationNumber",c.NotificationNumber);
                insertCommand.Parameters.AddWithValue("@courseName", c.CourseName);
                insertCommand.Parameters.AddWithValue("@mainTeacher", c.MainTeacher);
                insertCommand.Parameters.AddWithValue("@enrollmentLimit", c.EnrollmentLimit);
                insertCommand.Parameters.AddWithValue("@courseAvailability", c.CourseAvailability);
                insertCommand.Parameters.AddWithValue("@courseType", c.CourseType);
                insertCommand.Parameters.AddWithValue("@credits", c.Credits);
                return Convert.ToInt32(insertCommand.ExecuteScalar());
            }
        }

        /// <summary>
        /// 查询课程日志数据
        /// </summary>
        /// <returns></returns>
        public Course[] GetAllCourseLogs()
        {
            string selectSql = "SELECT * FROM addCourseLog";
            using (SQLiteCommand selectCommand = new SQLiteCommand(selectSql, this.connection))
            {
                using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                {
                    List<Course> courses = new List<Course>();
                    while (reader.Read())
                    {
                        Course course = new Course();
                        course.courseNumber = reader.GetString(1);
                        course.NotificationNumber = reader.GetString(2);
                        course.CourseName = reader.GetString(3);
                        course.MainTeacher = reader.GetString(4);
                        course.EnrollmentLimit = reader.GetString(5);
                        course.CourseAvailability = reader.GetString(6);
                        course.CourseType = reader.GetString(7);
                        course.Credits = reader.GetString(8);
                        courses.Add(course);
                    }
                    return courses.ToArray();
                }
            }
        }

        public int DeleteCourseLog(string notificationNumber)
        {
            string deleteSql = @"DELETE FROM addCourseLog 
            WHERE notificationNumber = @notificationNumber";

            using (SQLiteCommand command = new SQLiteCommand(deleteSql, this.connection))
            {
                command.Parameters.AddWithValue("@notificationNumber", notificationNumber);
                return command.ExecuteNonQuery();
            }
        }
    }


    public class LoginLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cookie { get; set; }
        public string Token { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
