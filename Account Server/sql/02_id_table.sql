USE my_database;
CREATE TABLE id_table (
    user_id INT UNSIGNED NOT NULL,
    user_name VARCHAR(15) NOT NULL,
    n_win INT UNSIGNED DEFAULT 0,
    n_loss INT UNSIGNED DEFAULT 0,
    delete_flag BOOLEAN DEFAULT FALSE, 
    PRIMARY KEY (user_id)
);