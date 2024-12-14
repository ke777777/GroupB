<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;
use Illuminate\Foundation\Support\Providers\EventServiceProvider as BaseServiceProvider;
use App\Events\MyEvent;

class EventServiceProvider extends BaseServiceProvider
{
    protected $listen = [
        MyEvent::class => [
            // リスナーがある場合はここにリスナーを追加
        ],
    ];

    public function boot()
    {
        parent::boot();
    }
}
