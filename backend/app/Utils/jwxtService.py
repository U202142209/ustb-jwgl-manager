# encoding: utf-8
'''
 @author :我不是大佬 
 @contact:2869210303@qq.com
 @wx     ;safeseaa
 @qq     ;2869210303
 @github ;https://github.com/U202142209
 @blog   ;https://blog.csdn.net/V123456789987654 
 @file   :jwxtService.py
 @time   :2023/11/12 17:54
  '''

import re
import time
import warnings
import requests

warnings.filterwarnings("ignore")


class JwAutoLogin:
    def __init__(self):
        self.sisURL = "https://sis.ustb.edu.cn"
        self.regex1 = re.compile(r'sid = "([a-z0-9]+?)"')
        self.headers = {
            'Connection': 'keep-alive',
            'Sec-Ch-Ua': '"Microsoft Edge";v="119", "Chromium";v="119", "Not?A_Brand";v="24"',
            'Sec-Ch-Ua-Mobile': '?0',
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0',
            'Sec-Ch-Ua-Platform': '"Windows"', 'Accept': '*/*', 'Sec-Fetch-Site': 'same-origin',
            'Sec-Fetch-Mode': 'cors', 'Sec-Fetch-Dest': 'empty', 'Accept-Encoding': 'gzip, deflate, br',
            'Accept-Language': 'zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6,zh-TW;q=0.5'}

    def getToken(self):
        for _ in range(5):
            r1 = requests.post('https://jwgl.ustb.edu.cn/glht/Logon.do?method=randToken', headers={
                "Referer": "https://jwgl.ustb.edu.cn/",
                "X-Requested-With": "XMLHttpRequest"
            }, verify=False)
            if r1.status_code != 200:
                print("r1 请求的状态码不对")
                continue
            rd = r1.json()
            server_id = r1.cookies.get("SERVERID")
            jsession_id = r1.cookies.get("JSESSIONID")
            random_token = rd["rand_token"]
            for _ in range(6):
                r = requests.get(
                    url='https://sis.ustb.edu.cn/connect/qrpage',
                    params={
                        'appid': 'bb304f1438b04a0c9e8fa35182eb42b5',
                        'return_url': 'https://jwgl.ustb.edu.cn/glht/Logon.do?method=weCharLogin',
                        'rand_token': random_token,
                        'embed_flag': '1'
                    }, verify=False)
                if r.status_code != 200:
                    print("状态码不对")
                    continue
                r = r.text
                m = self.regex1.search(r)
                if not m:
                    print("配对失败")
                    continue
                sid = m.groups()[0]
                return {
                    "sid": sid,
                    "random_token": random_token,
                    "jsession_id": jsession_id,
                    "server_id": server_id
                }
        return None

    # 自动登录的测试代码，
    def login(self):
        data = self.getToken()
        if data:
            print(data)
            imgURL = self.sisURL + f"/connect/qrimg?sid={data.get('sid', '')}"
            print("请前往认证链接：", imgURL)
            for i in range(15):
                res = self.getState(
                    sid=data["sid"],
                    rand_token=data["random_token"],
                    jsession_id=data["jsession_id"],
                    server_id=data["server_id"]
                )
                print(res)
                if res["text"]["state"] == 200:
                    break
                time.sleep(1.0)

    def getState(self, sid, rand_token, jsession_id, server_id):
        # global login, s
        cookie_str = ""
        url = self.sisURL + '/connect/state?sid=' + sid
        r = requests.get(url=url, headers=self.headers, verify=False)
        r_headers = {'Cache-Control': 'no-cache, no-store, must-revalidate',
                     'Content-Type': 'application/json;charset=utf-8', 'Expires': '0', 'Pragma': 'no-cache'}
        if r.status_code == 200 and r.json()["state"] == 200:
            # 可以开始登录
            url = "https://jwgl.ustb.edu.cn/glht/Logon.do?method=weCharLogin&appid=bb304f1438b04a0c9e8fa35182eb42b5&auth_code=" + \
                  r.json()["data"] + "&rand_token=" + rand_token
            ts = requests.session()
            ts.cookies.set("JSESSIONID", jsession_id, domain='jwgl.ustb.edu.cn')
            ts.cookies.set("SERVERID", server_id, domain='jwgl.ustb.edu.cn')
            res = ts.get(url, headers=self.headers, verify=False)
            if not '欢迎登录北京科技大学教务平台' in res.text:
                d = re.search('<span class="user">(.+?)</span>', res.text)
                ac = '未查询到姓名'
                if d:
                    ac = d.group(1)
                # print('login')
                cookies = ts.cookies.get_dict()
                cookie_list = []
                with open("cookies.txt", mode="w", encoding="utf-8") as f:
                    for key, value in cookies.items():
                        f.write(f'{key}={value}__________')
                        cookie_list.append(f'{key}={value}')
                cookie_str = "; ".join(cookie_list)
                # print(cookie_str, "\n\n\n\n\n")
                # print('贝壳王教务系统账号更新完毕，当前使用账号：' + ac)
                lr = ts.get(
                    "https://jwgl.ustb.edu.cn/xsxk/getfanxkkc?type=zybxk&xsid=&kcfalx=zx&opener=zybxk&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4",
                    headers=self.headers, verify=False)
                with open(file="courses.html", mode="w", encoding="utf-8") as f:
                    f.write(lr.text)
        else:
            print("获取状态成功：", r.text)
        return {
            "text": r.json(),
            # "headers": r_headers,
            "cookie_str": cookie_str,
            "status": r.status_code
        }
