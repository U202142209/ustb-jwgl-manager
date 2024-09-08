# encoding: utf-8
'''
 @author: 我不是大佬 
 @contact: 2869210303@qq.com
 @wx; safeseaa
 @qq; 2869210303
 @file: GetUserDataByCookies.py
 @time: 2023/8/29 10:02
  '''

import traceback
import warnings

import re
import requests
from lxml import etree

from ..Config.Config import CourseConfig

warnings.filterwarnings("ignore")


def dic2str(dic):
    return str(dic).replace("\'", "\"")


def getFirstString(list, idx=0):
    try:
        return list[idx].strip()
    except:
        return ""


# 通过cookie获取用户信息
class GetUserDataByCookies:
    def __init__(self, cookies):
        self.cookies = cookies
        self.headers = {
            'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7',
            'Accept-Language': 'zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6,zh-TW;q=0.5',
            'Cache-Control': 'max-age=0',
            'Connection': 'keep-alive',
            # Requests sorts cookies= alphabetically
            'Cookie': self.cookies,
            'Sec-Fetch-Dest': 'document',
            'Sec-Fetch-Mode': 'navigate',
            'Sec-Fetch-Site': 'none',
            'Sec-Fetch-User': '?1',
            'Upgrade-Insecure-Requests': '1',
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.62',
            'sec-ch-ua': '"Chromium";v="116", "Not)A;Brand";v="24", "Microsoft Edge";v="116"',
            'sec-ch-ua-mobile': '?0',
            'sec-ch-ua-platform': '"Windows"',
        }

    def getRequestText(self, url):
        try:
            tex = requests.get(
                url=url, headers=self.headers, verify=False).text
            return tex
        except:
            traceback.print_exc()
            return ""

    # 获取姓名
    def getStudentName(self):
        try:
            return getFirstString(
                etree.HTML(self.getRequestText("https://jwgl.ustb.edu.cn/framework/xsMain_bjkjdx.jsp")).xpath(
                    "/html/body/div/div[1]/div[2]/div[2]/span[2]/text()"))
        except:
            traceback.print_exc()
            return ""

    # 获取成绩页面源码
    def getGrades(self, type="class", jsonify=False):
        try:
            if type == "class":
                data = {'kksj': '', 'kcxz': '', 'kcmc': '', 'xsfs': 'all', }
                response = requests.post('https://jwgl.ustb.edu.cn/kscj/cjcx_list', headers=self.headers, data=data,
                                         verify=False)
                if not jsonify:
                    return response.text
                trs = etree.HTML(response.text).xpath("/html/body/div/table/tr")
                data = []
                thead = trs[0]
                ths = [x.strip() for x in thead.xpath("./th/text()")]
                for m in range(1, len(trs)):
                    tds = trs[m].xpath("./td")
                    items = {}
                    for i in range(len(tds)):
                        items[ths[i]] = getFirstString(tds[i].xpath("./text()"))
                    data.append(items)
                return data
            elif type == "chuangxinxuefen":
                return self.getRequestText("https://jwgl.ustb.edu.cn/pyfa/cxxf_query?type=cx")
            elif type == "english":
                return self.getRequestText("https://jwgl.ustb.edu.cn/kscj/yycj_list_10008")
            elif type == "rangeDetail":
                return self.getRequestText("https://jwgl.ustb.edu.cn/kscj/pmmx_list_10008")
        except:
            traceback.print_exc()
            return ""

    # 修改个人信息
    def getchangePersonData(self):
        return self.getRequestText("https://jwgl.ustb.edu.cn/grsz/grsz_xggrxx.do")

    # 毕业生信息登记，此页面包含重要数据
    def getBiYeShengXinXiHeDui(self):
        return self.getRequestText("https://jwgl.ustb.edu.cn/bygl/bysxx")

    # 获取教学计划
    def getjiaoXueJiHua(self):
        return self.getRequestText("https://jwgl.ustb.edu.cn/pyfa/pyfa_query")

    # 血液完成度
    def getxueYeWanChengDu(self):
        return self.getRequestText("https://jwgl.ustb.edu.cn/pyfa/toxywcqk")

    # 获取选课中心页面源码
    def getxuanKeZhongXin(self, jsonify=False):
        if not jsonify:
            return self.getRequestText("https://jwgl.ustb.edu.cn/xsxk/xsxkzx_index")
        try:
            trs = etree.HTML(self.getRequestText(
                "https://jwgl.ustb.edu.cn/xsxk/xsxkzx_index"
            )).xpath("/html/body/div/form/table/tr")
            data = []
            if len(trs) == 0:
                return []
            ths = [x.strip() for x in trs[0].xpath("./th/text()")]
            for m in range(1, len(trs)):
                tds = trs[m].xpath("./td")
                items = {}
                for i in range(len(tds)):
                    items[ths[i]] = getFirstString(tds[i].xpath("./text()"))
                # 获取onclick属性
                items["onclickText"] = getFirstString(trs[m].xpath("./td[4]/a/text()"))
                items["onclickHref"] = re.search("'(.+?)'", getFirstString(trs[m].xpath("./td[4]/a/@onclick"))).group(1)
                data.append(items)
            return data
        except:
            traceback.print_exc()
            return []

    # 获取不同系列的课程列表
    def getClassesDetails(self, type="zybxk", kcfalx="zx", opener="zybxk",
                          dqjx0502zbid=CourseConfig.dqjx0502zbid):
        params = {
            'type': type,
            'xsid': '',
            'kcfalx': kcfalx,
            'opener': opener,
            'dqjx0502zbid': dqjx0502zbid,
        }

        response = requests.get(
            # f'https://jwgl.ustb.edu.cn/xsxk/getfanxkkc?type=zybxk&xsid=&kcfalx=zx&opener=zybxk&dqjx0502zbid={CourseConfig.dqjx0502zbid}',
            'https://jwgl.ustb.edu.cn/xsxk/getfanxkkc',
            params=params, headers=self.headers, verify=False)
        return response.text

    # 获取用户的个人信息 班级： 年级： 专业
    def getdetail(self, jx0502zbid=CourseConfig.dqjx0502zbid):
        try:
            return getFirstString(etree.HTML(self.getRequestText(
                f"https://jwgl.ustb.edu.cn/xsxk/xsxkzx_zy?glyxk=1&jx0502zbid={jx0502zbid}&isgld=null"
            )).xpath("/html/body/div/div/font/text()"))
        except:
            traceback.print_exc()
            return ""

    # 使用正则表达式匹配函数字符串中的参数部分
    def _get_second_parameter(self, function_string):
        """
        请使用python的正则表达式，编写函数，解析下面的函数字符串中的函数的第二个参数 1029005 和第三个参数 202320241008982
        qhkc(this,'1029005','202320241008982');
        """
        match = re.search(r"\((.*?)\)", function_string)
        if match:
            # 使用逗号分隔参数
            parameter_list = match.group(1).split(",")
            if len(parameter_list) >= 2:
                # 返回第二个参数
                return parameter_list[1].strip("'")
        return None

    # 解析表格数据
    def _jiexiTable(self, resHTML, xpath="/html/body/div/div/form/table/tr"):
        # 解析数据
        trs = etree.HTML(resHTML).xpath(xpath)
        data = []
        ths = [x.strip() for x in trs[0].xpath("./th/text()")]
        for m in range(1, len(trs)):
            tds = trs[m].xpath("./td")
            items = {}
            for i in range(len(tds)):
                items[ths[i]] = getFirstString(tds[i].xpath("./text()"))
                # 获取 课程编号和通知单号
                items["课程编号_1"] = self._get_second_parameter(
                    getFirstString(trs[m].xpath("./@onclick")))
            data.append(items)
        return data

    def _getCoursesByTypeBase(self, url, jsonify=False):
        resHTML = self.getRequestText(url=url)
        if not jsonify:
            return resHTML
        return self._jiexiTable(resHTML=resHTML, xpath="/html/body/div/div/form/table/tr")

    # 获取必修课信息
    def getCompulsory(self, jsonify=False):
        return self._getCoursesByTypeBase(
            url=f"https://jwgl.ustb.edu.cn/xsxk/getfanxkkc?type=zybxk&xsid=&kcfalx=zx&opener=zybxk&dqjx0502zbid={CourseConfig.dqjx0502zbid}",
            jsonify=jsonify)

    # 专业选修课
    def getProfessionalElectives(self, jsonify=False):
        return self._getCoursesByTypeBase(
            url=f"https://jwgl.ustb.edu.cn/xsxk/getfanxkkc?type=zyxxk&xsid=&kcfalx=zx&opener=zyxxk&dqjx0502zbid={CourseConfig.dqjx0502zbid}",
            jsonify=jsonify)

    # 素质拓展课
    def getqualityDevelopmentCourse(self, jsonify=False):
        return self._getCoursesByTypeBase(
            url=f"https://jwgl.ustb.edu.cn/xsxk/getgxkkc.do?type=gxk&xsid=&kcfalx=zx&opener=gxk&dqjx0502zbid={CourseConfig.dqjx0502zbid}",
            jsonify=jsonify)

    # 搜索素质拓展课
    def searchqualityDevelopmentCourse(self, post_data=None):
        try:
            # 设置学期信息
            post_data["dqjx0502zbid"] = CourseConfig.dqjx0502zbid
            resHTML = requests.post(
                url="https://jwgl.ustb.edu.cn/xsxk/getgxkkc.do",
                headers=self.headers,
                data=post_data,
                verify=False).text
            if resHTML.__contains__("未查询到数据"):
                return []
            return self._jiexiTable(resHTML=resHTML, xpath="/html/body/div/div/form/table/tr")
        except:
            traceback.print_exc()
            return []

    # 跨专业选课
    def getinterdisciplinaryCourseSelection(self, jsonify=False):
        return self._getCoursesByTypeBase(
            url=f"https://jwgl.ustb.edu.cn/xsxk/getkzyxkkc.do?type=kzyxk&xsid=&kcfalx=zx&opener=kzyxk&dqjx0502zbid={CourseConfig.dqjx0502zbid}",
            jsonify=jsonify)

    # 班级课程
    def getclassLessons(self, jsonify=False):
        return self._getCoursesByTypeBase(
            url=f"https://jwgl.ustb.edu.cn/xsxk/getBjkc.do?type=bjkc&xsid=&kcfalx=zx&opener=bjkc&dqjx0502zbid={CourseConfig.dqjx0502zbid}",
            jsonify=jsonify)

    # 其他选课
    def getOtherCourseSelections(self, jsonify=False):
        return self.getRequestText(
            f"https://jwgl.ustb.edu.cn/xsxk/getcxxkkc.do?type=cxxk&xsid=&kcfalx=zx&opener=qtxk&dqjx0502zbid={CourseConfig.dqjx0502zbid}")

    # 辅修选课
    def getfuxuiClasses(self, jsonify=False):
        return self.getRequestText(
            f"https://jwgl.ustb.edu.cn/xsxk/getfanxkkc?type=fxxk&xsid=&kcfalx=fx&opener=fxxk&dqjx0502zbid={CourseConfig.dqjx0502zbid}")

    # 获取用户的个人信息 班级： 年级： 专业  含有重要数据
    def getClassNianjiZhuanYe(self, jx0502zbid=CourseConfig.dqjx0502zbid):
        info = self.getdetail(jx0502zbid=jx0502zbid)
        if not info:
            return {}
        return {
            "class": re.search(r'班级：(\S+)', info).group(1),
            "grade": re.search(r'年级：(\S+)', info).group(1),
            "major": re.search(r'专业：(\S+)', info).group(1),
        }

    # 获取已经选择的课程数据
    def getSelectedClasses(self, jsonify=False, dqjx0502zbid=CourseConfig.dqjx0502zbid):
        if not jsonify:
            return self.getRequestText(f"https://jwgl.ustb.edu.cn/xsxk/xsxkzx_xkjglist.do?dqjx0502zbid={dqjx0502zbid}")
        try:
            trs = etree.HTML(self.getRequestText(
                f"https://jwgl.ustb.edu.cn/xsxk/xsxkzx_xkjglist.do?dqjx0502zbid={dqjx0502zbid}")).xpath(
                "/html/body/div/div/form/table/tr")
            data = []
            ths = [x.strip() for x in trs[0].xpath("./th/text()")]
            for m in range(1, len(trs)):
                tds = trs[m].xpath("./td")
                items = {}
                for i in range(len(tds)):
                    items[ths[i]] = getFirstString(tds[i].xpath("./text()"))
                data.append(items)
            return data
        except:
            traceback.print_exc()
            return []

    # 获取课程详情  辅助选课的函数
    def getClassDetail(self, dqjx0502zbid=CourseConfig.dqjx0502zbid, jx02id=""):
        try:
            text = self.getRequestText(
                f"https://jwgl.ustb.edu.cn/xsxk/getkcxxlist.do?dqjx0502zbid={dqjx0502zbid}&jx02id={jx02id}")
            trs = etree.HTML(text).xpath(
                "/html/body/div/div/form/table/tr")
            data = []
            ths = [x.strip() for x in trs[0].xpath("./th/text()")]
            for m in range(1, len(trs)):
                tds = trs[m].xpath("./td")
                items = {}
                items["班级"] = ""
                for i in range(len(tds)):
                    items[ths[i]] = getFirstString(tds[i].xpath("./text()"))
                items["课程名称"] = getFirstString(tds[3].xpath("./a/text()"))
                expression = getFirstString(tds[1].xpath("./div/a[1]/@href"))
                pattern = r"xsxkOper\('([^']*)','([^']*)'"
                match = re.search(pattern, expression)
                if match:
                    items["通知单编号"] = match.group(1)
                    items["课程编号"] = match.group(2)
                data.append(items)
            if "未查询到数据" in str(data):
                return []
            return data
        except:
            traceback.print_exc()
            return []

    # 学籍卡片信息
    def getXuejiXinxi(self):
        return self.getRequestText("https://jwgl.ustb.edu.cn/grxx/xsxx")


if __name__ == '__main__':
    cookies = "JSESSIONID=26992FFD3B21206931307E783B7DDB07; SERVERID=127"
    g = GetUserDataByCookies(cookies=cookies)
    name = g.getStudentName()
    print(dic2str(name))
    print(dic2str(
        g.getClassNianjiZhuanYe()
    ))
