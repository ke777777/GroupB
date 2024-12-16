USE my_database;
DELIMITER //

CREATE PROCEDURE ad_insert(IN p_user_id INT, IN p_user_name VARCHAR(15))
BEGIN
    -- INSERT文でデータを挿入
    INSERT INTO id_table (user_id, user_name)
    VALUES (p_user_id, p_user_name);
    
    -- 処理が成功したら 'seikou!' を返す
    SELECT 'seikou!' AS result;
END //

DELIMITER ;