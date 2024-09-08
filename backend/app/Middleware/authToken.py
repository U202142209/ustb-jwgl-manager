# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :authToken.py
 @time   :2023/11/12 18:01
  '''

import datetime
import traceback

import jwt
from django.http import JsonResponse

from ..Config.Config import TOKEN_secret_key


def get_now_time():
    return datetime.datetime.now().strftime('%F %T')


def setError(msg):
    return JsonResponse({
        "timestamp": get_now_time(),
        "code": 500,
        "msg": msg,
        "data": [],
    }, json_dumps_params={'ensure_ascii': False},
        content_type="application/json; charset=utf-8")


def setSuccess(msg, data=None):
    return JsonResponse({
        "code": 200,
        "msg": msg,
        "data": data,
    },
        json_dumps_params={'ensure_ascii': False},
        content_type="application/json; charset=utf-8")


def CheckToken(view_func):
    """
    基于类的视图
    """

    def wrapper(self, request, *args, **kwargs):
        try:
            authorization_header = request.META.get('HTTP_AUTHORIZATION')
            if authorization_header:
                token = str(authorization_header).split(" ")[1]
                # 验证token
                jwt.decode(token, TOKEN_secret_key, algorithms=['HS256'])
                return view_func(self, request, *args, **kwargs)
            return setError(msg="token exists error")
        except jwt.ExpiredSignatureError:
            # token过期
            return setError(msg="Token expired")
        except jwt.InvalidTokenError:
            # token无效
            return setError(msg="Invalid token")
        except Exception as e:
            traceback.print_exc()
            return setError(msg="server error")
        # cookies = request.GET.get("cookies", "0")
        # # 检查数据将中是否登录了
        # if Student.objects.filter(cookies=cookies).exists():
        #     return view_func(request, *args, **kwargs)
        # else:
        #     return setError(msg="未登录")

    return wrapper


# 判断用户是否登录
def checkloginRestController(view_func):
    """
    基于函数的视图，检查登录的状态
    """

    def wrapper(request, *args, **kwargs):
        try:
            authorization_header = request.META.get('HTTP_AUTHORIZATION')
            if authorization_header:
                token = str(authorization_header).split(" ")[1]
                # 验证token
                payload = jwt.decode(token, TOKEN_secret_key, algorithms=['HS256'])
                if not payload["logined"]:
                    return setError("not logined.")
                # 将信息写入session
                request.session["name"] = payload["name"]
                request.session["personInfo"] = payload["personInfo"]
                request.session["cookie_str"] = payload["cookie_str"]
                request.session["token"] = token
                # request.session["payload"]=payload
                return view_func(request, *args, **kwargs)
            return setError(msg="token exists error")
        except jwt.ExpiredSignatureError:
            # token过期
            return setError(msg="Token expired，请重新登录")
        except jwt.InvalidTokenError:
            # token无效
            return setError(msg="Invalid token")
        except IndexError:
            # token = str(authorization_header).split(" ")[1]
            # IndexError: list index out of range
            traceback.print_exc()
            return setError(msg="token is needed")
        except Exception as e:
            traceback.print_exc()
            return setError(msg="server error")

    return wrapper
