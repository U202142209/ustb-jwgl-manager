# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :ExceptionMiddleware.py
 @time   :2023/11/12 17:06
  '''

from django.http import Http404, HttpResponseNotFound
from django.utils.deprecation import MiddlewareMixin
from ..models import ErrorLog
from ..Middleware.authToken import checkloginRestController, setError, setSuccess


# 使用中间件捕获 404 异常
class ExceptionMiddleware:
    def __init__(self, get_response):
        self.get_response = get_response

    def __call__(self, request):
        try:
            response = self.get_response(request)
        except Exception as e:
            # 捕获异常并保存到数据库
            error_log = ErrorLog(message=str(e))
            error_log.save()
            # 返回一个JSON响应，或者重定向到一个错误页面
            return setError(msg="服务器内部错误:"+str(e)+" 请联系微信管理员:safeseaa")
        return response
