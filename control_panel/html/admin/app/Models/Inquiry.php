<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Inquiry extends Model
{
    protected $table = 'inquiries';
    protected $fillable = ['inquiry_id', 'title', 'content', 'receiver_id', 'sender_id'];
}
