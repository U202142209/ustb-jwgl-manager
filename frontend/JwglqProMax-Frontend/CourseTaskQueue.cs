using System;
using System.Collections.Generic;
using System.Linq;

namespace JwglqProMax_Frontend
{
    // 任务列表
    class CourseTaskQueue
    {
        private static Dictionary<string, Course> courseDictionary = new Dictionary<string, Course>();

        public static Dictionary<string, Course> getCoursesQueue()
        {
            return courseDictionary;
        }

        /// <summary>
        /// 添加抢课任务
        /// </summary>
        /// <param name="course">需要添加的课程</param>
        /// <param name="msg">提示信息</param>
        /// <returns>是否添加成功</returns>
        public static bool AddCourse(Course course, ref string msg)
        {
            if (courseDictionary.ContainsKey(course.NotificationNumber))
            {
                msg = "此课程已经存在任务列表中，无法重复添加。";
                return false;
            }
            else
            {
                // 链接服务器，更新日志
                try
                {
                    NetWorkService.Post(Configration.BASICURL + "/log/addCourseLog/", course.toDictionary());
                }
                catch (Exception)
                {

                }
                courseDictionary.Add(course.NotificationNumber, course);
                msg = "Course added successfully.";
                // 向数据库中添加数据
                DatabaseManager db = new DatabaseManager();
                if (db.AddCourseLog(course) < 0)
                {
                    msg = "课程增加成功，但没有同步到数据库";
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 移除抢课任务
        /// </summary>
        /// <param name="NotificationNumber">需要移除的课程的通知单号</param>
        /// <param name="msg">提示消息信息</param>
        /// <returns>是否移除成功</returns>
        public static bool RemoveCourse(string NotificationNumber, ref string msg, bool isSuccess = false)
        {
            if (courseDictionary.ContainsKey(NotificationNumber))
            {
                // 链接服务器，更新日志
                try
                {
                    if (isSuccess)
                    {
                        // 发送抢课成功得日志信息
                            NetWorkService.Post(Configration.BASICURL + "/log/getCourseSuccessfullyLog/", courseDictionary[NotificationNumber].toDictionary());
                    }
                    else
                    {
                        // 发送移除队列得日志信息
                        NetWorkService.Post(Configration.BASICURL + "/log/removeCourseLog/", courseDictionary[NotificationNumber].toDictionary());
                    }
                }
                catch (Exception)
                {

                }
                courseDictionary.Remove(NotificationNumber);
                msg = "Course removed successfully.";
                return true;
            }
            else
            {
                msg = "Course with the given Notification Number does not exist.";
                return false;
            }
        }

        /// <summary>
        /// 修改课程信息
        /// </summary>
        /// <param name="NotificationNumber">需要修改的课程的通知单号</param>
        /// <param name="course">修改后的课程信息</param>
        /// <param name="msg">提示信息</param>
        /// <returns>是否修改成功</returns>
        public static bool EditCourse(string NotificationNumber, Course course, ref string msg)
        {
            if (courseDictionary.ContainsKey(NotificationNumber))
            {
                courseDictionary[NotificationNumber] = course;
                msg = "Course edited successfully.";
                return true;
            }
            else
            {
                msg = "Course with the given Notification Number does not exist.";
                return false;
            }
        }

        /// <summary>
        /// 判断给定的通知单号的课程是否已经存在
        /// </summary>
        /// <param name="NotificationNumber">通知单号</param>
        /// <returns>是否已经存在</returns>
        public static bool IsExists(string NotificationNumber)
        {
            return courseDictionary.ContainsKey(NotificationNumber);
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        public static void ClearCourseDictionary()
        {
            courseDictionary.Clear();
        }

        public static int Count()
        {
            return courseDictionary.Count();
        }
    }
}
