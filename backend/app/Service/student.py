# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :student.py
 @time   :2023/11/12 22:31
  '''
from ..models import Student
from ..Utils.GetUserDataByCookies import GetUserDataByCookies
from ..Utils.GetIpClass import GetIpClass


def create_or_updateStudent(g: GetUserDataByCookies, request=None):
    """
    创建或者更新学生信息
    :param g:携带的cookies的工具类
    :param request : django视图函数的请求对象，用于获取IP地址
    """
    name = g.getStudentName()
    ClassNianjiZhuanYe = g.getClassNianjiZhuanYe()
    # 查询用户数据表中是否已经存在了这个cookie
    stu = Student.objects.filter(name=name).first()
    if not stu:
        stu = Student()
        # 有些字段是不需要要再次修改的
        stu.name = name
        # stu.xuehao = ""
        stu.detail = g.getdetail()
        stu.classname = ClassNianjiZhuanYe["class"]
        stu.grade = ClassNianjiZhuanYe["grade"]
        stu.major = ClassNianjiZhuanYe["major"]
        stu.xuanKeZhongXin = g.getxuanKeZhongXin()
        stu.jiaoXueJiHua = g.getjiaoXueJiHua()
    # 需要更新的数据
    ## 各种选课.....
    stu.compulsory = g.getClassesDetails(type="zybxk", kcfalx="zx", opener="zybxk")
    # stu.professionalElectives = g.getClassesDetails(type="zyxxk", kcfalx="zx", opener="zyxxk")
    stu.professionalElectives = g.getProfessionalElectives()
    # stu.qualityDevelopmentCourse = g.getClassesDetails(type="gxk", kcfalx="zx", opener="gxk")
    stu.qualityDevelopmentCourse = g.getqualityDevelopmentCourse()
    # stu.interdisciplinaryCourseSelection = g.getClassesDetails(type="kzyxk", kcfalx="zx", opener="kzyxk")
    stu.interdisciplinaryCourseSelection = g.getinterdisciplinaryCourseSelection()
    # stu.classLessons = g.getClassesDetails(type="bjkc", kcfalx="zx", opener="bjkc")
    stu.classLessons = g.getclassLessons()
    # stu.OtherCourseSelections = g.getClassesDetails(type="cxxk", kcfalx="zx", opener="qtxk")
    stu.OtherCourseSelections = g.getOtherCourseSelections()
    # stu.fuxuiClasses = g.getClassesDetails(type="fxxk", kcfalx="zx", opener="fxxk")
    stu.fuxuiClasses = g.getfuxuiClasses()
    stu.englishGrades = g.getGrades(type="english")
    stu.biYeShengXinXiHeDui = g.getBiYeShengXinXiHeDui()
    stu.xueYeWanChengDu = g.getxueYeWanChengDu()
    stu.cookies = g.cookies
    # if request:
    #     stu.ip = GetIpClass().getIpFromRequest(request)
    # else:
    #     stu.ip = "0.0.0.0"
    stu.grades = g.getGrades(type="class")
    stu.chuangXinXueFen = g.getGrades(type="chuangxinxuefen")
    stu.rangDetail = g.getGrades(type="rangeDetail")
    stu.selectedClasses = g.getSelectedClasses()
    # 学籍信息
    stu.xuejikapian=g.getXuejiXinxi()
    stu.save()
    # print("新增用户信息成功！！！")
