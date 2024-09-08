# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :WechatLoginController.py
 @time   :2023/11/14 17:26
  '''

from django.forms.models import model_to_dict
from django.http import HttpResponse
from django.shortcuts import render

from ..models import Student


def StudentInfo(request, param):
    # raise Exception("出错了")
    template_name = 'StudentsInfo.html'
    stu = Student.objects.filter(cookies=param).first()
    if stu:
        context = {
            "stu": model_to_dict(stu)
        }
        if not request.user.is_authenticated:
            return HttpResponse(f"姓名:{stu.name}，详情:{stu.detail}")
        else:
            return render(request, template_name, context=context)
    return HttpResponse("ok")
