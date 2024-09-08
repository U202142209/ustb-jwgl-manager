import random
import smtplib
import traceback
from email.mime.text import MIMEText


class EmailService:
    signature = "【贝壳选课辅助工具】"

    mail_host = 'smtp.qq.com'
    port = 465
    send_by = '修改成你的邮箱'
    password = '修改成你的邮箱授权密码'

    @staticmethod
    def code():
        s = ""
        for i in range(4):
            num = random.randint(0, 9)
            upper_alpha = chr(random.randint(65, 90))
            num = random.choice([num, upper_alpha])
            s = s + str(num)
        return s

    @staticmethod
    def send_email(send_to, content, subject="验证码"):
        message = MIMEText(content, 'plain', 'utf-8')
        message["From"] = EmailService.send_by
        message['To'] = send_to
        message['Subject'] = subject
        smpt = smtplib.SMTP_SSL(EmailService.mail_host, EmailService.port, 'utf-8')
        smpt.login(EmailService.send_by, EmailService.password)
        smpt.sendmail(EmailService.send_by, send_to, message.as_string())

    @staticmethod
    def send_message(email, message, subject="消息提醒"):
        try:
            message = EmailService.signature + "消息提醒 ； " + message
            EmailService.send_email(email, message, subject=subject)
            return True
        except:
            # 返回发送失败
            return False

    @staticmethod
    def sendVerificateCode(send_to):
        verificate_code = EmailService.code()
        content = str(EmailService.signature + '您的验证码是；') + verificate_code + '  。如非本人操作，请忽略这条信息。'
        try:
            EmailService.send_email(send_to, content)
            return verificate_code
        except Exception as error:
            traceback.print_exc()
            return False

    @staticmethod
    def sendGetCourseSuccessMsg(
            sebt_to,
            course_name, course_number,
            course_notice_number, course_teacher):
        subject = "选课成功消息提醒"
        message = f"\n贝壳选课辅助工具成功为您抢到了您心仪的课程:\n课程名称:{course_name}，\n课程编号:{course_number}，\n通知单号:{course_notice_number}，\n授课教师:{course_teacher}\n通知已送达，请君查收"
        return EmailService.send_message(email=sebt_to, message=message, subject=subject)


if __name__ == '__main__':
    sebt_to = '2869210303@qq.com'
    service = EmailService()
    print(
        EmailService.sendGetCourseSuccessMsg(
            sebt_to=sebt_to,
            course_name="CourseName",
            course_number="courseNumber",
            course_notice_number="NotificationNumber",
            course_teacher="MainTeacher",
        )
    )
