<?php

namespace App\Events;

use Illuminate\Broadcasting\Channel;
use Illuminate\Broadcasting\InteractsWithSockets;
use Illuminate\Broadcasting\PresenceChannel;
use Illuminate\Broadcasting\PrivateChannel;
use Illuminate\Contracts\Broadcasting\ShouldBroadcast;
use Illuminate\Foundation\Events\Dispatchable;
use Illuminate\Queue\SerializesModels;

class MyEvent implements ShouldBroadcast
{
    use SerializesModels;

    public $data; // 通知するデータ（noticesやinquiriesの内容）
    public $type; // 通知の種類（notices, inquiriesなど）

    /**
     * イベントのインスタンスを作成
     *
     * @param  mixed  $data  通知するデータ
     * @param  string  $type  通知の種類（テーブル名など）
     * @return void
     */
    public function __construct($data, $type)
    {
        $this->data = $data;
        $this->type = $type;
    }


    // ブロードキャストされるチャンネルを指定
    public function broadcastOn()
    {
        return new Channel('my-channel');
    }

    // 任意でデータをブロードキャストする
    public function broadcastWith()
    {
        return [
            'title' => $this->notice->title,
            'content' => $this->notice->content,
            'type' => $this->type,
        ];
    }

    // use Dispatchable, InteractsWithSockets, SerializesModels;

    // /**
    //  * Create a new event instance.
    //  */
    // public function __construct()
    // {
    //     //
    // }

    // /**
    //  * Get the channels the event should broadcast on.
    //  *
    //  * @return array<int, \Illuminate\Broadcasting\Channel>
    //  */
    // public function broadcastOn(): array
    // {
    //     return [
    //         new PrivateChannel('channel-name'),
    //     ];
    // }
}
