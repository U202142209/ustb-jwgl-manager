# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :ClassesController.py
 @time   :2023/11/13 13:33
  '''

from django.views.decorators.http import require_POST
from django.views.decorators.csrf import csrf_exempt
from ..Middleware.authToken import checkloginRestController, setError, setSuccess
from ..Utils.GetUserDataByCookies import GetUserDataByCookies
from ..Config.Config import CourseConfig


# 这个函数目前没有被使用
@checkloginRestController
def getXuankeZhongxin(request):
    g = GetUserDataByCookies(cookies=request.session.get("cookie_str", "0"))
    data = g.getxuanKeZhongXin(jsonify=True)
    if not data:
        return setError(msg="获取选课中心数据失败，请检查是否登录超时（可重新登录）或联系技术支持解决问题。")
    return setSuccess(msg="ok", data=data)


# 获取已经选择的课程
@csrf_exempt
@require_POST
@checkloginRestController
def getSelectedClasses(request):
    g = GetUserDataByCookies(cookies=request.session.get("cookie_str", "a"))
    data = g.getSelectedClasses(
        jsonify=True, dqjx0502zbid=request.GET.get("dqjx0502zbid", CourseConfig.dqjx0502zbid))
    if not data:
        return setError(msg="获取课程数据失败，请检查是否登录超时（可重新登录）或联系技术支持解决问题。")
    return setSuccess(msg="ok", data=data)


@csrf_exempt
@require_POST
@checkloginRestController
def getClassDetail(request):
    # 需要 记录添加日志
    g = GetUserDataByCookies(cookies=request.session.get("cookie_str", "a"))
    data = g.getClassDetail(
        dqjx0502zbid=request.POST.get("dqjx0502zbid", CourseConfig.dqjx0502zbid),
        jx02id=request.POST.get("jx02id", "error ")
    )
    if not data:
        return setError(msg="获取课程信息失败，请检查课程编号是否输入正确，或此课程是否在本学期内存在")
    return setSuccess(msg="ok", data=data)


@csrf_exempt
@require_POST
@checkloginRestController
def getCoursesByType(request):
    ctype = request.POST.get("type", "a")
    g = GetUserDataByCookies(cookies=request.session.get("cookie_str", "a"))
    text = ""
    resData = []
    if ctype == "compulsory":
        text = "必修课"
        resData = g.getCompulsory(jsonify=True)
    elif ctype == "professionalElectives":
        text = "专业选修课"
        resData = g.getProfessionalElectives(jsonify=True)
    elif ctype == "qualityDevelopmentCourse":
        text = "素质拓展课"
        resData = g.getqualityDevelopmentCourse(jsonify=True)
    elif ctype == "interdisciplinaryCourseSelection":
        text = "跨专业选课"
        resData = g.getinterdisciplinaryCourseSelection(jsonify=True)
    elif ctype == "classLessons":
        text = "班班课程"
        resData = g.getclassLessons(jsonify=True)
    else:
        return setError("parameter error")
    data = {"type": text, "data": resData}
    return setSuccess(msg="ok", data=data)


@csrf_exempt
@require_POST
@checkloginRestController
def searchqualityDevelopmentCourse(request):
    g = GetUserDataByCookies(cookies=request.session.get("cookie_str", "a"))
    resData = g.searchqualityDevelopmentCourse(
        post_data=request.POST.dict())
    if resData:
        return setSuccess("ok", data={
            "type": "素质拓展课", "data":resData
        })
    return setError("未查询到课程...")
