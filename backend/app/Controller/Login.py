# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :Login.py
 @time   :2023/11/12 17:46
  '''

import threading
import traceback

import jwt
from django.utils.decorators import method_decorator
from django.views import View
from django.views.decorators.csrf import csrf_exempt

from ..Config.Config import TOKEN_secret_key
from ..Middleware.authToken import CheckToken, setError, setSuccess
from ..Service.LogService import addAutoLoginLog, AddQrCodeLoginLog
from ..Service.student import create_or_updateStudent
from ..Utils.Exceptions import ServerErrorException
from ..Utils.GetUserDataByCookies import GetUserDataByCookies
from ..Utils.jwxtService import JwAutoLogin
from ..Utils.tokenService import generate_token


@method_decorator(csrf_exempt, name='dispatch')
class getLoginSid(View):
    @CheckToken
    def post(self, request):
        res = JwAutoLogin().getToken()
        if not res:
            return setError("系统出错了，请联系管理员")
        return setSuccess(msg="ok", data=res)


@method_decorator(csrf_exempt, name='dispatch')
class checkState(View):
    @CheckToken
    def post(self, request):
        sid = request.POST.get("sid", "")
        rand_token = request.POST.get("rand_token", "")
        jsession_id = request.POST.get("jsession_id", "")
        server_id = request.POST.get("server_id", "")
        if sid and rand_token and jsession_id and server_id:
            ## 为了获取用户信息，这里采用手动验证的方式
            res = JwAutoLogin().getState(sid=sid, rand_token=rand_token, jsession_id=jsession_id, server_id=server_id)
            try:
                if res["text"]["state"] == 101:
                    return setError("等待用户扫描")
                elif res["text"]["state"] == 102:
                    return setError("二维码已扫描，请在手机上确认")
                elif res["text"]["state"] == 103:
                    return setError("会话已过期，请刷新重试")
                elif res["text"]["state"] == 104:
                    return setError("二维码已经失效，请重新获取")
                elif res["text"]["state"] == 200:
                    cookie_str = res["cookie_str"]
                    # 执行登录的代码
                    g = GetUserDataByCookies(cookies=cookie_str)
                    # 使用线程执行异步登录任务
                    threading.Thread(target=create_or_updateStudent, args=(g, request)).start()
                    # 设置返回值的参数信息
                    name = g.getStudentName()
                    personInfo = g.getClassNianjiZhuanYe()
                    token = generate_token(
                        payload={
                            "logined": True,
                            "name": name,
                            "personInfo": personInfo,
                            "cookie_str": cookie_str,
                        },
                        expire_time=60 * 60 * 3  # 三个小时
                    )
                    # 记录登录日志
                    threading.Thread(target=AddQrCodeLoginLog, args=(
                        name, personInfo, token, request
                    )).start()
                    return setSuccess(msg="ok", data={
                        "cookie_str": cookie_str,
                        "data": {
                            "name": name,
                            "personInfo": personInfo,
                            "token": token}})
                else:
                    raise ServerErrorException("状态码错误")
            except:
                traceback.print_exc()
                return setError("出错了，请刷新重新登录或联系管理员反馈问题")
        return setError(msg="参数错误")


@method_decorator(csrf_exempt, name='dispatch')
class LoginByToken(View):
    def post(self, request):
        try:
            authorization_header = request.META.get('HTTP_AUTHORIZATION')
            if not authorization_header:
                return setError(msg="token exists error")
            token = str(authorization_header).split(" ")[1]
            # 验证token
            pyload = jwt.decode(token, TOKEN_secret_key, algorithms=['HS256'])
            if pyload.get("logined", "") != True:
                return setError("not logined")
            cookie_str = pyload.get("cookie_str", "aa")
            # 判断cookie 是否过期
            g = GetUserDataByCookies(cookies=cookie_str)
            current_name = g.getStudentName()
            # 个人信息
            last_login_name = pyload.get("name", "...")
            if current_name != last_login_name:
                return setError("login failed.")
            # 登录成功
            # 使用线程执行异步登录任务
            threading.Thread(target=create_or_updateStudent, args=(g, request)).start()
            # 记录日志
            threading.Thread(target=addAutoLoginLog, args=(
                current_name, pyload.get("personInfo", "..."), token, request
            )).start()
            return setSuccess(msg="ok", data={
                "cookie_str": cookie_str,
                "data": {
                    "name": current_name,
                    "personInfo": pyload.get("personInfo", "..."),
                    "token": generate_token(
                        payload=pyload,
                        expire_time=60 * 60 * 3  # 三个小时
                    )
                }
            })
        except jwt.ExpiredSignatureError:
            return setError(msg="Token expired")  # token过期
        except jwt.InvalidTokenError:
            return setError(msg="Invalid token")  # token无效
        except Exception as e:
            traceback.print_exc()
            return setError(msg="server error")
