# // An highlighted block
import os

if os.name != "nt":
    import pymysql
    pymysql.install_as_MySQLdb()
