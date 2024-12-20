USE my_database;
DELIMITER $$

CREATE PROCEDURE update_game_count(
    IN p_user_id1 INT, 
    IN p_column_name1 VARCHAR(10), 
    IN p_user_id2 INT, 
    IN p_column_name2 VARCHAR(10), 
    OUT p_flag1 INT,
    OUT p_flag2 INT,
    OUT p_top_10_ranking JSON
)
BEGIN
    DECLARE v_n_win1 INT;
    DECLARE v_n_loss1 INT;
    DECLARE v_win_rate1 DECIMAL(5, 2);
    DECLARE v_user_name1 VARCHAR(15);
    DECLARE v_flag_value1 INT DEFAULT 0;
    DECLARE v_current_rank1 INT;
    DECLARE v_new_rank1 INT;
    DECLARE v_existing_win_rate1 DECIMAL(5, 2);
    DECLARE v_previous_user_rank1 INT;
    DECLARE v_existing_user1 INT;

    DECLARE v_n_win2 INT;
    DECLARE v_n_loss2 INT;
    DECLARE v_win_rate2 DECIMAL(5, 2);
    DECLARE v_user_name2 VARCHAR(15);
    DECLARE v_flag_value2 INT DEFAULT 0;
    DECLARE v_current_rank2 INT;
    DECLARE v_new_rank2 INT;
    DECLARE v_existing_win_rate2 DECIMAL(5, 2);
    DECLARE v_previous_user_rank2 INT;
    DECLARE v_existing_user2 INT;

    DECLARE v_user_id INT;
    DECLARE v_previous_win_rate DECIMAL(5, 2);
    DECLARE done INT DEFAULT FALSE;

    -- カーソルを宣言
    DECLARE cur CURSOR FOR
        SELECT user_id, win_rate
        FROM ranking_table
        ORDER BY win_rate DESC, user_id;  -- win_rateで降順、同じ場合はuser_idで安定化

    -- ハンドラの設定: カーソルの終了条件
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    -- id_tableからuser_nameを取得
    SELECT user_name INTO v_user_name1
    FROM id_table
    WHERE user_id = p_user_id1;

    SELECT user_name INTO v_user_name2
    FROM id_table
    WHERE user_id = p_user_id2;

    -- 現在のユーザーの順位を取得
    SELECT user_rank INTO v_current_rank1
    FROM ranking_table
    WHERE user_id = p_user_id1;

    SELECT user_rank INTO v_current_rank2
    FROM ranking_table
    WHERE user_id = p_user_id2;

    -- 'n_win' または 'n_loss' の場合、ゲーム結果を更新
    IF p_column_name1 = 'n_win' THEN
        UPDATE id_table
        SET n_win = n_win + 1
        WHERE user_id = p_user_id1;
    ELSEIF p_column_name1 = 'n_loss' THEN
        UPDATE id_table
        SET n_loss = n_loss + 1
        WHERE user_id = p_user_id1;
    END IF;

    IF p_column_name2 = 'n_win' THEN
        UPDATE id_table
        SET n_win = n_win + 1
        WHERE user_id = p_user_id2;
    ELSEIF p_column_name2 = 'n_loss' THEN
        UPDATE id_table
        SET n_loss = n_loss + 1
        WHERE user_id = p_user_id2;
    END IF;

    -- 最新のn_winとn_lossを取得
    SELECT n_win, n_loss INTO v_n_win1, v_n_loss1
    FROM id_table
    WHERE user_id = p_user_id1;

    SELECT n_win, n_loss INTO v_n_win2, v_n_loss2
    FROM id_table
    WHERE user_id = p_user_id2;

    -- n_winが10以上の場合のみ勝率を計算し、ランキングを更新
    IF v_n_win1 >= 10 THEN
        -- 勝率を計算
        SET v_win_rate1 = v_n_win1 / (v_n_win1 + v_n_loss1);

        -- 同じuser_idがranking_tableにあるかチェック
        SELECT COUNT(*) INTO v_existing_user1
        FROM ranking_table
        WHERE user_id = p_user_id1;

        -- ranking_tableにまだユーザーが存在しない場合、挿入
        IF v_existing_user1 = 0 THEN
            INSERT INTO ranking_table (user_id, user_name, win_rate, n_win, n_loss, user_rank)
            VALUES (p_user_id1, v_user_name1, v_win_rate1, v_n_win1, v_n_loss1, 1);  -- 最初に1位として挿入
        ELSE
            -- 既存ユーザーがいる場合、更新
            UPDATE ranking_table
            SET win_rate = v_win_rate1, n_win = v_n_win1, n_loss = v_n_loss1
            WHERE user_id = p_user_id1;
        END IF;
    END IF;

    IF v_n_win2 >= 10 THEN
        -- 勝率を計算
        SET v_win_rate2 = v_n_win2 / (v_n_win2 + v_n_loss2);

        -- 同じuser_idがranking_tableにあるかチェック
        SELECT COUNT(*) INTO v_existing_user2
        FROM ranking_table
        WHERE user_id = p_user_id2;

        -- ranking_tableにまだユーザーが存在しない場合、挿入
        IF v_existing_user2 = 0 THEN
            INSERT INTO ranking_table (user_id, user_name, win_rate, n_win, n_loss, user_rank)
            VALUES (p_user_id2, v_user_name2, v_win_rate2, v_n_win2, v_n_loss2, 1);  -- 最初に1位として挿入
        ELSE
            -- 既存ユーザーがいる場合、更新
            UPDATE ranking_table
            SET win_rate = v_win_rate2, n_win = v_n_win2, n_loss = v_n_loss2
            WHERE user_id = p_user_id2;
        END IF;
    END IF;

    -- ranking_tableの順位を計算して更新
    -- win_rateで降順に並べて順位を計算
    SET @rank := 0;  -- ランキングの初期化
    SET v_previous_win_rate := NULL;  -- 前回のwin_rateの初期化

    -- カーソルを開く
    OPEN cur;

    -- ループ処理
    read_loop: LOOP
        FETCH cur INTO v_user_id, v_win_rate1;  -- 修正：v_win_rate1に変更

        -- カーソルからデータを取得できたらループを終了
        IF done THEN
            LEAVE read_loop;
        END IF;

        -- win_rateが前回と同じなら順位を維持、異なれば順位を上げる
        IF v_win_rate1 = v_previous_win_rate THEN  -- 修正：v_win_rate1を使用
            -- 同じwin_rateの場合、順位は維持
            SET @rank := @rank;  -- 順位変更なし
        ELSE
            -- 異なるwin_rateの場合、順位を1つ増やす
            SET @rank := @rank + 1;
        END IF;

        -- ranking_tableの順位を更新
        UPDATE ranking_table 
        SET user_rank = @rank
        WHERE user_id = v_user_id;

        -- @previous_win_rateを更新
        SET v_previous_win_rate = v_win_rate1;

    END LOOP;

    -- カーソルを閉じる
    CLOSE cur;

    -- 一時テーブルに並べ替えた結果を保存
    CREATE TEMPORARY TABLE temp_ranking_table AS
    SELECT user_id, user_name, win_rate, n_win, n_loss, user_rank
    FROM ranking_table
    ORDER BY user_rank;

    -- 既存のranking_tableを削除
    DROP TABLE ranking_table;

    -- 並べ替えた結果をranking_tableとして再作成
    CREATE TABLE ranking_table AS
    SELECT * FROM temp_ranking_table;

    -- 一時テーブルを削除
    DROP TEMPORARY TABLE temp_ranking_table;

    -- ユーザーの新しい順位を取得
    SELECT user_rank INTO v_new_rank1
    FROM ranking_table
    WHERE user_id = p_user_id1;

    SELECT user_rank INTO v_new_rank2
    FROM ranking_table
    WHERE user_id = p_user_id2;

    -- ユーザーがranking_tableにもともと存在していたかどうかを確認し、フラッグ設定
    IF v_existing_user1 = 0 AND v_new_rank1 <= 10 THEN
        SET v_flag_value1 = 1;  -- ranking_tableに加わったかつ順位が10位以内に入った場合にフラッグを立てる
    ELSEIF v_new_rank1 < v_current_rank1 AND v_new_rank1 <= 10 THEN
        SET v_flag_value1 = 1;  -- 順位が上がったかつ順位が10位以内の場合にフラッグを立てる
    END IF;

    IF v_existing_user2 = 0 AND v_new_rank2 <= 10 THEN
        SET v_flag_value2 = 1;  -- ranking_tableに加わったかつ順位が10位以内に入った場合にフラッグを立てる
    ELSEIF v_new_rank2 < v_current_rank2 AND v_new_rank2 <= 10 THEN
        SET v_flag_value2 = 1;  -- 順位が上がったかつ順位が10位以内の場合にフラッグを立てる
    END IF;

    -- 勝率でランキングが更新された後、上位10件を取得
    SELECT JSON_ARRAYAGG(
            JSON_OBJECT(
                'user_id', user_id,
                'user_name', user_name,
                'win_rate', win_rate,
                'n_win', n_win,
                'n_loss', n_loss,
                'user_rank', user_rank
            )
        ) INTO p_top_10_ranking
    FROM ranking_table
    WHERE user_rank <= 10; -- 10件のみ取得

    -- フラッグの値を返す
    SET p_flag1 = v_flag_value1;
    SET p_flag2 = v_flag_value2;

END $$

DELIMITER ;