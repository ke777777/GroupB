<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Notice;

class NoticeController extends Controller
{
    public function index()
    {
        $notices = Notice::all();
        return view('notice.index', compact('notices'));
    }

    public function create()
    {
        return view('notice.create');
    }
    


    public function store(Request $request)
    {
        // バリデーションを行う（必要に応じて追加）
        $validated = $request->validate([
            'title' => 'required|string|max:255',
            'content' => 'required|string',
        ]);

        // データベースに新しい通知を保存
        $notice = new Notice();
        $notice->title = $validated['title'];
        $notice->content = $validated['content'];
        $notice->save();

        // 保存後に一覧ページへリダイレクト
        $notices = Notice::all(); // 通知一覧を再取得
        return view('dashboard', compact('notices'));
    }
}
