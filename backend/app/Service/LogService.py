# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :LogService.py
 @time   :2023/12/1 20:39
  '''
from ..models import Log
from ..Utils.GetIpClass import GetIpClass


# 自动记录日志
def addAutoLoginLog(name, personInfo, token, request):
    Log.objects.create(
        name=name,
        personInfo=personInfo,
        token=token,
        detail="使用token自动登录",
        ip=GetIpClass.getIpFromRequest(request)
    )


# 发送邮箱信息的日志
def addSendEmailLog(name, personInfo, token, request, email):
    Log.objects.create(
        name=name,
        personInfo=personInfo,
        token=token,
        detail=f"向邮箱：{email} 发送邮箱验证码成功",
        ip=GetIpClass.getIpFromRequest(request)
    )


def AddQrCodeLoginLog(name, personInfo, token, request):
    Log.objects.create(
        name=name,
        personInfo=personInfo,
        token=token,
        detail="扫码登录成功！",
        ip=GetIpClass.getIpFromRequest(request)
    )
# 增加课程日志


# 移除课程日志
