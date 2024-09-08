# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :EmailController.py
 @time   :2023/12/23 16:48
  '''
import threading

import jwt
from django.core.cache import cache
from django.views.decorators.csrf import csrf_exempt
from django.views.decorators.http import require_POST

from ..Config.Config import TOKEN_secret_key
from ..Middleware.authToken import checkloginRestController, setError, setSuccess
from ..Service.LogService import addSendEmailLog
from ..Utils.EmailService import EmailService
from ..models import Student


@csrf_exempt
@require_POST
@checkloginRestController
def sendEmailCode(request):
    email = request.POST.get("email", "").strip()
    if email and "@" in email:
        token = str(request.META.get('HTTP_AUTHORIZATION')).split(" ")[1]
        # 判断用户是否具有发送验证码的权限
        if cache.get("send_email_code_" + email):
            return setError("邮箱验证码已经发送，五分钟内只能发送一次")
        code = EmailService().sendVerificateCode(send_to=email)
        if not code:
            return setError("邮箱验证码发送失败.")
        # 加入缓存
        cache.set("send_email_code_" + email, code, timeout=60 * 5)
        pyload = jwt.decode(token, TOKEN_secret_key, algorithms=['HS256'])
        # 记录日志
        threading.Thread(target=addSendEmailLog, args=(
            pyload.get("name", "..."),
            pyload.get("personInfo", "..."),
            token, request, email
        )).start()
        return setSuccess(f"成功向邮箱:{email}发送了验证码\n请在五分钟内完成验证.", data={})
    return setError("请输入有效的邮箱地址")


@csrf_exempt
@require_POST
@checkloginRestController
def verifyEmailCode(request):
    code = request.POST.get("code", "")
    if not code:
        return setError("请输入邮箱验证码.")
    email = request.POST.get("email", "")
    if str(code).upper() == str(cache.get("send_email_code_" + email)).upper():
        token = str(request.META.get('HTTP_AUTHORIZATION')).split(" ")[1]
        pyload = jwt.decode(token, TOKEN_secret_key, algorithms=['HS256'])
        if Student.objects.filter(name=pyload["name"]).update(email=email) > 0:
            return setSuccess("邮箱验证成功")
        return setError("设置邮箱失败，请联系管理员解决")
    return setError("验证码不正确")


@csrf_exempt
@require_POST
@checkloginRestController
def getMyEmail(request):
    token = str(request.META.get('HTTP_AUTHORIZATION')).split(" ")[1]
    pyload = jwt.decode(token, TOKEN_secret_key, algorithms=['HS256'])
    email = Student.objects.filter(name=pyload["name"]).values("email").first()
    if email:
        return setSuccess("ok", email)
    return setError("not exists")

