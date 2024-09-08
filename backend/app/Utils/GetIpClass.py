# encoding: utf-8
'''
 @author: 我不是大佬 
 @contact: 2869210303@qq.com
 @wx; safeseaa
 @qq; 2869210303
 @file: GetIpClass.py
 @time: 2023/8/29 9:23
  '''

import requests
import time


class GetIpClass:
    @staticmethod
    def getLocationFromAlibaba(ip="60.23.156.211"):
        for i in range(10):
            time.sleep(1)
            try:
                res = requests.get(
                    url=f"https://ip.taobao.com/outGetIpInfo?ip={ip}&accessKey=alibaba-inc",verify=False
                )
                return res.json()
            except Exception as error:
                print("获取IP发生了错误：", error)
        return {}

    @staticmethod
    def getIpFromipapi(ip_address="60.23.156.211"):
        for i in range(10):
            try:
                return requests.get(
                    f'https://ipapi.co/{ip_address}/json/',
                    verify=False
                ).json()
            except Exception as error:
                print("获取IP的过程中发生了错误：", error)
                time.sleep(0.5)
        return {}

    @staticmethod
    def get_ip_form_requests():
        response = requests.get(
            'https://api64.ipify.org?format=json',verify=False
        ).json()
        return response["ip"]

    @staticmethod
    # X-Forwarded-For:简称XFF头，它代表客户端，也就是HTTP的请求端真实的IP，只有在通过了HTTP 代理或者负载均衡服务器时才会添加该项。
    def getIpFromRequest(request):
        '''获取请求者的IP信息'''
        x_forwarded_for = request.META.get('HTTP_X_FORWARDED_FOR')  # 判断是否使用代理
        if x_forwarded_for:
            ip = x_forwarded_for.split(',')[0]  # 使用代理获取真实的ip
        else:
            ip = request.META.get('REMOTE_ADDR')  # 未使用代理获取IP
        return ip
