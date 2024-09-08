import os

from ckeditor.fields import RichTextField
from django.db import models
from django.db.models.signals import post_save
from django.dispatch import receiver

from .Utils.func import get_nid


class ErrorLog(models.Model):
    created_at = models.DateTimeField(auto_now_add=True)
    message = models.TextField()

    def __str__(self):
        return self.name

    class Meta:
        verbose_name = "错误日志信息"
        verbose_name_plural = "错误日志信息"


@receiver(post_save, sender=ErrorLog)
def log_error(sender, instance, created, **kwargs):
    if created:
        # 在这里处理异常日志，例如发送邮件通知等
        pass


# 学生信息
class Student(models.Model):
    # 必要信息
    c_time = models.DateTimeField(verbose_name="创建时间", auto_now_add=True)
    latest_edit_time = models.DateTimeField(verbose_name="最近编辑时间", auto_now=True)
    name = models.CharField(verbose_name="学生姓名", max_length=50)
    cookies = models.CharField(verbose_name="cookies", max_length=500)
    email = models.EmailField(verbose_name="用户邮箱", default="", blank=True, null=True)

    ### 备注
    beizhu = RichTextField(verbose_name="备注", config_name='default', default="填写备注")

    # 个人信息
    xuehao = models.CharField(verbose_name="学号", max_length=50, default="",blank=True,null=True)
    detail = models.CharField(verbose_name="个人信息", max_length=100, default="")
    classname = models.CharField(verbose_name="班级", max_length=30, default="")
    grade = models.CharField(verbose_name="班级", max_length=20, default="")
    major = models.CharField(verbose_name="专业", max_length=30, default="")

    ip = models.CharField(max_length=18, default="0", verbose_name="用户IP地址")

    ### 额外信息
    grades = RichTextField(verbose_name="成绩", config_name='default', default="")
    chuangXinXueFen = RichTextField(verbose_name="创新学分", config_name='default', default="")
    englishGrades = RichTextField(verbose_name="英英（小语种）战绩", config_name='default', default="")
    rangeDetail = RichTextField(verbose_name="排名核对细节", config_name='default', default="",blank=True,null=True)
    biYeShengXinXiHeDui = RichTextField(verbose_name="毕业生信息核对", config_name='default', default="")

    ### 学籍管理
    # 学籍卡片 https://jwgl.ustb.edu.cn/grxx/xsxx
    xuejikapian = RichTextField(verbose_name="学籍卡片", config_name='default', default="")

    ### 教学计划
    jiaoXueJiHua = RichTextField(verbose_name="教学计划", config_name='default', default="")
    xueYeWanChengDu = RichTextField(verbose_name="学业进度", config_name='default', default="")

    ### 血迹管理 选课中心
    # https://jwgl.ustb.edu.cn/xsxk/xsxkzx_index
    xuanKeZhongXin = RichTextField(verbose_name="选股中心", config_name='default', default="")
    # https://jwgl.ustb.edu.cn/xsxk/getfanxkkc?type=zybxk&xsid=&kcfalx=zx&opener=zybxk&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4
    compulsory = RichTextField(verbose_name="必修课", config_name='default', default="")
    # https://jwgl.ustb.edu.cn/xsxk/getfanxkkc?type=zyxxk&xsid=&kcfalx=zx&opener=zyxxk&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4
    professionalElectives = RichTextField(verbose_name="专业选修课", config_name='default', default="")
    # https://jwgl.ustb.edu.cn/xsxk/getgxkkc.do?type=gxk&xsid=&kcfalx=zx&opener=gxk&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4
    qualityDevelopmentCourse = RichTextField(verbose_name="素质拓展课", config_name='default', default="")
    # https://jwgl.ustb.edu.cn/xsxk/getkzyxkkc.do?type=kzyxk&xsid=&kcfalx=zx&opener=kzyxk&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4
    interdisciplinaryCourseSelection = RichTextField(verbose_name="跨专业选课", config_name='default', default="")
    # https://jwgl.ustb.edu.cn/xsxk/getBjkc.do?type=bjkc&xsid=&kcfalx=zx&opener=bjkc&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4
    classLessons = RichTextField(verbose_name="班班课程", config_name='default', default="")
    # https://jwgl.ustb.edu.cn/xsxk/getcxxkkc.do?type=cxxk&xsid=&kcfalx=zx&opener=qtxk&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4
    OtherCourseSelections = RichTextField(verbose_name="其他选课", config_name='default', default="")
    # https://jwgl.ustb.edu.cn/xsxk/getfanxkkc?type=fxxk&xsid=&kcfalx=fx&opener=fxxk&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4
    fuxuiClasses = RichTextField(verbose_name="辅修选课", config_name='default', default="")
    # https://jwgl.ustb.edu.cn/xsxk/xsxkzx_xkjglist.do?xsid=&dqjx0502zbid=578A8C35017448B195767CBBCCC87BC4&glyxk=1
    selectedClasses = RichTextField(verbose_name="已选课程", config_name='default', default="")

    def __str__(self):
        return self.name

    class Meta:
        verbose_name = "学生信息管理"
        verbose_name_plural = "学生信息管理"
        ordering = ["-latest_edit_time"]


# 日志
class Log(models.Model):
    name = models.CharField(verbose_name="学生姓名", max_length=50)
    personInfo = models.CharField(verbose_name="个人信息", max_length=100, default="")
    token = models.TextField(verbose_name="token", default="")
    c_time = models.DateTimeField(verbose_name="创建时间", auto_now_add=True)
    detail = models.CharField(verbose_name="备注", default="", max_length=300)
    ip = models.CharField(max_length=18, default="", verbose_name="用户IP地址")

    def __str__(self):
        return str(str(self.name) + "  " + str(self.detail))

    class Meta:
        verbose_name = "系统日志"
        verbose_name_plural = "系统日志"


# 消息
class Notice(models.Model):
    c_time = models.DateTimeField(verbose_name="创廻时间", auto_now_add=True)
    latest_edit_time = models.DateTimeField(verbose_name="最近编辑时间", auto_now=True)
    content = RichTextField(verbose_name="公告内容", config_name='default', default="")
    nid = models.CharField(max_length=20, verbose_name="编号", default=get_nid())
    title = models.CharField(verbose_name="标题", max_length=50, default="")

    def __str__(self):
        return self.title

    class Meta:
        verbose_name = "通知公告管理"
        verbose_name_plural = "通知公告管理"


# 学生查询的课程信息
class ClassesLog(models.Model):
    # 重要参数
    course_name = models.CharField(verbose_name="课程名称", max_length=100, default="")
    course_number = models.CharField(verbose_name="课程编号", max_length=100)
    course_notice_number = models.CharField(verbose_name="通知单编号", max_length=50)

    # 每当有学生选择此课程时，增加学生选择的信息
    stu_name = models.TextField(verbose_name="学生姓名")

    course_teacher = models.CharField(verbose_name="授课教师", max_length=100)
    course_limited_num = models.SmallIntegerField(verbose_name="限选人数")
    course_allowance = models.SmallIntegerField(verbose_name="课余量")
    course_type = models.CharField(verbose_name="课程性质", max_length=50)
    course_credit = models.CharField(verbose_name="学分", max_length=10)
    quote_num = models.IntegerField(verbose_name="引用次数", default=1)

    ip = models.CharField(max_length=18, default="", verbose_name="用户IP地址")

    c_time = models.DateTimeField(verbose_name="创廻时间", auto_now_add=True)
    desc = models.CharField(verbose_name="备注", max_length=200, default="")

    def __str__(self):
        return str(str(self.stu_name) + " " + str(self.course_name) + " " + str(
            self.course_number) + " " + str(self.course_notice_number))

    class Meta:
        verbose_name = "课程日志信息"
        verbose_name_plural = "课程日志信息"


def get_file_path(instance, filename):
    return f"upload/{instance.upload_time.strftime('%Y/%m/%d')}/{filename}"


class File(models.Model):
    # file_path = models.FileField(upload_to=get_file_path, verbose_name="文件路径名")
    file_path = models.FileField(upload_to="upload/%Y/%m/%d/", verbose_name="文件跟路名")
    version = models.CharField(max_length=50, default="1.0.0", verbose_name="文件版本号")
    upload_time = models.DateTimeField(auto_now_add=True)
    latest_edit_time = models.DateTimeField(verbose_name="最近编辑时间", auto_now=True)
    name = models.CharField(max_length=60, verbose_name="文件名", default="图片")
    description = models.TextField(verbose_name="文件描述", default="")
    hash_value = models.CharField(max_length=350, verbose_name="文件哈希值", blank=True, null=True)

    def __str__(self):
        return self.name

    class Meta:
        verbose_name = "文件管理"
        verbose_name_plural = "文件管理"


# 通过django的信号机制实现自动删除
@receiver(models.signals.post_delete, sender=File)
def auto_delete_file(sender, instance, **kwargs):
    # 判断当前文件的哈希值，如果有多个文件，则不删除
    if File.objects.filter(hash_value=instance.hash_value).count() >= 1:
        # print("还有其他的文件存在，无需删除")
        return
    if instance.file_path:
        if os.path.isfile(instance.file_path.path):
            # print("删除文件成功")
            os.remove(instance.file_path.path)
            return
    # print("删除文件失败:路径不存在", instance.file_path.path)


class SystemConfiguration(models.Model):
    c_time = models.DateTimeField(verbose_name="创建时间", auto_now_add=True)
    latest_edit_time = models.DateTimeField(verbose_name="最近编辑时间", auto_now=True)
    name = models.CharField(verbose_name="名称", default="科大选课辅助系统配置", max_length=50)
    version = models.CharField(verbose_name="版本号", default="3.0.0", max_length=10)
    desc = models.TextField(verbose_name="详情")

    def __str__(self):
        return str(str(self.name) + " " + str(self.version))

    class Meta:
        verbose_name = "系统全局配置"
        verbose_name_plural = "系统全局配置"


class AbstructModel(models.Model):
    create_time = models.DateTimeField(auto_now_add=True, verbose_name="创建时间")
    update_time = models.DateTimeField(verbose_name="最近编辑时间", auto_now=True)

    class Meta:
        abstract = True


class FeedBack(AbstructModel):
    contract = models.CharField(verbose_name="联系方式", max_length=100, blank=True, null=True)
    content = models.TextField(verbose_name="反馈内容")
    photo = models.ImageField(
        verbose_name="附带图片",
        upload_to='images/',
        null=True, blank=True,
    )

    def __str__(self):
        return self.contract

    class Meta:
        verbose_name = "用户反馈"
        verbose_name_plural = "用户反馈"
        ordering = ["-update_time"]
