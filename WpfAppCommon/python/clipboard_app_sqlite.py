import sqlite3
dbname = 'clipboard_pyhton.db'


def create_masked_data_table():

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    cursor.execute("CREATE TABLE IF NOT EXISTS MASKED_TEXT (id INTEGER PRIMARY KEY AUTOINCREMENT, BEFORE TEXT, AFTER, TEXT)")
    # ENT_LABELとLAST_NUMの値を保存するテーブルの作成
    cursor.execute("CREATE TABLE IF NOT EXISTS TEXT_INT (id INTEGER PRIMARY KEY AUTOINCREMENT, ENT_LABEL TEXT, LAST_NUM INTEGER)")
    conn.commit()

    # データベースへのコネクションを閉じる。(必須)
    conn.close()

def select_masked_data_table(rowText: str):

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    # BEFOREがrowTextのデータを取得する
    cursor.execute("SELECT AFTER FROM MASKED_TEXT WHERE BEFORE = ?", (rowText,))
    
    rows = cursor.fetchall()
    conn.close()
    if len(rows) == 0:
        return None
    else:
        return rows[0][0]

def insert_masked_data_table(beforeText: str, afterText: str):

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    cursor.execute("INSERT INTO MASKED_TEXT (BEFORE, AFTER) VALUES (?, ?)", (beforeText, afterText))
    
    conn.commit()
    conn.close()

# テーブルTEXT_INTから指定されたENT＿LABELのINTを取得する
def select_text_int_table(ent_label: str):

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    # ENT_LABELがent_labelのデータを取得する
    cursor.execute("SELECT LAST_NUM FROM TEXT_INT WHERE ENT_LABEL = ?", (ent_label,))
    rows = cursor.fetchall()
    conn.close()
    if len(rows) == 0:
        return 0
    else:
        # ENT_LABELを返す
        return rows[0][0]
    
# テーブルTEXT_INTに指定されたENT＿LABELのlast_numを＋１する
def update_text_int_table(ent_label: str):

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    # ENT_LABELがent_labelのデータを取得する
    last_num = select_text_int_table(ent_label)
    if last_num == 0:
        cursor.execute("INSERT INTO TEXT_INT (ENT_LABEL, LAST_NUM) VALUES (?, 1)", (ent_label,))
    else:
        cursor.execute("UPDATE TEXT_INT SET LAST_NUM = ? WHERE ENT_LABEL = ?", (last_num + 1, ent_label))
    
    conn.commit()
    conn.close()

