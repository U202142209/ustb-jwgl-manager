# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :log.py
 @time   :2023/11/26 22:34
  '''

from django.core.cache import cache
from django.views.decorators.csrf import csrf_exempt
from django.views.decorators.http import require_POST

from ..Middleware.authToken import checkloginRestController, setError, setSuccess
from ..Utils.EmailService import EmailService
from ..Utils.GetIpClass import GetIpClass
from ..models import ClassesLog, Student


# 添加课程日志
@csrf_exempt
@require_POST
@checkloginRestController
def AddCourseLog(request):
    courseNumber = request.POST.get("courseNumber", "")
    NotificationNumber = request.POST.get("NotificationNumber", "")
    CourseName = request.POST.get("CourseName", "")
    MainTeacher = request.POST.get("MainTeacher", "")
    EnrollmentLimit = request.POST.get("EnrollmentLimit", "")
    CourseAvailability = request.POST.get("CourseAvailability", "")
    CourseType = request.POST.get("CourseType", "")
    Credits = request.POST.get("Credits", "Credits")
    if not (courseNumber and NotificationNumber and CourseName and MainTeacher):
        return setError("参数错误。")
    # 防止重复，一个小时之内只能添加一个
    key = request.session["name"] + courseNumber + NotificationNumber + CourseName
    if cache.get(key):
        return setError("已添加")
    ClassesLog.objects.create(
        course_name=CourseName,
        course_number=courseNumber,
        course_notice_number=NotificationNumber,
        course_teacher=MainTeacher,
        course_limited_num=EnrollmentLimit,
        course_allowance=CourseAvailability,
        course_type=CourseType,
        course_credit=Credits,
        stu_name=request.session["name"],
        desc="加入抢课任务队列",
        ip=GetIpClass().getIpFromRequest(request),
    )
    cache.set(key, "进入抢课任务队列", timeout=3600)  # 缓存1小时
    return setSuccess(msg="ok", data={})


# 记录移除课程日志
@csrf_exempt
@require_POST
@checkloginRestController
def RemoveCourseLog(request):
    courseNumber = request.POST.get("courseNumber", "")
    NotificationNumber = request.POST.get("NotificationNumber", "")
    CourseName = request.POST.get("CourseName", "")
    MainTeacher = request.POST.get("MainTeacher", "")
    EnrollmentLimit = request.POST.get("EnrollmentLimit", "")
    CourseAvailability = request.POST.get("CourseAvailability", "")
    CourseType = request.POST.get("CourseType", "")
    Credits = request.POST.get("Credits", "Credits")
    if not (courseNumber and NotificationNumber and CourseName and MainTeacher):
        return setError("参数错误。")
    key = "remove" + request.session["name"] + courseNumber + NotificationNumber + CourseName
    if cache.get(key):
        return setError("已添加")
    ClassesLog.objects.create(
        course_name=CourseName,
        course_number=courseNumber,
        course_notice_number=NotificationNumber,
        course_teacher=MainTeacher,
        course_limited_num=EnrollmentLimit,
        course_allowance=CourseAvailability,
        course_type=CourseType,
        course_credit=Credits,
        stu_name=request.session["name"],
        desc="移出任务队列",
        ip=GetIpClass().getIpFromRequest(request),
    )
    cache.set(key, "移出任务队列", timeout=60 * 10)  # 十分钟记录一次
    return setSuccess(msg="ok", data={})


# 抢课成功的日志
@csrf_exempt
@require_POST
@checkloginRestController
def getCourseSuccessfullyLog(request):
    courseNumber = request.POST.get("courseNumber", "")
    NotificationNumber = request.POST.get("NotificationNumber", "")
    CourseName = request.POST.get("CourseName", "")
    MainTeacher = request.POST.get("MainTeacher", "")
    EnrollmentLimit = request.POST.get("EnrollmentLimit", "")
    CourseAvailability = request.POST.get("CourseAvailability", "")
    CourseType = request.POST.get("CourseType", "")
    Credits = request.POST.get("Credits", "Credits")
    if not (courseNumber and NotificationNumber and CourseName and MainTeacher):
        return setError("参数错误。")
    # 记录系统日志
    # 发邮箱
    ClassesLog.objects.create(
        course_name=CourseName,
        course_number=courseNumber,
        course_notice_number=NotificationNumber,
        course_teacher=MainTeacher,
        course_limited_num=EnrollmentLimit,
        course_allowance=CourseAvailability,
        course_type=CourseType,
        course_credit=Credits,
        stu_name=request.session["name"],
        desc="抢课成功",
        ip=GetIpClass().getIpFromRequest(request),
    )
    # 获取用户信息，检查用户是否绑定邮箱
    email = Student.objects.filter(
        name=request.session["name"]
    ).values("email").first()
    if email:
        # 发送抢课得消息提醒
        email = email["email"]
        EmailService.sendGetCourseSuccessMsg(
            sebt_to=email,
            course_name=CourseName,
            course_number=courseNumber,
            course_notice_number=NotificationNumber,
            course_teacher=MainTeacher,
        )
        return setSuccess("ok", email)
    return setError("此用户没有绑定邮箱")
