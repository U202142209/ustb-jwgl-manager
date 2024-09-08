import hashlib

from django.contrib import admin
from django.contrib import admin
from django.urls import reverse
from django.utils.html import format_html

from .models import Student, ClassesLog, Log, SystemConfiguration, FeedBack, ErrorLog, File


@admin.register(Student)
class StudentAdmin(admin.ModelAdmin):
    list_display = ("name", "detail", "email", 'details_link', "beizhu", 'latest_edit_time', 'c_time')

    def details_link(self, obj):
        # 构造详情链接
        url = reverse('stu-info', args=[obj.cookies])
        return format_html('<a target="__blank" href="{}">查看详情</a>', url)

    details_link.short_description = "详细信息"  # 设置列标题

    search_fields = ["name", "detail", 'email', 'grades', 'biYeShengXinXiHeDui',
                     'xueYeWanChengDu', 'beizhu', 'biYeShengXinXiHeDui']

    readonly_fields = (
        "c_time", "latest_edit_time", "name", "cookies", "ip", "detail", "detail",
        "classname", "grade", "major"
    )


# @admin.register(Notice)
# class NoticeAdmin(admin.ModelAdmin):
#     list_display = ('title', 'id', 'nid', 'latest_edit_time')
#     search_fields = ['title', 'content', 'nid']

@admin.register(ErrorLog)
class ErrorLogAdmin(admin.ModelAdmin):
    list_display = ('message', 'created_at')
    search_fields = ['message']


@admin.register(File)
class FileAdmin(admin.ModelAdmin):
    list_display = ('id', 'name', 'version', 'file_path', 'latest_edit_time')
    search_fields = ["name", "description", "version"]
    readonly_fields = ('hash_value', 'latest_edit_time')

    def save_model(self, request, obj, form, change):
        """
        先计算文件的哈希值，然后检查数据库中是否存在相同的哈希值。
        如果存在，则将新文件的路径和哈希值更新为已存在的文件的路径和哈希值；
        如果不存在，则将文件保存到磁盘，并更新文件的路径和哈希值
        """
        # 计算文件的哈希值
        file = form.cleaned_data['file_path']
        file_hash = hashlib.md5(file.read()).hexdigest()

        # 检查哈希值是否存在于数据库中
        existing_file = File.objects.filter(hash_value=file_hash).first()

        if existing_file:
            # 如果哈希值存在，则更新文件路径和哈希值
            obj.file_path = existing_file.file_path
            obj.hash_value = existing_file.hash_value
            print("\n\n文件已经存在，")
        else:
            # 如果哈希值不存在，则保存文件到磁盘并更新哈希值和文件路径
            obj.hash_value = file_hash
            obj.file_path.save(file.name, file)
        super().save_model(request, obj, form, change)


@admin.register(ClassesLog)
class ClassesLogAdmin(admin.ModelAdmin):
    list_display = (
        "stu_name", "course_number", 'course_name', "course_teacher",
        "course_type", "course_limited_num", "course_allowance", "course_credit",
        "color_state",  "ip", "c_time"
    )

    search_fields = [
        'course_name', "course_number", "course_notice_number",
        "stu_name", "course_teacher", "course_type", "desc"]

    # 设置提醒字段得颜色
    # 对签收状态设置颜色
    def color_state(self, obj):
        if obj.desc == '移出任务队列':
            color_code = 'red'
        elif obj.desc == "抢课成功":
            color_code = 'green'
        else:
            color_code = 'blue'
        return format_html(
            '<span style="color:{};">{}</span>',
            color_code, obj.desc,
        )

    color_state.short_description = '备注'

    # 全部只读
    def get_readonly_fields(self, request, obj=None):
        return [field.name for field in self.model._meta.fields]

    # 不让修改
    def has_change_permission(self, request, obj=None):
        return False


@admin.register(Log)
class LogAdmin(admin.ModelAdmin):
    list_display = ("name", "personInfo", "detail", "c_time")
    search_fields = ["name", "personInfo", "detail"]

    def get_readonly_fields(self, request, obj=None):
        return [field.name for field in self.model._meta.fields]


@admin.register(SystemConfiguration)
class SystemConfigurationAdmin(admin.ModelAdmin):
    list_display = ("name", "version", "latest_edit_time", "c_time")

    def save_model(self, request, obj, form, change):
        # 判断是否为新增操作
        if not change:
            # 判断是否已经存在其他配置
            if SystemConfiguration.objects.exists():
                # 给管理后台添加错误提示信息
                self.message_user(request, "新增失败，系统配置已存在（只能有一个）", level='error')
                return
        super().save_model(request, obj, form, change)

    def delete_model(self, request, obj):
        print("SystemConfiguration.objects.count():", SystemConfiguration.objects.count())
        # 判断是否只剩下一个配置
        if SystemConfiguration.objects.count() == 1:
            # 给管理后台添加错误提示信息
            self.message_user(request, "删除失败，系统配置不能删除（必须存在一个系统配置）", level='error')
            return
        super().delete_model(request, obj)

    # def delete_view(self, request, object_id, extra_context=None):
    #     # 判断是否只剩下一个配置
    #     if SystemConfiguration.objects.count() == 1:
    #         # 给管理后台添加错误提示信息
    #         self.message_user(request, "删除失败，系统配置不能删除", level='error')
    #         return HttpResponseRedirect(request.path)
    #     return super().delete_view(request, object_id, extra_context)


@admin.register(FeedBack)
class FeedBackAdmin(admin.ModelAdmin):
    list_display = ("contract", "content", "show_photo", "update_time", "create_time")
    search_fields = ["contract", "content"]

    def show_photo(self, obj):
        if obj.photo:
            return format_html(
                f'<a href="{obj.photo.url}" target="_blank"><img width="60px" src="{obj.photo.url}" alt="图片展示失败"></a>')
        return format_html("")

    show_photo.short_description = "图片"


admin.site.site_title = "贝壳选课工具-后台数据中心"
admin.site.site_header = "贝壳选课工具-后台数据中心"
