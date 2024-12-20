USE my_database;
DELIMITER $$

CREATE FUNCTION ad_check(user_id_ INT UNSIGNED) 
RETURNS VARCHAR(50)
DETERMINISTIC
BEGIN
    DECLARE result VARCHAR(50);
    
    -- 引数とカラム名が競合しないようにuser_id_を使用
    IF EXISTS (SELECT 1 FROM id_table WHERE user_id = user_id_) THEN
        SET result = 'User found';
    ELSE
        SET result = 'User not found';
    END IF;
    
    RETURN result;
END $$

DELIMITER ;