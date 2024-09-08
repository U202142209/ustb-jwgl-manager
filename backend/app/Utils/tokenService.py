# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :token.py
 @time   :2023/11/12 17:21
  '''

import jwt
import datetime
from ..Config.Config import TOKEN_secret_key


# 生成token
def generate_token(payload, expire_time=60, secret_key=TOKEN_secret_key):
    """
    expire_time:token的失效事件，默认60秒
    """
    # 设置token的过期时间
    expire_datetime = datetime.datetime.utcnow() + datetime.timedelta(seconds=expire_time)
    # 添加过期时间到payload中
    payload['exp'] = expire_datetime
    # 生成token
    token = jwt.encode(payload, secret_key, algorithm='HS256')
    return token


# 验证token
def verify_token(token, secret_key="my_secret_key"):
    try:
        # 验证token
        payload = jwt.decode(token, secret_key, algorithms=['HS256'])
        return payload
    except jwt.ExpiredSignatureError:
        # token过期
        return 'Token expired'
    except jwt.InvalidTokenError:
        # token无效
        return 'Invalid token'


if __name__ == '__main__':
    # 示例用法
    payload = {'user_id': 123, "user_name": "zhao"}
    secret_key = 'your_secret_key'
    expire_time = 30  # token失效时间为30分钟

    # 生成token
    token = generate_token(payload, expire_time, secret_key=secret_key)
    print('Token:', token)

    # 验证token
    result = verify_token(token, secret_key)
    print('Result:', result)
