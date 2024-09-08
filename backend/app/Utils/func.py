# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :func.py
 @time   :2023/11/12 17:23
  '''
from datetime import datetime
import random


def get_nid():
    """产生随机数字"""
    return datetime.now().strftime('%Y%m%d%H%M%S') + str(random.randint(1000, 9999))
