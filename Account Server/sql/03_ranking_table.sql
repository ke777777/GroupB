USE my_database;
CREATE TABLE ranking_table (
    user_id INT PRIMARY KEY,          -- ユーザーID（ユニーク）
    user_name VARCHAR(15),            -- ユーザー名
    win_rate DECIMAL(5, 2),           -- 勝率（小数点2桁まで）
    n_win INT,                        -- 勝利数
    n_loss INT,                       -- 敗北数
    user_rank INT                     -- ユーザーランキング（追加）
);