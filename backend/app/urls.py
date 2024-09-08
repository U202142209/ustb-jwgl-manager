# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :urls.py
 @time   :2023/11/12 16:50
  '''

from django.urls import path

from .Controller import index, Login, ClassesController, LogController
from .Controller.EmailController import sendEmailCode, verifyEmailCode, getMyEmail
from .Controller.WechatLoginController import StudentInfo
from .Controller.DownloadController import download

urlpatterns = [
    # 首页
    path('', index.Index.as_view(), name='index'),
    # 获取random token
    path("randToken/", index.CreateToken.as_view(), name="randToken"),
    # 扫码登录
    path("glht/Logon.do/", Login.getLoginSid.as_view(), name="glht/Logon.do"),
    # 检查登录状态
    path("connect/state/", Login.checkState.as_view(), name="connect/state"),
    # 详细信息
    path("info/", index.InfoView.as_view(), name="info"),
    # 学生个人信息详情页面
    path("stu-info/<str:param>/", StudentInfo, name="stu-info"),
    # 使用token实现无感自动登录
    path("auto-login/", Login.LoginByToken.as_view(), name="auto-login"),

    # 获取已经选择的课程
    path("getSelectedClasses/", ClassesController.getSelectedClasses, name="getSelectedClasses"),
    # 查询课程
    path("getClassDetail/", ClassesController.getClassDetail, name="getClassDetail"),
    # 将必修课、专业选修课、素质拓展课、跨专业选课、班班课程/
    path("getCoursesByType/", ClassesController.getCoursesByType, name="getCoursesByType"),
    path("searchqualityDevelopmentCourse/", ClassesController.searchqualityDevelopmentCourse,
         name="searchqualityDevelopmentCourse"),
    # 发送邮箱消息
    path("sendEmailCode/", sendEmailCode, name="sendEmailCode"),
    # 验证邮箱
    path("verifyEmailCode/", verifyEmailCode, name="verifyEmailCode"),
    # 获取用户的邮箱
    path("getMyEmail/", getMyEmail, name="getMyEmail"),

    # 日志板块
    path("log/addCourseLog/", LogController.AddCourseLog, name="AddCourseLog"),
    path("log/removeCourseLog/", LogController.RemoveCourseLog, name="RemoveCourseLog"),
    path("log/getCourseSuccessfullyLog/", LogController.getCourseSuccessfullyLog, name="getCourseSuccessfullyLog"),
    # 反馈
    path("feedback/", index.feedback, name="feedback"),
    # 下载
    path('download/', download, name='download'),
    # 首页
    path("home/", index.home, name="home"),

    # 迁移数据库
    # path("migrate/",)

]
