# encoding: utf-8
'''
 @author: 我不是大佬 
 @contact: 2869210303@qq.com
 @wx; safeseaa
 @qq; 2869210303
 @file: DownloadController.py
 @time: 2023/8/31 18:45
  '''
from django.shortcuts import render
from ..models import File

# http://127.0.0.1:8000/api/download
def download(request):
    files = File.objects.exclude(name='图片').order_by("-upload_time")
    # files = File.objects.all()
    return render(request, 'download.html', {'files': files})
