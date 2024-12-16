<?php

use App\Http\Controllers\ProfileController;
use App\Http\Controllers\NoticeController;
use App\Http\Controllers\InquiryController;
use Illuminate\Support\Facades\Route;
use App\Models\UserInfo;
use App\Models\Notice;
use App\Models\Inquiry;

Route::get('/', function () {
    return view('welcome');
});

Route::get('/dashboard', function () {
    return view('dashboard');
})->middleware(['auth', 'verified'])->name('dashboard');


Route::get('/api/tabs/{tab}', function ($tab) {
    $views = [
        'dashboard' => 'dashboard.index',
        'userinfo' => 'userinfo.index', 
        'notice' => 'notice.index',
        'inquiry' => 'inquiry.index',
    ];

    if (!array_key_exists($tab, $views)) {
        return response('<p>指定されたタブは存在しません。</p>', 404);
    }

    if ($tab === 'userinfo') {
        // mysql_twoからデータを取得
        $userinfos = UserInfo::all();
        return view($views[$tab], compact('userinfos'))->render();
    }

    if ($tab === 'notice') {
        $notices = Notice::all(); // 通知データを取得
        return view($views[$tab], compact('notices'))->render();
    }

    if ($tab === 'inquiry') {
        $inquiries = Inquiry::all(); // 通知データを取得
        return view($views[$tab], compact('inquiries'))->render();
    }

    return view($views[$tab])->render();
});

//Notice//
// 通知作成ページを表示するルート
Route::get('/notice/create', [NoticeController::class, 'create'])->name('notice.create');

// 通知一覧を表示するルート（GETメソッド）
Route::get('/notice', [NoticeController::class, 'index'])->name('notice.index');

// 通知を保存するルート（POSTメソッド）
Route::post('/notice', [NoticeController::class, 'store'])->name('notice.store');

//Notice to Unity
Route::get('/api/notice', function () {
    return response()->json(Notice::all());
});

//Inquiry
Route::get('/inquiry/create', [InquiryController::class, 'create'])->name('inquiry.create');

Route::get('/inquiry', [InquiryController::class, 'index'])->name('inquiry.index');

Route::post('/inquiry', [InquiryController::class, 'storeFromWeb'])->name('inquiry.storeFromWeb');



//Inquiry to Unity
Route::get('/api/inquiry', function () {
    return response()->json(Inquiry::all());
});

Route::get('/api/getMaxInquiryId', [InquiryController::class, 'getMaxInquiryId']);

Route::post('/api/inquiry', [InquiryController::class, 'storeFromApi']);

Route::get('/api/reply', [InquiryController::class, 'replyFromApi']);

Route::post('/api/reply', [InquiryController::class, 'replyFromApi']);



//CSRF-Token to Unity
Route::get('/csrf-token', function () {
    return response()->json(['token' => csrf_token()]);
});

Route::middleware('auth')->group(function () {
    Route::get('/profile', [ProfileController::class, 'edit'])->name('profile.edit');
    Route::patch('/profile', [ProfileController::class, 'update'])->name('profile.update');
    Route::delete('/profile', [ProfileController::class, 'destroy'])->name('profile.destroy');
});

require __DIR__.'/auth.php';
