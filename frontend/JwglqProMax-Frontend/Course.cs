using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwglqProMax_Frontend
{
    class Course
    {
        public string courseNumber;        // 课程号
        public string NotificationNumber;  // 通知单编号
        public string CourseName;          // 课程名称
        public string MainTeacher;         // 主讲教师;
        public string EnrollmentLimit;     // 限选人数;
        public string CourseAvailability;  // 课余量
        public string CourseType;          // 课程性质;
        public string Credits;             // 学分;

        public Course()
        {
            // Parameterless constructor
        }

        public Course(string cousrseNumber, string notificationNumber, string courseName, string mainTeacher, string enrollmentLimit, string courseAvailability, string courseType, string credits)
        {
            this.courseNumber = cousrseNumber;
            this.NotificationNumber = notificationNumber;
            this.CourseName = courseName;
            this.MainTeacher = mainTeacher;
            this.EnrollmentLimit = enrollmentLimit;
            this.CourseAvailability = courseAvailability;
            this.CourseType = courseType;
            this.Credits = credits;
        }

        public Dictionary<string, string> toDictionary()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("courseNumber", this.courseNumber);
            data.Add("NotificationNumber", this.NotificationNumber);
            data.Add("CourseName", this.CourseName);
            data.Add("MainTeacher", this.MainTeacher);
            data.Add("EnrollmentLimit", this.EnrollmentLimit);
            data.Add("CourseAvailability", this.CourseAvailability);
            data.Add("CourseType", this.CourseType);
            data.Add("Credits", this.Credits);
            return data;
        }

        public override string ToString()
        {
            return this.CourseName;
        }

    }

}
