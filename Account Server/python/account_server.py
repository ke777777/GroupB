import pymysql
from pymysql import MySQLError
from http.server import BaseHTTPRequestHandler, HTTPServer
import urllib.parse
import sys
import signal
import json

class MySQLConnection:
    """MySQLデータベースへの接続を管理するクラス"""
    def __init__(self):
        self.connection = None
        self.cursor = None

    def connect(self):
        """MySQLへの接続を確立"""
        if not self.connection or not self.connection.open:
            try:
                self.connection = pymysql.connect(
                    host="tokuron-mysql",  # MySQLコンテナのホスト名
                    user="root",   # MySQLのユーザー名
                    password="secret123",  # MySQLのパスワード
                    database="my_database",  # 使用するデータベース名
                    cursorclass=pymysql.cursors.DictCursor  # 結果を辞書形式で取得
                )
                if self.connection.open:
                    self.cursor = self.connection.cursor()
                    print("MySQL connection established")
            except MySQLError as e:
                print(f"Error while connecting to MySQL: {e}")

    def close(self):
        """MySQL接続を閉じる"""
        if self.connection and self.connection.open:
            self.cursor.close()
            self.connection.close()
            print("MySQL connection closed")

class RequestHandler(BaseHTTPRequestHandler):
    db_connection = MySQLConnection()  # サーバー起動時に一度接続する

    def do_GET(self):
        """GETリクエストを処理"""
        path = self.path
        query_params = urllib.parse.parse_qs(self.path.split('?')[1]) if '?' in self.path else {}

        if path.startswith('/hello'):
            self.hello()
        elif path.startswith('/add_id'):
            user_id = query_params.get('user_id', [None])[0]
            user_name = query_params.get('user_name', [None])[0]
            self.add_id(user_id, user_name)
        elif path.startswith('/check_user_exists'):
            user_id = query_params.get('user_id', [None])[0]
            self.check_user_exists(user_id)
        elif path.startswith('/update_column_value'):
            # /update_column_value に対応する処理
            user_id = query_params.get('user_id', [None])[0]
            column_name = query_params.get('column_name', [None])[0]
            new_value = query_params.get('new_value', [None])[0]
            self.update_column_value(user_id, column_name, new_value)
        elif path.startswith('/get_column_value'):
            # /get_column_value に対応する処理
            user_id = query_params.get('user_id', [None])[0]
            column_name = query_params.get('column_name', [None])[0]
            self.get_column_value(user_id, column_name)
        elif path.startswith('/update_game_count'):
            # /update_game_count のリクエストに対応する処理
            user_id1 = query_params.get('user_id1', [None])[0]
            column_name1 = query_params.get('column_name1', [None])[0]
            user_id2 = query_params.get('user_id2', [None])[0]
            column_name2 = query_params.get('column_name2', [None])[0]
            self.update_game_count(user_id1, column_name1, user_id2, column_name2)
        else:
            self.not_found()

    def hello(self):
        """/helloのリクエストに対してHello Worldを返す"""
        self.send_response_with_data(200, 'Hello World')

    def add_id(self, user_id, user_name):
        """/add_idのリクエストでMySQLにIDとユーザーネームを追加"""
        if user_id is None or user_name is None:
            self.send_response_with_data(400, 'Missing user_id or user_name parameter.')
            return

        try:
            self.db_connection.cursor.callproc('ad_insert', (user_id, user_name))
            result = self.db_connection.cursor.fetchall()

            if result and result[0]['result'] == 'seikou!':
                self.send_response_with_data(200, f"ID {user_id} with name {user_name} added successfully.")
            else:
                self.send_response_with_data(500, 'Error occurred while inserting into the database.')

            self.db_connection.connection.commit()

        except MySQLError as e:
            self.handle_mysql_error(str(e))

    def check_user_exists(self, user_id):
        """/check_user_existsのリクエストでMySQLのad_check関数を呼び出す"""
        if user_id is None:
            self.send_response_with_data(400, 'Missing user_id parameter.')
            return

        try:
            self.db_connection.cursor.execute("SELECT ad_check(%s) AS result", (user_id,))
            result = self.db_connection.cursor.fetchone()

            if result and result['result'] == 'User found':
                self.send_response_with_data(200, 'User found. No action required.')
            else:
                self.send_response_with_data(404, 'User not found.')

        except MySQLError as e:
            self.handle_mysql_error(str(e))

    def update_column_value(self, user_id, column_name, new_value):
        """/update_column_valueのリクエストでMySQLのデータを更新"""
        if user_id is None or column_name is None or new_value is None:
            self.send_response_with_data(400, 'Missing user_id, column_name or new_value parameter.')
            return

        try:
            # 動的SQLを使用して指定されたカラムの値を更新
            sql_query = f"UPDATE id_table SET {column_name} = %s WHERE user_id = %s"
            self.db_connection.cursor.execute(sql_query, (new_value, user_id))

            # 変更をコミット
            self.db_connection.connection.commit()

            # 更新が成功した場合
            self.send_response_with_data(200, f"User ID {user_id} {column_name} updated to {new_value}.")

        except MySQLError as e:
            self.handle_mysql_error(str(e))

    def get_column_value(self, user_id, column_name):
        """/get_column_valueのリクエストでMySQLから指定されたカラムの値を取得"""
        if user_id is None or column_name is None:
            self.send_response_with_data(400, 'Missing user_id or column_name parameter.')
            return

        try:
            # 指定されたuser_idとcolumn_nameに基づいて値を取得
            sql_query = f"SELECT {column_name} FROM id_table WHERE user_id = %s"
            self.db_connection.cursor.execute(sql_query, (user_id,))
            result = self.db_connection.cursor.fetchone()

            if result:
                # カラムの値が取得できた場合
                column_value = result.get(column_name)
                if column_value is not None:
                    # 値をそのままレスポンスとして返す
                    self.send_response_with_data(200, str(column_value))
                else:
                    self.send_response_with_data(404, f"Column {column_name} not found for user {user_id}.")
            else:
                self.send_response_with_data(404, f"User {user_id} not found.")

        except MySQLError as e:
            self.handle_mysql_error(str(e))

    def update_game_count(self, user_id1, column_name1, user_id2, column_name2):
        """update_game_countストアドプロシージャを呼び出す"""
        if user_id1 is None or column_name1 is None or user_id2 is None or column_name2 is None:
            self.send_response_with_data(400, 'Missing user_id or column_name parameter.')
            return

        try:
            # 出力パラメータを @p_flag1, @p_flag2, @p_top_10_ranking として渡す
            self.db_connection.cursor.execute("""
                CALL update_game_count(%s, %s, %s, %s, @p_flag1, @p_flag2, @p_top_10_ranking);
            """, (user_id1, column_name1, user_id2, column_name2))

            # 出力パラメータを取得
            self.db_connection.cursor.execute('SELECT @p_flag1, @p_flag2, @p_top_10_ranking')
            result = self.db_connection.cursor.fetchone()

            # 出力パラメータの値を取得
            p_flag1 = result.get('@p_flag1')
            p_flag2 = result.get('@p_flag2')
            p_top_10_ranking = result.get('@p_top_10_ranking')

            # p_top_10_ranking はJSON形式で返されるため、辞書に変換
            top_10_ranking = json.loads(p_top_10_ranking) if p_top_10_ranking else []

            # クライアントに結果を送信
            response_message = {
                "p_flag1": p_flag1,
                "p_flag2": p_flag2,
                "p_top_10_ranking": top_10_ranking
            }
            self.send_response_with_data(200, json.dumps(response_message), content_type='application/json')

        except MySQLError as e:
            self.handle_mysql_error(str(e))

    def not_found(self):
        self.send_response_with_data(404, 'Not Found method')

    def send_response_with_data(self, status, message, content_type='text/html'):
        """共通のレスポンス送信メソッド"""
        self.send_response(status)
        self.send_header('Content-type', content_type)
        self.end_headers()
        self.wfile.write(message.encode())

    def handle_mysql_error(self, error_message):
        """MySQLエラーを一元的に処理するメソッド"""
        print(f"MySQL Error: {error_message}")
        if "Duplicate entry" in error_message:
            self.send_response_with_data(409, "User ID already exists.")
        else:
            self.send_response_with_data(500, f"Error occurred: {error_message}")

def run(server_class=HTTPServer, handler_class=RequestHandler, port=8080):
    server_address = ('0.0.0.0', port)
    httpd = server_class(server_address, handler_class)
    print(f'Starting server on port {port}...')

    # サーバー起動時にMySQLへの接続を一度行う
    handler_class.db_connection.connect()

    # サーバーの終了時にMySQL接続を閉じるための信号を設定
    def handle_exit_signal(signal, frame):
        print("Shutting down server...")
        handler_class.db_connection.close()  # 接続を閉じる
        httpd.server_close()  # サーバーを閉じる
        sys.exit(0)

    # Ctrl + C でサーバーを停止できるようにシグナルを処理
    signal.signal(signal.SIGINT, handle_exit_signal)

    httpd.serve_forever()

if __name__ == '__main__':
    run()