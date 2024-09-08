# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :index.py
 @time   :2023/11/12 16:54
  '''

from django.http import HttpResponse
from django.http import JsonResponse
from django.shortcuts import render
from django.utils.decorators import method_decorator
from django.views import View
from django.views.decorators.csrf import csrf_exempt

from ..Config.Config import AUTHOR, CourseConfig
from ..Utils.func import get_nid
from ..Utils.tokenService import generate_token
from ..forms import FeedBackForm
from ..models import SystemConfiguration, FeedBack


class Index(View):
    def get(self, request):
        return JsonResponse(
            data={'name': '北科大本科教务系统选课工具',
                  "author": AUTHOR
                  }, json_dumps_params={'ensure_ascii': False})


@method_decorator(csrf_exempt, name='dispatch')
class CreateToken(View):
    def post(self, request):
        return JsonResponse(
            data={
                "data": generate_token(
                    payload={
                        "logined": False,
                        "detail": "初次生成token",
                        "nid": get_nid()
                    }, expire_time=60 * 10  # 十分钟有效
                )
            }, json_dumps_params={'ensure_ascii': False})


class InfoView(View):
    def get(self, request):
        return JsonResponse(
            data={
                "info": SystemConfiguration.objects.values(
                    "name", "version", "desc", "latest_edit_time", "c_time").first(),
                "author": AUTHOR,
                "CourseConfig": {
                    "dqjx0502zbid": CourseConfig().dqjx0502zbid,
                    "dqjx0502zbid_name": CourseConfig.dqjx0502zbid_name
                }
            }, json_dumps_params={'ensure_ascii': False})


# http://127.0.0.1:8000/api/feedback
def feedback(request):
    if request.method == 'POST':
        form = FeedBackForm(request.POST, request.FILES)
        if form.is_valid():
            form.save()
            request.session["feedback"] = True
            request.session["id"] = form.instance.id
            return HttpResponse('反馈成功')
    else:
        if request.session.get("feedback", False):
            feedback_record = FeedBack.objects.get(id=int(
                request.session["id"]
            ))
            # 将查询到的数据与ModelForm结合起来
            form = FeedBackForm(instance=feedback_record)
        else:
            form = FeedBackForm()
    return render(request, 'feedback.html', {'form': form})


# http://127.0.0.1:8000/api/home
def home(request):
    return render(request, 'index.html')
