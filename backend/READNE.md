# 北科选课辅助工具文档

## 1. 项目简介

帮助不会写脚本的同学使用现成的软件选课

## 2. 运行项目

迁移数据库

```text
python manage.py makemigrations
python manage.py migrate
```

创建缓存表

```text
python manage.py createcachetable
```

创建超级管理员账户

```text
python manage.py createsuperuser
```

收集静态文件

```text
python manage.py collectappstatic
```

运行代码

```text
python manage.py runserver

```



