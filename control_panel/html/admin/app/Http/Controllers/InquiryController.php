<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Inquiry;

class InquiryController extends Controller
{
    public function index()
    {
        $inquiries = Inquiry::all();
        return view('inquiry.index', compact('inquiries'));
    }

    public function create()
    {
        return view('inquiry.create');
    }

    public function storeFromWeb(Request $request)
    {
        $validated = $request->validate([
            'inquiry_id' => 'required|string',
            'title' => 'required|string|max:255',
            'content' => 'required|string',
            'receiver_id' => 'required|string',
            'sender_id' => 'required|string',
        ]);

        // 新しいInquiryを保存
        $inquiry = new Inquiry();
        $inquiry->inquiry_id = $validated['inquiry_id'];
        $inquiry->title = $validated['title'];
        $inquiry->content = $validated['content'];
        $inquiry->sender_id = $validated['sender_id'];
        $inquiry->receiver_id = $validated['receiver_id'];
        $inquiry->save();

        // 保存後に一覧ページへリダイレクト
        $inquiries = Inquiry::all(); // 通知一覧を再取得
        return view('dashboard', compact('inquiries'));
    }

    public function storeFromApi(Request $request)
    {
        $validated = $request->validate([
            'title' => 'required|string|max:255',
            'content' => 'required|string',
            'sender_id' => 'required|string',
            'receiver_id' => 'required|string',
        ]);
    
        // 最大の inquiry_id を取得して +1 を計算
        $maxInquiryId = Inquiry::max('inquiry_id');
        $nextInquiryId = $maxInquiryId !== null ? $maxInquiryId + 1 : 1;
    
        // 新しいInquiryを作成
        $inquiry = Inquiry::create([
            'inquiry_id' => $nextInquiryId, // 最大値+1を設定
            'title' => $validated['title'],
            'content' => $validated['content'],
            'sender_id' => $validated['sender_id'],
            'receiver_id' => $validated['receiver_id'],
        ]);
    
        return response()->json([
            'status' => 'success',
            'message' => 'Inquiry created successfully',
            'data' => $inquiry,
            'id' => $inquiry->inquiry_id,
        ], 201); // ステータスコード201を明示的に設定
    }

    public function replyFromApi(Request $request)
    {
        $validated = $request->validate([
            'content' => 'required|string',
            'inquiry_id' => 'required|integer|exists:inquiries,inquiry_id', // inquiry_id を検証
            'sender_id' => 'required|string',
            'receiver_id' => 'required|string',
        ]);

        // inquiry_id が正しく渡されていれば、元の問い合わせを取得
        $originalInquiry = Inquiry::where('inquiry_id', $validated['inquiry_id'])->firstOrFail();

        // 必要な情報をサーバー側で補完して返信を作成
        $reply = Inquiry::create([
            'title' => 'Re: ' . $originalInquiry->title,
            'content' => $validated['content'],
            'inquiry_id' => $originalInquiry->inquiry_id,
            'sender_id' => $validated['sender_id'],
            'receiver_id' => $validated['receiver_id'],
        ]);

        return response()->json([
            'status' => 'success',
            'message' => 'Reply added successfully.',
            'reply' => $reply,
        ], 201);
    }


}
