<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class UserInfo extends Model
{
    use HasFactory;

    protected $connection = 'mysql_two';

    // 対応するテーブル名を指定
    protected $table = 'id_table';
    // protected $fillable = ['user_id', 'user_name', 'n_win', 'n_loss', 'delete_frag'];
}
